using UnityEngine;
using System.Collections;

public class LightsOut : MonoBehaviour
{
    public GameObject electricBoxPrefab;
    public Transform[] spawnPoints;
    public GameObject[] lightsToControl;
    public float flickerDuration = 3f;
    public float flickerInterval = 0.1f;

    private GameObject spawnedBox;

    public void StartEvent()
    {
        Debug.Log("LightsOut started!");
        StartCoroutine(LightsOutSequence());
    }

    IEnumerator LightsOutSequence()
    {
        foreach (var lightObj in lightsToControl)
            if (lightObj) lightObj.SetActive(false);

        float elapsed = 0f;
        while (elapsed < flickerDuration)
        {
            foreach (var lightObj in lightsToControl)
                if (lightObj) lightObj.SetActive(!lightObj.activeSelf);

            elapsed += flickerInterval;
            yield return new WaitForSeconds(flickerInterval);
        }

        foreach (var lightObj in lightsToControl)
            if (lightObj) lightObj.SetActive(false);

        if (electricBoxPrefab != null && spawnPoints != null && spawnPoints.Length > 0)
        {
            int i = Random.Range(0, spawnPoints.Length);
            spawnedBox = Instantiate(electricBoxPrefab, spawnPoints[i].position, Quaternion.identity);
        }
    }

    void Update()
    {
        if (spawnedBox != null && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Player interacted with the electric box!");
            Destroy(spawnedBox);
            foreach (var lightObj in lightsToControl)
                if (lightObj) lightObj.SetActive(true);
            Debug.Log("Lights restored.");
        }
    }
}
