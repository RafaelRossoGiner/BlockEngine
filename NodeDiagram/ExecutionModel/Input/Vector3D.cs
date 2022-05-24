using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BlockEngine
{
	public class Vector3D : NodeExecution
	{
		private Vector3 storedVect;
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
				storedVect = new Vector3(float.Parse(storedTextFields[0]), float.Parse(storedTextFields[1]), float.Parse(storedTextFields[2]));
			}
			catch
			{
				Debug.LogWarning("Could not parse an input node");
				// Handle Exception
			}
        }
		public override void Execute(int indx = 0)
		{
			ExecuteNext(storedVect);
		}
	}
}
