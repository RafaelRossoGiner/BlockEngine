using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BlockEngine
{
	public class Operation : NodeExecution
	{
		protected NodeInfo.VariableType currentValueType;

		protected Vector3 bufferVec;
		protected Vector3 auxVector;
		protected Vector3 minimalVector; // as placeholder, ensure no division by zero by adding epsilon to any divisor
		
		protected float bufferFloat;
		protected Color bufferColor;

		protected Dictionary<int, bool> inputsProvided;
		protected int currExecInputs;
		public override void LoadExecution(NodeExecution refNode, InteractableObject interactable, DiagramData diagram)
		{
			BeforeExecution();
			base.LoadExecution(refNode, interactable, diagram);
		}
		public override void BeforeExecution()
		{
			inputsProvided = new Dictionary<int, bool>();
			minimalVector = new Vector3(Vector3.kEpsilon, Vector3.kEpsilon, Vector3.kEpsilon);
			SetParams(storedSelectedOptionIndex);
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
					case 0: // +
						bufferVec += vec;
						break;
					case 1: // -
						bufferVec -= vec;
						break;
					case 2: // *
						bufferVec.Scale(vec);
						break;
					case 3: // ^
						bufferVec = Vector3.Cross(bufferVec, vec);
						break;
					case 4: // /
						auxVector = vec + minimalVector;
						bufferVec.Set(bufferVec.x / auxVector.x, bufferVec.y / auxVector.y, bufferVec.z / auxVector.z);
						break;
					default:
						break;
				}
			}

			if (currExecInputs == m_inputs.Count && currentValueType == NodeInfo.VariableType.Vector3)
            {
				ExecuteNext(bufferVec);
				inputsProvided.Clear();
            }
		}
		public override void Execute(Color col, int indx = 0)
		{
			inputsProvided[indx] = true;
			currExecInputs = inputsProvided.Count;
			if (currExecInputs == 1)
			{
				currentValueType = NodeInfo.VariableType.Vector3;
				bufferColor = col;
			}
			else
			{
				switch (storedSelectedOptionIndex)
				{
					case 0: // +
						bufferColor += col;
						break;
					case 1: // -
						bufferColor -= col;
						break;
					case 2: // *
						bufferColor *= col;
						break;
					case 3: // ^
						bufferColor *= bufferColor;
						break;
					case 4: // /
						break;
					default:
						break;
				}
			}

			if (currExecInputs == m_inputs.Count && currentValueType == NodeInfo.VariableType.Vector3)
			{
				ExecuteNext(bufferVec);
				inputsProvided.Clear();
			}
		}
		public override void Execute(float value, int indx = 0)
		{
			inputsProvided[indx] = true;
			currExecInputs = inputsProvided.Count;
			if (currExecInputs == 1)
			{
				currentValueType = NodeInfo.VariableType.Expresion;
				bufferFloat = value;
			}
			else
			{
				switch (storedSelectedOptionIndex)
				{
					case 0: // +
						bufferFloat += value;
						break;
					case 1: // -
						bufferFloat -= value;
						break;
					case 2: // *
						bufferFloat *= value;
						break;
					case 3: // ^
						bufferFloat *= value;
						break;
					case 4: // /
						if (value == 0)
                        {
							bufferFloat = float.MaxValue;
						}
                        else
                        {
							bufferFloat /= value;
						}
						break;
					default:
						break;
				}
			}

			if (currExecInputs == m_inputs.Count && currentValueType == NodeInfo.VariableType.Expresion)
			{
				ExecuteNext(bufferFloat);
				inputsProvided.Clear();
			}
		}
	}
}
