using UnityEngine;
using Unity.Netcode;


namespace BlockEngine
{

    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : NetworkBehaviour
    {

        // Non-static local attributes.
        private float speed = 6;
        private Camera mainCamera;
        private CharacterController controller;
        private Vector3 movInput;
        public LayerMask raycastTarget;
        [SerializeField]
        private Tools tools;
        private InteractableObject selectedInteractableObject;


        // Non-static network attributes.
        [SerializeField]
        private NetworkVariable<Vector3> netPosDir = new NetworkVariable<Vector3>();


        // Caché del cliente.
        private Vector3 oldInputPos = Vector3.zero;
        private Vector3 inputPosition = Vector3.zero;


        // Non-static methods.
        public override void OnNetworkSpawn()
        {
            transform.position = new Vector3(0, 3, 0);
            controller = GetComponent<CharacterController>();
            if (IsOwner)
            {

                movInput = new Vector3(0, 0, 0);

                mainCamera = Camera.main;

                tools = mainCamera.GetComponent<Tools>();
                tools.Init(gameObject);

                gameObject.layer = 6;
                HUD.SetPlayer(this, IsHost);
            }
        }

        public void Update()
        {
            if (IsClient && IsOwner)
                ClientInput();

            ClientMoveAndRotate();
        }

        public void ClientInput()
        {

            // Dirección de movimiento.
            if (!HUD.UIopened)
            {
                movInput.x = Input.GetAxis("Horizontal");
                movInput.z = Input.GetAxis("Vertical");
                inputPosition = mainCamera.transform.right * movInput.x + mainCamera.transform.forward * movInput.z;
                if (oldInputPos != inputPosition)
                {
                    oldInputPos = inputPosition;
                    UpdateClientPositionServerRpc(inputPosition * speed * Time.deltaTime);
                }
            }
            else
            {
                UpdateClientPositionServerRpc(Vector3.zero);
            }

            // Selección de herramientas.
            if (!HUD.UIopened)
            {
                tools.selectToolInput();
            }

            // Ejecución de la herramienta seleccionada.
            if (Input.GetKeyDown(KeyCode.Mouse0) && !HUD.UIopened)
            {

                if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.TransformDirection(Vector3.forward), out RaycastHit hit, Mathf.Infinity, raycastTarget))
                {

                    if (tools.selectedTool == Tool.SpawnTool) // Spawn tool.
                    {
                        SpawnController.instance.SpawnObjectServerRpc(hit.point, Quaternion.identity, SpawnMenu.selectedModelID);
                        StartCoroutine(tools.shootRay(hit.point));

                    }
                    else if (tools.selectedTool == Tool.MaterialTool) // Material tool.
                    {

                        selectedInteractableObject = hit.collider.gameObject.GetComponent<InteractableObject>();
                        if (selectedInteractableObject)
                            selectedInteractableObject.applyMaterialServerRpc(MaterialTool.selectedMaterial);
                        StartCoroutine(tools.shootRay(hit.point));

                    }
                    else if (tools.selectedTool == Tool.BehaviourTool) // Behaviour tool.
                    {

                        selectedInteractableObject = hit.collider.gameObject.GetComponent<InteractableObject>();
                        if (selectedInteractableObject)
                            selectedInteractableObject.Interact();
                        StartCoroutine(tools.shootRay(hit.point));

                    }
                    else if(tools.selectedTool == Tool.DeleteTool) // Delete tool.
                    {

                        selectedInteractableObject = hit.collider.gameObject.GetComponent<InteractableObject>();
                        if (selectedInteractableObject)
                            SpawnController.instance.DespawnObjectServerRpc(selectedInteractableObject.ObjectID);
                        StartCoroutine(tools.shootRay(hit.point));

                    }

                }

            }

        }

        public void ClientMoveAndRotate()
        {

            if (netPosDir.Value != Vector3.zero)
                controller.Move(netPosDir.Value);

        }

        [ServerRpc]
        public void UpdateClientPositionServerRpc(Vector3 newPos)
        {

            // Aquí sí tenemos acceso a las variables de red.
            netPosDir.Value = newPos;
        }

    }

}