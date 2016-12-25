using UnityEngine;
using System.Collections.Generic;

namespace NodeEditorFramework {

	public class ConnectionKnob : NodeKnob {
		public Color color = Color.white;
		/// <summary>
		/// The connected Knobs, if any.
		/// </summary>
		public List<ConnectionKnob> connections;
		/// <summary>
		/// Returns the first connection, or null if there are no connections.
		/// Its really just a convenience.
		/// </summary>
		public ConnectionKnob connection{ get { return ((connections == null || connections.Count == 0) ? null : connections [0]);} }

		//TODO determine proper visibility for this
		public List<ConnectionRule> connectionRules;
		/// <summary>
		/// Returns all additional ScriptableObjects this NodeKnob holds. 
		/// That means only the actual SOURCES, simple REFERENCES will not be returned
		/// This means all SciptableObjects returned here do not have it's source elsewhere
		/// </summary>
		public virtual ScriptableObject[] GetScriptableObjects () { return (ScriptableObject[])connectionRules.ToArray(); }

		/// <summary>
		/// Can we connect from this to a specified node?
		/// </summary>
		/// <param name="to">NodeKnob we are connecting to.</param>
		public virtual bool CanConnect (ConnectionKnob to){
			foreach (ConnectionRule km in connectionRules) {
				if (!km.CanConnect (to)) {
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Can we start connecting off of this?
		/// </summary>
		public virtual bool CanStartConnection (){
			foreach (ConnectionRule km in connectionRules) {
				if (!km.CanStartConnection ()) {
					return false;
				}
			}
			return true;
		}
	}

}