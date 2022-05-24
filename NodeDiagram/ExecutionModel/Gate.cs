using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BlockEngine
{
	public class Gate : NodeExecution
	{
		protected NodeInfo.VariableType currentValueType;
		protected Dictionary<int, bool> inputsProvided;
		protected int currExecInputs;

		private float bufferVal;
		private Vector3 bufferVec;
		private Color bufferCol;
		public override void LoadExecution(NodeExecution refNode, InteractableObject interactable, DiagramData diagram)
		{
			BeforeExecution();
			base.LoadExecution(refNode, interactable, diagram);
		}
		public override void BeforeExecution()
		{
			inputsProvided = new Dictionary<int, bool>();
		}
		public override void Execute(float val, int indx = 0)
		{
			if (indx == 0)
			{
				inputsProvided[indx] = true;
				bufferVal = val;
				currentValueType = NodeInfo.VariableType.Expresion;
			}
			else if (indx == 1 && val != 0) 
			{
				Debug.Log(val);
				Execute(indx);
			}
		}
		public override void Execute(Vector3 vec, int indx = 0)
		{
			inputsProvided[indx] = true;
			bufferVec = vec;
			currentValueType = NodeInfo.VariableType.Vector3;
		}
		public override void Execute(Color col, int indx = 0)
		{
			inputsProvided[indx] = true;
			bufferCol = col;
			currentValueType = NodeInfo.VariableType.Color;
		}
		public override void Execute(int indx = 0)
		{
			inputsProvided[indx] = true;
			if (indx == 0)
            {
				currentValueType = NodeInfo.VariableType.Signal;
			}
			else if(indx == 1)
            {
				currExecInputs = inputsProvided.Count;
				if (currExecInputs == m_inputs.Count)
				{
					switch (currentValueType)
					{
						case NodeInfo.VariableType.Signal:
							ExecuteNext();
							break;
						case NodeInfo.VariableType.Vector3:
							ExecuteNext(bufferVec);
							break;
						case NodeInfo.VariableType.Expresion:
							ExecuteNext(bufferVal);
							break;
						case NodeInfo.VariableType.Color:
							ExecuteNext(bufferCol);
							break;
					}
					inputsProvided.Clear();
				}
			}
        }
	}
}
