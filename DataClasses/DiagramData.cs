using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;

namespace BlockEngine {
    public class DiagramData
    {
        public static Dictionary<InteractableObject, DiagramData> ObjToDiagram = new Dictionary<InteractableObject, DiagramData>();
        public static Dictionary<string, NodeInfo> nodeInfoInstances = new Dictionary<string, NodeInfo>();
        // ===================== [ Serializable Data ] ===========================
        public Dictionary<int, NodeExecution> nodes = new Dictionary<int, NodeExecution>();
        public List<int> startingNodesID = new List<int>();
        public List<int> finalNodesID = new List<int>();
        public int nextNodeID = 0;
        public int m_objectID;
        // ===================== [ Runtime Data ] ================================
        private bool beingEdited;
        private bool isRemote = false;
        private List<NodeExecution> m_startingNodes = new List<NodeExecution>();
        private List<NodeExecution> m_finalNodes = new List<NodeExecution>();
        private InteractableObject m_object;
        // ===================== [ Properties ] ================================
        public bool BeingEdited { set { beingEdited = value; } }
        public bool IsRemote { get { return isRemote; } set { isRemote = value; } }
        // ===================== [ Class methods ] ================
        public static void LoadRequiredAssets()
        {
            nodeInfoInstances = new Dictionary<string, NodeInfo>();

            NodeInfo[] nodeInfos = Resources.FindObjectsOfTypeAll<NodeInfo>();
            foreach (NodeInfo info in nodeInfos)
            {
                //Debug.Log(info);
                nodeInfoInstances[info.name] = info;
            }
            Debug.Log("Node assets loaded with " + nodeInfoInstances.Count + " node types found!");

            // Call other required initializations
            NodeScrollController.InitializeAssetLists();
        }
        public static DiagramData ImportDiagramStructure(string jsonDiagram)
        {
            DiagramData diagram = JsonConvert.DeserializeObject<DiagramData>(jsonDiagram);
            diagram.isRemote = true;
            NodeInfo nodeInfo;
            Dictionary<int, NodeExecution> nodeExecsList = new Dictionary<int, NodeExecution>();
            List<int> keys = new List<int>(diagram.nodes.Keys);
            foreach (int nodeID in keys)
            {
                if (nodeInfoInstances.TryGetValue(diagram.nodes[nodeID].nodeType, out nodeInfo))
                {
                    nodeExecsList[nodeID] = diagram.nodes[nodeID];
                    diagram.nodes[nodeID] = (NodeExecution)Activator.CreateInstance(nodeInfo.Execution);
                }
                else
                {
                    Debug.LogError("Unnable to find NodeInfo asset called " + diagram.nodes[nodeID].nodeType + " on load");
                }
            }
            foreach (KeyValuePair<int, NodeExecution> node in nodeExecsList)
            {
                diagram.nodes[node.Key].CopyValues(node.Value);
                diagram.nodes[node.Key].LoadConnections(diagram);
            }
            foreach (int startNode in diagram.startingNodesID)
            {
                diagram.m_startingNodes.Add(diagram.nodes[startNode]);
            }
            return diagram;
        }
        // ===================== [ Constructor & Initialization ] ================
        public DiagramData(InteractableObject interactable)
		{
            if (interactable != null)
			{
                Initialize(interactable);
            }
		}
        public void UpdateFromRemote(DiagramData refDiagram)
        {
            isRemote = false;
            nodes.Clear();
            nodes = refDiagram.nodes;
            startingNodesID = refDiagram.startingNodesID;
            finalNodesID = refDiagram.finalNodesID;
            nextNodeID = refDiagram.nextNodeID;
            m_objectID = refDiagram.m_objectID;
        }
        public static DiagramData LoadOrCreateDiagram(InteractableObject interactable)
        {
            DiagramData diagram;
            if(LevelController.loadedWorld.worldDiagrams.TryGetValue(interactable.ObjectID, out diagram))
            {
                diagram.Initialize(interactable);
			}
			else
			{
                diagram = new DiagramData(interactable);
            }
            ObjToDiagram[interactable] = diagram;
            return diagram;
        }
        public void Initialize(InteractableObject interactable)
        {
            m_object = interactable;
            m_objectID = m_object.ObjectID;

            NodeInfo nodeInfo;
            Dictionary<int, NodeExecution> nodeExecsList = new Dictionary<int, NodeExecution>();
            List<int> keys = new List<int>(nodes.Keys);
            foreach (int nodeID in keys)
            {
                if (nodeInfoInstances.TryGetValue(nodes[nodeID].nodeType, out nodeInfo))
                {
                    nodeExecsList[nodeID] = nodes[nodeID];
                    nodes[nodeID] = (NodeExecution)Activator.CreateInstance(nodeInfo.Execution);
                }
                else
                {
                    Debug.LogError("Unnable to find NodeInfo asset called " + nodes[nodeID].nodeType + " on load");
                }
            }
            foreach (KeyValuePair<int, NodeExecution> node in nodeExecsList)
            {
                nodes[node.Key].LoadExecution(node.Value, m_object, this);
            }
            foreach(int startNode in startingNodesID)
            {
                m_startingNodes.Add(nodes[startNode]);
            }
            foreach (int finalNode in finalNodesID)
            {
                m_finalNodes.Add(nodes[finalNode]);
            }
            RefreshExecution();
        }
        // ===================== [ Diagram Node Manipulation ] ===================
        public NodeExecution CreateNode(NodeInfo node, Vector2 initialPos)
        {
            NodeExecution newExecution = (NodeExecution)Activator.CreateInstance(node.Execution);
            if (!isRemote)
            {
                newExecution.InitializeNewNode(nextNodeID, initialPos, node, m_object);
            }
            else
            {
                newExecution.InitializeNewNode(nextNodeID, initialPos, node);
            }

            nodes.Add(nextNodeID, newExecution);
            if (node.IsStartNode)
            {
                m_startingNodes.Add(newExecution);
                startingNodesID.Add(newExecution.nodeID);
            }
            if (node.IsFinalNode)
			{
                m_finalNodes.Add(newExecution);
                finalNodesID.Add(newExecution.nodeID);
            }
            nextNodeID++;
            return newExecution;
        }
        public bool DeleteNodeExecution(NodeExecution execution)
        {
            if (isRemote)
            {
                execution.RemovePredecentRemoteNodeConnections();
            }
            else
            {
                execution.RemovePredecentNodesConnections();
            }
            nodes.Remove(execution.nodeID);
            startingNodesID.Remove(execution.nodeID);
            m_startingNodes.Remove(execution);
            return true;
        }
        // ===================== [ Diagram Link Manipulation ] ==================
        public bool TryCreateLink(NodeHandle output, NodeHandle input)
        {
            bool created = false;
            if (output.ValueType == input.ValueType || input.ValueType == NodeInfo.VariableType.Any || output.ValueType == NodeInfo.VariableType.Any)
            {
                // Input accepts the provided type
                if (input.connectedHandles.Count == 0 || (!input.connectedHandles.Contains(output) && output.ValueType == input.connectedHandles[0].ValueType))
                {
                    // Only link if there is no other connection or the new connection has the same type of the previous ones
                    output.ParentNode.nodeExecution.AddOutput(output.HandleCount, input.ParentNode.nodeExecution, input.HandleCount);
                    created = true;
                }
            }
            return created;
        }

