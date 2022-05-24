using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BlockEngine
{

	public class SetGravity : NodeExecution
	{
		bool signalReceived;
		public override void BeforeExecution()
		{
			signalReceived = false;
			Execute();
		}
		public override void SetParams(params int[] op)
		{
			storedSelectedOptionIndex = op[0];
			Debug.Log("Gravity changed to: " + storedSelectedOptionIndex);
		}
		public override void Execute(bool condition, int indx = 0)
        {
			signalReceived = true;
			m_object.SetGravity(condition);
        }

		public override void Execute(float condition, int indx = 0)
		{
			signalReceived = true;
			m_object.SetGravity(condition != 0);
		}
		public override void Execute(int indx = 0)
		{
			if (!signalReceived)
			{
				switch (storedSelectedOptionIndex)
				{
					case 0: // YES
						m_object.SetGravity(true);
						break;
					case 1: // NO
						m_object.SetGravity(false);
						break;
					default:
						break;
				}
			}
		}
	}
}
