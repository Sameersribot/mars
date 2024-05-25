using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using DG.Tweening;

public class bot : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float shootingRange = 5f;
    public float fireRate = 1f;
    public GameObject bulletPrefab;

    private Transform player;
    private float nextFireTime;
    public GameObject explosionPrefab;

    void Start()
    {
        
    }

    

    void Update()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        // Move towards the player
        Vector2 distance = new Vector2(gameObject.transform.position.x - playerObj.transform.position.x, gameObject.transform.position.y - playerObj.transform.position.y);
        if(distance.magnitude <= 20f)
        {
            gameObject.transform.DOMove(playerObj.transform.position, 4f, false);
            gameObject.transform.DOLookAt(playerObj.transform.position, 0.1f);
        }
        // Check if within shooting range
        if (Vector2.Distance(transform.position, player.position) <= shootingRange && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + 1f / fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        GameObject bullet = PhotonNetwork.Instantiate(bulletPrefab.name, transform.position, transform.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(transform.up * 10f, ForceMode2D.Impulse);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "missile")
        {
            Instantiate(explosionPrefab, gameObject.transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
