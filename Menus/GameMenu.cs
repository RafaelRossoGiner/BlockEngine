using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using TMPro;


namespace BlockEngine
{
    public class GameMenu : MonoBehaviour
    {

        // Attributes.
        public TMP_InputField saveNameInput;
        public static string lastSaveFileName;


        // Methods.
        public void saveAndExit()
        {

            lastSaveFileName = saveNameInput.text;

            LevelController.SaveWorld(lastSaveFileName);
            HUD.UIopened = false;

            exit();
        }

        public void exit()
        {
            SceneManager.LoadScene("Main menu");
        }
    }
}
