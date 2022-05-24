using UnityEngine;
using TMPro;
using Unity.Netcode;


public class PlayerNameOverlay : NetworkBehaviour
{
     // Non-static attribute.
     [SerializeField]
     private TextMeshProUGUI playerNameText;
     private NetworkVariable<NetworkString> playerName = new NetworkVariable<NetworkString>();

     // Non-static method.
     public void Start()
     {

	  setOverlay();

     }

     /*
     Se ejecuta cuando este objeto es instanciado.
     */
     public override void OnNetworkSpawn()
     {

        if (IsServer)
            playerName.Value = "ADMIN " + OwnerClientId;
        else
            Debug.Log("Client cannot write here!");//playerName.Value = "User " + OwnerClientId;

     }

     /*
     Para colocar el overlay del nombre del jugador. Llamarlo cuando el jugador se haya conectado.
     */
     public void setOverlay()
     {

	  playerNameText.text = playerName.Value;

     }

}
