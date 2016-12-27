using UnityEngine;
using System.Collections.Generic;
using System;

namespace NodeEditorFramework {
	[Serializable]
	public class ConnectionKnob : NodeKnob {
		/// <summary>
		/// The connected Knobs, if any.
		/// </summary>
		public List<ConnectionKnob> connections;

		/// <summary> the first connection, or null if there are no connections.
		/// Its really just a convenience.
		/// </summary>
		public ConnectionKnob connection{ get { return ((connections == null || connections.Count == 0) ? null : connections [0]);} }

		public List<ConnectionRule> connectionRules;
		//No longer needed as connectionrules aren't scriptableobjects anymore
		public override ScriptableObject[] GetScriptableObjects () { return (ScriptableObject[])connectionRules.ToArray(); }

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

		public override void DrawKnob (){
			base.DrawKnob();
			foreach (ConnectionRule cr in connectionRules) {
				cr.Draw ();
			}
		}

		/// <summary>
		/// Creates a new Connection Knob and attaches it to a node
		/// </summary>
		public static ConnectionKnob CreateConnectionKnob(Node nodeBody, string inputName, NodeSide nodeSide = NodeSide.Right, float sidePosition = 20f){
			ConnectionKnob knob = CreateInstance <ConnectionKnob> ();
			knob.InitBase (nodeBody, nodeSide, sidePosition, inputName);
			nodeBody.nodeKnobs.Add (knob);
			return knob;
		}

		public override void Delete () {
			try{
				NodeEditor.curNodeCanvas.connections.RemoveAll (p => (p.A == this || p.B == this));
				foreach (ConnectionKnob c in connections) {
					c.connections.RemoveAll (p => p == this);
				}
			}catch(Exception e){
				Debug.LogWarning (e.Message + e.StackTrace);
			}
			base.Delete ();
		}

		public void OnEnable(){
			if(connections==null){
				connections = new List<ConnectionKnob> ();
			}
			if(connectionRules==null){
				Debug.LogWarning ("connectionrules was null!");
				connectionRules = new List<ConnectionRule> ();
			}else{
				Debug.Log ("Connectionrules is: "+connectionRules.Count);
				foreach (ConnectionRule cr in connectionRules) {
					if (cr != null) {
						Debug.Log (cr.ToString ());
						cr.knob = this;
					}
				}
			}
		}
	}

}