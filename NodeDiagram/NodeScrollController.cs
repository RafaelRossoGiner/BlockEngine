using UnityEngine.UI.Extensions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

namespace BlockEngine
{
	public class NodeScrollController : MonoBehaviour
	{
		[SerializeField]
		private Transform scrollContent;
		[SerializeField]
		private GameObject scrollOptionPrefab;
		[SerializeField]
		private DiagramEditor diagramController;

		// Unity is causing problems on asset loading through code. For now this list references all scriptable assets in order to 
		// force them to be serialized and loaded into memory for the DiagramData to find the appropiate references
		[SerializeField]
		private List<NodeInfo> nodeAssets;

		private static Dictionary<string, List<NodeInfo>> nodeCategories;
		
		public static void InitializeAssetLists()
        {
			nodeCategories = new Dictionary<string, List<NodeInfo>>();

			foreach (string categoryName in System.Enum.GetNames(typeof(NodeInfo.Categories)))
            {
				nodeCategories[categoryName] = new List<NodeInfo>();
			}

			List<NodeInfo> nodeCategoryList;
			foreach (KeyValuePair<string, NodeInfo> nodeInfo in DiagramData.nodeInfoInstances)
            {
				if (nodeCategories.TryGetValue(nodeInfo.Value.Category.ToString(), out nodeCategoryList))
                {
					nodeCategoryList.Add(nodeInfo.Value);
                }
            }
        }


		public void Start()
		{
			if (nodeCategories != null)
				Showcategories();
            else
            {
				DiagramData.LoadRequiredAssets();
				Showcategories();
			}
		}

		// Node Menu Handling
		public void Showcategories()
		{
			foreach (Transform child in scrollContent)
			{
				Destroy(child.gameObject);
			}
			foreach (KeyValuePair<string, List<NodeInfo>> nodeCategory in nodeCategories)
            {
				GameObject Button = Instantiate(scrollOptionPrefab, scrollContent);
				Button.GetComponentInChildren<TextMeshProUGUI>().text = nodeCategory.Key;
				Button.GetComponent<Button>().onClick.AddListener(delegate { ShowNodes(nodeCategory.Value); });
			}
		}

		public void ShowNodes(List<NodeInfo> NodeList)
		{
			foreach (Transform child in scrollContent)
			{
				Destroy(child.gameObject);
			}
			foreach (NodeInfo nodeInfo in NodeList)
			{
				GameObject nodeObj = Instantiate(scrollOptionPrefab, scrollContent);
				nodeObj.GetComponentInChildren<TextMeshProUGUI>().text = nodeInfo.NodeName;
				nodeObj.GetComponent<Button>().onClick.AddListener(delegate { diagramController.CreateAndDrawNode(nodeInfo); });
			}
		}
	}
}
