using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BlockEngine
{
	public class Position : NodeExecution
	{
		protected Vector3 position;
        public override void Execute(int indx = 0)
		{
			if (m_outputs.ContainsKey(0))
            {
				if (true || position != m_object.transform.position)
                {
					position = m_object.transform.position;
					ExecuteNext(position);
				}
			}
		}
	}
}
