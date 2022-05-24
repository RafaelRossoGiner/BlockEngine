using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace BlockEngine { 
    [CreateAssetMenu(fileName = "NodeInfo", menuName = "ScriptableObjects/NodeInfo", order = 1)]
    public class NodeInfo : ScriptableObject
    {
        public enum Categories { Inputs, Global, Control, Actuators, Operations };
        public enum VariableType { Expresion, Vector2, Vector3, Color, Signal, Any};
        [SerializeField]
        private Categories category;
        [SerializeField]
        private List<VariableType> inputTypes, outputTypes;
        [SerializeField]
        private int inputFields;
        [SerializeField]
        private List<TMP_Dropdown.OptionData> dropDownOptions;
        [SerializeField]
        private NodeExecution.ExecutionTypes executionType;
        [SerializeField]
        private bool isStartNode, isFinalNode;

        private System.Type executionClass;
        private string nodeName;

        // Properties
        public Categories Category
        {
            get { return category; }
        }
        public List<VariableType> InputTypes
        {
            get { return inputTypes; }
        }
        public List<VariableType> OutputTypes
        {
            get { return outputTypes; }
        }
        public int InputFields
		{
            get { return inputFields; }
		}
        public List<TMP_Dropdown.OptionData> DropDownOptions
        {
            get { return dropDownOptions; }
        }
        public bool IsStartNode
        {
            get { return isStartNode; }
        }
        public bool IsFinalNode
        {
            get { return isFinalNode; }
        }
        public System.Type Execution
        {
            get { return executionClass; }
        }
        public string NodeName
        {
            get { return nodeName; }
        }

        public void OnEnable()
        {
            nodeName = name;
            // Gets class type based on the value selected on the inspector 
            string executionName = System.Enum.GetName(executionType.GetType(), executionType);
            executionClass = System.Type.GetType(GetType().Namespace + "." + executionName);
            if (executionClass == null)
            {
                Debug.LogWarning("The executionType " + executionName + " on asset " + name + " could not be found on the project!");
            }
        }
    }
}