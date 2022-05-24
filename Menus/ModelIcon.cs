using UnityEngine;
using UnityEngine.UI;


namespace BlockEngine
{

    public class ModelIcon : MonoBehaviour
    {


        // Non-static attributes.
        public int modelID;
        public int categoryID;


        // Non-static methods.

        /*
        Set model's icon in spawn menu.
        */
        public void setIcon(Sprite newSprite)
        {

            gameObject.GetComponent<Image>().sprite = newSprite;

        }

        /*
        Change the selected model ID according to the selected model
        in the spawn menu.
        */
        public void setSelectedModel()
        {

            SpawnMenu.selectedModelID = modelID;
            SpawnMenu.selectedModelName = gameObject.name;

        }

        public void setSelectedMaterial() {

            MaterialTool.selectedMaterial = modelID;
        
        }

    }

}