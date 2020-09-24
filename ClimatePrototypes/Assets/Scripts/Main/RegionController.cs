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
	[SerializeField] protected Text timerText = default;
	public float damage = 0f; // out of 100?

	[HideInInspector] public bool paused = false;
	protected bool updated = false;
	protected virtual void Init() { }

	public World.Region region;
	protected static RegionController instance;
	[SerializeField] SpriteRenderer[] backgrounds = default;

	Material fadeMat;

	protected virtual void Awake() {
		instance = this;
		foreach (var s in backgrounds)
			s.transform.localScale = Vector3.one * GetScreenToWorldHeight / s.sprite.bounds.size.y;
	}

	protected virtual void Start() { fadeMat = new Material(Shader.Find("Screen/Fade")); }

	public static float GetScreenToWorldHeight { get => Camera.main.ViewportToWorldPoint(Vector2.one).y - Camera.main.ViewportToWorldPoint(Vector2.zero).y; }
	public static float GetScreenToWorldWidth { get => Camera.main.ViewportToWorldPoint(Vector2.one).x - Camera.main.ViewportToWorldPoint(Vector2.zero).x; }

	public void AssignRegion(string name) => region = (World.Region) System.Enum.Parse(typeof(World.Region), name);

	public void Intro(int visited) => StartCoroutine(IntroRoutine(visited));

	IEnumerator IntroRoutine(int visited, float time = .5f) {
		yield return StartCoroutine(FadeIn(time));
		_visited = visited;
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
		timerText.text = $"{Mathf.Floor(timer)}";
		if (timer < -1)
			return;
		if (timer <= 0) {
			timer = -2;
			GameOver();
			StartModel();
			// summon prompt
		}
	}

	protected virtual void GameOver() {
		timerText.text = "0";
		UIController.Instance.SetPrompt(true);
		Pause();
	}

	protected virtual void StartModel() {
		if (GameManager.Instance.runModel && !GameManager.Instance.runningModel) {
			GameManager.Instance.runningModel = true;
			System.Threading.Thread calcThread = new System.Threading.Thread(() => { World.Calc(); GameManager.Instance.runningModel = false; });
			calcThread.Priority = System.Threading.ThreadPriority.AboveNormal;
			calcThread.Start();
		}
	}

	IEnumerator FadeIn(float time) {
		for (var(start, step) = (Time.time, 0f); step < time; step = Time.time - start) {
			yield return null;
			fadeMat.SetFloat("_Alpha", 1 - step / time); // slow
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

	void OnRenderImage(RenderTexture src, RenderTexture dest) {
		Graphics.Blit(src, dest, fadeMat);
	}
}
