using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class missile : MonoBehaviour
{
    public GameObject explosionPrefab, rocket;
    private int playercollidedId, shooterId;
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "wall")
        {
            Destroy(this.gameObject);
            Instantiate(explosionPrefab, this.transform.position, Quaternion.identity);
        }
        /*else if (collision.gameObject.tag == "player")
        {
            PhotonView rocketPhotonView = rocket.GetPhotonView();
            PhotonView enemyPhotonView = collision.gameObject.GetPhotonView();
            if (rocketPhotonView != enemyPhotonView)
            {
                PhotonNetwork.Destroy(enemyPhotonView.gameObject);
                Instantiate(explosionPrefab, this.transform.position, Quaternion.identity);
                Destroy(this.gameObject);
            }
            
            
        }*/

    }
}
