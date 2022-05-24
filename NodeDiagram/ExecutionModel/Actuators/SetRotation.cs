using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlockEngine
{
	public class SetRotation : NodeExecution
	{
		public override void Execute(Vector3 vector, int indx = 0)
		{
			m_object.transform.rotation = Quaternion.Euler(vector);
		}
	}
}
