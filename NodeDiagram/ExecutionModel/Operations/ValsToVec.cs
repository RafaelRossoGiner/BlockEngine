using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BlockEngine
{
	public class ValsToVec : NodeExecution
	{
		protected Dictionary<int, bool> inputsProvided;
		protected int currExecInputs;

		private Vector3 bufferVec;
		public override void BeforeExecution()
		{
			inputsProvided = new Dictionary<int, bool>();
			bufferVec = Vector3.zero;
		}
		public override void Execute(float val, int indx = 0)
		{
			inputsProvided[indx] = true;
			switch (indx)
			{
				case 0:
					bufferVec.x = val;
					break;
				case 1:
					bufferVec.y = val;
					break;
				case 2:
					bufferVec.z = val;
					break;
				default:
					break;
			}

			currExecInputs = inputsProvided.Count;
			if (currExecInputs == m_inputs.Count)
            {
				ExecuteNext(bufferVec);
				inputsProvided.Clear();
            }
		}
	}
}
