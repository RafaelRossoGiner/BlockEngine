using UnityEngine;
using TMPro;
using Cinemachine;
using Unity.Netcode;


namespace BlockEngine
{

    public class HUD : MonoBehaviour
    {

        public enum CameraModes { FirstPerson, ThirdPerson };

        public static HUD instance;

        // Non-static attributes.
        [SerializeField]
        private Camera mainCamera;
        [SerializeField]
        private CinemachineVirtualCameraBase FPCamera, TPCamera;
        [SerializeField]
        private LayerMask raycastTarget;
        [SerializeField]
        private LayerMask FPCullingMask;
        [SerializeField]
        private LayerMask TPCullingMask;
        [SerializeField]
        private TextMeshProUGUI playerCountText;
        [SerializeField]
        private Tools tools;
        public GameObject spawnMenu,
                          gameMenu,
                          nodeDiagram,
                          materialMenu;
        public DiagramEditor diagramEditor;

        private PlayerController attachedPlayer;
        private CameraModes cameraMode;
        private CinemachineBrain cameraBrain;

        public static bool UIopened;
        private bool typing;


        // Non-static methods.
        public void Awake()
        {

            instance = this;
            // Initialize menus.
            spawnMenu.SetActive(false);
            gameMenu.SetActive(false);

            nodeDiagram.SetActive(false);
            DiagramEditor.SetReferences(diagramEditor);

            cameraBrain = mainCamera.GetComponent<CinemachineBrain>();

        }

        public void Start()
        {

            // Ajustar la cámara a primera persona por defecto.
            cameraMode = CameraModes.FirstPerson;
            Cursor.lockState = CursorLockMode.Locked;

            // Inicializar otros atributos.
            typing = false;

        }

        public static void SetPlayer(PlayerController newPlayer, bool isHost)
        {

            instance.attachedPlayer = newPlayer;

            if (instance.attachedPlayer != null)
            {
                instance.FPCamera.Follow = instance.attachedPlayer.gameObject.transform;
                instance.TPCamera.Follow = instance.attachedPlayer.gameObject.transform;
                instance.TPCamera.LookAt = instance.attachedPlayer.gameObject.transform;
            }
            else
            {
                Debug.LogWarning("Tried to make the camera follow a null PlayerController transform");
            }

            // Should not be neccesary as the messages are sent only to the specified individual
            if (isHost)
            {
                NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler("UpdateRemoteDiagram", InteractableObject.UpdateRemoteDiagram);
            }
            else
            {
                NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler("OpenRemoteDiagram", InteractableObject.OpenRemoteDiagram);
            }

        }

        public void Update()
        {

            playerCountText.text = $"Players in game: {PlayerManager.instance.playersInGame()}";

            if (!UIopened && Input.GetKeyDown(KeyCode.C)) // Camera Mode Switch.
                SwitchCameraMode();
            else if (Input.GetKeyDown(KeyCode.E) && tools.selectedTool == Tool.SpawnTool) // Spawn menu.
                SwitchMovelUI(0);
            else if (Input.GetKeyDown(KeyCode.Escape)) // Game menu.
                SwitchMovelUI(1);
            else if (Input.GetKeyDown(KeyCode.E) && tools.selectedTool == Tool.MaterialTool) // Material menu.
                SwitchMovelUI(2);

        }

        private void SwitchCameraMode()
        {

            if (cameraMode == CameraModes.FirstPerson)
            {

                FPCamera.Priority = 0;
                TPCamera.Priority = 1;
                mainCamera.cullingMask = TPCullingMask.value;
                cameraMode = CameraModes.ThirdPerson;

            }
            else
            {

                FPCamera.Priority = 1;
                TPCamera.Priority = 0;
                mainCamera.cullingMask = FPCullingMask.value;
                cameraMode = CameraModes.FirstPerson;

            }

        }

        public void UpdateCullingMask(ICinemachineCamera cam1, ICinemachineCamera cam2)
        {

            if (cameraMode == CameraModes.FirstPerson)
            {
                mainCamera.cullingMask = TPCullingMask.value;
            }
            else
            {
                mainCamera.cullingMask = FPCullingMask.value;
            }

        }

        private void SwitchMovelUI(uint menu)
        {

            // Only switch when not typing (for example, when typing the save file name).
            switch (menu)
            {

                case 0: // Spawn menu.
                    if (!spawnMenu.activeSelf)
                    {
                        if (!typing)
                        {

                            Cursor.lockState = CursorLockMode.None;
                            Cursor.visible = true;
                            spawnMenu.SetActive(true);

                            gameMenu.SetActive(false);
                            materialMenu.SetActive(false);

                            UIopened = true;

                        }
                    }
                    else
                    {

                        Cursor.lockState = CursorLockMode.Locked;
                        Cursor.visible = false;
                        spawnMenu.SetActive(false);
                        UIopened = false;

                    }
                    break;

                case 1: // Game menu.
                    if (!gameMenu.activeSelf)
                    {

                        typing = true;
                        Cursor.lockState = CursorLockMode.None;
                        Cursor.visible = true;
                        gameMenu.SetActive(true);

                        spawnMenu.SetActive(false);
                        materialMenu.SetActive(false);

                        UIopened = true;

                    }
                    else
                    {

                        typing = false;
                        Cursor.lockState = CursorLockMode.Locked;
                        Cursor.visible = false;
                        gameMenu.SetActive(false);
                        UIopened = false;

                    }
                    break;

                case 2: // Material menu.
                    if (!materialMenu.activeSelf)
                    {

                        if (!typing)
                        {

                            Cursor.lockState = CursorLockMode.None;
                            Cursor.visible = true;
                            materialMenu.SetActive(true);

                            gameMenu.SetActive(false);
                            spawnMenu.SetActive(false);

                            UIopened = true;

                        }

                    }
                    else
                    {

                        Cursor.lockState = CursorLockMode.Locked;
                        Cursor.visible = false;
                        materialMenu.SetActive(false);
                        UIopened = false;

                    }
                    break;

            }

            if (!UIopened)
                cameraBrain.enabled = true;
            else
                cameraBrain.enabled = false;

        }
        public static void OpenDiagram(InteractableObject objectToEdit)
        {
            // Graphic Methods
            instance.nodeDiagram.SetActive(true);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            instance.cameraBrain.enabled = false;
            UIopened = true;

            // Open and draw diagram
            instance.diagramEditor.OpenDiagram(objectToEdit);
        }
        public static void OpenRemoteDiagram(DiagramData diagram, ulong clientID)
        {
            // Graphic Methods
            instance.nodeDiagram.SetActive(true);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            instance.cameraBrain.enabled = false;
            UIopened = true;

            // Open and draw diagram
            instance.diagramEditor.OpenDiagram(diagram, clientID);
        }

        public static void CloseDiagram()
        {
            // Graphic Methods
            instance.nodeDiagram.SetActive(false);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            instance.cameraBrain.enabled = true;
            UIopened = false;
        }
    }
}
