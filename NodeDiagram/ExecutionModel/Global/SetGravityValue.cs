using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlockEngine
{
	public class SetGravityValue : NodeExecution
	{
		public override void Execute(Vector3 vector, int indx = 0)
		{
			Physics.gravity = vector;
		}

		public override void Execute(float vector, int indx = 0)
		{
			Physics.gravity = new Vector3(0, vector, 0);
		}
	}
}
