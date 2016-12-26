using UnityEngine;
using System.Collections.Generic;

namespace NodeEditorFramework {

	[System.Serializable]
	public class ConnectionKnob : NodeKnob {
		/// <summary>
		/// The connected Knobs, if any.
		/// </summary>
		public List<ConnectionKnob> connections=new List<ConnectionKnob>();
		/// <summary> the first connection, or null if there are no connections.
		/// Its really just a convenience.
		/// </summary>
		public ConnectionKnob connection{ get { return ((connections == null || connections.Count == 0) ? null : connections [0]);} }

		public List<ConnectionRule> connectionRules;
		/// <summary>
		/// Returns all additional ScriptableObjects this NodeKnob holds. 
		/// That means only the actual SOURCES, simple REFERENCES will not be returned
		/// This means all SciptableObjects returned here do not have it's source elsewhere
		/// </summary>
		//public override ScriptableObject[] GetScriptableObjects () { return (ScriptableObject[])connectionRules.ToArray(); }

		/// <summary>
		/// Can we connect from this to a specified node?
		/// </summary>
		/// <param name="to">NodeKnob we are connecting to.</param>
		public virtual bool CanConnect (ConnectionKnob to){
			foreach (ConnectionRule cr in connectionRules) {
				if (!cr.CanConnect (to)) {
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Can we start connecting off of this?
		/// </summary>
		public virtual bool CanStartConnection (){
			foreach (ConnectionRule cr in connectionRules) {
				if (!cr.CanStartConnection ()) {
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Can we pluck the connection off of this knob?
		/// (Currently only returned true by a CR_MaxConn with 1 connection max and 1 connection.
		/// </summary>
		public virtual bool CanPluck(){
			if (connections.Count == 1) {
				foreach (ConnectionRule cr in connectionRules) {
					if (cr.CanPluck ()) {
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Creates a new Connection Knob and attaches it to a node
		/// </summary>
		public static ConnectionKnob Create(Node nodeBody, string inputName, NodeSide nodeSide = NodeSide.Right, float sidePosition = 20f){
			ConnectionKnob knob = CreateInstance <ConnectionKnob> ();
			knob.connectionRules = new List<ConnectionRule> ();
			knob.InitBase (nodeBody, nodeSide, sidePosition, inputName);
			nodeBody.nodeKnobs.Add (knob);
			return knob;
		}
	}

}