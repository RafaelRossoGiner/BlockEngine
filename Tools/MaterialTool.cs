using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


namespace BlockEngine
{

    public class MaterialTool : NetworkBehaviour
    {

        // Non-static attributes.
        public static int selectedMaterial = 0;
        [System.NonSerialized]
        public static List<Material> materials;
        public static List<Texture2D> materialsTextures;


        // Non-static methods.
        public static void LoadMaterials()
        {

            materials = new List<Material>();
            materialsTextures = new List<Texture2D>();

            // Cargar todos los materiales que se encuentren en Resources//Materials.
            string materialsPath = Definitions.materialsPath;
            DirectoryInfo materialsDirectory = new DirectoryInfo(materialsPath);
            foreach (FileInfo file in materialsDirectory.GetFiles("*.png"))
            {

                Material material = new Material(Shader.Find("Specular"));
                Texture2D texture = new Texture2D(1, 1); // El tamaño será ajustado automáticamente en cuanto le asignemos el .png del material.

                texture.LoadImage(File.ReadAllBytes(materialsDirectory + "\\" + file.Name));
                texture.filterMode = FilterMode.Point;
                texture.wrapMode = TextureWrapMode.Clamp;
                material.mainTexture = texture;

                materials.Add(material);
                materialsTextures.Add(texture);

            }

        }

    }

}
