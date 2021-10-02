using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MessageSender : MonoBehaviour
{
	public enum MessageDataParamType { None, Bool, Int, Float, Double, String };

	[System.Serializable]
	public class MessageData : System.Object
	{
		public MessageData() { }

		public MessageData(GameObject _receiver, string _methodToCall, bool _useParam, string _paramString, MessageDataParamType _paramType = MessageDataParamType.String)
		{
			eventReceiver = _receiver;
			useParam = _useParam;
			paramType = _paramType;
			paramString = _paramString;
			methodToCall = _methodToCall;
		}

		public GameObject eventReceiver;
		public bool useParam;
		public MessageDataParamType paramType = MessageDataParamType.String;

		public string paramString;
		public string methodToCall;
	}

	public List<MessageData> animationEvents;
	public List<MessageData> onClickEvents;
	public List<MessageData> onEnabledEvents;
	//more to come



	//
	// This can be called from inside an animation
	//     id is the index in the receivers array
	//
	public void SendMessageFromAnimation(int id)
	{
		if (id >= animationEvents.Count || id < 0)
			return;

		InternalSendMessage(animationEvents, id);
	}

	//
	// This can be called from inside another behaviour
	//
	public static void SendMessage(MessageData msg)
	{
		if (msg.eventReceiver != null && !string.IsNullOrEmpty(msg.methodToCall))
		{
			if (msg.useParam)
			{
				switch (msg.paramType)
				{
					case MessageDataParamType.Bool:
						msg.eventReceiver.SendMessage(msg.methodToCall, bool.Parse(msg.paramString), SendMessageOptions.DontRequireReceiver);
						break;
					case MessageDataParamType.Int:
						msg.eventReceiver.SendMessage(msg.methodToCall, int.Parse(msg.paramString), SendMessageOptions.DontRequireReceiver);
						break;
					case MessageDataParamType.Float:
						msg.eventReceiver.SendMessage(msg.methodToCall, float.Parse(msg.paramString), SendMessageOptions.DontRequireReceiver);
						break;
					case MessageDataParamType.Double:
						msg.eventReceiver.SendMessage(msg.methodToCall, double.Parse(msg.paramString), SendMessageOptions.DontRequireReceiver);
						break;
					case MessageDataParamType.String:
						msg.eventReceiver.SendMessage(msg.methodToCall, msg.paramString, SendMessageOptions.DontRequireReceiver);
						break;
					default:
						msg.eventReceiver.SendMessage(msg.methodToCall, SendMessageOptions.DontRequireReceiver);
						break;
				}
			}
			else
				msg.eventReceiver.SendMessage(msg.methodToCall, SendMessageOptions.DontRequireReceiver);
		}
	}


	public void OnClick()
	{
		if (GetComponent<Collider>() == null || !GetComponent<Collider>().enabled)
			return;

		if (onClickEvents.Count <= 0)
			return;

		InternalSendMessageToAll(onClickEvents);
	}

	public void OnEnable()
	{
		if (onEnabledEvents == null || onEnabledEvents.Count <= 0)
			return;

		InternalSendMessageToAll(onEnabledEvents);
	}





	private void InternalSendMessageToAll(List<MessageData> data)
	{
		for (int i = 0; i < data.Count; i++)
			InternalSendMessage(data, i);
	}

	private void InternalSendMessage(List<MessageData> data, int idx)
	{
		if (idx >= data.Count || idx < 0)
			return;

		MessageData msg = data[idx];

		MessageSender.SendMessage(msg);
	}
}

