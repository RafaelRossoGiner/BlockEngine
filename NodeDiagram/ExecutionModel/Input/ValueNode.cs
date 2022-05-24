using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BlockEngine
{
	public class ValueNode : NodeExecution
	{
		private float storedVal;
		public override void LoadExecution(NodeExecution refNode, InteractableObject interactable, DiagramData diagram)
		{
			SetParams(refNode.storedTextFields);
			base.LoadExecution(refNode, interactable, diagram);
		}
		public override void SetParams(params string[] values)
		{
			storedTextFields = values;
			try
			{
				storedVal = float.Parse(values[0]);
				Debug.Log("Stored value " + storedVal);
			}
			catch
			{
				Debug.LogWarning("Could not parse an input node");
				// Handle Exception
			}
		}
        public override void Execute(int indx = 0)
        {
			ExecuteNext(storedVal);
		}
		public override void Execute(float value, int indx = 0)
		{
			storedVal = value;
			storedTextFields[0] = value.ToString();
		}
	}
}
