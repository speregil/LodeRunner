using UnityEngine;
using System.Collections;
// this class its sole purpose is to handle the marking of changed property 
// settings that were changed by code because of scene changes.
[ExecuteInEditMode]
public class OTPB : MonoBehaviour {

	OTObject otObject = null;	
	OTContainer otContainer = null;	
	OTAnimation otAnimation = null;	
	public Object otTarget
	{
		get
		{
			if (otObject!=null)
				return otObject;
			if (otContainer!=null)
				return otContainer;
			if (otAnimation!=null)
				return otAnimation;
			return null;
		}
	}
		
	void Start()
	{
		otObject = GetComponent<OTObject>();
		if (otObject==null)
		{
			otContainer = GetComponent<OTContainer>();
			if (otContainer == null)
				otAnimation = GetComponent<OTAnimation>();
		}
		
	}
	
}