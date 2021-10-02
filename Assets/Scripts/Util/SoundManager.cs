//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;


//public class SoundManager : XTLink
//{
//	public enum MusicEmitterState
//	{
//		Free,
//		InUse
//	}

//	public static int InvalidSoundHandle = -1;

//	public AudioSource playOneShotSource;
//	public float fadeDuration = 0.5f;

//	public List<AudioSource> musicSoundSource = new List<AudioSource>();
//	public List<AudioSource> loopingSources = new List<AudioSource>();

//	public bool AllowDuckingInTurboSpin = false;

//	private List<MusicClip> musicClips = new List<MusicClip>();
//	private List<MusicClip> loopingClips = new List<MusicClip>();
//	private VS_MusicDuckByOneShot duckByOneShot = null;
//	private VS_OneShotCooldown oneShotCooldown = null;
//	private float volumeFX = 1.0f;
//	private float volumeMusic = 1.0f;
//	private float internalVolumeMusic = 1.0f;
//	private float internalVolumeFX = 1.0f;
//	private bool initDone = false;

//	private int nextFreeMusicHandler = 0;
//	private int nextFreeLoopingHandler = 0;

//	public override void XTRegisterCallbacks()
//	{
//		XT.RegisterCallbackEvent(Vars.Evt_Internal_Init_SoundManager, OnSoundManagerInit);
//		XT.RegisterCallbackEvent(Vars.Evt_DataToCode_Pressed_SoundBtn, OnSoundPressed);
//		XT.RegisterCallbackEvent(Vars.Evt_DataToCode_Pressed_SoundFXBtn, OnSoundFXPressed);
//		XT.RegisterCallbackEvent(Vars.Evt_DataToCode_Pressed_MusicBtn, OnMusicPressed);
//		XT.RegisterCallbackEvent(Vars.Evt_Internal_SoundStateChanged, OnSoundStateChanged);
//		XT.RegisterCallbackObject(Vars.MusicDuckByOneShotObject, OnMusicDuckByOneShotObject);
//		XT.RegisterCallbackObject(Vars.OneShotCooldownObject, OnOneShotCooldownObject);
//	}

//	public override void XTInitVariablesAndEvents()
//	{
//		XT.SetObject(Vars.SoundManagerObject, this);
//	}

//	protected override void OnDestroy()
//	{
//		XT.UnregisterCallbackEvent(OnSoundManagerInit);
//		XT.UnregisterCallbackEvent(OnSoundStateChanged);
//		XT.UnregisterCallbackEvent(OnSoundPressed);
//		XT.UnregisterCallbackEvent(OnSoundFXPressed);
//		XT.UnregisterCallbackEvent(OnMusicPressed);
//		XT.UnregisterCallbackObject(OnMusicDuckByOneShotObject);
//	}

//	private void OnMusicDuckByOneShotObject(object OSDucker)
//	{
//		duckByOneShot = OSDucker as VS_MusicDuckByOneShot;
//	}

//	private void OnOneShotCooldownObject(object OSCooler)
//	{
//		oneShotCooldown = OSCooler as VS_OneShotCooldown;
//	}

//	private void OnMusicPressed()
//	{
//		SoundState sndState = XT.GetObject(Vars.SoundState) as SoundState;
//		sndState.oldMusicIsOn = sndState.musicIsOn;
//		sndState.musicIsOn = !sndState.musicIsOn;

//		sndState.gameSoundIsOn = sndState.musicIsOn || sndState.soundFXIsOn;

//		XT.SetBool(Vars.MusicIsOn, sndState.musicIsOn);
//		XT.SetObject(Vars.SoundState, sndState);
//		XT.TriggerEvent(Vars.Evt_Internal_SoundStateChanged);

//		XT.TriggerEvent(Vars.Evt_ToServer_UpdateSettingsOnServer);
//	}

//	private void OnSoundFXPressed()
//	{
//		SoundState sndState = XT.GetObject(Vars.SoundState) as SoundState;
//		sndState.oldSoundFXIsOn = sndState.soundFXIsOn;
//		sndState.soundFXIsOn = !sndState.soundFXIsOn;

//		sndState.gameSoundIsOn = sndState.musicIsOn || sndState.soundFXIsOn;

//		XT.SetObject(Vars.SoundState, sndState);
//		XT.TriggerEvent(Vars.Evt_Internal_SoundStateChanged);

//		XT.TriggerEvent(Vars.Evt_ToServer_UpdateSettingsOnServer);
//	}

//	private void OnSoundPressed()
//	{
//		SoundState sndState = XT.GetObject(Vars.SoundState) as SoundState;

