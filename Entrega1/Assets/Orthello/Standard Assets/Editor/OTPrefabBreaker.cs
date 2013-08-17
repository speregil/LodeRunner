#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(OTPB)), CanEditMultipleObjects]
public class OTPrefabBreaker : Editor
{	
    void OnEnable()
    {		
		// if we are presistent we reside on disk and therefore are a real prefab
		if (EditorUtility.IsPersistent(target)) {
			OTPB pb = target as OTPB;
			if (pb != null)
			{
				OTObject otObject = pb.GetComponent<OTObject>();
				if (otObject!=null) 
				{
					otObject._isPrefab = true;
					otObject.baseName = otObject.name;
				}
			}			
			return;		
		}
		EditorApplication.update += updateObjects;					
    }
	
	void OnDisable()
	{
		EditorApplication.update -= updateObjects;
	}
	
	void updateObjects()
	{
		for (int o=0; o<serializedObject.targetObjects.Length; o++)
		{
			OTPB pb = serializedObject.targetObjects[o] as OTPB;
			if (pb!=null && pb.otTarget != null)
				PrefabUtility.RecordPrefabInstancePropertyModifications(pb.otTarget);		
		}
	}	
	
}
#endif