using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BlockEngine
{
	public class ColorValue : NodeExecution
	{
		private Color storedCol;
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
				storedCol = new Color(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]), float.Parse(values[3]));
			}
			catch
			{
				Debug.LogWarning("Could not parse a color input node");
				// Handle Exception
			}
		}
        public override void Execute(int indx = 0)
        {
			ExecuteNext(storedCol);
		}
		public override void Execute(Vector3 value, int indx = 0)
		{
			storedCol.r = value.x;
			storedCol.g = value.y;
			storedCol.b = value.z;
			storedTextFields[0] = storedCol.r.ToString();
			storedTextFields[1] = storedCol.g.ToString();
			storedTextFields[2] = storedCol.b.ToString();
		}
		public override void Execute(float value, int indx = 0)
		{
			storedCol.a = value;
			storedTextFields[3] = value.ToString();
		}
	}
}
