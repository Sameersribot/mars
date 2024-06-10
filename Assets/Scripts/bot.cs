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
    public Sprite[] sprites;
    public PhotonView photonView;
    public GameObject missilePrefab;
    public ParticleSystem particleSystem;
    private Vector2 direction, distance, directionRandom;
    private SpriteRenderer renderer;
    private Transform player;
    private float nextFireTime, nextRandTime;
    private Rigidbody2D botRb;
    public GameObject explosionPrefab;


    void Start()
    {
        renderer = gameObject.GetComponent<SpriteRenderer>();
        float spriteChoose = Random.Range(1f, 5f);
        if(spriteChoose > 1f && spriteChoose < 2f)
        {
            renderer.sprite = sprites[0];
        }
        else if (spriteChoose > 2f && spriteChoose < 3f)
        {
            renderer.sprite = sprites[1];
        }
        else if (spriteChoose > 3f && spriteChoose < 4f)
        {
            renderer.sprite = sprites[2];
        }
        else if (spriteChoose > 4f && spriteChoose < 5f)
        {
            renderer.sprite = sprites[3];
        }
        botRb = gameObject.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        // Move towards the player
        distance = new Vector2(gameObject.transform.position.x - playerObj.transform.position.x, gameObject.transform.position.y - playerObj.transform.position.y);
        propulsion();
        

        if (distance.magnitude <= 25f)
        {
            gameObject.transform.DOMove(playerObj.transform.position, 4f, false);

            direction = (playerObj.transform.position - transform.position).normalized;

            // Calculate the angle between the object's forward vector and the direction vector
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            float newAngle = Mathf.LerpAngle(transform.rotation.z, angle, 20f * Time.deltaTime);

            // Apply the rotation to the object
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle-90f));

        }
        else if(nextRandTime < Time.time)
        {
            Vector2 randomMovement = new Vector2(transform.position.x + Random.Range(-6f, 6f), transform.position.y + Random.Range(-6f, 6f));
            gameObject.transform.DOMove(randomMovement, 3f, false);
            directionRandom = (randomMovement - new Vector2(transform.position.x, transform.position.y)).normalized;

            // Calculate the angle between the object's forward vector and the direction vector
            float angle = Mathf.Atan2(directionRandom.y, directionRandom.x) * Mathf.Rad2Deg;
            float newAngle = Mathf.LerpAngle(transform.rotation.z, angle, 20f * Time.deltaTime);

            // Apply the rotation to the object
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90f));
            nextRandTime = Time.time + 3f;
        }
        // Check if within shooting range
            Shoot();
    }

    void Shoot()
    {
        if (distance.magnitude < 12f && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + 1.5f;
            onClickFireMissile();
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "missile")
        {
            Instantiate(explosionPrefab, gameObject.transform.position, Quaternion.identity);
            Destroy(collision.gameObject); 
            Destroy(gameObject);
        }
    }
    [PunRPC]
    private void ShootBullet(Vector3 position, Quaternion rotation, Vector2 direction, Vector2 force)
    {
        GameObject bullet = Instantiate(missilePrefab, position, rotation);
        Rigidbody2D bulletRigidbody = bullet.GetComponent<Rigidbody2D>();
        //bulletRigidbody.velocity = currentDirection * missileSpeed;
        bulletRigidbody.AddForce(force);
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), bullet.GetComponent<Collider2D>());
    }
    public void onClickFireMissile()
    {

        Vector3 missilePosition = new Vector3(this.transform.position.x + botRb.velocity.x * 0.2f, this.transform.position.y + botRb.velocity.x * 0.2f, this.transform.position.z);
        Vector2 missileDirection = new Vector2(transform.rotation.x, transform.rotation.y);
        
        Vector2 randomForce = direction *1250f;

        //photonView.RPC("ShootTriBomb", RpcTarget.AllViaServer, missilePosition, this.transform.rotation, missileDirection);
        photonView.RPC("ShootBullet", RpcTarget.AllViaServer, missilePosition, this.transform.rotation, missileDirection, randomForce);
    }
    void propulsion()
    {
        if(botRb.velocity.magnitude > 0.1f)
        {
            particleSystem.Play(true);
            particleSystem.startLifetime = botRb.velocity.magnitude * 0.1f;
        }
        else
        {
            particleSystem.Stop();
        }
    }
}
