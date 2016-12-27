using System;
using System.Linq;
using UnityEngine;

using NodeEditorFramework.Utilities;

namespace NodeEditorFramework {
	[Serializable]
	public class CR_Directional : ConnectionRule {
		public bool isInput;
		[NonSerialized]
		protected internal Texture2D directionalTexture;

		public static CR_Directional Create (ConnectionKnob parent,bool isInput=false){
			CR_Directional inst = CreateInstance<CR_Directional> ();
			inst.isInput = isInput;
			inst.knob = parent;
			return inst;
		}


		/// <summary>
		/// Can we connect from this to a specified node?
		/// </summary>
		/// <param name="to">NodeKnob we are connecting to.</param>
		public override bool CanConnect (ConnectionKnob to){
			CR_Directional dir = ((CR_Directional)to.connectionRules.Find (x => x is CR_Directional));
			//the ^ is an XOR operator, it returns true if they are different.
			return ((dir != null) && (dir.isInput ^ isInput));
		}

		/// <summary>
		/// Can we start connecting off of this?
		/// </summary>
		public override bool CanStartConnection (){
			//TODO Possibly return !isInput instead of true, if we don't want people drawing connections from input to output.
			//(that was the old behaviour)
			return true;
		}

		public override void Draw () {
			if (directionalTexture == null)
				ReloadModifiedTexture ();
			Rect knobRect = knob.GetGUIKnob ();
			//Not necessary, but its better to make sure we're colored right, white?
			GUI.color = Color.white;
			GUI.DrawTexture (knobRect, directionalTexture);
		}

		void ReloadModifiedTexture(){
			ReloadTexture ();
			if (directionalTexture == null)
				throw new UnityException ("CR_Directional couldn't load its DirectionalKnob.png!");
			if (knob.side != NodeSide.Right) 
			{ // Rotate Knob texture according to the side it's used on
				ResourceManager.SetDefaultResourcePath (NodeEditor.editorPath + "Resources/");
				int rotationSteps = NodeKnob.getRotationStepsAntiCW (NodeSide.Right, (NodeSide)((((int)knob.side)+(isInput? 1 : -1))%4+1));

				// Get standard texture in memory
				ResourceManager.MemoryTexture memoryTex = ResourceManager.FindInMemory (directionalTexture);
				if (memoryTex != null)
				{ // Texture does exist in memory, so built a mod including rotation
					string[] mods = new string[memoryTex.modifications.Length+1];
					memoryTex.modifications.CopyTo (mods, 0);
					mods[mods.Length-1] = "Rotation:" + rotationSteps;
					// Try to find the rotated version in memory
					Texture2D directionalTextureInMemory = ResourceManager.GetTexture (memoryTex.path, mods);
					if (directionalTextureInMemory != null)
					{ // Rotated version does exist
						directionalTexture = directionalTextureInMemory;
					}
					else 
					{ // Rotated version does not exist, so create and reord it
						directionalTexture = RTEditorGUI.RotateTextureCCW (directionalTexture, rotationSteps);
						ResourceManager.AddTextureToMemory (memoryTex.path, directionalTexture, mods.ToArray ());
					}
				}
				else
				{ // If it does not exist in memory, we have no path for it so we just silently rotate and use it
					// Note that this way we have to rotate it over and over again later on
					directionalTexture = RTEditorGUI.RotateTextureCCW (directionalTexture, rotationSteps);
				}
			}
		}
		void ReloadTexture(){
			//Doesn't actually load it from the disk more then once, a reference is stored in the memory.
			directionalTexture = ResourceManager.LoadTexture ("Textures/DirectionalKnob.png");
		}
	}
}

