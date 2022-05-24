using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BlockEngine
{
	public class If_Else : NodeExecution
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
			inputsProvided[indx] = true;
			if (indx == 0)
			{
				bufferVal = val;
				currentValueType = NodeInfo.VariableType.Expresion;
			}
			else if (indx == 1)
			{
				currExecInputs = inputsProvided.Count;
				if (currExecInputs == m_inputs.Count)
				{
					if (val != 0)
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
					}
					else
					{
						switch (currentValueType)
						{
							case NodeInfo.VariableType.Signal:
								ExecuteNext(1);
								break;
							case NodeInfo.VariableType.Vector3:
								ExecuteNext(bufferVec, 1);
								break;
							case NodeInfo.VariableType.Expresion:
								ExecuteNext(bufferVal, 1);
								break;
							case NodeInfo.VariableType.Color:
								ExecuteNext(bufferCol, 1);
								break;
						}
					}
					inputsProvided.Clear();
				}
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
			currentValueType = NodeInfo.VariableType.Signal;
		}
	}
}
