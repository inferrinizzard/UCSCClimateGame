using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound {
	public string name { get => clip.name; }
	public AudioClip clip;
	public AudioManager.AudioType type;
	[Range(0, 1)] public float volume = 1;
	[Range(.1f, .3f)] public float pitch = 1;
	[HideInInspector] public AudioSource source;
}
