using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BlockEngine
{
	public class Oscillator : NodeExecution
	{
		private float speed;
		private float storedVal;
		public override void LoadExecution(NodeExecution refNode, InteractableObject interactable, DiagramData diagram)
		{
			SetParams(refNode.storedTextFields);
			base.LoadExecution(refNode, interactable, diagram);
		}
		public override void SetParams(params int[] op)
		{
			storedSelectedOptionIndex = op[0];
		}
		public override void SetParams(params string[] values)
		{
			storedTextFields = values;
			try
			{
				speed = float.Parse(values[0]);
				Debug.Log("Stored speed " + speed);
			}
			catch
			{
				Debug.LogWarning("Could not parse an input node");
				// Handle Exception
			}
		}
		public override void Execute(int indx = 0)
		{
			storedVal += speed * Time.deltaTime;
			ExecuteNext(Mathf.Sin(storedVal));
		}
	}
}
