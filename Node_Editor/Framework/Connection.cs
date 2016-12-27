using UnityEngine;
using System;

namespace NodeEditorFramework{
	[Serializable]
	public class Connection{
		public Connection(ConnectionKnob one,ConnectionKnob two){
			if (one.GetHashCode () > two.GetHashCode ()) {
				this.A = one;
				this.B = two;
			} else {
				this.B = one;
				this.A = two;
			}
		}

		public override System.Int32 GetHashCode () {
			return A.GetHashCode ()+B.GetHashCode();
		}

		public override bool Equals (object other) {
			if (other is Connection) {
				return (other as Connection).A == A && (other as Connection).B == B;
			} else {
				return false;
			}
		}
		public ConnectionKnob A;
		public ConnectionKnob B;
	}
}