using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[System.Serializable]
public class CAT_Action_ObjActivate : CAT_Action
{
	public List<GameObject> ToEnableList = new List<GameObject>();
	public List<GameObject> ToDisableList = new List<GameObject>();

	public override CAT_Action InternalCopy()
	{
		CAT_Action_ObjActivate copy = new CAT_Action_ObjActivate();

		CopyCatActionMembers(copy);

		copy.ToEnableList = new List<GameObject>();
		foreach (GameObject obj in ToEnableList)
			copy.ToEnableList.Add(obj);

		copy.ToDisableList = new List<GameObject>();
		foreach (GameObject obj in ToDisableList)
			copy.ToDisableList.Add(obj);

		return copy;
	}

	protected override void OnStateChanged_Running()
	{
		foreach (GameObject obj in ToEnableList)
			obj.SetActive(true);
		foreach (GameObject obj in ToDisableList)
			obj.SetActive(false);

		ChangeState(CATState.Finished);
	}

	public override bool DrawCustomAction(Object _target, SerializedProperty propAction)
	{
		SerializedProperty iterator = propAction.Copy();
		iterator.NextVisible(true);
		int startingDepth = iterator.depth;

		GUILayout.BeginVertical();
		do
		{
			GUILayout.BeginHorizontal();
			if (GUILayout.Button(new GUIContent("[Clear Missing]", "Removes missig references"), EditorStyles.miniButton))
			{
				int index = 0;
				while (index < iterator.arraySize)
				{
					SerializedProperty element = iterator.GetArrayElementAtIndex(index);
					if (element.objectReferenceValue == null)
						iterator.DeleteArrayElementAtIndex(index);
					else
						index++;
				}
				iterator.serializedObject.ApplyModifiedProperties();
			}
			GUILayout.FlexibleSpace();
			if (GUILayout.Button(new GUIContent("[Add Selection]", "Adds screen selection to the array"), EditorStyles.miniButton))
			{
				for (int i = 0; i < Selection.gameObjects.Length; i++)
				{
					iterator.arraySize++;
					SerializedProperty element = iterator.GetArrayElementAtIndex(iterator.arraySize - 1);
					element.objectReferenceValue = Selection.gameObjects[i];
				}
				iterator.serializedObject.ApplyModifiedProperties();
			}
			GUILayout.EndHorizontal();

			EditorGUILayout.PropertyField(iterator, true);
			if (!iterator.NextVisible(false))
				break;
		}
		while (iterator.depth == startingDepth);
		GUILayout.EndVertical();

		return true;
	}
}
