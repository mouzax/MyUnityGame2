using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoSaveOnQuit : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";

    void OnApplicationQuit() { DoSave(); }
    void OnApplicationPause(bool paused) { if (paused) DoSave(); }

    void DoSave()
    {
        var playerObj = GameObject.FindGameObjectWithTag(playerTag);
        if (!playerObj) return;

        var pos = playerObj.transform.position;
        var scene = SceneManager.GetActiveScene().name;
        SaveGame.Save(scene, pos);
    }
}