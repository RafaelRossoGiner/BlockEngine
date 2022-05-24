using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;


namespace BlockEngine
{

    public class Model
    {

        // Static attributes.
        private static Vector3 lineValues = new Vector3();
        private static List<Vector3> tempNormals = new List<Vector3>(),
                 tempVCoords = new List<Vector3>();
        private static List<Vector2> tempUV = new List<Vector2>();
        private static List<List<int[]>> faces = new List<List<int[]>>();
        private static List<int[]> triplets = new List<int[]>();
        private static List<int> submeshes = new List<int>();
        private static List<string> useMaterial = new List<string>(),
                materials = new List<string>(),
                mapKDs = new List<string>();

        public static Dictionary<int, Mesh> modelMesh = new Dictionary<int, Mesh>();
        public static Dictionary<int, Material[]> modelMaterials = new Dictionary<int, Material[]>();

        // Non-static attributes.
        public string ObjectName;
        public int meshID;
        public int materialID = -1; // If equal to -1 then the default material assigned to the corresponding model is used.
        public Vector3 pos;
        public Quaternion rotation;

        // Static methods.

        /*
        Load all models found inside the model folder.
        */
        public static void loadModels()
        {

            string modelFolderPath = Definitions.modelsPath,
               filename;
            DirectoryInfo modelDirectory = new DirectoryInfo(modelFolderPath);
            foreach (FileInfo file in modelDirectory.GetFiles("*.obj"))
            {

                filename = file.Name.Substring(0, file.Name.IndexOf('.'));
                loadModel(modelFolderPath, filename + ".obj", filename + ".mtl");

            }

        }

        /*
        Load a model from the given path
        */
        public static void loadModel(string directoryPath, string objFilename, string mtlFilename)
        {

            string modelName = "undefined",
               materialLib;
            string[] token;
            int[] faceVertex,
              triangles;


            // REMEMBER TO ADD PROPER EXCEPTION HANDLING.

            /*
            Read .obj file 
            */
            foreach (string line in File.ReadLines(directoryPath + "\\" + objFilename))
            {

                if (line != "" && !line.StartsWith("#"))
                {

                    token = line.Split(' ');

                    switch (token[0])
                    {

                        case ("o"):
                            modelName = token[1];
                            break;

                        case ("mtllib"):
                            materialLib = token[1];
                            break;

                        case ("usemtl"):
                            useMaterial.Add(token[1]);
                            faces.Add(new List<int[]>());
                            break;

                        case ("vt"):

                            // UVs have two coordinates.
                            lineValues.x = float.Parse(token[1]);
                            lineValues.y = float.Parse(token[2]);

                            tempUV.Add(lineValues);
                            break;

                        case ("vn"):

                            // Normals have three coordinates.
                            lineValues.x = float.Parse(token[1]);
                            lineValues.y = float.Parse(token[2]);
                            lineValues.z = float.Parse(token[3]);

                            tempNormals.Add(lineValues);
                            break;

                        case ("v"):

                            // Vertex coords have three coordinates.
                            lineValues.x = float.Parse(token[1]);
                            lineValues.y = float.Parse(token[2]);
                            lineValues.z = float.Parse(token[3]);

                            tempVCoords.Add(lineValues);
                            break;

                        case ("f"):

                            // Faces can contain three or more indices and indices
                            // can be present in many formats.
                            for (int i = 1; i < token.Length; i++)
                            {

                                faceVertex = Array.ConvertAll(token[i].Split('/'),
                                       x =>
                                       {

                                           if (String.IsNullOrEmpty(x))
                                               return 0;
                                           else
                                               return int.Parse(x);

                                       });

                                faces[faces.Count - 1].Add(faceVertex);


                            }
                            break;

                    }

                }

            }

            /*
            Read .mtl file.
            */
            foreach (string line in File.ReadLines(directoryPath + "\\" + mtlFilename))
            {

                if (line != "" && !line.StartsWith("#"))
                {

                    token = line.Split(' ');

                    switch (token[0])
                    {

                        case ("newmtl"):
                            materials.Add(token[1]);
                            break;

                        case ("map_Kd"):
                            mapKDs.Add(token[1]);
                            break;

                    }

                }

            }

            /*
            Start building Unity's mesh.
            */
            Mesh mesh = new Mesh();
            for (int i = 0; i < faces.Count; i++)
            {

                for (int j = 0; j < faces[i].Count; j++)
                    triplets.Add(faces[i][j]);

                submeshes.Add(faces[i].Count);

            }

            Vector3[] vertices = new Vector3[triplets.Count],
                      normals = new Vector3[triplets.Count];
            Vector2[] uvs = new Vector2[triplets.Count];

            for (int i = 0; i < triplets.Count; i++)
            {

                // .obj faces' indices start at 1.
                vertices[i] = tempVCoords[triplets[i][0] - 1];

                // There is a case where a face has vertex coord, normal
                // and no texture coordinate.
                if (triplets[i][1] > 0)
                    uvs[i] = tempUV[triplets[i][1] - 1];

                normals[i] = tempNormals[triplets[i][2] - 1];

            }

            mesh.name = modelName;
            mesh.vertices = vertices;
            mesh.uv = uvs;
            mesh.normals = normals;
            mesh.subMeshCount = submeshes.Count;

            /*
            Deal with possible submeshes.
            */
            int vertex = 0;
            for (int i = 0; i < submeshes.Count; i++)
            {

                triangles = new int[submeshes[i]];

                for (int j = 0; j < submeshes[i]; j++)
                {

                    triangles[j] = vertex;

                    vertex++;

                }

                mesh.SetTriangles(triangles, i);

            }

            /*
            End with .obj processing.
            */
            mesh.RecalculateBounds();
            mesh.Optimize();


            /*
            Process materials.
            */
            Material[] meshMaterials = new Material[useMaterial.Count];

            int index;
            Texture2D texture;

            for (int i = 0; i < useMaterial.Count; i++)
            {

                index = materials.IndexOf(useMaterial[i]);

                meshMaterials[i] = new Material(Shader.Find("Diffuse"));
                meshMaterials[i].name = materials[index];


                // Add textures if there are.

                if (mapKDs.Count != 0)
                {

                    texture = new Texture2D(1, 1); // El tamaño será ajustado automáticamente en cuanto le asignemos la imagen de la textura.
                    texture.LoadImage(File.ReadAllBytes(directoryPath + "\\" + mapKDs[index]));
                    texture.filterMode = FilterMode.Point;
                    meshMaterials[i].mainTexture = texture;

                }

            }

            /*
            Store mesh and materials.
            */
            modelMesh.Add(modelMesh.Count, mesh);
            modelMaterials.Add(modelMaterials.Count, meshMaterials);


            /*
            Clear temp data.
            */
            tempNormals.Clear();
            tempVCoords.Clear();
            tempUV.Clear();
            faces.Clear();
            triplets.Clear();
            submeshes.Clear();
            useMaterial.Clear();
            materials.Clear();
            mapKDs.Clear();

        }

