using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class multiplayerSpawner : MonoBehaviour
{
    public GameObject playerPrefab;

    private void Awake()
    {
        SpawnPlayer();
    }
    private void SpawnPlayer()
    {
        float randomValue = Random.Range(-1f, 1f);
        PhotonNetwork.Instantiate(playerPrefab.name, new Vector2(this.transform.position.x * randomValue, this.transform.position.y), Quaternion.identity, 0);
    }
}
