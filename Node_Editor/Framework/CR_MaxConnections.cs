using UnityEngine;
using System;
using System.Linq;

namespace NodeEditorFramework {
	public class CR_MaxConnections : ConnectionRule {
		public int maxConnections=1;

		public static CR_MaxConnections CreateMaxConnections(ConnectionKnob parent){
			CR_MaxConnections crmc = CreateInstance<CR_MaxConnections> ();
			crmc.knob=parent;
			return crmc;
		}
		public static CR_MaxConnections CreateMaxConnections(ConnectionKnob parent, int maxConnections){
			CR_MaxConnections crmc = CreateMaxConnections (parent);
			crmc.maxConnections = maxConnections;
			return crmc;
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

