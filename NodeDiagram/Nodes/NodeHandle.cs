using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
namespace BlockEngine
{
	public class NodeHandle : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
	{
		public enum HandleType { Input, Output }

		public HandleType handleType;
		[SerializeField]
		private Sprite InputTexture, OutputTexture;
		[SerializeField]
		Color selected, unselected, deleteSelected;

		private static DiagramEditor diagramController;

		public List<NodeHandle> connectedHandles;
		private NodeInfo.VariableType valueType;
		private NodeController m_parentNode;
		private RectTransform m_rtf;
		private Image image;
		private int handleCount;

		// Properties
		public NodeInfo.VariableType ValueType
		{
			get { return valueType; }
		}
		public NodeController ParentNode
		{
			get { return m_parentNode; }
		}
		public RectTransform Rtf
		{
			get { return m_rtf; }
		}
		public int HandleCount
        {
			get { return handleCount; }
        }

		// Methods
		public void Awake()
		{
			m_rtf = GetComponent<RectTransform>();
			image = GetComponent<Image>();
		}
		public static void SetDiagramController(DiagramEditor controller)
		{
			diagramController = controller;
		}
		public void InitializeHandle(HandleType type, NodeController parentNode, NodeInfo.VariableType valType, int handleIndex)
		{
			m_parentNode = parentNode;
			handleType = type;
			valueType = valType;
			handleCount = handleIndex;
			connectedHandles = new List<NodeHandle>();

			switch (handleType)
			{
				case HandleType.Input:
					image.sprite = InputTexture;
					break;
				case HandleType.Output:
					image.sprite = OutputTexture;
					break;
				default:
					Debug.Log("Unknown Extension Type encountered");
					break;
			}
		}
		// UI Events

		public void OnBeginDrag(PointerEventData eventData)
		{
			if (handleType == HandleType.Output)
			{
				diagramController.StartLinking(this);
			}
		}
		public void OnDrag(PointerEventData eventData)
		{

		}
		public void OnEndDrag(PointerEventData eventData)
		{
			diagramController.StopLinking();
		}

		// Selectable
		public void OnPointerEnter(PointerEventData eventData)
		{
			DiagramEditor.CurrentEditor.SetInputHandleText("=>" + valueType.ToString());
			switch (DiagramEditor.CurrentEditor.mode)
			{
				case DiagramEditor.Modes.deleteLink:
					image.color = deleteSelected;
					break;
				case DiagramEditor.Modes.drag:
					image.color = selected;
					break;
				default:
					break;
			}
		}
		public void OnPointerExit(PointerEventData eventData)
		{
			DiagramEditor.CurrentEditor.SetInputHandleText("");
			image.color = unselected;
		}
	}
}
