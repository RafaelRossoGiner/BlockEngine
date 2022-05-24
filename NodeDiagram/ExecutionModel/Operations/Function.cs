using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BlockEngine
{
	public class Function : NodeExecution
	{
		protected float bufferVal;

		public override void SetParams(params int[] op)
        {
			storedSelectedOptionIndex = op[0];
        }
		public override void Execute(Vector3 vec, int indx = 0)
		{

		}
		public override void Execute(float value, int indx = 0)
		{
			switch (storedSelectedOptionIndex)
			{
				case 0: // sin
					bufferVal = Mathf.Sin(value);
					break;
				case 1: // cos
					bufferVal = Mathf.Cos(value);
					break;
				case 2: // tan
					bufferVal = Mathf.Tan(value);
					break;
				case 3: // abs
					bufferVal = Mathf.Abs(value);
					break;
				case 4: // sign
					bufferVal = Mathf.Sign(value);
					break;
				default:
					break;
			}
			ExecuteNext(bufferVal);
		}
	}
}