//		sndState.gameSoundIsOn = !sndState.gameSoundIsOn;
//		if (sndState.gameSoundIsOn)
//		{
//			sndState.musicIsOn = sndState.oldMusicIsOn;
//			sndState.soundFXIsOn = sndState.oldSoundFXIsOn;
//		}
//		else
//		{
//			sndState.oldMusicIsOn = sndState.musicIsOn;
//			sndState.oldSoundFXIsOn = sndState.soundFXIsOn;

//			if (!sndState.musicIsOn && !sndState.soundFXIsOn)
//				sndState.oldMusicIsOn = sndState.oldSoundFXIsOn = true;

//			sndState.musicIsOn = false;
//			sndState.soundFXIsOn = false;
//		}

//		XT.SetBool(Vars.MusicIsOn, sndState.musicIsOn);
//		XT.SetObject(Vars.SoundState, sndState);
//		XT.TriggerEvent(Vars.Evt_Internal_SoundStateChanged);

//		XT.TriggerEvent(Vars.Evt_ToServer_UpdateSettingsOnServer);
//	}


//	public void PlaySimple(AudioClip sound)
//	{
//		if (oneShotCooldown != null && oneShotCooldown.IsOnCooldown(sound))
//			return;

//		playOneShotSource.PlayOneShot(sound, AudioListener.volume * volumeFX * internalVolumeFX);

//		if (duckByOneShot != null)
//			if (sound != null)
//				duckByOneShot.OneShotStartsPlaying(sound, true);
//	}

//	public int PlayMusic(AudioClip music)//touched
//	{
//		if (music == null)
//			return -1;

//		MusicClip mClip = GetMusicClipByAudioClip(music);

//		PlayMusicClip(mClip, true);
//		return mClip.handler;
//	}

//	public MusicClip GetMusicClipByAudioClip(AudioClip clip)
//	{
//		if (clip == null)
//			return null;

//		MusicClip mClip = null;
//		for (int i = 0; i < musicClips.Count; i++)
//		{
//			if (musicClips[i].clip == clip)
//			{
//				mClip = musicClips[i];
//				break;
//			}
//		}

//		if (mClip == null)
//		{
//			mClip = new MusicClip();
//			mClip.maxVolume = 1f;
//			mClip.volumeFadeInDuration = fadeDuration;
//			mClip.volumeFadeOutDuration = fadeDuration;
//			mClip.clip = clip;
//		}

//		return mClip;
//	}

//	public void RestartMusic()//untouched, but should be fine
//	{
//		for (int handler = 0; handler < musicClips.Count; handler++)
//		{
//			if (musicClips[handler].isPlaying && musicSoundSource[handler].clip != null)
//				musicSoundSource[handler].Play();
//		}
//	}

//	public void StopMusic(ref int handle)
//	{
//		for (int i = 0; i < musicClips.Count; i++)
//		{
//			if (musicClips[i].handler == handle)
//				StopMusicClip(musicClips[i], true);
//		}

//		handle = InvalidSoundHandle;
//	}

//	public int PlayLooping(AudioClip sound)//touched
//	{
//		if (sound == null)
//			return -1;

//		MusicClip lClip = GetLoopingClipByAudioClip(sound);

//		PlayLoopingClip(lClip, false);
//		return lClip.handler;
//	}

//	public MusicClip GetLoopingClipByAudioClip(AudioClip sound)
//	{
//		if (sound == null)
//			return null;

//		MusicClip lClip = null;
//		for (int i = 0; i < loopingClips.Count; i++)
//		{
//			if (loopingClips[i].clip == sound)
//			{
//				lClip = loopingClips[i];
//				break;
//			}
//		}

//		if (lClip == null)
//		{
//			lClip = new MusicClip();
//			lClip.maxVolume = 1f;
//			lClip.volumeFadeInDuration = fadeDuration;
//			lClip.volumeFadeOutDuration = fadeDuration;
//			lClip.clip = sound;
//		}

//		return lClip;
//	}

//	public void StopLoopingSound(ref int handle)//touched
//	{
//		for (int i = 0; i < loopingClips.Count; i++)
//		{
//			if (loopingClips[i].handler == handle)
//				StopLoopingClip(loopingClips[i], false);
//		}

//		handle = InvalidSoundHandle;
//	}

//	public void MuteMusic()//touched
//	{
//		internalVolumeMusic = 0f;
//		XT.TriggerEvent(Vars.Evt_Internal_MuteMusic);
//	}

//	public void UnmuteMusic()//touched
//	{
//		internalVolumeMusic = 1f;
//		XT.TriggerEvent(Vars.Evt_Internal_UnmuteMusic);
//	}

