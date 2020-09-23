using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Audio;

public class SoundTester : MonoBehaviour {
	[System.Serializable] public class SoundTestPair { public KeyCode key; public AudioClip clip; }
	public List<SoundTestPair> sounds;

	void Update() {
		foreach (var pair in sounds)
			if (Input.GetKeyDown(pair.key))
				AudioManager.Instance.PlaySound(pair.clip);
	}
}
