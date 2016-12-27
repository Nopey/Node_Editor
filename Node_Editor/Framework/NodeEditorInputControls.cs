using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using NodeEditorFramework;
using NodeEditorFramework.Utilities;

namespace NodeEditorFramework 
{
	/// <summary>
	/// Collection of default Node Editor controls for the NodeEditorInputSystem
	/// </summary>
	public static class NodeEditorInputControls
	{
		#region Canvas Context Entries

		[ContextFillerAttribute (ContextType.Canvas)]
		private static void FillAddNodes (NodeEditorInputInfo inputInfo, GenericMenu canvasContextMenu) 
		{ // Show all nodes, and if a connection is drawn, only compatible nodes to auto-connect
			NodeEditorState state = inputInfo.editorState;
			List<Node> displayedNodes = state.partialConnection != null? NodeTypes.getCompatibleNodes (state.partialConnection) : NodeTypes.nodes.Keys.ToList ();
			foreach (Node compatibleNode in displayedNodes)
			{
				if (NodeCanvasManager.CheckCanvasCompability (compatibleNode, inputInfo.editorState.canvas.GetType ()))
					canvasContextMenu.AddItem (new GUIContent ("Add " + NodeTypes.nodes[compatibleNode].address), false, CreateNodeCallback, new NodeEditorInputInfo (compatibleNode.GetID, state));
			}
		}

		private static void CreateNodeCallback (object infoObj)
		{
			NodeEditorInputInfo callback = infoObj as NodeEditorInputInfo;
			if (callback == null)
				throw new UnityException ("Callback Object passed by context is not of type NodeEditorInputInfo!");

			callback.SetAsCurrentEnvironment ();
			//TODO Reimplement Connection creation
			try{
			Node.Create (callback.message, NodeEditor.ScreenToCanvasSpace (callback.inputPos), callback.editorState.partialConnection);
			}catch(Exception e){
				Debug.LogWarning (e.Message + " and stacktrace "+e.StackTrace);
			}
			callback.editorState.partialConnection = null;
			NodeEditor.RepaintClients ();
		}

		#endregion

		#region Node Context Entries

		[ContextEntryAttribute (ContextType.Node, "Delete Node")]
		private static void DeleteNode (NodeEditorInputInfo inputInfo) 
		{
			inputInfo.SetAsCurrentEnvironment ();
			if (inputInfo.editorState.focusedNode != null) 
			{
				inputInfo.editorState.focusedNode.Delete ();
				inputInfo.inputEvent.Use ();
			}
		}

		[ContextEntryAttribute (ContextType.Node, "Duplicate Node")]
		private static void DuplicateNode (NodeEditorInputInfo inputInfo) 
		{
			//TODO Reimplement duplication
			inputInfo.SetAsCurrentEnvironment ();
			NodeEditorState state = inputInfo.editorState;
			if (state.focusedNode != null) 
			{ // Create new node of same type
				Node duplicatedNode = Node.Create (state.focusedNode.GetID, NodeEditor.ScreenToCanvasSpace (inputInfo.inputPos), state.partialConnection);
				state.selectedNode = state.focusedNode = duplicatedNode;
				state.partialConnection = null;
				inputInfo.inputEvent.Use ();
			}
		}

		#endregion

		#region Node Keyboard Control

		// Main Keyboard_Move method
		[HotkeyAttribute(KeyCode.UpArrow, EventType.KeyDown)]
		[HotkeyAttribute(KeyCode.LeftArrow, EventType.KeyDown)]
		[HotkeyAttribute(KeyCode.RightArrow, EventType.KeyDown)]
		[HotkeyAttribute(KeyCode.DownArrow, EventType.KeyDown)]
		private static void KB_MoveNode(NodeEditorInputInfo inputInfo)
		{
			if (GUIUtility.keyboardControl > 0)
				return;
			NodeEditorState state = inputInfo.editorState;
			if (state.selectedNode != null)
			{ 
				Vector2 pos = state.selectedNode.rect.position;
				int shiftAmount = inputInfo.inputEvent.shift? 50 : 10;

				if (inputInfo.inputEvent.keyCode == KeyCode.RightArrow)
					pos = new Vector2(pos.x + shiftAmount, pos.y);
				else if (inputInfo.inputEvent.keyCode == KeyCode.LeftArrow)
					pos = new Vector2(pos.x - shiftAmount, pos.y);
				else if (inputInfo.inputEvent.keyCode == KeyCode.DownArrow)
					pos = new Vector2(pos.x, pos.y + shiftAmount);
				else if (inputInfo.inputEvent.keyCode == KeyCode.UpArrow)
					pos = new Vector2(pos.x, pos.y - shiftAmount);

				state.selectedNode.rect.position = pos;
				inputInfo.inputEvent.Use();
			}
			NodeEditor.RepaintClients();

		}