//	public void MuteSoundFX()
//	{
//		internalVolumeFX = 0f;
//		UpdateSoundEmittersVolume();
//	}

//	public void UnmuteSoundFX()
//	{
//		internalVolumeFX = 1f;
//		UpdateSoundEmittersVolume();
//	}

//	public void MuteLoopingSound(int handle)//touched
//	{
//		for (int i = 0; i < loopingClips.Count; i++)
//		{
//			if (loopingClips[i].handler == handle)
//				MuteLoopingClip(loopingClips[i], false);
//		}
//	}

//	public void UnmuteLoopingSound(int handle)//touched
//	{
//		for (int i = 0; i < loopingClips.Count; i++)
//		{
//			if (loopingClips[i].handler == handle)
//				UnmuteLoopingClip(loopingClips[i], false);
//		}
//	}

//	private void OnSoundManagerInit()//touched
//	{
//		initDone = true;
//		OnSoundStateChanged();
//	}

//	private void OnSoundStateChanged()//untouched
//	{
//		if (!initDone)
//			return;

//		SoundState sndState = (XT.GetObject(Vars.SoundState) as SoundState);
//		volumeFX = 1f;
//		volumeMusic = 1f;

//		if (!sndState.globalSoundIsOn || !sndState.gameSoundIsOn)
//			volumeFX = volumeMusic = 0f;

//		if (!sndState.soundFXIsOn)
//			volumeFX = 0f;

//		if (!sndState.musicIsOn)
//			volumeMusic = 0f;

//		UpdateSoundEmittersVolume();
//	}

//	private void UpdateSoundEmittersVolume()
//	{
//		playOneShotSource.volume = volumeFX * internalVolumeFX;
//	}

//	private void PrepareNewMusicClip(MusicClip mClip)
//	{
//		int handler = -1;

//		if (nextFreeMusicHandler < musicSoundSource.Count)
//			handler = nextFreeMusicHandler;
//		else
//			handler = GetNewMusicSource();

//		nextFreeMusicHandler++;
//		musicSoundSource[handler].clip = mClip.clip;
//		musicSoundSource[handler].loop = true;
//		musicSoundSource[handler].spatialBlend = 0.0f;
//		mClip.handler = handler;
//		musicClips.Add(mClip);
//	}

//	private void PrepareNewLoopingClip(MusicClip lClip)
//	{
//		int handler = -1;

//		if (nextFreeLoopingHandler < loopingSources.Count)
//			handler = nextFreeLoopingHandler;
//		else
//			handler = GetNewLoopingSource();

//		nextFreeLoopingHandler++;
//		loopingSources[handler].clip = lClip.clip;
//		loopingSources[handler].loop = true;
//		loopingSources[handler].spatialBlend = 0.0f;
//		lClip.handler = handler;
//		loopingClips.Add(lClip);
//	}

//	private int GetNewMusicSource()
//	{
//		GameObject newMusicSource = new GameObject("MusicSoundSource" + musicSoundSource.Count);
//		newMusicSource.AddComponent<AudioSource>();
//		newMusicSource.GetComponent<AudioSource>().playOnAwake = false;
//		newMusicSource.transform.parent = transform;

//		newMusicSource.transform.localScale = Vector3.zero;
//		newMusicSource.transform.localPosition = Vector3.zero;
//		newMusicSource.transform.localEulerAngles = Vector3.zero;

//		musicSoundSource.Add(newMusicSource.GetComponent<AudioSource>());

//		return musicSoundSource.Count - 1;
//	}

//	private int GetNewLoopingSource()
//	{
//		GameObject newLoopingSource = new GameObject("LoopingSource" + loopingSources.Count);
//		newLoopingSource.AddComponent<AudioSource>();
//		newLoopingSource.GetComponent<AudioSource>().playOnAwake = false;
//		newLoopingSource.transform.parent = transform;

//		newLoopingSource.transform.localScale = Vector3.zero;
//		newLoopingSource.transform.localPosition = Vector3.zero;
//		newLoopingSource.transform.localEulerAngles = Vector3.zero;

//		loopingSources.Add(newLoopingSource.GetComponent<AudioSource>());

//		return loopingSources.Count - 1;
//	}

//	public void PlayMusicClip(MusicClip musicClip, bool useFading)
//	{
//		if (musicClip.handler == InvalidSoundHandle)
//			PrepareNewMusicClip(musicClip);

//		if (musicClip.isPlaying)
//			return;

