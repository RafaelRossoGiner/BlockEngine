using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BlockEngine
{
	public class VecToVals : NodeExecution
	{
		public override void Execute(Vector3 vec, int indx = 0)
		{
			ExecuteNext(vec.x, 0);
			ExecuteNext(vec.y, 1);
			ExecuteNext(vec.z, 2);
		}
	}
}
