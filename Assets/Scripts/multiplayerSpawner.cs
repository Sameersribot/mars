using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class multiplayerSpawner : MonoBehaviour
{
    public GameObject Brown, white;

    private void Awake()
    {
        SpawnPlayer();
    }
    private void SpawnPlayer()
    {
        if(PhotonNetwork.NickName.Length >= 8)
        {
            float randomValue = Random.Range(-1f, 1f);
            PhotonNetwork.Instantiate(Brown.name, new Vector2(this.transform.position.x * randomValue, this.transform.position.y), Quaternion.identity);
        }
        if(PhotonNetwork.NickName.Length < 8)
        {
            float randomValue = Random.Range(-1f, 1f);
            PhotonNetwork.Instantiate(white.name, new Vector2(this.transform.position.x * randomValue, this.transform.position.y), Quaternion.identity);
        }
    }
}
