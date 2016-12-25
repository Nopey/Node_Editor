using System;
using System.Linq;

namespace NodeEditorFramework {
	public class CR_MaxConnections : ConnectionRule {
		public int maxConnections;

		public CR_MaxConnections(int maxConnections=1){
			this.maxConnections = maxConnections;
		}

		/// <summary>
		/// Can we connect from this to a specified node?
		/// </summary>
		/// <param name="to">NodeKnob we are connecting to.</param>
		public virtual bool CanConnect (ConnectionKnob to){
			return CanStartConnection ();
		}

		/// <summary>
		/// Can we start connecting off of this?
		/// </summary>
		public virtual bool CanStartConnection (){
			return (knob.connectionRules.Count<maxConnections);
		}
	}
}

