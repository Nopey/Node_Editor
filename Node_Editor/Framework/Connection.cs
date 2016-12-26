using UnityEngine;
using System.Collections;

namespace NodeEditorFramework{
	public class Connection : ScriptableObject {
		public static Connection Set(ConnectionKnob one,ConnectionKnob two){
			Connection c = ScriptableObject.CreateInstance<Connection> ();
			if (one.GetHashCode () > two.GetHashCode ()) {
				c.A = one;
				c.B = two;
			} else {
				c.B = one;
				c.A = two;
			}
			return c;
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