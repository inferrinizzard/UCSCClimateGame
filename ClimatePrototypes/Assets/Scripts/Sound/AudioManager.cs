using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : Singleton<AudioManager> {
	public Sound[] sounds;
	AudioSource sfxSource1,
	sfxSource2,
	sfxSource3,
	musicSource;
	public enum AudioType { SFX, Music }

	void Start() {
		while (GetComponents<AudioSource>().Length > 0)
			Destroy(GetComponent<AudioSource>());
		sfxSource1 = gameObject.AddComponent<AudioSource>();
		sfxSource2 = gameObject.AddComponent<AudioSource>();
		sfxSource3 = gameObject.AddComponent<AudioSource>();
		musicSource = gameObject.AddComponent<AudioSource>();
	}

	public void Play(Sound sound) {
		AudioSource channel = GetChannel(sound.type);
		sound.source = channel;
		channel.volume = sound.volume;
		channel.pitch = sound.pitch;
		channel.Play();
	}

	AudioSource GetChannel(AudioType type) {
		if (type == AudioType.Music)
			return musicSource;
		else {
			if (sfxSource1.clip == null)
				return sfxSource1;
			if (sfxSource2.clip == null)
				return sfxSource2;
			return sfxSource3;
		}
	}
}
