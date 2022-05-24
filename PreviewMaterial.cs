using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;


namespace BlockEngine
{

    public class PreviewMaterial : MonoBehaviour
    {

        // Static attributes.
        public static PreviewMaterial instance = null;
        public static List<string> categories = new List<string>();
        public static List<Sprite> icons = new List<Sprite>();
        public static List<int> iconCatID = new List<int>();
        public static Dictionary<string, int> iconCat = new Dictionary<string, int>();


        // Non-static attributes.
        [SerializeField]
        private Camera previewCamera;
        public GameObject previewModel;


        // Non-static methods.
        public PreviewMaterial()
        {

            if (instance == null)
                instance = this;
            else
                throw new SystemException("An instance of 'PreviewModel' already exists!");

        }

        public static void load()
        {

            // Detect all defined categories.
            string[] token;
            int catID;
            // Avoid duplicates on the list from previous load
            categories.Clear();
            foreach (string line in File.ReadLines(Path.Combine(Definitions.materialsPath, "categories.txt")))
            {

                token = line.Split(':');

                if ((catID = categories.IndexOf(token[1])) != -1)
                {

                    iconCat[token[0]] = catID + 1;

                }
                else
                {

                    categories.Add(token[1]);
                    iconCat[token[0]] = categories.Count;

                }

            }

            // Get icon of all models
            DirectoryInfo modelDirectory = new DirectoryInfo(Definitions.materialsPath);
            string filename;
            int ID = 0;
            foreach (FileInfo file in modelDirectory.GetFiles("*.png"))
            {

                filename = file.Name.Substring(0, file.Name.IndexOf('.'));

                // Asociar icono y categoría.
                icons.Add(instance.getIcon(ID));
                iconCatID.Add(iconCat[filename]);

                ID++;

            }

        }

        public Sprite getIcon(int ID)
        {

            Texture2D texture = MaterialTool.materialsTextures[ID];

            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0)); // TODO: that vector2 has to be created only once per spawn menu start(). Call it previewOrigin

        }

    }

}
