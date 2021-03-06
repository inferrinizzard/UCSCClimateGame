using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class RegionController : MonoBehaviour {
	[HideInInspector] public int visits = 0;
	[SerializeField] GameObject introPrefab = default;
	public RegionIntro intro;
	protected GameObject introBlock;

	[SerializeField] protected Text timerText = default;
	protected float timer = 60f;
	public float damage = 0f; // out of 100?

	[HideInInspector] public bool paused = false;
	protected bool updated = false; // TODO: this variable currently does nothing
	protected virtual void Init() { }

	public World.Region region;
	protected static RegionController instance;
	[SerializeField] SpriteRenderer[] backgrounds = default;

	protected virtual void Awake() {
		instance = this;
		foreach (var s in backgrounds)
			s.transform.localScale = Vector3.one * GetScreenToWorldHeight / s.sprite.bounds.size.y;
	}

	public static float GetScreenToWorldHeight { get => Camera.main.ViewportToWorldPoint(Vector2.one).y - Camera.main.ViewportToWorldPoint(Vector2.zero).y; } // TODO: global variable class
	public static float GetScreenToWorldWidth { get => Camera.main.ViewportToWorldPoint(Vector2.one).x - Camera.main.ViewportToWorldPoint(Vector2.zero).x; } // TODO: global variable class

	public void AssignRegion(string name) => region = (World.Region) System.Enum.Parse(typeof(World.Region), name);

	public void Intro(int visited) => StartCoroutine(IntroRoutine(visited));

	IEnumerator IntroRoutine(int visited, float time = .5f) {
		yield return StartCoroutine(Camera.main.GetComponent<CameraFade>().FadeIn(time));
		visits = visited;
		if (intro[visited].Length == 0)
			yield break;
		SetPause(1);
		introBlock = Instantiate(introPrefab); // could read different prefab from scriptable obj per visit // store func calls on scriptable obj?
		var introText = introBlock.GetComponentInChildren<Text>();
		var introButton = introBlock.GetComponentInChildren<Button>(true);
		introButton?.onClick.AddListener(new UnityEngine.Events.UnityAction(() => SetPause(0)));
		yield return StartCoroutine(UIController.ClickToAdvance(introText, intro[visited], introButton.gameObject));
		Init();
	}

	protected virtual void Update() {
		timer -= Time.deltaTime;
		timerText.text = $"{Mathf.Max(0, Mathf.Floor(timer))}";
		if (timer < -1) // if region does not have timer, like City
			return;
		if (timer <= 0) {
			timer = -2; // -2 is finished state
			GameOver();
			StartModel();
		}
	}

	protected virtual void GameOver() {
		timerText.text = "0";
		UIController.Instance.SetPrompt(true);
		Pause();
	}

	/// <summary> Opens a new thread to run the EBM model in the background while Unity manages UI </summary>
	protected virtual void StartModel() {
		if (GameManager.Instance.runModel && !GameManager.Instance.runningModel) {
			GameManager.Instance.runningModel = true;
			System.Threading.Thread calcThread = new System.Threading.Thread(() => { World.Calc(); GameManager.Instance.runningModel = false; });
			calcThread.Priority = System.Threading.ThreadPriority.AboveNormal;
			calcThread.Start();
		}
	}

	void SetPause(int on) => paused = (Time.timeScale = 1 - on) == 0;

	protected void Pause() {
		if (!paused) {
			SetPause(1);
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