//		musicClip.isPlaying = true;
//		musicSoundSource[musicClip.handler].volume = 0f;

//		if (!musicClip.isMuted)
//		{
//			if (useFading && musicClip.volumeFadeInDuration > 0f)
//			{
//				musicClip.volumeLevel = 0f;
//				musicClip.volumeState = MusicVolumeState.fadingIn;
//			}
//			else
//			{
//				musicClip.volumeLevel = 1f;
//				musicClip.volumeState = MusicVolumeState.idle;

//				if (XT.GetBool(Vars.MusicIsOn))
//					musicSoundSource[musicClip.handler].volume = musicClip.GetVolume() * internalVolumeMusic;
//			}
//		}

//		if (musicClip.clip != null)
//			musicSoundSource[musicClip.handler].Play();
//	}

//	public void StopMusicClip(MusicClip musicClip, bool useFading)
//	{
//		if (musicClip.handler == InvalidSoundHandle)
//			PrepareNewMusicClip(musicClip);

//		musicClip.isPlaying = false;

//		if (useFading && musicClip.volumeFadeOutDuration > 0f)
//			musicClip.volumeState = MusicVolumeState.fadingOut;
//		else
//		{
//			musicSoundSource[musicClip.handler].volume = 0f;
//			musicSoundSource[musicClip.handler].Stop();
//			musicClip.volumeLevel = 0f;
//			musicClip.volumeState = MusicVolumeState.idle;
//		}
//	}

//	public void MuteMusicClip(MusicClip musicClip, bool useFading)
//	{
//		if (musicClip.handler == InvalidSoundHandle)
//			PrepareNewMusicClip(musicClip);

//		musicClip.isMuted = true;

//		if (useFading && musicClip.volumeFadeOutDuration > 0f)
//			musicClip.volumeState = MusicVolumeState.fadingOut;
//		else
//		{
//			musicSoundSource[musicClip.handler].volume = 0f;
//			musicClip.volumeState = MusicVolumeState.idle;
//			musicClip.volumeLevel = 0f;
//		}
//	}

//	public void UnmuteMusicClip(MusicClip musicClip, bool useFading)
//	{
//		if (musicClip.handler == InvalidSoundHandle)
//			PrepareNewMusicClip(musicClip);

//		musicClip.isMuted = false;

//		if (useFading && musicClip.volumeFadeInDuration > 0f)
//			musicClip.volumeState = MusicVolumeState.fadingIn;
//		else
//		{
//			musicClip.volumeLevel = 1f;
//			musicClip.volumeState = MusicVolumeState.idle;

//			if (XT.GetBool(Vars.MusicIsOn))
//				musicSoundSource[musicClip.handler].volume = musicClip.GetVolume() * internalVolumeMusic;
//		}
//	}

//	public void DuckMusicClip(MusicClip musicClip, int duckingStepIndex, float delayToAutoUnduck, bool useFading)
//	{
//		if (!AllowDuckingInTurboSpin)
//			if (XT.GetBool(Vars.ContinuousSpin))
//				return;

//		if (musicClip.handler == InvalidSoundHandle)
//			PrepareNewMusicClip(musicClip);

//		if (delayToAutoUnduck > 0f)
//		{
//			if (!musicClip.duckingSteps[duckingStepIndex].autoUnduck)
//			{
//				musicClip.duckingSteps[duckingStepIndex].autoUnduck = true;
//				musicClip.duckingSteps[duckingStepIndex].duckCount++;
//			}

//			if (delayToAutoUnduck > musicClip.duckingSteps[duckingStepIndex].autoUnduckTimer)
//			{
//				musicClip.duckingSteps[duckingStepIndex].autoUnduckShouldFade = useFading && musicClip.duckingSteps[duckingStepIndex].fadeOutDuration > 0f;
//				musicClip.duckingSteps[duckingStepIndex].autoUnduckTimer = delayToAutoUnduck;
//			}
//		}
//		else
//		{
//			musicClip.duckingSteps[duckingStepIndex].duckCount++;
//		}

//		musicClip.UpdateDuckingStepPriority();

//		if (useFading && musicClip.duckingSteps[duckingStepIndex].fadeInDuration > 0f)
//		{
//			musicClip.duckingFadeInDuration = musicClip.duckingSteps[duckingStepIndex].fadeInDuration;
//			musicClip.duckingState = MusicVolumeState.fadingIn;
//		}
//		else
//		{
//			musicClip.duckingVolume = musicClip.duckingSteps[musicClip.priorityDuckingStepIndex].targetVolume;
//			musicClip.duckingState = MusicVolumeState.idle;

