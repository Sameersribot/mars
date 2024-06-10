using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class missile : MonoBehaviour
{
    public GameObject explosionPrefab, rocket;
    public RocketController ownerId;

    private void Update()
    {
        if(Vector2.Distance(ownerId.gameObject.transform.position, transform.position) > 20f)
        {
            Destroy(this.gameObject);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "wall")
        {
            Destroy(this.gameObject);
            Instantiate(explosionPrefab, this.transform.position, Quaternion.identity);
        }
        else if(collision.gameObject.tag == "bot")
        {
            ownerId.AddKill();
        }
    }
}
