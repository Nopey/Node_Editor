using UnityEngine;

namespace NodeEditorFramework {
	/// <summary>
	/// Defines rules for when you can and cannot make a connection to this knob.
	/// </summary>
	[System.Serializable]
	public abstract class ConnectionRule {//: ScriptableObject {
		public ConnectionKnob knob;

		/// <summary>
		/// Can we connect from this to a specified node?
		/// </summary>
		/// <param name="to">NodeKnob we are connecting to.</param>
		public virtual bool CanConnect (ConnectionKnob to){
			return true;
		}

		/// <summary>
		/// Can we start connecting off of this?
		/// </summary>
		public virtual bool CanStartConnection (){
			return true;
		}
		/// <summary>
		/// Can we pluck the connection off of this node?
		/// </summary>
		public virtual bool CanPluck (){
			return false;
		}
		/// <summary>
		/// Draw whatever this rules indicators are
		/// </summary>
		public virtual void Draw(){

		}
	}
}

