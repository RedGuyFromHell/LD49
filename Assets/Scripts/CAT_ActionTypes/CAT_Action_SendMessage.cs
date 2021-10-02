using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CAT_Action_SendMessage : CAT_Action
{
	public MessageSender.MessageData data = new MessageSender.MessageData();

	public override CAT_Action InternalCopy()
	{
		CAT_Action_SendMessage copy = new CAT_Action_SendMessage();

		CopyCatActionMembers(copy);

		copy.data = new MessageSender.MessageData(data.eventReceiver, data.methodToCall, data.useParam, data.paramString, data.paramType);

		return copy;
	}

	protected override void OnStateChanged_Running()
	{
		MessageSender.SendMessage(data);

		ChangeState(CATState.Finished);
	}
}
