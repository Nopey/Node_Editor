using UnityEngine;
using System.Collections.Generic;

namespace NodeEditorFramework{

	[System.Serializable]
	public class CalculationKnob : ConnectionKnob {
		public bool isInput;
		//TODO Invent neater way of organizing Knobs' create functions
		public static CalculationKnob CreateCalculationKnob(Node nodeBody, string name, bool isInput=false, NodeSide nodeSide = 0, float sidePosition = 20f){
			CalculationKnob knob = CreateInstance <CalculationKnob> ();
			knob.connectionRules = new List<ConnectionRule> ();
			knob.connectionRules.Add (CR_Directional.Create (knob as ConnectionKnob, isInput));
			if (isInput) {
				knob.connectionRules.Add (CR_MaxConnections.Create (knob as ConnectionKnob));
			}
			knob.isInput = isInput;
			knob.InitBase (nodeBody, nodeSide == 0 ? (isInput ? NodeSide.Left : NodeSide.Right) : nodeSide, sidePosition, name);
			return knob;
		}
	}
}