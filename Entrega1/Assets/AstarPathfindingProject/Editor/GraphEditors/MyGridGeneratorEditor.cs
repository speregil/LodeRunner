// This script is part of the tutorial series "Making a 2D game with Unity3D using only free tools"
// http://www.rocket5studios.com/tutorials/make-a-2d-game-in-unity3d-using-only-free-tools-part-1
// Contributed by Adrian Seeto of FunMobGames.com
using UnityEngine;
using UnityEditor;
using Pathfinding;
using System.Collections;

[CustomGraphEditor (typeof(MyGridGraph),"My Grid Graph")]
public class MyGridGraphEditor : GridGraphEditor {
	
	public override void OnInspectorGUI (NavGraph target) 
	{	
		base.OnInspectorGUI(target);
		Separator();
		
		MyGridGraph graph = target as MyGridGraph;
		
		
		GUILayout.BeginHorizontal ();
		graph.levelLayerMask = EditorGUILayoutx.LayerMaskField ("Level Layers",graph.levelLayerMask);
		GUILayout.EndHorizontal ();
	}
}