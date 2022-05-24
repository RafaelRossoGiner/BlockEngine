using System;
using UnityEngine;
using TMPro;


public class Error : MonoBehaviour
{

     // Static attributes.
     public static Error instance = null;

     // Non-static attributes.
     [SerializeField]
     private TextMeshProUGUI errorText;
     private bool wasCursorLocked;


     // Non-static methods.
     public Error()
     {

	  if (instance != null)
	       throw new SystemException("An instance of 'Error' already exists!");
	  else
	       instance = this;

     }

     public void displayError(string message) {

	  if (Cursor.lockState != CursorLockMode.None)
	  {

	       wasCursorLocked = true;
	       Cursor.lockState = CursorLockMode.None;
	       Cursor.visible = true;

	  }
	  else
	       wasCursorLocked = false;

	  gameObject.SetActive(true);
	  errorText.text = message;

     }

     public void closeError() {

	  gameObject.SetActive(false);

	  if (wasCursorLocked) {

	       Cursor.lockState = CursorLockMode.Locked;
	       Cursor.visible = false;
	  
	  }
     
     }

}
