using System;
using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;


namespace BlockEngine
{

    public class SpawnController : NetworkBehaviour
    {

        // Static attributes.
        public static SpawnController instance = null;

        // Net variables
        private NetworkVariable<int> netSkyboxID = new NetworkVariable<int>();

        // Non-static attributes.
        [SerializeField]
        private GameObject playerPrefab;
        private int maxInstanceCount = 3; // DEBUG.

        public Dictionary<int, InteractableObject> objects;
        private Stack<int> reusableIDs;

        // Non-static methods.
        public void Awake()
        {

            if (instance)
                throw new SystemException("An instance of SpawnController already exists!");
            else
            {
                objects = new Dictionary<int, InteractableObject>();
                instance = this;
                if (IsHost)
				{
                    GetComponent<NetworkObject>().Spawn();
                }
            }
        }

        public override void OnNetworkSpawn()
        {
            if (IsHost)
            {
                SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId);
                objects = new Dictionary<int, InteractableObject>();
                reusableIDs = new Stack<int>();
                netSkyboxID.Value = LevelController.loadedWorld.skyboxID;
            }
            else
            {
                SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId);
            }
            LevelController.ApplySkybox(netSkyboxID.Value);
        }

        public void LoadObjects(ref World loadedWorld)
        {
            Dictionary<int, Model> newModels = new Dictionary<int, Model>(loadedWorld.worldModels.Count);
            Dictionary<int, DiagramData> newDiagrams = new Dictionary<int, DiagramData>(loadedWorld.worldDiagrams.Count);
            int objectID;
            int modelID;
            Vector3 pos;
            Quaternion rot;

            foreach (KeyValuePair<int, Model> modelPair in loadedWorld.worldModels)
            {
                objectID = objects.Count;
                modelID = modelPair.Value.meshID;
                pos = modelPair.Value.pos;
                rot = modelPair.Value.rotation;

                Debug.Log("Loaded object " + objectID + " | Model " + modelID + " | Pos " + pos);
                GameObject newObject = Instantiate(LevelController.instance.objectTemplate, pos, rot); // Podemos hacer esto porque el servidor es también cliente.
                Model model = new Model(); // Modelo local.

                // Datos de guardado local.
                model.pos = pos;
                model.meshID = modelID;
                model.materialID = modelPair.Value.materialID;

                // Guardar el modelo del objeto en el diccionario local.
                newModels[objectID] = model;

                // Añadir referencias al InteractableObject local para luego pasar estos datos a los otros clientes.
                InteractableObject interactable = newObject.GetComponent<InteractableObject>();
                interactable.SetID(objectID);
                interactable.model = newModels[objectID];

                // Guardar el objecto en el diccionario local.
                objects[objectID] = interactable;

                // Add diagram behaviour.
                interactable.diagram = loadedWorld.worldDiagrams[modelPair.Key];
                interactable.diagram.Initialize(interactable);
                DiagramData.ObjToDiagram[interactable] = interactable.diagram;

                // Add diagram to local dictionary.
                newDiagrams[objectID] = interactable.diagram;

                // Spawnear el objeto del lado del servidor. Este método solo puede ser llamado por el servidor.
                // Con esto se instancia el objeto en los demás clientes y a partir de aquí configuramos
                // adecuadamente los atributos del objeto instanciado en cada cliente a través del método
                // onNetworkSpawn() del componente NetObject asociado a este objeto recién creado.
                newObject.GetComponent<NetworkObject>().Spawn();
            }
            loadedWorld.worldModels = newModels;
            loadedWorld.worldDiagrams = newDiagrams;
        }
        [ServerRpc(RequireOwnership = false)]
        public void SpawnObjectServerRpc(Vector3 pos, Quaternion rot, int modelID, int objectID = -1)
        {
            if (objectID == -1)
            {
                if (reusableIDs.Count > 0)
                {
                    objectID = reusableIDs.Pop();
                }
                else
                {
                    objectID = objects.Count;
                    //objectID = LevelController.loadedWorld.worldModels.Count;
                }
            }
            Debug.Log("Spawned object " + objectID + " | Model " + modelID + " | Pos " + pos);
            GameObject newObject = Instantiate(LevelController.instance.objectTemplate, pos, rot); // Podemos hacer esto porque el servidor es también cliente.
            Model model = new Model(); // Modelo local.

            // Datos de guardado local.
            model.pos = pos;
            model.meshID = modelID;
            model.materialID = -1;

            // Guardar el modelo del objeto en el diccionario local.
            if (!LevelController.loadedWorld.worldModels.ContainsKey(objectID))
            {
                LevelController.loadedWorld.worldModels.Add(objectID, model);
            }

            // Añadir referencias al InteractableObject local para luego pasar estos datos a los otros clientes.
            InteractableObject interactable = newObject.GetComponent<InteractableObject>();
            interactable.SetID(objectID);
            interactable.model = LevelController.loadedWorld.worldModels[objectID];

            // Guardar el objecto en el diccionario local.
            objects[objectID] = interactable;

            // Add diagram behaviour.
            // If there is a diagram with the indicated ID, associate it. If there is not, create a new one with that ID.
            interactable.diagram = DiagramData.LoadOrCreateDiagram(interactable); // This requires for the object ID to be properly set.
            if (!LevelController.loadedWorld.worldDiagrams.ContainsKey(objectID))
            {
                LevelController.loadedWorld.worldDiagrams.Add(objectID, interactable.diagram);
            }

            // Spawnear el objeto del lado del servidor. Este método solo puede ser llamado por el servidor.
            // Con esto se instancia el objeto en los demás clientes y a partir de aquí configuramos
            // adecuadamente los atributos del objeto instanciado en cada cliente a través del método
            // onNetworkSpawn() del componente NetObject asociado a este objeto recién creado.
            newObject.GetComponent<NetworkObject>().Spawn();
        }

        [ServerRpc(RequireOwnership = false)]
        public void DespawnObjectServerRpc(int objectID)
        {

            objects[objectID].netObject.Despawn();
            LevelController.loadedWorld.worldModels.Remove(objectID);
            LevelController.loadedWorld.worldDiagrams.Remove(objectID);
            Destroy(objects[objectID].gameObject);
            reusableIDs.Push(objectID);

        }

        [ServerRpc(RequireOwnership = false)]
        public void SpawnPlayerServerRpc(ulong clientID)
		{
            // Crear jugador
            GameObject newObject = Instantiate(playerPrefab);
            newObject.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientID);
            Debug.Log("Spawned player");
        }

        public void SetSkybox(int skyboxID)
        {
            netSkyboxID.Value = skyboxID;
            SetSkyboxClientRPC(skyboxID);
        }

        [ClientRpc]
        public void SetSkyboxClientRPC(int skyboxID)
        {
            LevelController.ApplySkybox(skyboxID);
        }
    }
}