        public bool TryDeleteLink(NodeHandle output, NodeHandle input)
        {
            bool created = false;
            if (output.ValueType == input.ValueType || input.ValueType == NodeInfo.VariableType.Any || output.ValueType == NodeInfo.VariableType.Any)
            {
                // Input accepts the provided type
                if (input.connectedHandles.Count == 0 || (!input.connectedHandles.Contains(output) && output.ValueType == input.connectedHandles[0].ValueType))
                {
                    // Only link if there is no other connection or the new connection has the same type of the previous ones
                    output.ParentNode.nodeExecution.AddOutput(output.HandleCount, input.ParentNode.nodeExecution, input.HandleCount);
                    created = true;
                }
            }
            return created;
        }
        // ===================== [ Diagram Utility ] ==========================
        public DiagramData FromJson(string jsonDiagram)
        {
            return JsonConvert.DeserializeObject<DiagramData>(jsonDiagram);
        }
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.None, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        }

        // ===================== [ Execution ] ================================
        public void RefreshExecution()
		{
            foreach (KeyValuePair<int, NodeExecution> node in nodes)
			{
                node.Value.BeforeExecution();
			}
		}
        public void Execute()
        {
            if (!beingEdited && m_startingNodes != null)
            {
                foreach (NodeExecution startPoint in m_startingNodes)
                {
                    startPoint.Execute();
                }
            }
        }
    }
}