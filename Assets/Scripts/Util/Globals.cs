using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Globals
{
	public static bool InputBlocked = true;
	public static List<TimeChannel> timeChannels = new List<TimeChannel>();
	public static bool isMobile = false;
	public static bool isMini = false;
	public static bool GamePaused = false;
	public static Dictionary<string, string> gameConfig = null;

	public static void SetLayerRecursively(GameObject obj, int layer)
	{
		obj.layer = layer;
		for (int i = 0; i < obj.transform.childCount; i++)
		{
			SetLayerRecursively(obj.transform.GetChild(i).gameObject, layer);
		}
	}

	public static Camera GetCameraForObject(GameObject go)
	{
		Camera[] allCameras = Camera.allCameras;
		int layerMask = 1 << go.layer;

		for (int i = 0; i < allCameras.Length; i++)
		{
			if (allCameras[i].cullingMask == layerMask)
				return allCameras[i];
		}

		return null;
	}

	public static TimeChannel RegisterToTimeChannel(MonoBehaviour mb, int id)
	{
		for (int i = 0; i < timeChannels.Count; i++)
		{
			if (timeChannels[i].id == id)
			{
				if (timeChannels[i].members.Contains(mb))
				{
					return timeChannels[i];
				}

				timeChannels[i].members.Add(mb);
				return timeChannels[i];
			}
		}

		TimeChannel tc = new TimeChannel(id);
		tc.members.Add(mb);
		timeChannels.Add(tc);
		return tc;
	}

	public static void UnregisterFromTimeChannel(MonoBehaviour mb, int id)
	{
		for (int i = 0; i < timeChannels.Count; i++)
		{
			if (timeChannels[i].id == id)
			{
				if (timeChannels[i].members.Contains(mb))
				{
					timeChannels[i].members.Remove(mb);
					if (timeChannels[i].members.Count == 0)
					{
						timeChannels.Remove(timeChannels[i]);
					}
					return;
				}
			}
		}
	}
}

public class TimeChannel
{
	public int id = 0;
	public List<MonoBehaviour> members;
	public float currentTime = 0f;

	public TimeChannel(int id)
	{
		members = new List<MonoBehaviour>();
		this.id = id;
	}

	public bool IsChannelMaster(MonoBehaviour mb)
	{
		return members[0] == mb;
	}
}

public static class TimeScaler
{
	static Dictionary<string, float> timeScales = new Dictionary<string, float>();

	public static float GetDeltaTime(string id)
	{
		float scale = 1;
		if (timeScales.ContainsKey(id))
			scale = timeScales[id];

		return Time.deltaTime * scale;
	}

	public static void SetTimeScale(string id, float scale)
	{
		if (!timeScales.ContainsKey(id))
			timeScales.Add(id, scale);
		else
			timeScales[id] = scale;
	}

	public static void ClearTimeScale(string id)
	{
		if (timeScales.ContainsKey(id))
			timeScales.Remove(id);
	}
}
