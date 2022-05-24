using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Unity.Netcode;
using System;

namespace BlockEngine {
    public class DiagramEditor : MonoBehaviour
    {
        public enum Modes { deleteNode, deleteLink, drag };
        [SerializeField]
        private GameObject nodePrefab, linkPrefab;
        [SerializeField]
        private Transform diagramContent, linkContent;
        [SerializeField]
        private Image deleteNode, deleteLink;
        [SerializeField]
        private Color deleteSelected, deleteUnselected;
        [SerializeField]
        private UILineConnector mouseLinkConnector;
        [SerializeField]
        private GameObject mouseTarget;
        [SerializeField]
        private TextMeshProUGUI mouseText, mouseInText;
        [SerializeField]
        private GraphicRaycaster m_Raycaster;

        private DiagramData currentDiagram;

        private static DiagramEditor currentEditor;
        private static NodeHandle lastSelectedHandle;

        private static Dictionary<GameObject, int> objToID;
        private static Dictionary<int, NodeController> IDtoController;
        private static Dictionary<Tuple<NodeHandle, NodeHandle>, UILineConnector> handlesToLink;

        private EventSystem m_EventSystem;
        private PointerEventData m_PointerEventData;

        public Modes mode;

        // Multiplayer
        private bool isExternal;
        private ulong senderID;
        // Properties
        public DiagramData CurrentDiagram { 
            get { return currentDiagram; } 
            set { currentDiagram = value; }
        }
        public static DiagramEditor CurrentEditor
        {
            get { return currentEditor; }
        }
        public static NodeHandle LastSelectedHandle
        {
            get { return lastSelectedHandle; }
        }