//			if (XT.GetBool(Vars.MusicIsOn))
//				musicSoundSource[musicClip.handler].volume = musicClip.GetVolume() * internalVolumeMusic;
//		}
//	}

//	public void UnduckMusicClip(MusicClip musicClip, int duckingStepIndex, bool useFading)
//	{
//		if (musicClip.handler == InvalidSoundHandle)
//			PrepareNewMusicClip(musicClip);

//		if (musicClip.duckingSteps[duckingStepIndex].duckCount > 0)
//		{
//			if (musicClip.duckingSteps[duckingStepIndex].duckCount == 1 && musicClip.duckingSteps[duckingStepIndex].autoUnduck)
//				return;

//			musicClip.duckingSteps[duckingStepIndex].duckCount--;

//			if (musicClip.duckingSteps[duckingStepIndex].duckCount > 0)
//				return;
//		}

//		musicClip.UpdateDuckingStepPriority();

//		if (useFading && musicClip.duckingSteps[duckingStepIndex].fadeOutDuration > 0f)
//		{
//			if (musicClip.priorityDuckingStepIndex == -1)
//			{
//				musicClip.duckingFadeOutDuration = musicClip.duckingSteps[duckingStepIndex].fadeOutDuration;
//				musicClip.duckingState = MusicVolumeState.fadingOut;
//			}
//			else
//			{
//				musicClip.duckingFadeInDuration = musicClip.duckingSteps[duckingStepIndex].fadeInDuration;
//				musicClip.duckingFadeOutDuration = musicClip.duckingSteps[duckingStepIndex].fadeOutDuration;
//				musicClip.duckingState = MusicVolumeState.fadingIn;
//			}
//		}
//		else
//		{
//			if (musicClip.priorityDuckingStepIndex == -1)
//				musicClip.duckingVolume = 1f;
//			else
//				musicClip.duckingVolume = musicClip.duckingSteps[musicClip.priorityDuckingStepIndex].targetVolume;

//			musicClip.duckingState = MusicVolumeState.idle;

//			if (XT.GetBool(Vars.MusicIsOn))
//				musicSoundSource[musicClip.handler].volume = musicClip.GetVolume() * internalVolumeMusic;
//		}
//	}

//	public void PlayLoopingClip(MusicClip loopingClip, bool useFading)
//	{
//		if (loopingClip.handler == InvalidSoundHandle)
//			PrepareNewLoopingClip(loopingClip);

//		if (loopingClip.isPlaying)
//			return;

//		loopingClip.isPlaying = true;
//		loopingSources[loopingClip.handler].volume = 0f;

//		if (!loopingClip.isMuted)
//		{
//			if (useFading && loopingClip.volumeFadeInDuration > 0f)
//			{
//				loopingClip.volumeLevel = 0f;
//				loopingClip.volumeState = MusicVolumeState.fadingIn;
//			}
//			else
//			{
//				loopingClip.volumeLevel = 1f;
//				loopingClip.volumeState = MusicVolumeState.idle;

//				SoundState sndState = XT.GetObject(Vars.SoundState) as SoundState;
//				if (sndState.soundFXIsOn)
//					loopingSources[loopingClip.handler].volume = loopingClip.GetVolume() * internalVolumeFX;
//			}
//		}

//		if (loopingClip.clip != null)
//			loopingSources[loopingClip.handler].Play();
//	}

//	public void StopLoopingClip(MusicClip loopingClip, bool useFading)
//	{
//		if (loopingClip.handler == InvalidSoundHandle)
//			PrepareNewLoopingClip(loopingClip);

//		loopingClip.isPlaying = false;

//		if (useFading && loopingClip.volumeFadeOutDuration > 0f)
//			loopingClip.volumeState = MusicVolumeState.fadingOut;
//		else
//		{
//			loopingSources[loopingClip.handler].volume = 0f;
//			loopingSources[loopingClip.handler].Stop();
//			loopingClip.volumeLevel = 0f;
//			loopingClip.volumeState = MusicVolumeState.idle;
//		}
//	}

//	public void MuteLoopingClip(MusicClip loopingClip, bool useFading)
//	{
//		if (loopingClip.handler == InvalidSoundHandle)
//			PrepareNewLoopingClip(loopingClip);

//		loopingClip.isMuted = true;

//		if (useFading && loopingClip.volumeFadeOutDuration > 0f)
//			loopingClip.volumeState = MusicVolumeState.fadingOut;
//		else
//		{
//			loopingSources[loopingClip.handler].volume = 0f;
//			loopingClip.volumeState = MusicVolumeState.idle;
//			loopingClip.volumeLevel = 0f;
//		}
//	}

