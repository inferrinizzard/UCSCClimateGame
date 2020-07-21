using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class RegionController : MonoBehaviour {
	[HideInInspector] public static int _visited = 0;
	public RegionIntro intro;
	[SerializeField] GameObject introPrefab = default;
	protected GameObject introBlock;

	protected float timer = 60f;
	public float damage = 0f; // out of 100?

	[HideInInspector] public bool paused = false;
	protected bool updated = false;
	protected abstract void GameOver();

	public World.Region region;
	public static RegionController Instance;
	[SerializeField] SpriteRenderer[] backgrounds = default;

	protected virtual void Awake() {
		Instance = this;
		foreach (var s in backgrounds)
			s.transform.localScale = Vector3.one * GetScreenToWorldHeight / s.sprite.bounds.size.y;
	}

	public static float GetScreenToWorldHeight { get => Camera.main.ViewportToWorldPoint(new Vector2(1, 1)).y * 2; }
	public static float GetScreenToWorldWidth { get => Camera.main.ViewportToWorldPoint(new Vector2(1, 1)).x * 2; }

	public void AssignRegion(string name) => region = (World.Region) System.Enum.Parse(typeof(World.Region), name);

	public void Intro(int visited) {
		Debug.Log(intro);
		_visited = visited;
		if (intro[visited].Length == 0)
			return;
		SetPause(1);
		introBlock = Instantiate(introPrefab); // could read different prefab from scriptable obj per visit // store func calls on scriptable obj?
		var introText = introBlock.GetComponentInChildren<Text>();
		var introButton = introBlock.GetComponentInChildren<Button>(true);
		introButton?.onClick.AddListener(new UnityEngine.Events.UnityAction(() => SetPause(0)));
		StartCoroutine(UIController.ClickToAdvance(introText, intro[visited], introButton.gameObject));
	}

	protected virtual void Update() {
		if (timer < -1)
			return;
		if ((timer -= Time.deltaTime) <= 0) {
			timer = 0;
			GameOver();
			// start model thread
			// summon prompt
		}
	}

	void SetPause(int on) => paused = (Time.timeScale = 1 - on) == 0;

	protected void Pause(bool activatePrompt = true) {
		Debug.Log(timer);
		if (!paused) {
			SetPause(1);
			UIController.Instance.SetPrompt(activatePrompt);
			updated = false;
		}
	}

	protected void TriggerUpdate(System.Action updateEBM) {
		if (!updated) {
			updateEBM();
			updated = true;
		}
	}
}
