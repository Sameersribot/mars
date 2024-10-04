using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class missile : MonoBehaviour
{
    public GameObject explosionPrefab;
    public RocketController ownerId;

    private void Awake()
    {
        /*if (ownerId.gameObject.GetPhotonView() != null)
            shooter = ownerId.gameObject.GetPhotonView();
        else shooter = null;*/
    }
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
        if (collision.gameObject.tag == "bot" && gameObject.tag != "hammer")
        {
            ownerId.AddKill();
            //ownerId.whokilldwho.text = ownerId.name + "killed bot"; 
        }
        else if (collision.gameObject.tag == "bot" && gameObject.tag == "hammer")
        {
            Instantiate(explosionPrefab, gameObject.transform.position, Quaternion.identity);

            Destroy(collision.gameObject);
        }
    }
}
