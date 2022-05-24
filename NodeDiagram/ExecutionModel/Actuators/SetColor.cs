using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


namespace BlockEngine
{

	public class SetColor : NodeExecution
	{
		public override void Execute(Color color, int indx = 0)
		{
			m_object.applyColorServerRpc(color);
		}
	}
}
