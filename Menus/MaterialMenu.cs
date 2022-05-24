using System.Collections.Generic;
using UnityEngine;


namespace BlockEngine
{

	public class MaterialMenu : MonoBehaviour
	{

		// Static attributes.
		public static List<ModelIcon> materialMenuIcons = new List<ModelIcon>();


		// Non-static attributes.
		[SerializeField]
		private GameObject categoryTemplate;
		[SerializeField]
		private GameObject materialButtonTemplate;


		// Non-static methods.

		/*
		Cargar modelos y categorías de modelos
		*/
		public void Start()
		{

			/*
			Categorías.
			*/

			// Poner una categoría que incluya a todos los modelos.
			GameObject categoryButton = Instantiate(categoryTemplate);
			MaterialCategory materialAllCategory = categoryButton.GetComponent<MaterialCategory>();

			categoryButton.SetActive(true);
			materialAllCategory.setText("All");
			materialAllCategory.setID(0);

			categoryButton.transform.SetParent(categoryTemplate.transform.parent, false);

			int ID = 1;
			for (int i = 0; i < PreviewMaterial.categories.Count; i++)
			{

				categoryButton = Instantiate(categoryTemplate);
				materialAllCategory = categoryButton.GetComponent<MaterialCategory>();

				categoryButton.SetActive(true);
				materialAllCategory.setText(PreviewMaterial.categories[i]);
				materialAllCategory.setID(ID);

				categoryButton.transform.SetParent(categoryTemplate.transform.parent, false);

				ID++;

			}


			/*
			Modelos.
			*/
			for (int i = 0; i < PreviewMaterial.icons.Count; i++)
			{

				GameObject modelIcon = Instantiate(materialButtonTemplate);
				modelIcon.SetActive(true);
				modelIcon.transform.SetParent(materialButtonTemplate.transform.parent, false);

				ModelIcon modelIc = modelIcon.GetComponent<ModelIcon>();
				modelIc.setIcon(PreviewMaterial.icons[i]);
				modelIc.modelID = i;
				modelIc.categoryID = PreviewMaterial.iconCatID[i];
				materialMenuIcons.Add(modelIc);

			}

		}

	}

}
