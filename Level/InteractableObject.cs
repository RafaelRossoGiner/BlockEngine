using Newtonsoft.Json;
using System.IO;
using Unity.Netcode;
using UnityEngine;


namespace BlockEngine
{

    public class InteractableObject : NetworkBehaviour
    {

        [SerializeField]
        private LayerMask interactableLayer = 7;


        // Non-static attributes.
        public Model model;
        public DiagramData diagram;
        public NetworkObject netObject;
        private Outline m_Outline;
        private Rigidbody rigidBody;
        private MeshRenderer meshRenderer;
        private Color color;


        // Network Attributes.
        private NetworkVariable<int> netObjectID = new NetworkVariable<int>();
        private NetworkVariable<int> netMeshID = new NetworkVariable<int>();
        private NetworkVariable<int> netMaterialID = new NetworkVariable<int>();
        private NetworkVariable<bool> beingEdited = new NetworkVariable<bool>();
        private NetworkVariable<Vector3> netVelocity = new NetworkVariable<Vector3>();
        private NetworkVariable<Color> netColor = new NetworkVariable<Color>();


        // Caché.
        //private int materialID;


        // Properties.
        public int ObjectID { get { return netObjectID.Value; } }


        // Non-static methods.
        public void SetID(int id)
        {

            if (IsHost)
                netObjectID.Value = id;

        }

        public override void OnNetworkSpawn()
        {

            // Ensure this item is interactable by using the corresponding layer.
            gameObject.layer = interactableLayer;


            // Inicializar atributos.
            meshRenderer = GetComponent<MeshRenderer>();
            netObject = GetComponent<NetworkObject>();

            // Get Rigidbody Reference.
            rigidBody = gameObject.GetComponent<Rigidbody>();
            rigidBody.useGravity = true;

            if (IsHost)
            {

                netMeshID.Value = model.meshID;
                netMaterialID.Value = model.materialID;
                beingEdited.Value = false;
                netVelocity.Value = Vector3.zero;
                netColor.Value = Color.white;
            }
            else
            {

                model = new Model();

                // Guardar los datos del modelo del objeto en el diccionario local.
                if (!LevelController.loadedWorld.worldModels.ContainsKey(netObjectID.Value))
                    LevelController.loadedWorld.worldModels.Add(netObjectID.Value, model);

                model.meshID = netMeshID.Value;
                model.materialID = netMaterialID.Value;

                //Set Color
                color = netColor.Value;

                gameObject.transform.position = model.pos;
                gameObject.transform.rotation = model.rotation;
            }

            model.ApplyMesh(gameObject, model.materialID);
            model.ApplyColor(gameObject, color);
            AddOutline();

            // Ajustar hitbox.
            MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
            meshCollider.convex = true;
            meshCollider.bounds.center.Set(0, gameObject.GetComponent<MeshRenderer>().bounds.size.y / 2, 0);

        }

        void Update()
        {

            if (IsHost)
            {

                if (!beingEdited.Value && diagram != null)
                    diagram.Execute();

                // Aplicar movimiento de la gravity gun.
                if (netVelocity.Value != Vector3.zero)
                    rigidBody.velocity = netVelocity.Value;

            }

            if (IsClient)
            {

                if (model.materialID != netMaterialID.Value)
                {
                    model.materialID = netMaterialID.Value;
                    model.ApplyMaterial(gameObject, model.materialID);
                }
                if (color != netColor.Value)
                {
                    color = netColor.Value;
                    model.ApplyColor(gameObject, color);
                }
            }

        }

        public void AddOutline()
        {

            // Coger componentes.
            m_Outline = gameObject.AddComponent<Outline>();
            m_Outline.OutlineColor = new Color(1f, 0f, 0.8279877f);
            m_Outline.enabled = false;

        }

        // This method is called when the player wants to interact with this object.
        public void Interact()
        {

            if (IsHost)
                HUD.OpenDiagram(this);
            else
            {

                // Client must request diagram info.
                GetSerializedDiagramServerRpc(NetworkManager.Singleton.LocalClientId);

            }

        }

        public void SetGravity(bool isActive)
        {
            if (rigidBody != null)
            {
                rigidBody.useGravity = isActive;
            }
        }

        // Network methods and remote procedure calls.
        [ServerRpc(RequireOwnership = false)]
        public void GetSerializedDiagramServerRpc(ulong clientID)
        {

            // Reassure diagram ID is updated.
            diagram.m_objectID = netObjectID.Value;
            string jsonString = diagram.ToJson();
            FastBufferWriter writer = new FastBufferWriter(jsonString.Length * sizeof(char) * 2, Unity.Collections.Allocator.Temp);
            writer.WriteValueSafe(jsonString);
            NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage("OpenRemoteDiagram", clientID, writer, NetworkDelivery.ReliableFragmentedSequenced);
            beingEdited.Value = true;
        }

        public static void OpenRemoteDiagram(ulong clientID, FastBufferReader reader) // Should only be called on the server.
        {

            reader.ReadValueSafe(out string jsonDiagram);
            HUD.OpenRemoteDiagram(DiagramData.ImportDiagramStructure(jsonDiagram), clientID);

        }

        public static void UpdateRemoteDiagram(ulong clientID, FastBufferReader reader)
        {

            reader.ReadValueSafe(out string jsonDiagram);
            DiagramData netDiagram = JsonConvert.DeserializeObject<DiagramData>(jsonDiagram);
            InteractableObject localInteractable = SpawnController.instance.objects[netDiagram.m_objectID];
            netDiagram.IsRemote = false;
            localInteractable.diagram = netDiagram;
            localInteractable.diagram.Initialize(localInteractable);
            localInteractable.diagram.RefreshExecution();

            localInteractable.beingEdited.Value = false;
        }

        [ServerRpc(RequireOwnership = false)]
        public void updateGrabbedRBVelocityServerRpc(Vector3 targetPos)
        {
            netVelocity.Value = (targetPos - rigidBody.position) / Time.fixedDeltaTime * 0.3f / rigidBody.mass;
        }

        [ServerRpc(RequireOwnership = false)]
        public void grabServerRpc()
        {

            //rigidBody.useGravity = false;
            rigidBody.freezeRotation = true;
            rigidBody.isKinematic = false;

        }

        [ServerRpc(RequireOwnership = false)]
        public void releaseServerRpc(bool setKinematic)
        {

            //rigidBody.useGravity = true;
            rigidBody.freezeRotation = false;
            rigidBody.isKinematic = setKinematic;
            netVelocity.Value = Vector3.zero;

        }

        [ServerRpc(RequireOwnership = false)]
        public void applyMaterialServerRpc(int selectedMaterial)
        {
            if (selectedMaterial >= 0 && selectedMaterial < MaterialTool.materials.Count)
            {
                netMaterialID.Value = selectedMaterial;
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void applyColorServerRpc(Color color)
        {
            netColor.Value = color;
        }
    }
}