//	public void UnmuteLoopingClip(MusicClip loopingClip, bool useFading)
//	{
//		if (loopingClip.handler == InvalidSoundHandle)
//			PrepareNewLoopingClip(loopingClip);

//		loopingClip.isMuted = false;

//		if (useFading && loopingClip.volumeFadeInDuration > 0f)
//			loopingClip.volumeState = MusicVolumeState.fadingIn;
//		else
//		{
//			loopingClip.volumeLevel = 1f;
//			loopingClip.volumeState = MusicVolumeState.idle;

//			SoundState sndState = XT.GetObject(Vars.SoundState) as SoundState;
//			if (sndState.soundFXIsOn)
//				loopingSources[loopingClip.handler].volume = loopingClip.GetVolume() * internalVolumeFX;
//		}
//	}

//	public void DuckLoopingClip(MusicClip loopingClip, int duckingStepIndex, float delayToAutoUnduck, bool useFading)
//	{
//		if (loopingClip.handler == InvalidSoundHandle)
//			PrepareNewLoopingClip(loopingClip);

//		if (delayToAutoUnduck > 0f)
//		{
//			if (!loopingClip.duckingSteps[duckingStepIndex].autoUnduck)
//			{
//				loopingClip.duckingSteps[duckingStepIndex].autoUnduck = true;
//				loopingClip.duckingSteps[duckingStepIndex].duckCount++;
//			}

//			if (delayToAutoUnduck > loopingClip.duckingSteps[duckingStepIndex].autoUnduckTimer)
//			{
//				loopingClip.duckingSteps[duckingStepIndex].autoUnduckShouldFade = useFading && loopingClip.duckingSteps[duckingStepIndex].fadeOutDuration > 0f;
//				loopingClip.duckingSteps[duckingStepIndex].autoUnduckTimer = delayToAutoUnduck;
//			}
//		}
//		else
//		{
//			loopingClip.duckingSteps[duckingStepIndex].duckCount++;
//		}

//		loopingClip.UpdateDuckingStepPriority();

//		if (useFading && loopingClip.duckingSteps[duckingStepIndex].fadeInDuration > 0f)
//		{
//			loopingClip.duckingFadeInDuration = loopingClip.duckingSteps[duckingStepIndex].fadeInDuration;
//			loopingClip.duckingState = MusicVolumeState.fadingIn;
//		}
//		else
//		{
//			loopingClip.duckingVolume = loopingClip.duckingSteps[loopingClip.priorityDuckingStepIndex].targetVolume;
//			loopingClip.duckingState = MusicVolumeState.idle;

//			SoundState sndState = XT.GetObject(Vars.SoundState) as SoundState;
//			if (sndState.soundFXIsOn)
//				loopingSources[loopingClip.handler].volume = loopingClip.GetVolume() * internalVolumeFX;
//		}
//	}

//	public void UnduckLoopingClip(MusicClip loopingClip, int duckingStepIndex, bool useFading)
//	{
//		if (loopingClip.handler == InvalidSoundHandle)
//			PrepareNewLoopingClip(loopingClip);

//		if (loopingClip.duckingSteps[duckingStepIndex].duckCount > 0)
//		{
//			if (loopingClip.duckingSteps[duckingStepIndex].duckCount == 1 && loopingClip.duckingSteps[duckingStepIndex].autoUnduck)
//				return;

//			loopingClip.duckingSteps[duckingStepIndex].duckCount--;

//			if (loopingClip.duckingSteps[duckingStepIndex].duckCount > 0)
//				return;
//		}

//		loopingClip.UpdateDuckingStepPriority();

//		if (useFading && loopingClip.duckingSteps[duckingStepIndex].fadeOutDuration > 0f)
//		{
//			if (loopingClip.priorityDuckingStepIndex == -1)
//			{
//				loopingClip.duckingFadeOutDuration = loopingClip.duckingSteps[duckingStepIndex].fadeOutDuration;
//				loopingClip.duckingState = MusicVolumeState.fadingOut;
//			}
//			else
//			{
//				loopingClip.duckingFadeInDuration = loopingClip.duckingSteps[duckingStepIndex].fadeInDuration;
//				loopingClip.duckingFadeOutDuration = loopingClip.duckingSteps[duckingStepIndex].fadeOutDuration;
//				loopingClip.duckingState = MusicVolumeState.fadingIn;
//			}
//		}
//		else
//		{
//			if (loopingClip.priorityDuckingStepIndex == -1)
//				loopingClip.duckingVolume = 1f;
//			else
//				loopingClip.duckingVolume = loopingClip.duckingSteps[loopingClip.priorityDuckingStepIndex].targetVolume;

