using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using UnityEngine.Audio;
using Photon.Realtime;
using System;

public class RocketController : MonoBehaviour
{
    public PhotonView photonView, otherPlayerPhotonView;
    public float speed = 5f;  // Speed of the rocket movement
    public float rotationSpeed = 200f;  // Speed of the rocket rotation
    public Joystick joystick;
    private Rigidbody2D rb;
    public Vector2 currentDirection, movement;    // Current movement direction of the rocket
    public Vector3 otherPlayerPosition;
    private ParticleSystem propulsion;
    public GameObject playerCamera, cvCam, otherPlayerCollision;
    public GameObject playerCanvas, restartCanvas;
    public GameObject background;
    public Text text;
    private AudioSource audio;
    public CinemachineVirtualCamera virtualCamera;
    public float fuelCapacity = 100f;
    public float fuelConsumptionRate = 4f;
    public float refuelRate = 20f;
    public Slider slider;
    public float currentFuel, fuelpower;
    public GameObject explosionPrefab;

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
        }  
    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        propulsion = GetComponent<ParticleSystem>();
        audio = GetComponent<AudioSource>();
        currentFuel = fuelCapacity;

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
        virtualCamera.m_Lens.FieldOfView = Mathf.Clamp(40.7f + rb.velocity.magnitude , 40.7f, 59.2f);
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
                    PhotonNetwork.Destroy(gameObject);
                    photonView.RPC("collisionWithRocket", RpcTarget.AllViaServer, transform.position, Quaternion.identity);
                    restartCanvas.SetActive(true);
                }
            }
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
    //[PunRPC]
    /*private void ShootBullet(Vector3 position, Quaternion rotation, Vector2 direction)
    {
        GameObject bullet = Instantiate(bulletPrefab, position, rotation);
        Rigidbody2D bulletRigidbody = bullet.GetComponent<Rigidbody2D>();
        bulletRigidbody.velocity = direction.normalized * 10f; // Adjust bullet speed as needed
    }*/
    /*public void onClickFire() 
    {
        photonView.RPC("ShootBullet", RpcTarget.AllViaServer, bulletSpawnPoint.position, bulletSpawnPoint.rotation, currentDirection);
    }*/
    [PunRPC]
    private void collisionWithRocket(Vector3 position, Quaternion rotation)
    {
        GameObject bullet = Instantiate(explosionPrefab, position, rotation);
    }
    public void restartMatch()
    {
        SceneManager.LoadScene("lobbyscene");
    }
}


