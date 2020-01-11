using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour {
    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public static void Transition(string scene) {
        SceneManager.LoadScene(scene);
    }

    static IEnumerator LoadScene(string name) {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(name);
        asyncLoad.allowSceneActivation = false;
        while (!asyncLoad.isDone) {
            // loading scene here
            if (asyncLoad.progress >.9f)
                asyncLoad.allowSceneActivation = true;
            yield return null;
        }
    }
}
