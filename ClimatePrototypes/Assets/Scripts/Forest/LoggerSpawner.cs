using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class LoggerSpawner : MonoBehaviour {
	[SerializeField] GameObject loggerPrefab = default;
	[SerializeField] float interval = 5;
	void Start() {
		var loggerSpawner = StartCoroutine(SpawnLogger(interval));
	}

	IEnumerator SpawnLogger(float delay) {
		yield return new WaitForSeconds(delay);
		if ((ForestController.Instance as ForestController).activeTrees.Count > 0) {
			var targetIndex = (int) (Random.value * (ForestController.Instance as ForestController).activeTrees.Count);
			var target = (ForestController.Instance as ForestController).activeTrees[targetIndex];
			(ForestController.Instance as ForestController).activeTrees.RemoveAt(targetIndex);

			SetLoggerTarget(target, LoggerActions.Chop);
		}
		StartCoroutine(SpawnLogger(delay));
	}

	public void SetLoggerTarget(Vector3Int pos, UnityEngine.Events.UnityAction<Logger> onReached) {
		var newLogger = (ForestController.Instance as ForestController).NewAgent(loggerPrefab, transform.position, (Vector3) pos) as Logger;
		newLogger.choppingTile = pos;

		newLogger.OnReached.AddListener((PathfindingAgent agent) => onReached.Invoke(agent as Logger));
		newLogger.OnReturn.AddListener(() => { Debug.Log("logger returned"); });
	}
}
