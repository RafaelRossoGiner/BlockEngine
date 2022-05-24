using UnityEngine;
using TMPro;


namespace BlockEngine
{

	public class MaterialCategory : MonoBehaviour
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
				for (int i = 0; i < MaterialMenu.materialMenuIcons.Count; i++)
				{

					MaterialMenu.materialMenuIcons[i].gameObject.SetActive(true);

				}

			}
			else
			{

				// Model loading in spawn menu.
				for (int i = 0; i < MaterialMenu.materialMenuIcons.Count; i++)
					MaterialMenu.materialMenuIcons[i].gameObject.SetActive(MaterialMenu.materialMenuIcons[i].categoryID == ID_);

			}

		}

	}

}