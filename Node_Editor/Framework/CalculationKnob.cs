using UnityEngine;
using System.Collections.Generic;

namespace NodeEditorFramework{
	public class CalculationKnob : ConnectionKnob {
		public bool isInput;
		//TODO set up chain of overrides for the Knobs' create functions
		public static CalculationKnob CreateCalculationKnob(Node nodeBody, string name, bool isInput=false, NodeSide nodeSide = 0, float sidePosition = 20f){
			CalculationKnob knob = CreateInstance <CalculationKnob> ();
			knob.connectionRules = new List<ConnectionRule> ();
			knob.connectionRules.Add (CR_Directional.CreateDirectional (knob as ConnectionKnob, isInput));
			if (isInput) {
				knob.connectionRules.Add (CR_MaxConnections.CreateMaxConnections (knob as ConnectionKnob));
			}
			knob.isInput = isInput;
			knob.InitBase (nodeBody, nodeSide == 0 ? (isInput ? NodeSide.Left : NodeSide.Right) : nodeSide, sidePosition, name);
			return knob;
		}
	}
}