using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound {
	public string name { get => clip.name; }
	public AudioClip clip;
	public AudioManager.AudioType type;
	[Range(0, 1)] public float volume = .5f;
	[Range(-3f, 3f)] public float pitch = 1;
	[HideInInspector] public AudioSource source;

	public Sound(AudioClip clip, float volume = .5f, float pitch = 1) {
		this.clip = clip;
		this.volume = volume;
		this.pitch = pitch;
	}

	public override string ToString() => $"Sound:{name} @{{volume:{volume}, pitch:{pitch}}}";

	public void Play() => AudioManager.Instance.Play(this);
}
