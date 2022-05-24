using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


namespace BlockEngine
{
	public class SetTexture : NodeExecution
	{
		public override void BeforeExecution()
		{
			Execute();
		}
		public override bool DynamicDropdown(ref TMP_Dropdown dropdown)
        {
			dropdown.ClearOptions();
			List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
			int cont = 0;
			foreach (Sprite mat in PreviewMaterial.icons)
            {
				options.Add(new TMP_Dropdown.OptionData(cont.ToString(), mat));
				cont++;
            }
			dropdown.options = options;
			return true;
        }

		public override void SetParams(params int[] op)
		{
			storedSelectedOptionIndex = op[0];
		}
		public override void Execute(float value, int indx = 0)
		{
			m_object.applyMaterialServerRpc(Mathf.RoundToInt(value));
		}
		public override void Execute(int indx = 0)
		{
			m_object.applyMaterialServerRpc(storedSelectedOptionIndex);
		}
	}
}
