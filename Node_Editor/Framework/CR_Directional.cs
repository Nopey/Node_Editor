using System;
using System.Linq;

namespace NodeEditorFramework {
	public class CR_Directional : ConnectionRule {
		public bool isInput;
		//TODO decide on default directionality, currently defaults to output
		public CR_Directional(bool isInput=false){
			this.isInput = isInput;
		}

		/// <summary>
		/// Can we connect from this to a specified node?
		/// </summary>
		/// <param name="to">NodeKnob we are connecting to.</param>
		public virtual bool CanConnect (ConnectionKnob to){
			CR_Directional dir = ((CR_Directional)to.connectionRules.Find (x => x is CR_Directional));
			//the ^ is an XOR operator, it returns true if they are different.
			return ((dir != null) && (dir.isInput ^ isInput));
		}

		/// <summary>
		/// Can we start connecting off of this?
		/// </summary>
		public virtual bool CanStartConnection (){
			//TODO Possibly return !isInput instead of true, if we don't want people drawing connections from input to output.
			return true;
		}
	}
}

