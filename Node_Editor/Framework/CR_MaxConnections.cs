using UnityEngine;
using System;
using System.Linq;

namespace NodeEditorFramework {
	[Serializable]
	public class CR_MaxConnections : ConnectionRule {
		public int maxConnections;

		public static CR_MaxConnections Create(ConnectionKnob parent, int maxConnections=1){
			CR_MaxConnections inst = CreateInstance<CR_MaxConnections> ();
			inst.knob=parent;
			inst.maxConnections = maxConnections;
			return inst;
		}
		/// <summary>
		/// Can we connect from this to a specified node?
		/// </summary>
		/// <param name="to">NodeKnob we are connecting to.</param>
		public override bool CanConnect (ConnectionKnob to){
			return CanStartConnection ();
		}

		/// <summary>
		/// Can we start connecting off of this?
		/// </summary>
		public override bool CanStartConnection (){
			return (knob.connections.Count<maxConnections);
		}

		public override bool CanPluck () {
			//Don't have to check the actual number of connections as thats already checked.
			return (maxConnections == 1);
		}
	}
}

