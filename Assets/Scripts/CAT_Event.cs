using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;


[System.Serializable]
public class CoolIndex : System.Object
{
	public CoolIndex(int _whichList, int _indexInList)
	{
		whichList = _whichList;
		indexInList = _indexInList;
	}

	public int whichList;
	public int indexInList;
}

[System.Serializable]
public class CoolData : System.Object, IEnumerator, IEnumerable
{
	public CAT_RegisteredActionTypes data = new CAT_RegisteredActionTypes();

	public List<CoolIndex> positions = new List<CoolIndex>();

	private List<IList> listCache = null;

	private int iterPosition = 0;


	public int GetSize()
	{
		return positions.Count;
	}

	public CAT_Action GetAtIndex(int i)
	{
		CoolIndex idx = positions[i];

		IList l = GetList(idx.whichList);

		CAT_Action ret = l[idx.indexInList] as CAT_Action;

		return ret;
	}

	public void Add(CAT_Action newAction)
	{
		for (int i = 0; i < CAT_RegisteredActionTypes.actionTypes.Length; i++)
		{
			if (newAction.GetType() == CAT_RegisteredActionTypes.actionTypes[i])
			{
				IList l = GetList(i);
				l.Add(newAction);

				positions.Add(new CoolIndex(i, l.Count - 1));

				return;
			}
		}

		Debug.LogError("xCAT: You tried to register an unknown action type (" + newAction.GetType() + "). Please register it in this file!");
	}

	public void RemoveAt(int i)
	{
		CoolIndex idx = positions[i];

		IList l = GetList(idx.whichList);
		l.RemoveAt(idx.indexInList);

		positions.RemoveAt(i);

		foreach (CoolIndex k in positions)
		{
			if ((k.whichList == idx.whichList) && (k.indexInList > idx.indexInList))
			{
				k.indexInList--;
			}
		}
	}

	public void GetListNameAndIndexFor(int inIndex, out string name, out int realIndex)
	{
		name = "list_" + positions[inIndex].whichList;
		realIndex = positions[inIndex].indexInList;
	}


	public CAT_Action this[int index]
	{
		get
		{
			return GetAtIndex(index);
		}
	}

	//IEnumerator and IEnumerable require these methods.
	public IEnumerator GetEnumerator()
	{
		Reset();
		return (IEnumerator)this;
	}

	//IEnumerator
	public bool MoveNext()
	{
		iterPosition++;
		return (iterPosition < positions.Count);
	}

	//IEnumerable
	public void Reset()
	{
		iterPosition = -1;
	}

	//IEnumerable
	public object Current
	{
		get { return GetAtIndex(iterPosition); }
	}


	private IList GetList(int i)
	{
		if (listCache == null)
		{
			//invalidate the cache
			int newSize = CAT_RegisteredActionTypes.actionTypes.Length;
			listCache = new List<IList>(newSize);
			for (int k = 0; k < newSize; k++)
				listCache.Add(null);
		}

		if (listCache[i] != null)
			return listCache[i];

		FieldInfo fi = data.GetType().GetField("list_" + i);

		IList il = fi.GetValue(data) as IList;

		listCache[i] = il;

		return il;
	}
}

[System.Serializable]
public class CAT_Event : System.Object
{
	public enum CallbackType
	{
		EndOfEvent
		//more to come
	}

	public delegate void Callback(CallbackType type, System.Object data);

	public int id;

	public string userFriendlyName;

	public CoolData actions = new CoolData();
	public bool playAutomatically = false;

	//used in the inspector
	public bool isExpanded = true;
	public int newActionIndex = 0;
	public bool debugBreak = false;
	public string timeScaleID;

	private float startTime = -1f;
	private float curTime = -1f;

	private event Callback callback;

	private bool isRunning = false;

	[System.NonSerialized] public bool isEnabled = true;

	public bool IsRunning()
	{
		return isRunning;
	}


	public void Start()
	{
		if (!isEnabled)
			return;

		if (debugBreak)
		{
			Debug.Log("xCAT: Event in debug break!");
		}

		if (IsRunning())
		{
			Debug.LogError("xCAT: Trying to start an event twice! Aborting! (name = '" + userFriendlyName + "', running_time = " + (curTime - startTime) + ")");
			return;
		}

		int finishedCount = 0;

		foreach (CAT_Action a in actions)
		{
			if (a.isEnabled)
			{
				if (a.delay <= 1.0e-3)
				{
					a.ChangeState(CATState.Running);

					if (a.state == CATState.Finished)
						finishedCount++;
				}
				else
				{
					a.ChangeState(CATState.Waiting);
				}
			}
			else
			{
				a.ChangeState(CATState.Finished);
				finishedCount++;
			}
		}

		startTime = curTime = Time.time;
		isRunning = true;

		InternalUpdateRunningState(finishedCount);
	}


	public void Stop()
	{
		if (debugBreak)
		{
			Debug.Log("xCAT: Event in debug break!");
		}

		int finishedCount = 0;

		foreach (CAT_Action a in actions)
		{
			if (a.state == CATState.Running)
			{
				a.ChangeState(CATState.Stopping);

				if (a.state == CATState.Finished)
					finishedCount++;
			}
			else if (a.state == CATState.Waiting)
			{
				a.ChangeState(CATState.Finished);
				finishedCount++;
			}
			else if (a.state == CATState.Finished)
			{
				finishedCount++;
			}
		}

		InternalUpdateRunningState(finishedCount);
	}

	public void Update()
	{
		int finishedCount = 0;

		curTime += TimeScaler.GetDeltaTime(timeScaleID);

		foreach (CAT_Action a in actions)
		{
			switch (a.state)
			{
				case CATState.Waiting:
					{
						if (curTime > (a.delay + startTime))
							a.ChangeState(CATState.Running);
					}
					break;

				case CATState.Running:
				case CATState.Stopping:
					{
						a.CATUpdate();
					}
					break;

				case CATState.Finished:
					{
						finishedCount++;
					}
					break;

				case CATState.Stopped:
				default:
					break;
			}
		}

		InternalUpdateRunningState(finishedCount);
	}

	private void InternalUpdateRunningState(int finishedActionsCount)
	{
		if (finishedActionsCount == actions.GetSize())
		{
			foreach (CAT_Action a in actions)
				a.ChangeState(CATState.Stopped);

			//signal the end
			if (callback != null)
				callback(CallbackType.EndOfEvent, this);

			isRunning = false;
		}
	}

	public void RegisterCallback(Callback newCallback)
	{
		callback += newCallback;
	}

	public void UnregisterCallback(Callback toRemoveCallback)
	{
		callback -= toRemoveCallback;
	}

	public CAT_Event CopyEventContent()
	{
		CAT_Event copy = new CAT_Event();

		copy.id = -1;
		copy.userFriendlyName = userFriendlyName;
		copy.playAutomatically = playAutomatically;
		copy.isExpanded = isExpanded;
		copy.newActionIndex = newActionIndex;
		copy.debugBreak = debugBreak;

		foreach (CAT_Action originalAction in actions)
		{
			CAT_Action ca = originalAction.InternalCopy();
			copy.actions.Add(ca);
		}

		return copy;
	}

}
