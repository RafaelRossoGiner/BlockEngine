using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlockEngine
{
	public class SetPosition : NodeExecution
	{
		public override void Execute(Vector3 vector, int indx = 0)
		{
			m_object.transform.position = vector;
		}
	}
}
