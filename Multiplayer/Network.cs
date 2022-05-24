using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UNET;

namespace BlockEngine
{

     public class Network
     {

	  /*
	  Start the game as host.
	  A host is both server and client.
	  */
	  public static void startHost()
	  {

	       if (NetworkManager.Singleton.StartHost())
	       {

		    Debug.Log("Host game started on " + NetworkManager.Singleton.GetComponent<UNetTransport>().ConnectAddress + ":" + NetworkManager.Singleton.GetComponent<UNetTransport>().ConnectPort);

	       }
	       else
	       {

		    Error.instance.displayError("Couldn't start hosted game");

	       }

	  }

	  /*
	  Start the game as client.
	  A client connects to a host.
	  */
	  public static void startClient()
	  {

	       if (NetworkManager.Singleton.StartClient())
	       {
		    
		    Debug.Log("Connecting to host...");

	       }
	       else
	       {

		    Error.instance.displayError("Error while trying to start connecting to host!");

	       }

	  }

     }

}
