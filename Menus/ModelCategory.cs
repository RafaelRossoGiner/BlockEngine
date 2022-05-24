using UnityEngine;
using TMPro;

namespace BlockEngine { 

     public class ModelCategory : MonoBehaviour
     {

	  [SerializeField]
	  private TextMeshProUGUI name_;
	  private int ID_;

	  public void setText(string name)
	  {

	       name_.text = name;

	  }

	  public void setID(int ID)
	  {

	       ID_ = ID;

	  }

	  public void onClick()
	  {

	       if (ID_ == 0) 
	       {

		    // Model loading in spawn menu.
		    for (int i = 0; i < SpawnMenu.spawnMenuIcons.Count; i++)
		    {

			 SpawnMenu.spawnMenuIcons[i].gameObject.SetActive(true);

		    }

	       }
	       else
	       {

		    // Model loading in spawn menu.
		    for (int i = 0; i < SpawnMenu.spawnMenuIcons.Count; i++)
			 SpawnMenu.spawnMenuIcons[i].gameObject.SetActive(SpawnMenu.spawnMenuIcons[i].categoryID == ID_);

	       }

	  }

     }

}