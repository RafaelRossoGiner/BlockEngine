using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BlockEngine
{
	public class Rotation : NodeExecution
	{
		protected Vector3 rotation;
        public override void Execute(int indx = 0)
		{
			if (m_outputs.ContainsKey(0))
            {
				rotation = m_object.transform.rotation.eulerAngles;
				ExecuteNext(rotation);
			}
		}
	}
}
