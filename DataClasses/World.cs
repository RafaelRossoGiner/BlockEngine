using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace BlockEngine
{

    public class World
    {

        // Attributes.
        public int worldID;
        public int skyboxID;
        // World options like global gravity, light, sea level, skybox...
        public Dictionary<int, DiagramData> worldDiagrams = new Dictionary<int, DiagramData>();
        public Dictionary<int, Model> worldModels = new Dictionary<int, Model>();

        // Methods.
        public World(int worldID)
        {
            this.worldID = worldID;
        }

        /*
		Save a world into a formatted json file.
		What is saved is the world template ID and the objects and node diagrams that have been created
		and still exist.
		*/
        public static void SaveWorld(string saveName, World loadedWorld)
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

        /*
        Load a previously saved world in a json file.
        All objects and node diagrams are spawned in the world with this method.
        */
        public static World LoadWorld(string worldName = "")
        {
            string filePath = Path.Combine(Definitions.savesPath, worldName + ".json");
            if (File.Exists(filePath))
            {
                using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {

                    using (StreamReader reader = new StreamReader(stream))
                    {

                        string jsonString = reader.ReadToEnd();
                        return JsonConvert.DeserializeObject<World>(jsonString);
                    }
                }
            }
            else
            {
                return null;
            }
        }
    }
}
