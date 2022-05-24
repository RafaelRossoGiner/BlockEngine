using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;


namespace BlockEngine
{

    public class LevelController : MonoBehaviour
    {

        public static bool m_isHost = false;

        // Static attributes.
        public static string m_worldName;
        public static World loadedWorld = null;
        public static LevelController instance = null;


        // Non-static attributes.
        public List<Material> skyboxOptions;
        public GameObject objectTemplate;
        public GameObject spawnController;


        // Non-static methods.

        /*
        Cosas a realizar cuando se carga el mundo.
        */
        public void Awake()
        {

            // Inicializar instancia.
            if (instance == null)
                instance = this;
            else
                Debug.LogWarning("An instance of LevelController already exists!");

            DiagramData.LoadRequiredAssets();
            if (m_isHost)
            {

                if (m_worldName == "")
                    NewWorld();
                else
                    LoadWorld(m_worldName);
                Instantiate(spawnController);
            }
            else
                ConnectWorld();

        }
        public void Start()
        {
            if (m_isHost)
                SpawnController.instance.LoadObjects(ref loadedWorld);
        }

        public static void ApplySkybox(int skyboxID)
        {
            if (skyboxID >= 0 && skyboxID < instance.skyboxOptions.Count)
            {
                loadedWorld.skyboxID = skyboxID;
                RenderSettings.skybox = instance.skyboxOptions[loadedWorld.skyboxID];
                DynamicGI.UpdateEnvironment();
            }
        }

        // World-Loading static methods.
        public static void PrepareWorld(bool isHost, string worldName = "")
        {

            m_isHost = isHost;
            m_worldName = worldName;

        }

        /*
        Save a world into a formatted json file.
        What is saved is the world template ID and the objects and node diagrams that have been created
        and still exist.
        */
        public static void SaveWorld(string saveName)
        {
            if (m_isHost)
            {
                string filePath = Path.Combine(Definitions.savesPath, saveName + ".json");
                using (FileStream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    using (StreamWriter writer = new StreamWriter(stream))
                    {

                        string jsonedWorld = JsonConvert.SerializeObject(loadedWorld, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                        writer.Write(jsonedWorld);

                    }
                }
            }
        }

        public void NewWorld()
        {

            DiagramData.LoadRequiredAssets();
            loadedWorld = new World(0);

        }

        public void ConnectWorld()
        {

            DiagramData.LoadRequiredAssets();
            loadedWorld = new World(0);

        }

        /*
        Load a previously saved world in a json file.
        All objects and node diagrams are spawned in the world with this method.
        */
        public void LoadWorld(string worldName = "")
        {

            loadedWorld = World.LoadWorld(worldName);
            if (loadedWorld != null)
                Debug.Log(loadedWorld.worldID + " has " + loadedWorld.worldModels.Count + " models and " + loadedWorld.worldDiagrams.Count + " diagrams");
            else
            {

                Debug.LogWarning("World file could not be found, creating new world instead");
                NewWorld();

            }

        }
    }
}