		#endregion

		#region Node Dragging

		[EventHandlerAttribute (EventType.MouseDown, 110)] // Priority over hundred to make it call after the GUI
		private static void HandleNodeDraggingStart (NodeEditorInputInfo inputInfo) 
		{
			if (GUIUtility.hotControl > 0)
				return; // GUI has control

			NodeEditorState state = inputInfo.editorState;
			if (inputInfo.inputEvent.button == 0 && state.focusedNode != null && state.focusedNode == state.selectedNode && state.focusedNodeKnob == null) 
			{ // Clicked inside the selected Node, so start dragging it
				state.dragNode = true;
				state.dragStart = inputInfo.inputPos;
				state.dragPos = state.focusedNode.rect.position; // Need this here because of snapping
				state.dragOffset = Vector2.zero;
				inputInfo.inputEvent.delta = Vector2.zero;
			}
		}

		[EventHandlerAttribute (EventType.MouseDrag)]
		private static void HandleNodeDragging (NodeEditorInputInfo inputInfo) 
		{
			NodeEditorState state = inputInfo.editorState;
			if (state.dragNode) 
			{ // If conditions apply, drag the selected node, else disable dragging
				if (state.selectedNode != null && GUIUtility.hotControl == 0)
				{ // Calculate new position for the dragged object
					state.dragOffset = inputInfo.inputPos-state.dragStart;
					state.selectedNode.rect.position = state.dragPos + state.dragOffset*state.zoom;
					NodeEditorCallbacks.IssueOnMoveNode (state.selectedNode);
					NodeEditor.RepaintClients ();
				} 
				else
					state.dragNode = false;
			}
		}

		[EventHandlerAttribute (EventType.MouseDown)]
		[EventHandlerAttribute (EventType.MouseUp)]
		private static void HandleNodeDraggingEnd (NodeEditorInputInfo inputInfo) 
		{
			inputInfo.editorState.dragNode = false;
		}

		#endregion

		#region Window Panning

		[EventHandlerAttribute (EventType.MouseDown, 100)] // Priority over hundred to make it call after the GUI
		private static void HandleWindowPanningStart (NodeEditorInputInfo inputInfo) 
		{
			if (GUIUtility.hotControl > 0)
				return; // GUI has control

			NodeEditorState state = inputInfo.editorState;
			if ((inputInfo.inputEvent.button == 0 || inputInfo.inputEvent.button == 2) && state.focusedNode == null) 
			{ // Left- or Middle clicked on the empty canvas -> Start panning
				state.panWindow = true;
				state.dragStart = inputInfo.inputPos;
				state.dragOffset = Vector2.zero;
			}
		}

		[EventHandlerAttribute (EventType.MouseDrag)]
		private static void HandleWindowPanning (NodeEditorInputInfo inputInfo) 
		{
			NodeEditorState state = inputInfo.editorState;
			if (state.panWindow) 
			{ // Calculate change in panOffset
				Vector2 panOffsetChange = state.dragOffset;
				state.dragOffset = inputInfo.inputPos - state.dragStart;
				panOffsetChange = (state.dragOffset - panOffsetChange) * state.zoom;
				// Apply panOffsetChange to panOffset
				state.panOffset += panOffsetChange;
				NodeEditor.RepaintClients ();
			}
		}

		[EventHandlerAttribute (EventType.MouseDown)]
		[EventHandlerAttribute (EventType.MouseUp)]
		private static void HandleWindowPanningEnd (NodeEditorInputInfo inputInfo) 
		{
			inputInfo.editorState.panWindow = false;
		}

		#endregion

		#region Connection

