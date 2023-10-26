using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using UnityEngine.Audio;
using System;

public class RocketController : MonoBehaviour
{
    public PhotonView photonView, otherPlayerPhotonView;
    public float speed = 5f;  // Speed of the rocket movement
    public float rotationSpeed = 200f, missileSpeed;  // Speed of the rocket rotation
    public Joystick joystick;
    private Rigidbody2D rb;
    public Vector2 currentDirection, movement;    // Current movement direction of the rocket
    public Vector3 otherPlayerPosition, missilePosition, playerSpwanPosition;
    private ParticleSystem propulsion;
    public GameObject playerCamera, cvCam, otherPlayerCollision;
    public GameObject playerCanvas;
    public GameObject background;
    public Text text;
    private AudioSource audio;
    private int playerId;
    public CinemachineVirtualCamera virtualCamera;
    public float fuelCapacity = 100f;
    public float fuelConsumptionRate = 4f;
    public float refuelRate = 20f;
    public Slider slider;
    public float currentFuel, fuelpower;
    public GameObject explosionPrefab, missile, bulletMissile; //restartButton
     
    // public AudioSource audio;
    private void Awake()
    {
        PhotonNetwork.SendRate = 40; //Default is 30
        PhotonNetwork.SerializationRate = 10;
        fuelpower = 60f;
        if (photonView.IsMine)
        {
            playerCanvas.SetActive(true);
            playerCamera.SetActive(true);
            cvCam.SetActive(true);
            text.text = PhotonNetwork.NickName;
            //restartButton.SetActive(false);
        }  
    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        propulsion = GetComponent<ParticleSystem>();
        audio = GetComponent<AudioSource>();
        currentFuel = fuelCapacity;
        playerSpwanPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        // Find the Cinemachine virtual camera by its tag
        

        virtualCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CinemachineVirtualCamera>();
    }

    void Update()
    {
        slider.value = currentFuel / fuelCapacity;
    }
    void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            rocketMovement();
            //  dont know what its about ..background.transform.position = new Vector2(transform.position.x, transform.position.y);
        }
    }

    private void rocketMovement()
    {
        movement = new Vector2(joystick.Horizontal * speed, joystick.Vertical * speed);
        zoon();
        
        if (movement.magnitude > 0.1f)// && currentFuel >= 0f
        {
            float dotProduct = Vector3.Dot(rb.velocity.normalized, currentDirection);
            Debug.Log(dotProduct);
            if (dotProduct < 0)
            {
                rb.AddForce(movement * 5f);
            }
            else if (dotProduct > 0)
            {
                Debug.Log(rb.velocity.magnitude);
                if(rb.velocity.magnitude > 15)
                {
                    rb.AddForce(movement/5);
                }
                else
                {
                    rb.AddForce(movement);
                }
            }

            //rb.velocity =movement;
            propulsion.Play(true);
            propulsion.startLifetime = movement.magnitude * 0.182f;
            currentDirection = Vector2.Lerp(currentDirection, movement.normalized, rotationSpeed * Time.fixedDeltaTime);
            float angle = Mathf.Atan2(currentDirection.y, currentDirection.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle - 90);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
            float fuelConsumed = fuelConsumptionRate * Time.deltaTime;
            currentFuel -= fuelConsumed;
            audio.Play();
           
        }
        else
        {
            propulsion.Stop();
            audio.Pause();
        }
    }
    void zoon()
    {
        // Adjust the virtual camera's field of view
        virtualCamera.m_Lens.FieldOfView = Mathf.Clamp(55.5f + rb.velocity.magnitude , 55.5f, 69.1f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "petrolpump")
        {
            StartRefueling();
        }


        if (collision.gameObject.tag == "Player")
        {
            if (photonView.IsMine)
            {
                Rigidbody2D otherRigidbody = collision.gameObject.GetComponent<Rigidbody2D>();

                // Compare the magnitudes of velocities
                float myVelocityMagnitude = rb.velocity.magnitude;
                float otherVelocityMagnitude = otherRigidbody.velocity.magnitude;

                if (myVelocityMagnitude > otherVelocityMagnitude)
                {
                    // Destroy the other player's game object
                    PhotonNetwork.Destroy(collision.gameObject);
                    photonView.RPC("collisionWithRocket", RpcTarget.AllViaServer, collision.gameObject.transform.position, Quaternion.identity);
                }
                else if (myVelocityMagnitude < otherVelocityMagnitude)
                {
                    // Destroy the current player's game object
                    photonView.RPC("collisionWithRocket", RpcTarget.AllViaServer, transform.position, Quaternion.identity);
                }
            }
        }
        else if(collision.gameObject.tag == "missile")
        {
            Destroy(collision.gameObject);
            photonView.RPC("collisionWithRocket", RpcTarget.AllViaServer, transform.position, Quaternion.identity);
            //restartButton.SetActive(true);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "petrolpump")
        {
            StopRefueling();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "fuel")
        {
            currentFuel = Mathf.Clamp(currentFuel + fuelpower, 0f, fuelCapacity);
            Destroy(collision.gameObject);
        }
        
        else if(collision.gameObject.tag == "redPower")
        {
            Destroy(collision.gameObject);
        }
        else if(collision.gameObject.tag == "bluePower")
        {
            Destroy(collision.gameObject);
        }
    }

    private void StartRefueling()
    {
        InvokeRepeating("IncreaseFuel", 0f, 0.1f);
    }

    private void StopRefueling()
    {
        CancelInvoke("IncreaseFuel");
    }
    private void IncreaseFuel()
    {
        currentFuel = Mathf.Clamp(currentFuel + refuelRate * Time.deltaTime, 0f, fuelCapacity);
    }
    /*private void SpawnPetrol()
    {
        spawnPoint = new Vector2(20f * UnityEngine.Random.Range(-1f, 1f), 20f * UnityEngine.Random.Range(-1f, 1f));
        PhotonNetwork.Instantiate(petrolPrefab.name, spawnPoint, Quaternion.identity);
    }*/
    [PunRPC]
    private void ShootBullet(Vector3 position, Quaternion rotation, Vector2 direction, Vector2 force)
    {
        GameObject bullet = Instantiate(bulletMissile, position, rotation);
        Rigidbody2D bulletRigidbody = bullet.GetComponent<Rigidbody2D>();
        //bulletRigidbody.velocity = currentDirection * missileSpeed;
        bulletRigidbody.AddForce(force);
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), bullet.GetComponent<Collider2D>());
    }
    public void onClickFire() 
    {
        //if(movement.magnitude > 0)ss
        //{
            missilePosition = new Vector3(this.transform.position.x + rb.velocity.x * 0.1f, this.transform.position.y + rb.velocity.y * 0.1f, this.transform.position.z);
            Vector2 missileDirection = new Vector2(transform.up.x, transform.up.y);
            Vector2 randomForce = currentDirection * missileSpeed*50f;
            photonView.RPC("ShootBullet", RpcTarget.AllViaServer,  missilePosition, this.transform.rotation, missileDirection, randomForce);
            rb.AddForce(new Vector2(UnityEngine.Random.Range(-25f, 25f), UnityEngine.Random.Range(-25f, 25f)));
        //}
    }
    [PunRPC]
    private void collisionWithRocket(Vector3 position, Quaternion rotation)
    {
        GameObject bullet = Instantiate(explosionPrefab, position, rotation);
        if (photonView.IsMine)
        {
            this.gameObject.SetActive(false);
            Invoke("Respawn", 3f);
        }
    }
    
    private void Respawn()
    {
        // Set the position and enable the GameObject
        transform.position = playerSpwanPosition;
        this.gameObject.SetActive(true);
    }
}


