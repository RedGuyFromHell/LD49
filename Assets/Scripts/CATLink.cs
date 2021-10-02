using UnityEngine;
using System.Collections;

[System.Serializable]
public class CATLink : System.Object
{
	public int id;
	public CAT_ContainerInterface cat;

	public static implicit operator int(CATLink e)
	{
		return e.id;
	}

	public void Start()
	{
		if (cat != null)
			cat.StartEvent(id);
	}

	public void Stop()
	{
		if (cat != null)
			cat.StopEvent(id);
	}

	public bool IsRunning()
	{
		if (cat != null)
			return cat.IsEventRunning(id);
		else
			return false;
	}

	public void RegisterCallback(CAT_Event.Callback newCallback)
	{
		if (cat != null)
			cat.RegisterCallback(id, newCallback);
	}

	public void UnregisterCallback(CAT_Event.Callback toRemoveCallback)
	{
		if (cat != null)
			cat.UnregisterCallback(id, toRemoveCallback);
	}
}
