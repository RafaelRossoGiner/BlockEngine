using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BlockEngine
{
	public class Comparator : NodeExecution
	{
		protected NodeInfo.VariableType currentValueType;

		protected Vector3 bufferVec;
		protected float bufferFloat;
		protected bool result;

		protected Dictionary<int, bool> inputsProvided;
		protected int maxInputs;
		protected int currExecInputs;
		public override void LoadExecution(NodeExecution refNode, InteractableObject interactable, DiagramData diagram)
		{
			BeforeExecution();
			base.LoadExecution(refNode, interactable, diagram);
		}
        protected override void AddInput(int outHandleIndx, NodeExecution prevNode, int inHanleIndx)
        {
            base.AddInput(outHandleIndx, prevNode, inHanleIndx);
			maxInputs++;
		}
		public override void BeforeExecution()
		{
			inputsProvided = new Dictionary<int, bool>();
			Debug.Log("Comparator Node has to wait for " + maxInputs + " inputs at most");
		}
		public override void SetParams(params int[] op)
        {
			storedSelectedOptionIndex = op[0];
        }
		public override void Execute(Vector3 vec, int indx = 0)
		{
			inputsProvided[indx] = true;
			currExecInputs = inputsProvided.Count;
			if (currExecInputs == 1)
            {
				currentValueType = NodeInfo.VariableType.Vector3;
				bufferVec = vec;
            }
            else
            {
				switch (storedSelectedOptionIndex)
				{
					case 0: // =
						result = bufferVec == vec;
						break;
					case 1: // !=
						result = bufferVec != vec;
						break;
					case 2: // >
						result = bufferVec.magnitude > vec.magnitude;
						break;
					case 3: // <
						result = bufferVec.magnitude < vec.magnitude;
						break;
					case 4: // >=
						result = bufferVec.magnitude >= vec.magnitude;
						break;
					case 5: // <=
						result = bufferVec.magnitude <= vec.magnitude;
						break;
					default:
						break;
				}
			}

			if (currExecInputs == maxInputs && currentValueType == NodeInfo.VariableType.Vector3)
            {
				ExecuteNext(bufferVec);
				inputsProvided.Clear();
            }
		}
		public override void Execute(float val, int indx = 0)
		{
			inputsProvided[indx] = true;
			currExecInputs = inputsProvided.Count;
			if (currExecInputs == 1)
			{
				currentValueType = NodeInfo.VariableType.Expresion;
				bufferFloat = val;
			}
			else
			{
				switch (storedSelectedOptionIndex)
				{
					case 0: // =
						result = bufferFloat == val;
						break;
					case 1: // !=
						result = bufferFloat != val;
						break;
					case 2: // >
						result = bufferFloat > val;
						break;
					case 3: // <
						result = bufferFloat < val;
						break;
					case 4: // >=
						result = bufferFloat >= val;
						break;
					case 5: // <=
						result = bufferFloat <= val;
						break;
					default:
						break;
				}
			}

			if (currExecInputs == maxInputs && currentValueType == NodeInfo.VariableType.Expresion)
			{
				if (result)
					ExecuteNext(1f);
				else
					ExecuteNext(0f);
				inputsProvided.Clear();
			}
		}
	}
}
