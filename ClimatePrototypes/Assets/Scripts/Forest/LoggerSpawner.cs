using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class LoggerSpawner : MonoBehaviour {
	[SerializeField] GameObject loggerPrefab = default;
	[SerializeField] float interval = 15;
	void Start() {
		var loggerSpawner = StartCoroutine(SpawnLogger(interval));
	}

	IEnumerator SpawnLogger(float delay) {
		yield return new WaitForSeconds(delay);
		if (ForestController.Instance.activeTrees.Count > 0) {
			// shuffle, sort, shift
			var targetIndex = (int) (Random.value * ForestController.Instance.activeTrees.Count);
			var target = ForestController.Instance.activeTrees[targetIndex];
			ForestController.Instance.activeTrees.RemoveAt(targetIndex);

			SetLoggerTarget(target, LoggerActions.Chop);
		}
		StartCoroutine(SpawnLogger(delay));
	}

	public void SetLoggerTarget(Vector3Int pos, UnityEngine.Events.UnityAction<Logger> onReached) {
		var newLogger = ForestController.Instance.NewAgent(loggerPrefab, transform.position, (Vector3) pos) as Logger;
		newLogger.choppingTile = pos;

		newLogger.OnReached.AddListener((PathfindingAgent agent) => onReached.Invoke(agent as Logger));
		newLogger.OnReturn.AddListener(() => { Debug.Log("logger returned"); });
	}
}
