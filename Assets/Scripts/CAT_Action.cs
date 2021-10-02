using UnityEngine;
using System.Collections;


public enum CATState
{
	// Waiting to get started. 
	//  When an event gets started all its actions are set to waiting. They will be started based on the delay parameter
	Waiting,

	// it is running and it is actually doing stuff
	Running,

	// it finished its job
	Finished,

	// a stop was requested
	Stopping,

	// this action and all actions in the event are finished and stopped
	Stopped
}


[System.Serializable]
public class CAT_Action : System.Object
{
	[HideInInspector] public float delay = 0f;

	//used in the inspector
	[HideInInspector] public bool isExpanded = true;

	[HideInInspector] public bool isEnabled = true;

	[HideInInspector] public bool debugBreak = false;

	protected CATState internalState = CATState.Stopped;


	public virtual CAT_Action InternalCopy()
	{
		return null;
	}

	protected void CopyCatActionMembers(CAT_Action copy)
	{
		copy.delay = delay;
		copy.internalState = CATState.Stopped;
		copy.debugBreak = false;
		copy.isExpanded = isExpanded;
		copy.isEnabled = isEnabled;
	}

	public CATState state
	{
		get { return internalState; }
	}


	public void ChangeState(CATState newState)
	{
		if (debugBreak)
		{
			Debug.Log("xCAT: Action in debug break!");
		}

		if (internalState == newState)
		{
			Debug.Log("xCAT: Trying to change state twice! Investigate!");
			return;
		}

		internalState = newState;

		switch (newState)
		{
			case CATState.Waiting:
				{
				}
				break;

			case CATState.Running:
				{
					OnStateChanged_Running();
				}
				break;

			case CATState.Finished:
				{
				}
				break;

			case CATState.Stopping:
				{
					OnStateChanged_Stopping();
				}
				break;

			case CATState.Stopped:
				{
				}
				break;

			default:
				{
					Debug.LogError("xCAT: Wait, what? How did i get here?");
				}
				break;
		}
	}

	protected virtual void OnStateChanged_Running()
	{
		ChangeState(CATState.Finished);
	}

	protected virtual void OnStateChanged_Stopping()
	{
		ChangeState(CATState.Finished);
	}

	public virtual void CATUpdate()
	{
	}

#if UNITY_EDITOR
	public virtual bool DrawCustomAction(UnityEngine.Object _target, UnityEditor.SerializedProperty propAction)
	{
		return false;
	}
#endif
}