//			loopingClip.duckingState = MusicVolumeState.idle;

//			SoundState sndState = XT.GetObject(Vars.SoundState) as SoundState;
//			if (sndState.soundFXIsOn)
//				loopingSources[loopingClip.handler].volume = loopingClip.GetVolume() * internalVolumeFX;
//		}
//	}

//	protected void UpdateMusicVolume()
//	{
//		for (int i = 0; i < musicClips.Count; i++)
//		{
//			if (musicSoundSource[musicClips[i].handler].volume != musicClips[i].GetVolume() * volumeMusic * internalVolumeMusic)
//				musicSoundSource[musicClips[i].handler].volume = musicClips[i].GetVolume() * volumeMusic * internalVolumeMusic;
//		}
//	}

//	protected void UpdateLoopingClipsVolume()
//	{
//		for (int i = 0; i < loopingClips.Count; i++)
//		{
//			if (loopingSources[loopingClips[i].handler].volume != loopingClips[i].GetVolume() * volumeFX * internalVolumeFX)
//				loopingSources[loopingClips[i].handler].volume = loopingClips[i].GetVolume() * volumeFX * internalVolumeFX;
//		}
//	}

//	protected void ComputeMusicClipsVolume()
//	{
//		for (int i = 0; i < musicClips.Count; i++)
//		{
//			if (musicClips[i].volumeState == MusicVolumeState.fadingIn)
//			{
//				if (musicClips[i].volumeLevel < 1f)
//					musicClips[i].volumeLevel += Time.deltaTime * (1f / musicClips[i].volumeFadeInDuration);

//				if (musicClips[i].volumeLevel >= 1f)
//				{
//					musicClips[i].volumeLevel = 1f;
//					musicClips[i].volumeState = MusicVolumeState.idle;
//				}
//			}
//			else if (musicClips[i].volumeState == MusicVolumeState.fadingOut)
//			{
//				if (musicClips[i].volumeLevel > 0f)
//					musicClips[i].volumeLevel -= Time.deltaTime * (1f / musicClips[i].volumeFadeOutDuration);

//				if (musicClips[i].volumeLevel <= 0f)
//				{
//					musicClips[i].volumeLevel = 0f;
//					musicClips[i].volumeState = MusicVolumeState.idle;

//					if (!musicClips[i].isPlaying)
//						StopMusicClip(musicClips[i], false);
//				}
//			}

//			for (int j = 0; j < musicClips[i].duckingSteps.Count; j++)
//			{
//				if (musicClips[i].duckingSteps[j].autoUnduck)
//				{
//					if (musicClips[i].duckingSteps[j].autoUnduckTimer > 0f)
//						musicClips[i].duckingSteps[j].autoUnduckTimer -= Time.deltaTime;
//					else
//					{
//						musicClips[i].duckingSteps[j].autoUnduck = false;
//						UnduckMusicClip(musicClips[i], j, musicClips[i].duckingSteps[j].autoUnduckShouldFade);
//					}
//				}
//			}

//			if (musicClips[i].duckingState == MusicVolumeState.fadingIn)
//			{
//				if (musicClips[i].duckingVolume < musicClips[i].duckingSteps[musicClips[i].priorityDuckingStepIndex].targetVolume)
//				{
//					musicClips[i].duckingVolume += Time.deltaTime * (1f / musicClips[i].duckingFadeOutDuration);

//					if (musicClips[i].duckingVolume >= musicClips[i].duckingSteps[musicClips[i].priorityDuckingStepIndex].targetVolume)
//					{
//						musicClips[i].duckingVolume = musicClips[i].duckingSteps[musicClips[i].priorityDuckingStepIndex].targetVolume;
//						musicClips[i].duckingState = MusicVolumeState.idle;
//					}
//				}
//				else if (musicClips[i].duckingVolume > musicClips[i].duckingSteps[musicClips[i].priorityDuckingStepIndex].targetVolume)
//				{
//					musicClips[i].duckingVolume -= Time.deltaTime * (1f / musicClips[i].duckingFadeInDuration);

//					if (musicClips[i].duckingVolume <= musicClips[i].duckingSteps[musicClips[i].priorityDuckingStepIndex].targetVolume)
//					{
//						musicClips[i].duckingVolume = musicClips[i].duckingSteps[musicClips[i].priorityDuckingStepIndex].targetVolume;
//						musicClips[i].duckingState = MusicVolumeState.idle;
//					}
//				}
//			}
//			else if (musicClips[i].duckingState == MusicVolumeState.fadingOut)
//			{
//				if (musicClips[i].duckingVolume < 1f)
//					musicClips[i].duckingVolume += Time.deltaTime * (1f / musicClips[i].duckingFadeOutDuration);

