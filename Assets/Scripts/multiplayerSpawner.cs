using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class multiplayerSpawner : MonoBehaviour
{
    public GameObject[] skins; 

    public GameObject petrolPrefab;  // Reference to the petrol prefab
    public Vector2 spawnPoint;    // Point where petrol objects should be spawned
    public float spawnInterval = 15f;  // Time interval between petrol spawns

    private void Awake()
    {
        SpawnPlayer();
    }
    private void Start()
    {
        InvokeRepeating("SpawnPetrol", spawnInterval, spawnInterval);
    }
    private void SpawnPlayer()
    {
        for (int i = 0; i < skins.Length; i++)
        {
            if (PlayerPrefs.GetInt("MyIntegerData") == i)
            {
                GameObject currentObject = skins[i];

                // Do something with the current GameObject
                float randomValue = Random.Range(-1f, 1f);
                PhotonNetwork.Instantiate(currentObject.name, new Vector2(this.transform.position.x * randomValue, this.transform.position.y), Quaternion.identity);
            }
            else
            {
                Debug.Log("Integer Value not found");
            }
        }
    }
    private void SpawnPetrol()
    {
        // Generate a random position within a specified range
        float x = UnityEngine.Random.Range(-25f, 25f);
        float y = UnityEngine.Random.Range(-25f, 25f);
        Vector3 randomPosition = new Vector3(x, y, -2.2f);

        // Instantiate the object at the random position
        Instantiate(petrolPrefab, randomPosition, Quaternion.identity);
    }
}
