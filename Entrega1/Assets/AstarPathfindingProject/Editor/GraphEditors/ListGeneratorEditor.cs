using UnityEngine;
using UnityEditor;
using System.Collections;
using Pathfinding;

[CustomGraphEditor (typeof(ListGraph),"ListGraph")]
public class ListGraphEditor : GraphEditor {

	public override void OnInspectorGUI (NavGraph target) {
		ListGraph graph = target as ListGraph;

/*
#if UNITY_3_3
		graph.root = (Transform)EditorGUILayout.ObjectField (new GUIContent ("Root","All childs of this object will be used as nodes, if it is not set, a tag search will be used instead (see below)"),graph.root,typeof(Transform));
#else
		graph.root = (Transform)EditorGUILayout.ObjectField (new GUIContent ("Root","All childs of this object will be used as nodes, if it is not set, a tag search will be used instead (see below)"),graph.root,typeof(Transform),true);
#endif
*/
		graph.root = ObjectField (new GUIContent ("Root","All childs of this object will be used as nodes, if it is not set, a tag search will be used instead (see below)"),graph.root,typeof(Transform),true) as Transform;
		
		graph.recursive = EditorGUILayout.Toggle (new GUIContent ("Recursive","Should childs of the childs in the root GameObject be searched"),graph.recursive);
		graph.searchTag = EditorGUILayout.TagField (new GUIContent ("Tag","If root is not set, all objects with this tag will be used as nodes"),graph.searchTag);
		
		if (graph.root != null) {
			GUILayout.Label ("All childs "+(graph.recursive ? "and sub-childs ":"") +"of 'root' will be used as nodes\nSet root to null to use a tag search instead",AstarPathEditor.helpBox);
		} else {
			GUILayout.Label ("All object with the tag '"+graph.searchTag+"' will be used as nodes"+(graph.searchTag == "Untagged" ? "\nNote: the tag 'Untagged' cannot be used" : ""),AstarPathEditor.helpBox);
		}
		
		graph.maxDistance = EditorGUILayout.FloatField ("Max Distance",graph.maxDistance);
		graph.limits = EditorGUILayout.Vector3Field ("Max Distance (axis aligned)",graph.limits);
		graph.raycast = EditorGUILayout.Toggle ("Raycast",graph.raycast);
		 
		editor.GUILayoutx.BeginFadeArea (graph.raycast,"raycast");
			
			EditorGUI.indentLevel++;
			
		 	graph.thickRaycast = EditorGUILayout.Toggle ("Thick Raycast",graph.thickRaycast);
		 	
		 	editor.GUILayoutx.BeginFadeArea (graph.thickRaycast,"thickRaycast");
		 	
		 		graph.thickRaycastRadius = EditorGUILayout.FloatField ("Raycast Radius",graph.thickRaycastRadius);
		 	
		 	editor.GUILayoutx.EndFadeArea ();
		 	
			//graph.mask = 1 << EditorGUILayout.LayerField ("Mask",(int)Mathf.Log (graph.mask,2));
			graph.mask = EditorGUILayoutx.LayerMaskField ("Mask",graph.mask);
			EditorGUI.indentLevel--;
			
		editor.GUILayoutx.EndFadeArea ();
	}
	
	public override void OnDrawGizmos () {
			
		ListGraph graph = target as ListGraph;
		
		//Debug.Log ("Gizmos "+(graph == null)+" "+target);
		if (graph == null) {
			return;
		}
		
		//Handles.color = new Color (0.161F,0.341F,1F,0.5F);
		Gizmos.color = new Color (0.161F,0.341F,1F,0.5F);
		//for (int i=0;i<graph.nodes.Length;i++) {
		
		if (graph.root != null) {
			DrawChildren (graph, graph.root);
		} else {
			
			GameObject[] gos = GameObject.FindGameObjectsWithTag (graph.searchTag);
			for (int i=0;i<gos.Length;i++) {
				Gizmos.DrawCube (gos[i].transform.position,Vector3.one*HandleUtility.GetHandleSize(gos[i].transform.position)*0.1F);
			}
		}
	}
	
	public void DrawChildren (ListGraph graph, Transform tr) {
		foreach (Transform child in tr) {
			Gizmos.DrawCube (child.position,Vector3.one*HandleUtility.GetHandleSize(child.position)*0.1F);
			//Handles.CubeCap (-1,graph.nodes[i].position,Quaternion.identity,HandleUtility.GetHandleSize(graph.nodes[i].position)*0.1F);
			//Gizmos.DrawCube (nodes[i].position,Vector3.one);
			if (graph.recursive) DrawChildren (graph, child);
		}
	}
}
