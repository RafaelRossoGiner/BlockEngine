using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace BlockEngine
{
	public class NodeExecution
	{
		public enum ExecutionTypes { Position, Rotation, Scale, SetPosition, SetRotation, SetScale, If_Else, Vector3D, Vector2D, Operation, ValueNode, SetGravity, Oscillator, SetTexture, Function, SetGravityValue, SetSkybox, ColorValue, SetColor, PulseActivator, Gate, Comparator, RandomValue, ValsToVec, VecToVals, CreateObject }
		// ===================== [ Serializable Data ] ===========================
		public int nodeID;
		public Vector2 nodePosition;
		public Dictionary<int, List<(int, int)>> connections = new Dictionary<int, List<(int, int)>>();
		// Connections structure: Dict ExitIndex -> List (NodeID , inputIndex)
		// Inputs structure: Dict InputIndex -> List (NodeID , outputIndex)
		public string[] storedTextFields;
		public int storedSelectedOptionIndex;
		public string nodeType;
		// ===================== [ Runtime Data ] ================================
		protected InteractableObject m_object;
		protected Dictionary<int, Dictionary<NodeExecution, List<int>>> m_outputs = new Dictionary<int, Dictionary<NodeExecution, List<int>>>();
		protected bool alreadyExecuted;
		// ===================== [ Backward Runtime Data ] ================================
		protected Dictionary<int, Dictionary<NodeExecution, List<int>>> m_inputs = new Dictionary<int, Dictionary<NodeExecution, List<int>>>();
		// Properties
		// =================== [ Initialization Methods ] ==========================
		public void InitializeNewNode(int newID, Vector2 initialPos, NodeInfo nodeInfo, InteractableObject newInteractable = null)
        {
			// New Node Creation, Not called when loading node from file
			nodeID = newID;
			nodePosition = initialPos;
			nodeType = nodeInfo.name;

			storedTextFields = new string[nodeInfo.InputFields];
			storedSelectedOptionIndex = 0;

			m_object = newInteractable;

			for (int i = 0; i < nodeInfo.InputTypes.Count; i++)
			{
				m_inputs[i] = new Dictionary<NodeExecution, List<int>>();
			}

			for (int i = 0; i < nodeInfo.OutputTypes.Count; i++)
            {
				m_outputs[i] = new Dictionary<NodeExecution, List<int>>();
            }
		}
		public virtual void LoadExecution(NodeExecution refNode, InteractableObject interactable, DiagramData diagram)
		{
			// May be called in the node creation or node de-serialization

			refNode.m_object = interactable;
			CopyValues(refNode);
			LoadConnections(diagram);

			// In case there are any parameters to indicate
			SetParams(refNode.storedTextFields);
			SetParams(refNode.storedSelectedOptionIndex);
		}
		public virtual void CopyValues(NodeExecution refNode)
		{
			// Copy node
			nodeID = refNode.nodeID;
			nodePosition = refNode.nodePosition;
			connections = refNode.connections;
			nodeType = refNode.nodeType;

			m_object = refNode.m_object;

			// Optional node info
			storedTextFields = refNode.storedTextFields;
			storedSelectedOptionIndex = refNode.storedSelectedOptionIndex;

			// Call parameter assigmnent for the optional dropdown option
			SetParams(storedSelectedOptionIndex);
		}
		public void LoadConnections(DiagramData diagram)
		{
			// Read connections serializable dictionaries and instantiate the corresponding reference dictionaries
			alreadyExecuted = false;
			NodeExecution nextNode;

			int cont = 0;
            foreach (NodeInfo.VariableType output in DiagramData.nodeInfoInstances[nodeType].OutputTypes)
			{
				m_outputs[cont++] = new Dictionary<NodeExecution, List<int>>();
            }

			// Initialize Connection References
			foreach (KeyValuePair<int, List<(int, int)>> connectionList in connections)
			{
				if (connectionList.Value.Count > 0)
				{
					m_outputs[connectionList.Key] = new Dictionary<NodeExecution, List<int>>();
				}
				foreach ((int, int) nodeId in connectionList.Value)
				{
					nextNode = diagram.nodes[nodeId.Item1];
					if (m_outputs[connectionList.Key].TryGetValue(diagram.nodes[nodeId.Item1], out List<int> connection))
                    {
						connection.Add(nodeId.Item2);
					}
                    else
                    {
						m_outputs[connectionList.Key][diagram.nodes[nodeId.Item1]] = new List<int>() { nodeId.Item2 };
					}
					nextNode.AddInput(connectionList.Key, this, nodeId.Item2);
				}
			}
		}
		// =================== [ Node Configuration Methods ] ==========================
		// If the method returns false, the static dropdown assigned on the scriptable object is used for the node.
		// Return true if the node dropdown should not be overwritten with the values assigned on the scriptable object.
		public virtual bool DynamicDropdown(ref TMP_Dropdown dropdown) { return false; }
		// =================== [ Execution Methods ] ==========================
		// Executes when the diagram is loaded for the first time (world load) or the diagram is refreshed (editor closed)
		public virtual void BeforeExecution() { }
		public virtual void SetParams(params int[] values) { }
		public virtual void SetParams(params string[] values) { }
		public virtual void SetParams(params float[] values) { }
		public virtual void SetParams(params Vector3[] values) { }
		// All Execute methods are called by the previous nodes with the corresponding values,
		// except for the nodes marked as starting nodes which are called by the diagram itself
		// Signal Executions
		public virtual void Execute(int indx = 0) { }
		// Data Executions
		public virtual void Execute(Vector2 vector, int indx = 0) { }
		public virtual void Execute(Vector3 vector, int indx = 0) { }
		public virtual void Execute(float expresion, int indx = 0) { }
		public virtual void Execute(bool condition, int indx = 0) { }
		public virtual void Execute(Color color, int indx = 0) { }

		// Propagate Execution
		public virtual void ExecuteNext(int index = 0)
        {
            foreach (KeyValuePair<NodeExecution, List<int>> nextNode in m_outputs[index])
			{
				foreach (int inputHanlde in nextNode.Value)
                {
					nextNode.Key.Execute(inputHanlde);
				}
			}
		}

		public virtual void ExecuteNext(Vector2 value, int index = 0)
		{
			foreach (KeyValuePair<NodeExecution, List<int>> nextNode in m_outputs[index])
			{
				foreach (int inputHanlde in nextNode.Value)
				{
					nextNode.Key.Execute(value, inputHanlde);
				}
			}
		}

		public virtual void ExecuteNext(Vector3 value, int index = 0)
		{
			foreach (KeyValuePair<NodeExecution, List<int>> nextNode in m_outputs[index])
			{
				foreach (int inputHanlde in nextNode.Value)
				{
					nextNode.Key.Execute(value, inputHanlde);
				}
			}
		}

		public virtual void ExecuteNext(float value, int index = 0)
		{
			foreach (KeyValuePair<NodeExecution, List<int>> nextNode in m_outputs[index])
			{
				foreach (int inputHanlde in nextNode.Value)
				{
					nextNode.Key.Execute(value, inputHanlde);
				}
			}
		}

		public virtual void ExecuteNext(bool value, int index = 0)
		{
			foreach (KeyValuePair<NodeExecution, List<int>> nextNode in m_outputs[index])
			{
				foreach (int inputHanlde in nextNode.Value)
				{
					nextNode.Key.Execute(value, inputHanlde);
				}
			}
		}

		public virtual void ExecuteNext(Color value, int index = 0)
		{
			foreach (KeyValuePair<NodeExecution, List<int>> nextNode in m_outputs[index])
			{
				foreach (int inputHanlde in nextNode.Value)
				{
					nextNode.Key.Execute(value, inputHanlde);
				}
			}
		}

		// Backward Executions
		public virtual void GetSig(ref bool isDenied, int indx = 0) { }
		public virtual Vector2 GetVec2(ref bool isDenied, int indx = 0) { return Vector2.zero; }
		public virtual Vector3 GetVec3(ref bool isDenied, int indx = 0) { return Vector3.zero; }
		public virtual float GetFloat(ref bool isDenied, int indx = 0) { return 0f; }
		public virtual bool GetBool(ref bool isDenied, int indx = 0) { return false; }
		public virtual Color GetCol(ref bool isDenied, int indx = 0) { return Color.white; }

		// Linking Methods
		public virtual void AddOutput(int outHandleIndx, NodeExecution nextNode, int inHandleIndx)
		{
			List<int> outputHandleList;
			Debug.Log("Added: " + GetType().Name + "(" + outHandleIndx + ")-> " + nextNode.GetType().Name + " on input " + inHandleIndx);

			if (m_outputs.TryGetValue(outHandleIndx, out Dictionary<NodeExecution, List<int>> outputNodesList))
            {
				if (outputNodesList.TryGetValue(nextNode, out outputHandleList))
				{
					outputHandleList.Add(inHandleIndx);
					connections[outHandleIndx].Add((nextNode.nodeID, inHandleIndx));
				}
				else
				{
					outputNodesList[nextNode] = new List<int>() { inHandleIndx };
					connections[outHandleIndx] = new List<(int, int)>() { (nextNode.nodeID, inHandleIndx) };
				}
            }
            else
            {
				outputHandleList = new List<int>() { inHandleIndx };
				m_outputs[outHandleIndx] = new Dictionary<NodeExecution, List<int>>();
				m_outputs[outHandleIndx][nextNode] = outputHandleList;
				connections[outHandleIndx] = new List<(int, int)>() { (nextNode.nodeID, inHandleIndx) };
			}
			nextNode.AddInput(outHandleIndx, this, inHandleIndx);
		}
		public virtual void RemoveOutput(int outHandleIndx, NodeExecution nextNode, int inHandleIndx)
		{
			Debug.Log("Removed: " + GetType().Name + "(" + outHandleIndx + ")-> " + nextNode.GetType().Name + " on input " + inHandleIndx);

			m_outputs[outHandleIndx][nextNode].Remove(inHandleIndx);
			connections[outHandleIndx].Remove((nextNode.nodeID, inHandleIndx));

			nextNode.m_inputs[inHandleIndx][this].Remove(outHandleIndx);
		}
		public virtual void RemoveRemoteNodeOutput(int outHandleIndx, NodeExecution nextNode, int inHandleIndx)
		{
			Debug.Log("Removed: " + GetType().Name + "(" + outHandleIndx + ")-> " + nextNode.GetType().Name + " on input " + inHandleIndx);
			connections[outHandleIndx].Remove((nextNode.nodeID, inHandleIndx));
			nextNode.m_inputs[inHandleIndx][this].Remove(outHandleIndx);
		}
		protected virtual void AddInput(int outHandleIndx, NodeExecution prevNode, int inHandleIndx)
		{
			List<int> inputHandleList;
			if (m_inputs.TryGetValue(inHandleIndx, out Dictionary<NodeExecution, List<int>> inputNodesList))
            {
				if (inputNodesList.TryGetValue(prevNode, out inputHandleList))
				{
					inputHandleList.Add(outHandleIndx);
				}
				else
				{
					inputNodesList[prevNode] = new List<int>() { outHandleIndx };
				}
            }
            else
            {
				inputHandleList = new List<int>() { outHandleIndx };
				m_inputs[inHandleIndx] = new Dictionary<NodeExecution, List<int>>();
				m_inputs[inHandleIndx][prevNode] = inputHandleList;
			}
		}
		
		public virtual void RemovePredecentNodesConnections()
		{
			foreach (KeyValuePair<int, Dictionary<NodeExecution, List<int>>> inputList in m_inputs)
			{
				foreach (KeyValuePair<NodeExecution, List<int>> inputNodes in inputList.Value)
				{
					foreach (int outputHandle in inputNodes.Value)
					{
						inputNodes.Key.m_outputs[outputHandle][this].Remove(inputList.Key);
						inputNodes.Key.connections[outputHandle].Remove((nodeID, inputList.Key));
					}
				}
			}
		}
		public virtual void RemovePredecentRemoteNodeConnections()
		{
			Debug.Log("Called remove conn");
			foreach (KeyValuePair<int, Dictionary<NodeExecution, List<int>>> inputList in m_inputs)
			{
				foreach (KeyValuePair<NodeExecution, List<int>> inputNodes in inputList.Value)
				{
					foreach (int outputHandle in inputNodes.Value)
					{
						Debug.Log("Removed connection " + outputHandle + " to " + nodeType + " on " + inputList.Key);
						inputNodes.Key.connections[outputHandle].Remove((nodeID, inputList.Key));
					}
				}
			}
		}
	}
}