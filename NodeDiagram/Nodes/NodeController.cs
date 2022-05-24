using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace BlockEngine
{
	public class NodeController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
	{
		[SerializeField]
		private GameObject nodeHandlePrefab, TextInputPrefab;
		[SerializeField]
		private RectTransform inputList, body, inputs, outputList;
		[SerializeField]
		private TextMeshProUGUI bodyText;
		[SerializeField]
		Color selected, unselected, deleteSelected;

		public List<TMP_InputField> inputComponents;
		public TMP_Dropdown dropDownComponent;

		[System.NonSerialized]
		public NodeExecution nodeExecution;
		[System.NonSerialized]
		public NodeInfo nodeInfo;
		[System.NonSerialized]
		public List<NodeHandle> inputHandles, outputHandles;
		[System.NonSerialized]
		public int my_ID;

		private Image image;

		private RectTransform m_rtf;
		private Vector2 prevPos;
		private Vector2 startMouse;
		// Methods
		public void Awake()
		{
			m_rtf = GetComponent<RectTransform>();
			inputComponents = new List<TMP_InputField>();
			inputHandles = new List<NodeHandle>();
			outputHandles = new List<NodeHandle>();
			image = GetComponent<Image>();
		}
		// Initialize the node on the diagram
		public void Initialize(NodeInfo assignedNodeInfo, NodeExecution execution)
		{
			image.color = unselected;

			// Data initialization
			nodeInfo = assignedNodeInfo;
			bodyText.text = nodeInfo.name;

			nodeExecution = execution;
			my_ID = execution.nodeID;

			// Handle (Input/Output) initialization
			int handleIndex = 0;
			foreach (NodeInfo.VariableType inputType in nodeInfo.InputTypes)
			{
				inputHandles.Add(Instantiate(nodeHandlePrefab, inputList).GetComponent<NodeHandle>());
				inputHandles[handleIndex].InitializeHandle(NodeHandle.HandleType.Input, this, inputType, handleIndex);
				handleIndex++;
			}
			handleIndex = 0;
			foreach (NodeInfo.VariableType outputType in nodeInfo.OutputTypes)
			{
				outputHandles.Add(Instantiate(nodeHandlePrefab, outputList).GetComponent<NodeHandle>());
				outputHandles[handleIndex].InitializeHandle(NodeHandle.HandleType.Output, this, outputType, handleIndex);
				handleIndex++;
			}
			m_rtf.anchoredPosition = nodeExecution.nodePosition;

			// Additional input field initialization
			if (nodeInfo.InputFields < 1)
			{
				inputs.gameObject.SetActive(false);
				inputComponents = null;
			}
			else
			{
				inputs.gameObject.SetActive(true);
				TMP_InputField inputField;
				for (int i = 0; i < nodeInfo.InputFields; i++)
				{
					inputField = Instantiate(TextInputPrefab, inputs).GetComponent<TMP_InputField>();
					inputField.SetTextWithoutNotify(nodeExecution.storedTextFields[i]);
					inputComponents.Add(inputField);
				}
			}

			// Check if the node has it's own dynamically defined dropdown
			if (execution.DynamicDropdown(ref dropDownComponent))
            {
				dropDownComponent.gameObject.SetActive(true);
				dropDownComponent.SetValueWithoutNotify(execution.storedSelectedOptionIndex);
				dropDownComponent.RefreshShownValue();
			}
            else
            {
				// Use the dropdown defined on the scriptable object (if any)
				if (nodeInfo.DropDownOptions.Count < 1)
				{
					dropDownComponent.gameObject.SetActive(false);
				}
				else
				{
					dropDownComponent.AddOptions(nodeInfo.DropDownOptions);
					dropDownComponent.gameObject.SetActive(true);
					dropDownComponent.SetValueWithoutNotify(execution.storedSelectedOptionIndex);
					dropDownComponent.RefreshShownValue();
				}
			}
		}

		// Drag Controller
		public void OnBeginDrag(PointerEventData eventData)
		{
			if (DiagramEditor.CurrentEditor.mode == DiagramEditor.Modes.drag)
            {
				prevPos = m_rtf.anchoredPosition;
				startMouse = eventData.position;
			}
		}

		public void OnDrag(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
				return;
			if (DiagramEditor.CurrentEditor.mode == DiagramEditor.Modes.drag)
			{
				m_rtf.anchoredPosition = prevPos + eventData.position - startMouse;
			}
		}
		public void OnEndDrag(PointerEventData eventData)
		{
			if (DiagramEditor.CurrentEditor.mode == DiagramEditor.Modes.drag)
			{
				nodeExecution.nodePosition = m_rtf.anchoredPosition;
			}
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			if (DiagramEditor.CurrentEditor.mode == DiagramEditor.Modes.deleteNode)
			{
				DiagramEditor.CurrentEditor.DeleteNode(this);
			}
		}

		// Selectable
        public void OnPointerEnter(PointerEventData eventData)
        {
			if (DiagramEditor.LastSelectedHandle == null)
            {
				switch (DiagramEditor.CurrentEditor.mode)
				{
					case DiagramEditor.Modes.deleteNode:
						image.color = deleteSelected;
						break;
					case DiagramEditor.Modes.drag:
						image.color = selected;
						break;
					default:
						break;
				}
			}
		}

        public void OnPointerExit(PointerEventData eventData)
        {
			image.color = unselected;
		}
    }
}