//				if (musicClips[i].duckingVolume >= 1f)
//				{
//					musicClips[i].duckingVolume = 1f;
//					musicClips[i].duckingState = MusicVolumeState.idle;
//				}
//			}
//		}
//	}

//	protected void ComputeLoopingClipsVolume()
//	{
//		for (int i = 0; i < loopingClips.Count; i++)
//		{
//			if (loopingClips[i].volumeState == MusicVolumeState.fadingIn)
//			{
//				if (loopingClips[i].volumeLevel < 1f)
//					loopingClips[i].volumeLevel += Time.deltaTime * (1f / loopingClips[i].volumeFadeInDuration);

//				if (loopingClips[i].volumeLevel >= 1f)
//				{
//					loopingClips[i].volumeLevel = 1f;
//					loopingClips[i].volumeState = MusicVolumeState.idle;
//				}
//			}
//			else if (loopingClips[i].volumeState == MusicVolumeState.fadingOut)
//			{
//				if (loopingClips[i].volumeLevel > 0f)
//					loopingClips[i].volumeLevel -= Time.deltaTime * (1f / loopingClips[i].volumeFadeOutDuration);

//				if (loopingClips[i].volumeLevel <= 0f)
//				{
//					loopingClips[i].volumeLevel = 0f;
//					loopingClips[i].volumeState = MusicVolumeState.idle;

//					if (!loopingClips[i].isPlaying)
//						StopLoopingClip(loopingClips[i], false);
//				}
//			}

//			for (int j = 0; j < loopingClips[i].duckingSteps.Count; j++)
//			{
//				if (loopingClips[i].duckingSteps[j].autoUnduck)
//				{
//					if (loopingClips[i].duckingSteps[j].autoUnduckTimer > 0f)
//						loopingClips[i].duckingSteps[j].autoUnduckTimer -= Time.deltaTime;
//					else
//					{
//						loopingClips[i].duckingSteps[j].autoUnduck = false;
//						UnduckLoopingClip(loopingClips[i], j, loopingClips[i].duckingSteps[j].autoUnduckShouldFade);
//					}
//				}
//			}

//			if (loopingClips[i].duckingState == MusicVolumeState.fadingIn)
//			{
//				if (loopingClips[i].duckingVolume < loopingClips[i].duckingSteps[loopingClips[i].priorityDuckingStepIndex].targetVolume)
//				{
//					loopingClips[i].duckingVolume += Time.deltaTime * (1f / loopingClips[i].duckingFadeOutDuration);

//					if (loopingClips[i].duckingVolume >= loopingClips[i].duckingSteps[loopingClips[i].priorityDuckingStepIndex].targetVolume)
//					{
//						loopingClips[i].duckingVolume = loopingClips[i].duckingSteps[loopingClips[i].priorityDuckingStepIndex].targetVolume;
//						loopingClips[i].duckingState = MusicVolumeState.idle;
//					}
//				}
//				else if (loopingClips[i].duckingVolume > loopingClips[i].duckingSteps[loopingClips[i].priorityDuckingStepIndex].targetVolume)
//				{
//					loopingClips[i].duckingVolume -= Time.deltaTime * (1f / loopingClips[i].duckingFadeInDuration);

//					if (loopingClips[i].duckingVolume <= loopingClips[i].duckingSteps[loopingClips[i].priorityDuckingStepIndex].targetVolume)
//					{
//						loopingClips[i].duckingVolume = loopingClips[i].duckingSteps[loopingClips[i].priorityDuckingStepIndex].targetVolume;
//						loopingClips[i].duckingState = MusicVolumeState.idle;
//					}
//				}
//			}
//			else if (loopingClips[i].duckingState == MusicVolumeState.fadingOut)
//			{
//				if (loopingClips[i].duckingVolume < 1f)
//					loopingClips[i].duckingVolume += Time.deltaTime * (1f / loopingClips[i].duckingFadeOutDuration);

//				if (loopingClips[i].duckingVolume >= 1f)
//				{
//					loopingClips[i].duckingVolume = 1f;
//					loopingClips[i].duckingState = MusicVolumeState.idle;
//				}
//			}
//		}
//	}

//	private void Update()
//	{
//		ComputeMusicClipsVolume();
//		ComputeLoopingClipsVolume();
//		UpdateMusicVolume();
//		UpdateLoopingClipsVolume();
//	}
//}
