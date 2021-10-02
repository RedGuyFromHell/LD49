using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class CAT_ContainerInterface : MonoBehaviour
{
    public abstract bool IsEventRunning(int id);

    public abstract void StartEvent(int id);

    public abstract void StopEvent(int id);

    public abstract void StopAllRunningEvents();

    public abstract void RegisterCallback(int eventId, CAT_Event.Callback newCallback);

    public abstract void UnregisterCallback(int eventId, CAT_Event.Callback toRemoveCallback);

    public abstract string EditorUtils_GetEventName(int eventId);

    public void StartEventByMessage(string id)
    {
        StartEvent(int.Parse(id));
    }
}
