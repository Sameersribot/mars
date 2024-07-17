using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class multiplayerSpawner : MonoBehaviour
{
    public GameObject[] skins;
    public GameObject[] powers;
    public GameObject[] missiles;
    public GameObject[] weapons;
    public GameObject petrolPrefab;  // Reference to the petrol prefab
    public GameObject bot;

    public Vector2 spawnPoint;    // Point where petrol objects should be spawned
    private float spawnInterval = 15f;  // Time interval between petrol spawns
    private float spawnPowerInterval = 15f;  // Time interval between petrol spawns
    public float spawnMissilesInterval = 8f;

    private void Awake()
    {
        SpawnPlayer();
        botSpawner();
    }
    private void Start()
    {
        InvokeRepeating("SpawnPetrol", spawnInterval, spawnInterval);
        InvokeRepeating("SpawnPower", spawnPowerInterval, spawnPowerInterval);
        //InvokeRepeating("SpawnWeapons", spawnInterval, spawnInterval);
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
                PhotonNetwork.Instantiate(currentObject.name, new Vector2(-40f, 324.2f), Quaternion.identity);
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
        float x = UnityEngine.Random.Range(-73f, -12f);
        float y = UnityEngine.Random.Range(336f, 310f);
        Vector3 randomPosition = new Vector3(x, y, -2.2f);

        // Instantiate the object at the random position
        Instantiate(petrolPrefab, randomPosition, Quaternion.identity);
    }

    private void SpawnPower()
    {
        float x = Random.Range(-66f, 25.1f);
        float y = Random.Range(326.5f, 515f);
        Vector3 randomPowerPosition = new Vector3(x, y, -2.2f);
        int typeOfPower = UnityEngine.Random.Range(0, powers.Length + 1);

        for (int i = 0; i < powers.Length ; i++)
        {
            if (typeOfPower == i)
            {
                GameObject currentPower = powers[i];

                // Do something with the current GameObject
                Instantiate(currentPower, randomPowerPosition, Quaternion.identity);

            }
            else
            {
                Debug.Log("Integer Value not found");
            }
        }
    }
    private void spawnMissile()
    {
        float x = UnityEngine.Random.Range(-73f, -12f);
        float y = UnityEngine.Random.Range(336f, 310f);
        Vector3 randomPowerPosition = new Vector3(x, y, -2.2f);
        int typeOfPower = UnityEngine.Random.Range(0, missiles.Length + 1);

        for (int i = 0; i < missiles.Length; i++)
        {
            if (typeOfPower == i)
            {
                GameObject currentPower = missiles[i];

                // Do something with the current GameObject
                PhotonNetwork.Instantiate(currentPower.name, randomPowerPosition, Quaternion.identity);

            }
            else
            {
                Debug.Log("Integer Value not found");
            }
        }
    }
    private void botSpawner()
    {
        int num = PhotonNetwork.CurrentRoom.PlayerCount;
        Debug.Log(num);
        /*if (num == 1)
        {
            for (int k = 0; k < 20 - num; k++)
            {
                PhotonNetwork.Instantiate(bot.name, new Vector2(31f + Random.Range(-5f, 5f), 421f + Random.Range(-20f, 20f)), Quaternion.identity);
            }
        }*/
    }
    private void SpawnWeapons()
    {
        float x = Random.Range(-66f, 25.1f);
        float y = Random.Range(326.5f, 515f);
        Vector3 randomPowerPosition = new Vector3(x, y, -2.2f);
        int typeOfPower = UnityEngine.Random.Range(0, weapons.Length + 1);

        for (int i = 0; i < weapons.Length; i++)
        {
            if (typeOfPower == i)
            {
                GameObject currentweapon = weapons[i];

                // Do something with the current GameObject
                Instantiate(currentweapon, randomPowerPosition, Quaternion.identity);

            }
            else
            {
                Debug.Log("Integer Value not found");
            }
        }
    }
}
