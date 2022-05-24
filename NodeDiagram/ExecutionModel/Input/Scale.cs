using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BlockEngine
{
	public class Scale : NodeExecution
	{
		protected Vector3 scale;
        public override void Execute(int indx = 0)
		{
			if (m_outputs.ContainsKey(0))
            {
				scale = m_object.transform.localScale;
				ExecuteNext(scale);
			}
		}
	}
}
