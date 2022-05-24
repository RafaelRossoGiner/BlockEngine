using UnityEngine;

public class Config : MonoBehaviour
{

     // Attributes. 
     public float mouseSensibility { get; set; } = 1;
     public float volume { get; set; } = 1;
     public bool allowFullscreen { get; set; } = false;
     public string width { get; set; } = "800";
     public string height { get; set; } = "600";

     // Methods
     public void applyConfig()
     {

	  PlayerPrefs.SetFloat("volume", volume);

	  Screen.fullScreen = allowFullscreen;

	  Screen.SetResolution(int.Parse(width), int.Parse(height), false);

     }

}
