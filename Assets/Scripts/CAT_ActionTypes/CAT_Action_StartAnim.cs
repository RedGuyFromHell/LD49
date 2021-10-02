using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CAT_Action_StartAnim : CAT_Action
{
	public GameObject target;
	public string animClipName;
	public WrapMode mode = WrapMode.Default;
	public float speed = 1.0f;

	public override CAT_Action InternalCopy()
	{
		CAT_Action_StartAnim copy = new CAT_Action_StartAnim();

		CopyCatActionMembers(copy);

		copy.target = target;
		copy.animClipName = animClipName;
		copy.mode = mode;
		copy.speed = speed;

		return copy;
	}

	protected override void OnStateChanged_Running()
	{
		if (target == null || animClipName == string.Empty || target.GetComponent<Animation>() == null)
		{
			ChangeState(CATState.Finished);
			return;
		}

		if (!target.activeInHierarchy)
			Debug.LogError("Trying to start an animation on an inactive object (" + target.name + ")");

		Animation anim = target.GetComponent<Animation>();

		anim.Stop();

		anim[animClipName].wrapMode = mode;
		anim[animClipName].speed = speed;
		if (speed < 0)
		{
			//we want to play it backwards
			anim[animClipName].time = anim[animClipName].length;
		}
		anim.Play(animClipName);
	}

	protected override void OnStateChanged_Stopping()
	{
		if (target != null && target.GetComponent<Animation>() != null && target.GetComponent<Animation>().isPlaying)
			target.GetComponent<Animation>().Stop();

		ChangeState(CATState.Finished);
	}

	public override void CATUpdate()
	{
		if (state == CATState.Running)
		{
			if (target.GetComponent<Animation>().isPlaying == false)
				ChangeState(CATState.Finished);
		}
	}

#if UNITY_EDITOR
	public override bool DrawCustomAction(UnityEngine.Object _target, UnityEditor.SerializedProperty propAction)
	{
		GameObject oldTarget = target;
		target = EditorGUILayout.ObjectField("Target", target, typeof(GameObject), true) as GameObject;

		if (oldTarget != target)
		{
			SetDirtyHack.SetDirty(_target);
		}

		if (target == null)
		{
			EditorGUILayout.HelpBox("You must set a target for this CAT_Action_StartAnim!", MessageType.Error);
			return true;
		}

		if (target.GetComponent<Animation>() == null)
		{
			EditorGUILayout.HelpBox("You must place an animation component on target object: " + target.name + "!", MessageType.Error);
			return true;
		}

		List<string> animClips = new List<string>();
		int selectedAnimIndex = 0;

		foreach (AnimationClip animClip in AnimationUtility.GetAnimationClips(target))
			if (animClip != null)
			{
				if (animClipName == animClip.name)
				{
					selectedAnimIndex = animClips.Count;
				}
				animClips.Add(animClip.name);
			}

		if (animClips.Count == 0)
		{
			EditorGUILayout.HelpBox("No animation clips on animation component on object " + target.name + "!", MessageType.Error);
			return true;
		}

		selectedAnimIndex = EditorGUILayout.Popup("Clip: " + AnimationUtility.GetAnimationClips(target)[selectedAnimIndex].length.ToString("F3"), selectedAnimIndex, animClips.ToArray());

		WrapMode oldMode = mode;
		mode = (WrapMode)EditorGUILayout.EnumPopup("Mode:", mode);
		if (oldMode != mode)
		{
			SetDirtyHack.SetDirty(_target);
		}

		float oldSpeed = speed;
		speed = EditorGUILayout.FloatField("Speed:", speed);
		if (oldSpeed != speed)
		{
			SetDirtyHack.SetDirty(_target);
		}

		string oldAnimClipName = animClipName;
		animClipName = animClips[selectedAnimIndex];
		if (oldAnimClipName != animClipName)
		{
			SetDirtyHack.SetDirty(_target);
		}

		return true;
	}
#endif
}
