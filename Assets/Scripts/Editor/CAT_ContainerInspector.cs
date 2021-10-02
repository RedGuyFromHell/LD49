using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

[CustomEditor(typeof(CAT_Container))]
public class CAT_ContainerInspector : Editor
{
	CAT_Container cat = null;
	SerializedObject so = null;
	SerializedProperty propEvents = null;
	List<SerializedProperty> propCurEvents = null;
	List<SerializedProperty> propCurActions = null;
	List<SerializedProperty> propCurActionsData = null;

	protected void OnEnable()
	{
		cat = target as CAT_Container;
		so = new SerializedObject(cat);

		RefreshCache();
	}

	protected void RefreshCache()
	{
		so.Update();
		propEvents = so.FindProperty("events");
		propCurEvents = new List<SerializedProperty>();
		propCurActions = new List<SerializedProperty>();
		propCurActionsData = new List<SerializedProperty>();

		for (int idxEvent = 0; idxEvent < cat.events.Count; idxEvent++)
		{
			propCurEvents.Add(propEvents.GetArrayElementAtIndex(idxEvent));
			propCurActions.Add(propCurEvents[propCurEvents.Count - 1].FindPropertyRelative("actions"));
			propCurActionsData.Add(propCurActions[propCurActions.Count - 1].FindPropertyRelative("data"));
		}
	}

