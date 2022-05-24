using System;
using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UNET;
using System.Net;
using System.Linq;

namespace BlockEngine
{

    public class MainMenu : MonoBehaviour
    {

        // Non-static attributes.
        [SerializeField]
        private GameObject worldButtonTemplate;
        [SerializeField]
        private Transform worldList;
        public TMP_InputField connectIPInput;
        public TMP_InputField connectPortInput;
        public TMP_InputField hostPortInput;
        public Button connectButton;


        // Methods.

        /*
        Load everything that needs to be loaded only once during game startup here.
        */
        public void Start()
        {
            Definitions.CreateDirectoryStructure();

            // Cargar todos los materiales y categorías.
            Model.loadModels();
            MaterialTool.LoadMaterials();
            PreviewMaterial.load();

            // Cargar todas las partidas guardadas.
            DirectoryInfo savesDirectory = new DirectoryInfo(Definitions.savesPath);
            if (savesDirectory.Exists)
            {
                foreach (FileInfo file in savesDirectory.GetFiles("*.json"))
                {
                    string worldName = file.Name.Substring(0, file.Name.IndexOf('.'));
                    GameObject worldButton = Instantiate(worldButtonTemplate, worldList);
                    worldButton.GetComponentInChildren<TextMeshProUGUI>().text = worldName;
                    worldButton.GetComponent<Button>().onClick.AddListener(delegate { loadGame(worldName); });
                }

            }

        }

        /*
        Create a new empty level with a (W.I.P) world template selected.
        */
        public void newGame()
        {
            // Ejemplos de callbacks y modificación de variables de red.
            NetworkManager.Singleton.OnClientConnectedCallback += PlayerManager.instance.registerPlayer;
            NetworkManager.Singleton.OnClientDisconnectCallback += PlayerManager.instance.registerPlayerLeft;

            // Manejar aspectos del multijugador.
            NetworkManager.Singleton.GetComponent<UNetTransport>().ServerListenPort = int.Parse(hostPortInput.text);

            string localIP = Dns.GetHostEntry(Dns.GetHostName()).AddressList.First(f => f.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToString();
            NetworkManager.Singleton.GetComponent<UNetTransport>().ConnectAddress = localIP;

            Network.startHost();

            LevelController.PrepareWorld(true);

            SceneManager.LoadScene("Level");

        }

        public void loadGame(string worldName) // TODO. ADD WORLD TEMPLATE ID SUPPORT.
        {
            // Ejemplos de callbacks y modificación de variables de red.
            NetworkManager.Singleton.OnClientConnectedCallback += PlayerManager.instance.registerPlayer;
            NetworkManager.Singleton.OnClientDisconnectCallback += PlayerManager.instance.registerPlayerLeft;

            // Manejar aspectos del multijugador.
            NetworkManager.Singleton.GetComponent<UNetTransport>().ServerListenPort = int.Parse(hostPortInput.text);

            string localIP = Dns.GetHostEntry(Dns.GetHostName()).AddressList.First(f => f.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToString();
            NetworkManager.Singleton.GetComponent<UNetTransport>().ConnectAddress = localIP;

            Network.startHost();

            LevelController.PrepareWorld(true, worldName);

            SceneManager.LoadScene("Level");

        }

        /*
        Conectar con un servidor.
        */
        public void connectToGame()
        {

            try
            {

                connectButton.interactable = false;

                NetworkManager.Singleton.GetComponent<UNetTransport>().ConnectAddress = connectIPInput.text;
                NetworkManager.Singleton.GetComponent<UNetTransport>().ConnectPort = int.Parse(connectPortInput.text);

                // Ejemplos de callbacks y modificación de variables de red.
                NetworkManager.Singleton.OnClientConnectedCallback += PlayerManager.instance.registerPlayer;
                NetworkManager.Singleton.OnClientDisconnectCallback += PlayerManager.instance.registerPlayerLeft;

                // Manejar aspectos del multijugador.
                Network.startClient();

                StartCoroutine(WaitForConnection());

            }
            catch (Exception e)
            {

                connectButton.interactable = true;
                Error.instance.displayError("Error while connecting!\n" + e.Message);

            }

        }

        IEnumerator WaitForConnection()
        {

            yield return new WaitForSeconds(4);

            if (!NetworkManager.Singleton.IsConnectedClient)
            {

                Error.instance.displayError("Could not connect to host game!");
                NetworkManager.Singleton.Shutdown();
                connectButton.interactable = true;

            }
            else
            {
                Debug.LogError("Connected to host game!");
                LevelController.PrepareWorld(false);

                SceneManager.LoadScene("Level");

            }
        }

    }

}
