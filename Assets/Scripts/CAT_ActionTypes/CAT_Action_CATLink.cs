using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CAT_Action_CATLink : CAT_Action
{
	public enum ActionType
	{
		StartEvent,
		StopEvent,
		StopAll,
		StartEvent_NoWait
	}

	public ActionType actionType;
	public CATLink eventId;



	public override CAT_Action InternalCopy()
	{
		CAT_Action_CATLink copy = new CAT_Action_CATLink();

		CopyCatActionMembers(copy);

		copy.actionType = actionType;
		copy.eventId = new CATLink();
		copy.eventId.id = eventId.id;
		copy.eventId.cat = eventId.cat;

		return copy;
	}

	protected override void OnStateChanged_Running()
	{
		CAT_ContainerInterface target = eventId.cat;
		if (target != null)
		{
			switch (actionType)
			{
				case ActionType.StartEvent:
					target.RegisterCallback(eventId, Callback_EventEnded);
					target.StartEvent(eventId);
					break;

				case ActionType.StopEvent:
					target.StopEvent(eventId);
					ChangeState(CATState.Finished);
					break;

				case ActionType.StopAll:
					target.StopAllRunningEvents();
					ChangeState(CATState.Finished);
					break;

				case ActionType.StartEvent_NoWait:
					target.StartEvent(eventId);
					ChangeState(CATState.Finished);
					break;
			}
		}
		else
		{
			ChangeState(CATState.Finished);
		}
	}

	protected override void OnStateChanged_Stopping()
	{
		CAT_ContainerInterface target = eventId.cat;
		if (target != null && actionType == ActionType.StartEvent)
		{
			target.UnregisterCallback(eventId, Callback_EventEnded);
			target.StopEvent(eventId);
		}

		ChangeState(CATState.Finished);
	}

	protected void Callback_EventEnded(CAT_Event.CallbackType type, System.Object data)
	{
		CAT_ContainerInterface target = eventId.cat;
		if (type == CAT_Event.CallbackType.EndOfEvent && actionType == ActionType.StartEvent)
		{
			target.UnregisterCallback(eventId, Callback_EventEnded);
			ChangeState(CATState.Finished);
		}
	}
}
