using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BlockEngine
{
	public class RandomValue : NodeExecution
	{
		protected float min, max;

		public override void LoadExecution(NodeExecution refNode, InteractableObject interactable, DiagramData diagram)
		{
			BeforeExecution();
			base.LoadExecution(refNode, interactable, diagram);
			min = 0f;
			max = 10f;
			storedTextFields = new string[] { min.ToString(), max.ToString() };
		}
		public override void SetParams(params string[] values)
		{
			storedTextFields = values;
			try
			{
				min = float.Parse(values[0]);
				max = float.Parse(values[1]);
			}
			catch
			{
				Debug.LogWarning("Could not parse a random value node");
				// Handle Exception
			}
		}
		public override void Execute(float val, int indx = 0)
		{
			if (indx == 0)
            {
				min = val;
			}
            else
            {
				max = val;
            }
		}
		public override void Execute(int indx = 0)
		{
			ExecuteNext(Random.Range(min, max));
        }
	}
}
