using System.Collections.Generic;
using UnityEngine;


namespace BlockEngine
{

    public class SpawnMenu : MonoBehaviour
    {

        // Static attributes.
        public static int selectedModelID = 0;
        public static string selectedModelName = null;
        public static List<ModelIcon> spawnMenuIcons = new List<ModelIcon>();


        // Non-static attributes.
        [SerializeField]
        private GameObject categoryTemplate;
        [SerializeField]
        private GameObject modelTemplate;


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
            ModelCategory modelCategory = categoryButton.GetComponent<ModelCategory>();

            categoryButton.SetActive(true);
            modelCategory.setText("All");
            modelCategory.setID(0); // Identifying by numerical ID is faster than by using a string.

            categoryButton.transform.SetParent(categoryTemplate.transform.parent, false);

            int ID = 1;
            for (int i = 0; i < PreviewModel.categories.Count; i++)
            {

                categoryButton = Instantiate(categoryTemplate);
                modelCategory = categoryButton.GetComponent<ModelCategory>();

                categoryButton.SetActive(true);
                modelCategory.setText(PreviewModel.categories[i]);
                modelCategory.setID(ID); // Identifying by numerical ID is faster than by using a string.

                categoryButton.transform.SetParent(categoryTemplate.transform.parent, false);

                ID++;

            }


            /*
            Modelos.
            */
            for (int i = 0; i < PreviewModel.icons.Count; i++)
            {

                GameObject modelIcon = Instantiate(modelTemplate);
                modelIcon.SetActive(true);
                modelIcon.transform.SetParent(modelTemplate.transform.parent, false);

                ModelIcon modelIc = modelIcon.GetComponent<ModelIcon>();
                modelIc.setIcon(PreviewModel.icons[i]);
                modelIc.modelID = i;
                modelIc.categoryID = PreviewModel.iconCatID[i];
                spawnMenuIcons.Add(modelIc);

            }

        }

    }

}