        /*
        Apply a model (mesh with related materials) to a gameObject.
        */
        public void ApplyMesh(GameObject gameObject)
        {

            MeshFilter filter = gameObject.GetComponent<MeshFilter>();
            MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();

            filter.mesh = modelMesh[meshID];
            renderer.materials = modelMaterials[meshID];

        }

        /*
        Apply a model (mesh with custom material) to a gameObject.
        */
        public void ApplyMesh(GameObject gameObject, int customMaterialID)
        {
            MeshFilter filter = gameObject.GetComponent<MeshFilter>();
            MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();

            filter.mesh = modelMesh[meshID];

            if (customMaterialID >= 0 && customMaterialID < MaterialTool.materials.Count)
            {
                Material[] newMaterials = new Material[renderer.materials.Length];
                for (int i = 0; i < renderer.materials.Length; i++)
                    newMaterials[i] = MaterialTool.materials[customMaterialID];

                renderer.materials = newMaterials;
            }
            else
            {
                renderer.materials = modelMaterials[meshID];
            }
        }
        public void ApplyMaterial(GameObject gameObject, int customMaterialID)
        {
            MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();

            if (customMaterialID >= 0 && customMaterialID < MaterialTool.materials.Count)
            {
                Material[] newMaterials = new Material[renderer.materials.Length];
                for (int i = 0; i < renderer.materials.Length; i++)
                    newMaterials[i] = MaterialTool.materials[customMaterialID];

                renderer.materials = newMaterials;
            }
            else
            {
                renderer.materials = modelMaterials[meshID];
            }
        }
        public void ApplyColor(GameObject gameObject, Color color)
        {
            MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
            foreach (Material mat in renderer.materials)
            {
                mat.color = color;
            }
        }
    }
}
