using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawnOnLoad : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private bool onlyIfSceneMatches = true;

    void Start()
    {
        if (!player)
        {
            var found = GameObject.FindGameObjectWithTag("Player");
            if (found) player = found.transform;
        }

        if (!player) return;

        if (SaveGame.TryGet(out string savedScene, out Vector3 savedPos))
        {
            if (!onlyIfSceneMatches || savedScene == SceneManager.GetActiveScene().name)
                player.position = savedPos;
        }
    }
}