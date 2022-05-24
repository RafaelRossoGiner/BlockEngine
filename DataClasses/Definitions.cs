using UnityEngine;
using System.IO;


namespace BlockEngine
{
    static public class Definitions
    {

        // Static atrributes.
        static public string savesPath = Path.Combine(Application.dataPath, "Resources", "Saves");
        static public string modelsPath = Path.Combine(Application.dataPath, "Resources", "Models");
        static public string materialsPath = Path.Combine(Application.dataPath, "Resources", "Materials");


        // Static methods.
        static public void CreateDirectoryStructure()
        {

            // TODO
            if (!Directory.Exists(savesPath))
            {
                Directory.CreateDirectory(savesPath);
            }
            if (!Directory.Exists(modelsPath))
            {
                Directory.CreateDirectory(modelsPath);
            }
            if (!Directory.Exists(materialsPath))
            {
                Directory.CreateDirectory(materialsPath);
            }
        }

    }
}