        // ===================== [ Initialization ] ==================================
        public void Start()
        {
            // Parameter Initialization
            m_EventSystem = EventSystem.current;
            currentEditor = this;

            // Node to cursor link behaviour
            NodeHandle.SetDiagramController(this);
            mouseTarget.SetActive(false);
            mouseLinkConnector.gameObject.SetActive(false);
        }
        // ===================== [ Editor Manipulation ] ================================
        public static void SetReferences(DiagramEditor editor)
        {
            editor.Start();
            // Data initialization 
            objToID = new Dictionary<GameObject, int>();
            IDtoController = new Dictionary<int, NodeController>();
            handlesToLink = new Dictionary<Tuple<NodeHandle, NodeHandle>, UILineConnector>();
        }
        public void OpenDiagram(InteractableObject objectToEdit)
        {
            // Open Local Diagram
            isExternal = false;

            currentDiagram = objectToEdit.diagram;
            currentDiagram.BeingEdited = true;

            // Graphic Methods
            DrawDiagram(currentDiagram);
        }
        public void OpenDiagram(DiagramData diagram, ulong newSenderID)
        {
            // Open External Diagram
            isExternal = true;
            senderID = newSenderID;

            currentDiagram = diagram;
            currentDiagram.BeingEdited = true;

            // Graphic Methods
            DrawDiagram(currentDiagram);
        }
        // ===================== [ UI Button Callbacks ] =====================
        public void CloseDiagram()
        {
            if (!isExternal)
            {
                if (currentDiagram != null)
                {
                    currentDiagram.RefreshExecution();
                    currentDiagram.BeingEdited = false;
                    currentDiagram = null;
                }
            }
            else
            {
                string jsonString = currentDiagram.ToJson();
                FastBufferWriter writer = new FastBufferWriter(jsonString.Length * sizeof(char) * 2, Unity.Collections.Allocator.Temp);
                writer.WriteValueSafe(jsonString);
                NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage("UpdateRemoteDiagram", senderID, writer, NetworkDelivery.ReliableFragmentedSequenced);
            }
            HUD.CloseDiagram();
        }
        public void DeleteButtonClicked()
        {
            if(mode == Modes.deleteNode)
            {
                mode = Modes.drag;
            }
            else
            {
                mode = Modes.deleteNode;
            }
            UpdateUIButtons();
        }
        public void DeleteLinkButtonClicked()
        {
            if (mode == Modes.deleteLink)
            {
                mode = Modes.drag;
            }
            else
            {
                mode = Modes.deleteLink;
            }
            UpdateUIButtons();
        }
        // ===================== [ Utility Functions ] =====================
        private void UpdateUIButtons()
        {
            // Color appropiatedly
            switch(mode)
            {
                case Modes.drag:
                    deleteNode.color = deleteUnselected;
                    deleteLink.color = deleteUnselected;
                    break;
                case Modes.deleteNode:
                    deleteNode.color = deleteSelected;
                    deleteLink.color = deleteUnselected;
                    break;
                case Modes.deleteLink:
                    deleteNode.color = deleteUnselected;
                    deleteLink.color = deleteSelected;
                    break;
            }
        }
        // ===================== [ General Diagram Manipulation ] =====================
        public void DrawDiagram(DiagramData diagramData)
        {
            ClearDiagram();

            // Draw diagram
            if (diagramData != null) 
            { 
                foreach(KeyValuePair<int, NodeExecution> node in diagramData.nodes)
                {
                    DrawNode(node.Value, DiagramData.nodeInfoInstances[node.Value.nodeType]);
                }
                foreach (KeyValuePair<int, NodeExecution> node in diagramData.nodes)
                {
                    DrawNodeLinks(node.Key);
                }
            }
            else
            {
                Debug.LogError("Trying to draw a null diagram reference!");
            }
        }
        public void ClearDiagram()
        {
            mode = Modes.drag;
            UpdateUIButtons();

            handlesToLink.Clear();
            IDtoController.Clear();
            objToID.Clear();

            foreach (Transform child in diagramContent)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in linkContent)
            {
                Destroy(child.gameObject);
            }
        }
        // ===================== [ Node Manipulation ] ================================
        public void CreateAndDrawNode(NodeInfo nodeInfo)
        {
            // Create Node
            NodeExecution nodeExecution = currentDiagram.CreateNode(nodeInfo, new Vector2(0f, 0f));
            DrawNode(nodeExecution, DiagramData.nodeInfoInstances[nodeExecution.nodeType]);
        }
        public void DeleteNode(NodeController node)
        {
            // First try to delete node in diagram
            if (currentDiagram.DeleteNodeExecution(node.nodeExecution))
            {
                // If succesful, remove all links and node itself.
                // Eliminate input links
                foreach (NodeHandle inputHandle in node.inputHandles)
                {
                    if (inputHandle.connectedHandles.Count > 0)
                    {
                        foreach (NodeHandle outputHandle in inputHandle.connectedHandles)
                        {
                            Tuple<NodeHandle, NodeHandle> key = new Tuple<NodeHandle, NodeHandle>(outputHandle, inputHandle);
                            if (handlesToLink.TryGetValue(key, out UILineConnector conn))
                            {
                                Destroy(conn.gameObject);
                                handlesToLink.Remove(key);
                            }
                        }
                    }
                }
                // Eliminate output links
                foreach (NodeHandle outputHandle in node.outputHandles)
                {
                    if (outputHandle.connectedHandles.Count > 0)
                    {
                        foreach (NodeHandle inputHandle in outputHandle.connectedHandles)
                        {
                            Tuple<NodeHandle, NodeHandle> key = new Tuple<NodeHandle, NodeHandle>(outputHandle, inputHandle);
                            if(handlesToLink.TryGetValue(key, out UILineConnector conn))
                            {
                                Destroy(conn.gameObject);
                                handlesToLink.Remove(key);
                            }
                        }
                    }
                }
                // Eliminate node
                objToID.Remove(node.gameObject);
                IDtoController.Remove(node.my_ID);
                Destroy(node.gameObject);
            }
        }
        public void DrawNode(NodeExecution nodeExecution, NodeInfo nodeInfo)
        {
            // Visualize Node
            NodeController NodeVisualElement = Instantiate(nodePrefab, transform).GetComponent<NodeController>();
            NodeVisualElement.Initialize(nodeInfo, nodeExecution);
            if (nodeInfo.InputFields > 0)
            {
                for (int i = 0; i < NodeVisualElement.inputComponents.Count; i++){
                    NodeVisualElement.inputComponents[i].onEndEdit.AddListener(delegate { ChangedInputFields(NodeVisualElement); });
                }
                ChangedInputFields(NodeVisualElement); // Call to initialize default selection element.
            }

            if (NodeVisualElement.dropDownComponent.enabled)
			{
                NodeVisualElement.dropDownComponent.onValueChanged.AddListener(delegate { ChangedDropDownOption(NodeVisualElement); });
                ChangedDropDownOption(NodeVisualElement); // Call to initialize default selection element.
            }
            objToID[NodeVisualElement.gameObject] = NodeVisualElement.my_ID;
            IDtoController[NodeVisualElement.my_ID] = NodeVisualElement;
        }
        public void ChangedInputFields(NodeController node) 
        {
            string[] newValues = new string[node.inputComponents.Count];
            for(int i = 0; i < node.inputComponents.Count; i++)
			{
                newValues[i] = node.inputComponents[i].text;
			}
            node.nodeExecution.SetParams(newValues);
        }
        public void ChangedDropDownOption(NodeController node)
		{
            node.nodeExecution.SetParams(node.dropDownComponent.value);
		}
        // ===================== [ Link Manipulation ] ================================
        public void CreateLink(NodeHandle outputHandle, NodeHandle inputHandle)
        {
            if (currentDiagram.TryCreateLink(outputHandle, inputHandle))
            {
                DrawLink(outputHandle, inputHandle);
            }
        }
        public void DeleteLink(NodeHandle outputHandle, NodeHandle inputHandle)
        {
            Tuple<NodeHandle, NodeHandle> key = new Tuple<NodeHandle, NodeHandle>(outputHandle, inputHandle);
            if (handlesToLink.TryGetValue(key, out UILineConnector connector))
            {
                outputHandle.connectedHandles.Clear();
                inputHandle.connectedHandles.Clear();

                if (!isExternal)
                {
                    outputHandle.ParentNode.nodeExecution.RemoveOutput(outputHandle.HandleCount, inputHandle.ParentNode.nodeExecution, inputHandle.HandleCount);
                }
                else
                {
                    outputHandle.ParentNode.nodeExecution.RemoveRemoteNodeOutput(outputHandle.HandleCount, inputHandle.ParentNode.nodeExecution, inputHandle.HandleCount);
                }

                Destroy(connector.gameObject);
                handlesToLink.Remove(key);
            }
        }
        public void DrawNodeLinks(int nodeId)
        {
            NodeController nodeController = IDtoController[nodeId];
            foreach (KeyValuePair<int, List<(int, int)>> connectionList in nodeController.nodeExecution.connections)
            {
                foreach ((int, int) connectionPoint in connectionList.Value)
                {
                    DrawLink(nodeController.outputHandles[connectionList.Key], IDtoController[connectionPoint.Item1].inputHandles[connectionPoint.Item2]);
                }
            }
        }
        public void DrawLink(NodeHandle outputHandle, NodeHandle inputHandle)
        {
            Debug.Log("Drawn link " + outputHandle.ParentNode.nodeInfo.name + " to " + inputHandle.ParentNode.nodeInfo.name);
            UILineConnector visualLink = Instantiate(linkPrefab, linkContent).GetComponent<UILineConnector>();
            visualLink.transforms = new RectTransform[2] { outputHandle.Rtf, inputHandle.Rtf };

            outputHandle.connectedHandles.Add(inputHandle);
            inputHandle.connectedHandles.Add(outputHandle);
            handlesToLink[new Tuple<NodeHandle, NodeHandle>(outputHandle, inputHandle)] = visualLink;
        }
        public void StartLinking(NodeHandle callerHandle)
        {
            lastSelectedHandle = callerHandle;
            mouseTarget.gameObject.SetActive(true);
            mouseLinkConnector.gameObject.SetActive(true);
            mouseLinkConnector.transforms[1] = callerHandle.Rtf;

            // mouse variable type text
            mouseText.text = callerHandle.ValueType.ToString();
        }
        public void SetInputHandleText(string text)
        {
            mouseInText.text = text;
        }
        public void StopLinking()
        {
            m_PointerEventData = new PointerEventData(m_EventSystem);
            m_PointerEventData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();

            m_Raycaster.Raycast(m_PointerEventData, results);
            foreach (RaycastResult result in results)
            {
                if (result.gameObject.CompareTag("NodeHandle"))
                {
                    NodeHandle handleToLink = result.gameObject.GetComponent<NodeHandle>();
                    switch (mode)
                    {
                        case Modes.drag:
                            CreateLink(lastSelectedHandle, handleToLink);
                            break;
                        case Modes.deleteNode:
                            break;
                        case Modes.deleteLink:
                            DeleteLink(lastSelectedHandle, handleToLink);
                            break;
                        default:
                            break;
                    }
                    break;
                }
            }
            mouseTarget.gameObject.SetActive(false);
            mouseLinkConnector.gameObject.SetActive(false);
            lastSelectedHandle = null;
        }
    }
}
