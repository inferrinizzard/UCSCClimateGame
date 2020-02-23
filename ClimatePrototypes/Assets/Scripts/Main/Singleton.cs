using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component {
	protected static T instance;
	public static T Instance {
		get {
			if (!instance) {
				instance = FindObjectOfType<T>();
				if (!instance) {
					instance = new GameObject().AddComponent<T>();
					instance.name = typeof(T).Name;
				}
			}
			return instance;
		}
	}

	public virtual void Awake() {
		if (!instance) {
			instance = this as T;
			DontDestroyOnLoad(this.gameObject);
		} else
			Destroy(gameObject);
	}
}