		[EventHandlerAttribute (EventType.MouseDown)]
		private static void HandleConnectionDrawing (NodeEditorInputInfo inputInfo) 
		{
			NodeEditorState state = inputInfo.editorState;
			if (inputInfo.inputEvent.button == 0 && state.focusedNodeKnob != null)
			{ // Left-Clicked on a NodeKnob, so check if any of them is a nodeInput or -Output
				if (state.focusedNodeKnob is ConnectionKnob)
				{ 
					if ((state.focusedNodeKnob as ConnectionKnob).CanPluck ()) {
						//TODO delete the Connection here too
						state.partialConnection = ((ConnectionKnob)state.focusedNodeKnob).connection;
						int hash = ((ConnectionKnob)state.focusedNodeKnob).connection.GetHashCode () + state.focusedNodeKnob.GetHashCode ();
						NodeEditor.curNodeCanvas.connections.RemoveAll(p=>p.GetHashCode()==hash);
						((ConnectionKnob)state.focusedNodeKnob).Delete ();
						inputInfo.inputEvent.Use ();
					} else {
						// Output clicked -> Draw new connection from it
						if ((state.focusedNodeKnob as ConnectionKnob).CanStartConnection ()) {
							state.partialConnection = (ConnectionKnob)state.focusedNodeKnob;
							inputInfo.inputEvent.Use ();
						}
					}
				}
			}
		}

		[EventHandlerAttribute (EventType.MouseUp)]
		private static void HandleApplyConnection (NodeEditorInputInfo inputInfo) 
		{
			NodeEditorState state = inputInfo.editorState;
			if (inputInfo.inputEvent.button == 0
			    && state.partialConnection != null
			    && state.focusedNode != null
			    && state.focusedNodeKnob != null
			    && state.focusedNodeKnob is ConnectionKnob
			    && ((ConnectionKnob)state.focusedNodeKnob).CanConnect (state.partialConnection)
			    && state.partialConnection.CanConnect ((ConnectionKnob)state.focusedNodeKnob)) { // An input was clicked, it'll will now be connected
				ConnectionKnob target = state.focusedNodeKnob as ConnectionKnob;
				target.connections.Add (state.partialConnection);
				state.canvas.connections.Add (Connection.Set(state.partialConnection, target));
				state.partialConnection.connections.Add(target);
				target.connections.Add(state.partialConnection);
				inputInfo.inputEvent.Use ();
			}
			state.partialConnection = null;
		}

		#endregion

		#region Zoom

		[EventHandlerAttribute (EventType.ScrollWheel)]
		private static void HandleZooming (NodeEditorInputInfo inputInfo) 
		{
			inputInfo.editorState.zoom = (float)Math.Round (Math.Min (4.0, Math.Max (0.6, inputInfo.editorState.zoom + inputInfo.inputEvent.delta.y / 15)), 2);
			NodeEditor.RepaintClients ();
		}

		#endregion

		#region Navigation

		[HotkeyAttribute (KeyCode.N, EventType.KeyDown)]
		private static void HandleStartNavigating (NodeEditorInputInfo inputInfo) 
		{
			if (GUIUtility.keyboardControl > 0)
				return;
			inputInfo.editorState.navigate = true;
		}

		[HotkeyAttribute (KeyCode.N, EventType.KeyUp)]
		private static void HandleEndNavigating (NodeEditorInputInfo inputInfo) 
		{
			if (GUIUtility.keyboardControl > 0)
				return;
			inputInfo.editorState.navigate = false;
		}

		#endregion

		#region Node Snap

		[HotkeyAttribute (KeyCode.LeftControl, EventType.KeyDown, 60)] // 60 ensures it is checked after the dragging was performed before
		[HotkeyAttribute (KeyCode.LeftControl, EventType.KeyUp, 60)]
		private static void HandleNodeSnap (NodeEditorInputInfo inputInfo) 
		{
			NodeEditorState state = inputInfo.editorState;
			if (state.selectedNode != null)
			{ // Snap selected Node's position to multiples of 10
				Vector2 pos = state.selectedNode.rect.position;
				pos = new Vector2 (Mathf.RoundToInt (pos.x/10) * 10, Mathf.RoundToInt (pos.y/10) * 10);
				state.selectedNode.rect.position = pos;
				inputInfo.inputEvent.Use ();
			}
			NodeEditor.RepaintClients ();
		}

		#endregion

	}

}

