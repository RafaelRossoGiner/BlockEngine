using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlockEngine
{
    public class PulseActivator : NodeExecution
    {
        float time;
        float targetTime;
        public override void LoadExecution(NodeExecution refNode, InteractableObject interactable, DiagramData diagram)
        {
            base.LoadExecution(refNode, interactable, diagram);
            time = 0;
            targetTime = -1;
        }
        public override void BeforeExecution()
        {
            targetTime = float.Parse(storedTextFields[0]);
        }
        public override void SetParams(params string[] values)
        {
            storedTextFields = values;
            try
            {
                targetTime = float.Parse(storedTextFields[0]);
                Debug.Log("Stored time value " + targetTime);
            }
            catch
            {
                Debug.LogWarning("Could not parse a Pulse node");
                // Handle Exception
            }
        }
        public override void Execute(int indx = 0)
        {
            if (targetTime > 0)
            {
                time += Time.deltaTime;
                if (time >= targetTime)
                {
                    ExecuteNext();
                    time = 0f;
                }
            }
        }
    }
}