	public override void OnInspectorGUI()
	{
		//CAT_Container cat = target as CAT_Container;
		so.Update();


		// Get all implementation of CAT_Action
		List<System.Type> availableActionTypes = new List<System.Type>();
		List<string> availableActionTypeNames = new List<string>();
		foreach (System.Type t in typeof(CAT_Action).Assembly.GetTypes())
			if (typeof(CAT_Action).IsAssignableFrom(t) && (t != typeof(CAT_Action)))
			{
				availableActionTypes.Add(t);
				availableActionTypeNames.Add(GetNiceActionNameFromType(t));
			}


		EditorGUILayout.BeginVertical();
		{
			if (cat.events != null)
			{
				//header
				EditorGUILayout.BeginHorizontal(/*GUI.skin.box*/);


				Color oldColor = GUI.color;
				GUI.color = Color.red;
				bool dbgMode = GUILayout.Toggle(cat.debugMode, new GUIContent("Debug Mode", "Toggle more debug info"));
				if (dbgMode != cat.debugMode)
				{
					cat.debugMode = dbgMode;
					SetDirtyHack.SetDirty(target);
				}
				GUI.color = oldColor;

				bool moreOptions = GUILayout.Toggle(cat.moreOptions, new GUIContent("More options", "Toggle more UI options"));
				if (moreOptions != cat.moreOptions)
				{
					cat.moreOptions = moreOptions;
					SetDirtyHack.SetDirty(target);
				}


				GUILayout.FlexibleSpace();
				if (GUILayout.Button(new GUIContent("Collapse all", "Collapse all events and actions"), EditorStyles.miniButton, GUILayout.MaxWidth(80.0f)))
				{
					ExpandAll(cat, false);
					SetDirtyHack.SetDirty(target);
				}
				if (GUILayout.Button(new GUIContent("Expand all", "Expand all events and actions"), EditorStyles.miniButton, GUILayout.MaxWidth(80.0f)))
				{
					ExpandAll(cat, true);
					SetDirtyHack.SetDirty(target);
				}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.Space();


				//event list bellow:
				for (int idxEvent = 0; idxEvent < cat.events.Count; idxEvent++)
				{
					EditorGUILayout.BeginVertical(/*GUI.skin.box*/);

					CAT_Event ev = cat.events[idxEvent];

					EditorGUI.indentLevel += 1;

					EditorGUILayout.BeginHorizontal(GUI.skin.box);
					{
						oldColor = GUI.color;
						if (HasEventIdDuplicates(cat.events, ev))
						{
							GUI.color = Color.red;
						}
						bool evIsExpanded = EditorGUILayout.Foldout(ev.isExpanded, idxEvent.ToString() + ". Event:");
						GUI.color = oldColor;
						if (evIsExpanded != ev.isExpanded)
						{
							ev.isExpanded = evIsExpanded;
							SetDirtyHack.SetDirty(target);
						}

						GUILayout.FlexibleSpace();

						oldColor = GUI.color;
						GUI.backgroundColor = Color.green;
						string newId = EditorGUILayout.TextField("", ev.id.ToString(), GUILayout.MaxWidth(60.0f));
						int unused;
						if (newId != ev.id.ToString() && int.TryParse(newId, out unused))
						{
							ev.id = int.Parse(newId);
							SetDirtyHack.SetDirty(target);
						}
						GUI.backgroundColor = oldColor;

						GUILayout.FlexibleSpace();

						string newName = EditorGUILayout.TextField("", ev.userFriendlyName);
						if (newName != ev.userFriendlyName)
						{
							ev.userFriendlyName = newName;
							SetDirtyHack.SetDirty(target);
						}

						if (cat.debugMode)
						{
							oldColor = GUI.color;
							GUI.color = Color.red;
							bool isDebugBreakEvent = GUILayout.Toggle(ev.debugBreak, new GUIContent("", ""));
							if (isDebugBreakEvent != ev.debugBreak)
							{
								ev.debugBreak = isDebugBreakEvent;
								SetDirtyHack.SetDirty(target);
							}
							GUI.color = oldColor;
						}
					}
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.BeginHorizontal(GUI.skin.box);
					{
						GUILayout.FlexibleSpace();

						bool playAutomatically = GUILayout.Toggle(ev.playAutomatically, new GUIContent("Play automatically", "Start event automatically"));
						if (playAutomatically != ev.playAutomatically)
						{
							ev.playAutomatically = playAutomatically;
							SetDirtyHack.SetDirty(target);
						}

						if (GUILayout.Button(new GUIContent("Copy", "Copy entire event"), EditorStyles.miniButton, GUILayout.MaxWidth(60.0f)))
						{
							CAT_CopyUtils.CopyEvent(ev);
							return;
						}

						if (EditorApplication.isPlaying)
						{
							oldColor = GUI.color;
							GUI.color = Color.green;
							if (GUILayout.Button(new GUIContent("Test", "Run this event"), EditorStyles.miniButton, GUILayout.MaxWidth(60.0f)))
							{
								cat.StopAllRunningEvents();
								cat.StartEvent(ev.id);
								return;
							}
							GUI.color = oldColor;
						}

						if (cat.moreOptions)
						{
							if (GUILayout.Button(new GUIContent("Sort", "Sort actions by delay"), EditorStyles.miniButton, GUILayout.MaxWidth(60.0f)))
							{
								SortByDelay(ev);
								SetDirtyHack.SetDirty(target);
								return;
							}

							if (GUILayout.Button(new GUIContent("Delete", "Delete this event"), EditorStyles.miniButton, GUILayout.MaxWidth(50.0f)))
							{
								if (EditorUtility.DisplayDialog("This is a title", "Are you sure you want to delete this event?", "Yes", "No"))
								{
									cat.events.RemoveAt(idxEvent);
									RefreshCache();
									SetDirtyHack.SetDirty(target);

									idxEvent--;
									continue;
								}
							}
							/*
							if (GUILayout.Button(new GUIContent("^", "Move up"), EditorStyles.miniButton, GUILayout.MaxWidth(50.0f)))
							{
							}

							if (GUILayout.Button(new GUIContent("v", "Move down"), EditorStyles.miniButton, GUILayout.MaxWidth(50.0f)))
							{
							}
							 */
						}
					}
					EditorGUILayout.EndHorizontal();


					if (ev.isExpanded)
					{
						EditorGUI.indentLevel += 1;

						ev.timeScaleID = EditorGUILayout.TextField("Time Scale ID", ev.timeScaleID);

						for (int i = 0; i < ev.actions.GetSize(); i++)
						{
							CAT_Action action = ev.actions[i];

							EditorGUILayout.BeginHorizontal();
							{

								string str = action.isEnabled ? "Action " : "(DISABLED) Action ";
								bool actionIsExpanded = EditorGUILayout.Foldout(action.isExpanded, str + i.ToString() + ": (" + GetNiceActionNameFromType(action.GetType()) + ")");
								if (actionIsExpanded != action.isExpanded)
								{
									action.isExpanded = actionIsExpanded;
									SetDirtyHack.SetDirty(target);
								}

								GUILayout.FlexibleSpace();

								if (cat.debugMode)
								{
									oldColor = GUI.color;
									GUI.color = Color.red;
									EditorGUILayout.LabelField(new GUIContent(action.state.ToString(), ""));
									GUI.color = oldColor;
								}

								float newDelay = EditorGUILayout.FloatField(new GUIContent("", "Delay"), action.delay, GUILayout.MaxWidth(90.0f));
								if (Mathf.Abs(newDelay - action.delay) > 1e-4)
								{
									action.delay = newDelay;
									SetDirtyHack.SetDirty(target);
								}

								bool isActionEnabled = GUILayout.Toggle(action.isEnabled, new GUIContent("", "Toggle the disabled state of this action"));
								if (isActionEnabled != action.isEnabled)
								{
									action.isEnabled = isActionEnabled;
									SetDirtyHack.SetDirty(target);
								}

								if (cat.debugMode)
								{
									oldColor = GUI.color;
									GUI.color = Color.red;
									bool isDebugBreak = GUILayout.Toggle(action.debugBreak, new GUIContent("", ""));
									if (isDebugBreak != action.debugBreak)
									{
										action.debugBreak = isDebugBreak;
										SetDirtyHack.SetDirty(target);
									}

									GUI.color = oldColor;
								}

								//if (cat.moreOptions)
								{
									if (GUILayout.Button(new GUIContent("X", "Remove this action"), EditorStyles.miniButton, GUILayout.MaxWidth(20.0f)))
									{
										if (EditorUtility.DisplayDialog("", "Are you sure you want to delete this action?", "Yes", "No"))
										{
											ev.actions.RemoveAt(i);

											SetDirtyHack.SetDirty(target);

											i--;
											continue;
										}
									}

									if (GUILayout.Button(new GUIContent("Copy", "Copy this action"), EditorStyles.miniButton, GUILayout.MaxWidth(60.0f)))
									{
										CAT_CopyUtils.CopyAction(ev.actions[i]);
									}
								}
							}
							EditorGUILayout.EndHorizontal();

							if (action.isExpanded)
							{
								EditorGUI.indentLevel += 1;

								EditorGUILayout.BeginVertical();


								//SerializedObject so = new SerializedObject (cat);
								//SerializedProperty propEvents = so.FindProperty("events");
								//SerializedProperty propCurEvent = propEvents.GetArrayElementAtIndex(idxEvent);

								//SerializedProperty propActions = propCurEvent.FindPropertyRelative("actions");
								//SerializedProperty propActionsData = propActions.FindPropertyRelative("data");

								int indexProp;
								string nameProp;
								ev.actions.GetListNameAndIndexFor(i, out nameProp, out indexProp);

								SerializedProperty propActionList = propCurActionsData[idxEvent].FindPropertyRelative(nameProp);

								SerializedProperty propCurAction = propActionList.GetArrayElementAtIndex(indexProp);

								if (!action.DrawCustomAction(target, propCurAction))
									DrawPropertyChildren(propCurAction);

								so.ApplyModifiedProperties();

								EditorGUILayout.EndVertical();

								EditorGUI.indentLevel -= 1;
							}

							EditorGUILayout.Space();
						}


						EditorGUILayout.BeginHorizontal();
						{
							ev.newActionIndex = EditorGUILayout.Popup(ev.newActionIndex, availableActionTypeNames.ToArray());
							if (GUILayout.Button(new GUIContent("New", "Add a new action of this type"), EditorStyles.miniButton, GUILayout.MaxWidth(40.0f)))
							{
								MethodInfo method = this.GetType().GetMethod("CreateInstanceHack");
								MethodInfo generic = method.MakeGenericMethod(availableActionTypes[ev.newActionIndex]);
								CAT_Action newAction = (CAT_Action)generic.Invoke(this, null);


								ev.actions.Add(newAction);
								RefreshCache();
								SetDirtyHack.SetDirty(target);
							}

							bool guiEnabled = GUI.enabled;
							GUI.enabled = CAT_CopyUtils.HasACopiedAction();
							if (GUILayout.Button(new GUIContent("Paste", "Paste"), EditorStyles.miniButton, GUILayout.MaxWidth(40.0f)))
							{
								CAT_Action newAction = CAT_CopyUtils.PasteAction();
								ev.actions.Add(newAction);
								SetDirtyHack.SetDirty(target);
							}
							GUI.enabled = guiEnabled;
						}
						EditorGUILayout.EndHorizontal();


						EditorGUI.indentLevel -= 1;
					}

					EditorGUI.indentLevel -= 1;
					EditorGUILayout.EndVertical();

					EditorGUILayout.Space();

				}

			}


			EditorGUILayout.Space();

			EditorGUILayout.BeginHorizontal();
			{
				if (GUILayout.Button(new GUIContent("Add event", "Add a new event"), EditorStyles.miniButton, GUILayout.MaxWidth(80.0f)))
				{
					CAT_Event newEv = new CAT_Event();
					newEv.id = cat.events.Count;
					newEv.userFriendlyName = "Empty Event";
					cat.events.Add(newEv);
					SetDirtyHack.SetDirty(target);
				}

				GUI.enabled = CAT_CopyUtils.HasACopiedEvent();
				if (GUILayout.Button(new GUIContent("Paste event", "Paste copied event"), EditorStyles.miniButton, GUILayout.MaxWidth(80.0f)))
				{
					CAT_Event eventCopy = CAT_CopyUtils.PasteEvent();
					cat.events.Add(eventCopy);
					RefreshCache();
					SetDirtyHack.SetDirty(target);
					return;
				}
				GUI.enabled = true;
			}
			EditorGUILayout.EndHorizontal();
		}
		EditorGUILayout.EndVertical();

		if (SetDirtyHack.CATdirtySet)
		{
			Repaint();
			SetDirtyHack.CATdirtySet = false;
		}
	}

