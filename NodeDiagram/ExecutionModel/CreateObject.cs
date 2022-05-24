using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


namespace BlockEngine
{
	public class CreateObject : NodeExecution
	{
		Vector3 bufferPos;
		Vector3 bufferRot;
        public override void LoadExecution(NodeExecution refNode, InteractableObject interactable, DiagramData diagram)
        {
            base.LoadExecution(refNode, interactable, diagram);
			bufferPos = Vector3.zero;
			bufferRot = Vector3.zero;
        }
        public override bool DynamicDropdown(ref TMP_Dropdown dropdown)
        {
			dropdown.ClearOptions();
			List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
			int cont = 0;
			foreach (Sprite mat in PreviewModel.icons)
            {
				options.Add(new TMP_Dropdown.OptionData(cont.ToString()));
				cont++;
            }
			dropdown.options = options;
			return true;
        }

		public override void SetParams(params int[] op)
		{
			storedSelectedOptionIndex = op[0];
		}
		public override void Execute(float val, int indx = 0)
		{
			if (indx == 0)
            {
				storedSelectedOptionIndex = Mathf.RoundToInt(val);
			}
			else if (indx == 3 && val != 0)
            {
				Execute(indx);
            }
		}
		public override void Execute(Vector3 vec, int indx = 0)
		{
			if (indx == 1)
				bufferPos = vec;
			else if (indx == 2)
				bufferRot = vec;
		}
		public override void Execute(int indx = 0)
		{
			if(storedSelectedOptionIndex >= 0 && storedSelectedOptionIndex < Model.modelMesh.Count)
            {
				SpawnController.instance.SpawnObjectServerRpc(bufferPos, Quaternion.Euler(bufferRot), storedSelectedOptionIndex);
			}
		}
	}
}
