using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomLoader : NetworkSceneManagerBase
{
    // Canvas to be shown on room loads
    [SerializeField] Canvas loadingCanvas;

    // Text object to log debugs
    [SerializeField] TMPro.TMP_Text debugText;

    private void Awake()
    {
        loadingCanvas.gameObject.SetActive(false);
    }

    protected override IEnumerator SwitchScene(SceneRef prevScene, SceneRef newScene, FinishedLoadingDelegate finished)
    {
        loadingCanvas.gameObject.SetActive(true);

        LogText($"Switching Scene from <color=yellow><b>{prevScene}</b></color> to <color=yellow><b>{newScene}</b></color>");

        // list of networked objects in the scene
        List<NetworkObject> sceneObjects = new List<NetworkObject>();

        yield return SceneManager.LoadSceneAsync(newScene, LoadSceneMode.Single);

        var loadedScene = SceneManager.GetSceneAt(newScene);
        LogText($"Loaded scene: <color=lime><b>{loadedScene}</b></color>");
        sceneObjects = FindNetworkObjects(loadedScene, disable: false);

        // Delay one frame
        yield return null;
        finished(sceneObjects);

        LogText($"Switched Scene from <color=yellow><b>{prevScene}</b></color> to <color=yellow><b>{newScene}</b></color> - loaded <color=yellow><b>{sceneObjects.Count}</b></color> scene objects");

        loadingCanvas.gameObject.SetActive(false);
    }

    public void LogText(string msg)
    {
        if (debugText)
            debugText.text += "\n" + msg;

        Debug.Log(msg);
    }
}