	private void SortByDelay(CAT_Event _event)
	{
		for (int i = 0; i < _event.actions.GetSize(); i++)
		{
			for (int j = i + 1; j < _event.actions.GetSize(); j++)
			{
				if (_event.actions[i].delay > _event.actions[j].delay)
				{
					CoolIndex _tempPos = new CoolIndex(0, 0);
					_tempPos = _event.actions.positions[i];
					_event.actions.positions[i] = _event.actions.positions[j];
					_event.actions.positions[j] = _tempPos;
				}
			}
		}
	}

	public CAT_Action CreateInstanceHack<T>() where T : CAT_Action, new()
	{
		return new T();
	}

	public void DrawPropertyChildren(SerializedProperty prop)
	{
		SerializedProperty iterator = prop.Copy();
		iterator.NextVisible(true);
		int startingDepth = iterator.depth;

		GUILayout.BeginVertical();
		do
		{
			EditorGUILayout.PropertyField(iterator, true);
			if (!iterator.NextVisible(false))
				break;
		}
		while (iterator.depth == startingDepth);
		GUILayout.EndVertical();
	}

	private string GetNiceActionNameFromType(System.Type type)
	{
		string s = type.ToString().Substring(11);
		return s;
	}

	private void ExpandAll(CAT_Container c, bool newState)
	{
		foreach (CAT_Event ev in c.events)
		{
			ev.isExpanded = newState;
			foreach (CAT_Action a in ev.actions)
				a.isExpanded = newState;
		}
	}

	private bool HasEventIdDuplicates(List<CAT_Event> _events, CAT_Event _event)
	{
		for (int evtIndex = 0; evtIndex < _events.Count; evtIndex++)
		{
			if (_events[evtIndex] != _event && _events[evtIndex].id == _event.id)
			{
				return true;
			}
		}
		return false;
	}
}
