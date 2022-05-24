using System;
using UnityEngine;
using Unity.Netcode;


namespace BlockEngine
{

    /*
    Singleton to manage players in a multiplayer game. 
    */
    public class PlayerManager : NetworkBehaviour
    {
        // Static attributes.
        public static PlayerManager instance;

        // Hold if it is host until player instantiation
        public static bool isHost;
        public static bool alreadyInstantiated;


        // Non-static attributes.
        private NetworkVariable<int> playersInGame_ = new NetworkVariable<int>(); // Puedes asignar permisos para ver quien modifica las variables de red.


        // Non-static methods.
        public void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(this);
            }
        }

        public int playersInGame()
        {

            return playersInGame_.Value;

        }

        /*
        Método a ejecutar con un jugador entra a la partida.
        */
        public void registerPlayer(ulong ID) // Tienes que meter ese parámetro "IP" para que se pueda registrar en el callback bien.
        {

            Debug.Log(ID + " joined");

            if (IsServer) // Only returns true if the one executing this action is server.
                playersInGame_.Value++;

        }

        /*
        Método a ejecutar con un jugador abandona la partida.
        */
        public void registerPlayerLeft(ulong ID)
        {

            Debug.Log(ID + " left");

            if (IsServer) // Only returns true if the one executing this action is server.
                playersInGame_.Value--;

        }

    }

}
