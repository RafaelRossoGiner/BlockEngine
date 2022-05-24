using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlockEngine
{
	public class SetScale : NodeExecution
	{
		public override void Execute(Vector3 vector, int indx = 0)
		{
			vector.x = Mathf.Abs(vector.x);
			vector.y = Mathf.Abs(vector.y);
			vector.z = Mathf.Abs(vector.z);
			m_object.transform.localScale = vector;
		}
	}
}
