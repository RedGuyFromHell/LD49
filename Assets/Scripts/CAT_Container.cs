using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CAT_Container : CAT_ContainerInterface
{
    public List<CAT_Event> events = new List<CAT_Event>();

    public bool debugMode; //used in the inspector
    public bool moreOptions; //used in the inspector

    public override bool IsEventRunning(int id)
    {
        return GetEventByID(id).IsRunning();
    }

    public override void StartEvent(int id)
    {
        GetEventByID(id).Start();
    }

    public override void StopEvent(int id)
    {
        GetEventByID(id).Stop();
    }

    public override void StopAllRunningEvents()
    {
        foreach (CAT_Event ev in events)
        {
            if (ev.IsRunning())
                ev.Stop();
        }
    }

    void Awake()
    {
        for (int evIndex = 0; evIndex < events.Count; evIndex++)
        {
            for (int evtIndex = evIndex + 1; evtIndex < events.Count; evtIndex++)
            {
                if (events[evIndex].id == events[evtIndex].id)
                {
                    Debug.LogError("Cat_Container " + name + " has duplicate Event IDs!", gameObject);
                }
            }
        }
    }

    void OnEnable()
    {
        foreach (CAT_Event ev in events)
        {
            if (ev.playAutomatically)
                ev.Start();
        }
    }

    void OnDisable()
    {
        StopAllRunningEvents();
    }

    public void Update()
    {
        foreach (CAT_Event ev in events)
        {
            if (ev.IsRunning())
                ev.Update();
        }
    }

    public override void RegisterCallback(int eventId, CAT_Event.Callback newCallback)
    {
        GetEventByID(eventId).RegisterCallback(newCallback);
    }

    public override void UnregisterCallback(int eventId, CAT_Event.Callback toRemoveCallback)
    {
        GetEventByID(eventId).UnregisterCallback(toRemoveCallback);
    }

    private CAT_Event GetEventByID(int id)
    {
        foreach (CAT_Event ev in events)
        {
            if (ev.id == id)
                return ev;
        }

        return null;
    }

    public override string EditorUtils_GetEventName(int eventId)
    {
        string name = "*Invalid Event ID*";

        CAT_Event ev = GetEventByID(eventId);
        if (ev != null)
            name = ev.userFriendlyName;

        return name;
    }

    public void DisableEvent(string id)
    {
        DisableEvent(int.Parse(id));
    }

    public void DisableEvent(int id)
    {
        GetEventByID(id).isEnabled = false;
    }

    public void EnableEvent(string id)
    {
        EnableEvent(int.Parse(id));
    }

    public void EnableEvent(int id)
    {
        GetEventByID(id).isEnabled = true;
    }
}
