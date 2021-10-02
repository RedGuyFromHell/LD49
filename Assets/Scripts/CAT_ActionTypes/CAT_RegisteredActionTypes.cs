using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;


[System.Serializable]
public class CAT_RegisteredActionTypes : System.Object
{
	static public System.Type[] actionTypes =
	{
		typeof(CAT_Action_ObjActivate),
		typeof(CAT_Action_StartAnim),
		typeof(CAT_Action_SendMessage),
		typeof(CAT_Action_CATLink),
		//typeof(CAT_Action_SendMessageMultipleReceivers),
		typeof(CAT_Action_PlayAudioClip),
		//typeof(CAT_Action_StopAudioClip),
		//typeof(CAT_Action_SwitchLayer),
		typeof(CAT_Action_SetTransform),
		//typeof(CAT_Action_MusicController),

		//typeof(CAT_Action_TriggerXTEvent),
		//typeof(CAT_Action_SetXTBool),
		//typeof(CAT_Action_SpineAnimation),
		//typeof(CAT_Action_SpineAnimation_NoWait)
		//add more here
	};


	public List<CAT_Action_ObjActivate> list_0 = new List<CAT_Action_ObjActivate>();
	public List<CAT_Action_StartAnim> list_1 = new List<CAT_Action_StartAnim>();
	public List<CAT_Action_SendMessage> list_2 = new List<CAT_Action_SendMessage>();
	public List<CAT_Action_CATLink> list_3 = new List<CAT_Action_CATLink>();
	//public List<CAT_Action_SendMessageMultipleReceivers> list_4 = new List<CAT_Action_SendMessageMultipleReceivers>();
	public List<CAT_Action_PlayAudioClip> list_5 = new List<CAT_Action_PlayAudioClip>();
	//public List<CAT_Action_StopAudioClip> list_6 = new List<CAT_Action_StopAudioClip>();
	//public List<CAT_Action_SwitchLayer> list_8 = new List<CAT_Action_SwitchLayer>();
	public List<CAT_Action_SetTransform> list_9 = new List<CAT_Action_SetTransform>();
	//public List<CAT_Action_MusicController> list_10 = new List<CAT_Action_MusicController>();

	//public List<CAT_Action_TriggerXTEvent> list_7 = new List<CAT_Action_TriggerXTEvent>();
	//public List<CAT_Action_SetXTBool> list_11 = new List<CAT_Action_SetXTBool>();
	//public List<CAT_Action_SpineAnimation> list_12 = new List<CAT_Action_SpineAnimation>();
	//public List<CAT_Action_SpineAnimation_NoWait> list_13 = new List<CAT_Action_SpineAnimation_NoWait>();
	//add more here
}
