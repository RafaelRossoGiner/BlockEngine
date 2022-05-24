using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;


namespace BlockEngine
{

    public class PreviewModel : MonoBehaviour
    {

        // Static attributes.
        public static PreviewModel instance = null;
        public static List<string> categories = new List<string>();
        public static List<Sprite> icons = new List<Sprite>();
        public static List<int> iconCatID = new List<int>();
        public static Dictionary<string, int> iconCat = new Dictionary<string, int>();


        // Non-static attributes.
        [SerializeField]
        private Camera previewCamera;
        public GameObject previewModel;


        // Non-static methods.
        public PreviewModel()
        {

            if (instance == null)
                instance = this;
            else
                throw new SystemException("An instance of 'PreviewModel' already exists!");

        }

        public void Start()
        {
            loadModels();
        }

        public static void loadModels()
        {

            // Detect all defined categories.
            string[] token;
            int catID;
            foreach (string line in File.ReadLines(Path.Combine(Definitions.modelsPath, "categories.txt")))
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
            DirectoryInfo modelDirectory = new DirectoryInfo(Definitions.modelsPath);
            string filename;
            int ID = 0;
            // Avoid duplicating entries on the list from previous loads
            icons.Clear();
            iconCatID.Clear();
            foreach (FileInfo file in modelDirectory.GetFiles("*.obj"))
            {

                filename = file.Name.Substring(0, file.Name.IndexOf('.'));

                Model.loadModel(Definitions.modelsPath, file.Name, filename + ".mtl");

                // Make a preview of the mode.
                Model model = new Model();
                model.meshID = ID;
                model.materialID = ID;
                model.ApplyMesh(instance.previewModel);

                // Asociar icono y categoría.
                icons.Add(instance.getIcon());
                iconCatID.Add(iconCat[filename]);

                ID++;

            }

        }

        public Sprite getIcon()
        {

            int resX = previewCamera.pixelWidth,
            resY = previewCamera.pixelHeight,
            clipX = 0,
            clipY = 0;


            // Make sure that the resolution of the preview image that
            // we are going to generate is square.
            if (resX > resY) // If resolution is landscape mode.
                clipX = resX - resY;
            else if (resY > resX) // If resolution is portrait mode.
                clipY = resY - resX;

            // Make sure that the model fits inside the camera's view.
            previewCamera.orthographicSize = previewModel.GetComponent<Renderer>().bounds.extents.y + 1.0f;

            // Take a 'screenshot' of the model to make a preview of it.
            Texture2D texture = new Texture2D(resX - clipX, resY - clipY, TextureFormat.RGBA32, false);
            RenderTexture renderTexture = new RenderTexture(resX, resY, 24); // A camera can draw to this texture instead of drawing to the screen.

            previewCamera.targetTexture = renderTexture;
            RenderTexture.active = renderTexture;

            previewCamera.Render();
            texture.ReadPixels(new Rect(clipX / 2, clipY / 2, resX - clipX, resY - clipY), 0, 0);
            texture.Apply();

            // Clean things.
            previewCamera.targetTexture = null;
            RenderTexture.active = null;
            Destroy(renderTexture); // TODO: change renderTexture to be an object outside getIcon so it doesn't get destroyed everytime!

            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0)); // TODO: that vector2 has to be created only once per spawn menu start(). Call it previewOrigin

        }

    }

}