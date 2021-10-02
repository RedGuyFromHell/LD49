using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CAT_Action_PlayAudioClip : CAT_Action
{
	public AudioClip audioClip;

	public override CAT_Action InternalCopy()
	{
		CAT_Action_PlayAudioClip copy = new CAT_Action_PlayAudioClip();

		CopyCatActionMembers(copy);

		copy.audioClip = audioClip;

		return copy;
	}

	protected override void OnStateChanged_Running()
	{
		//SoundManager mgr = XT.GetObject(Vars.SoundManagerObject) as SoundManager;

		//mgr.PlaySimple(audioClip);

		//ChangeState(CATState.Finished);
	}
}
