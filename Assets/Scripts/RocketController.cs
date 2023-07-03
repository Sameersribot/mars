using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
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
    public GameObject playerCanvas;
    public GameObject background;
    public Text text;
    private AudioSource audio;
    public CinemachineVirtualCamera virtualCamera;
    public float fuelCapacity = 100f;
    public float fuelConsumptionRate = 4f;
    public float refuelRate = 20f;
    public Slider slider;
    public float currentFuel, fuelpower;

    // public AudioSource audio;
    private void Awake()
    {
        PhotonNetwork.SendRate = 40; //Default is 30
        PhotonNetwork.SerializationRate = 20;
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
            Debug.Log(rb.velocity.magnitude);
        }
    }

    private void rocketMovement()
    {
        movement = new Vector2(joystick.Horizontal * speed, joystick.Vertical * speed);
        zoon();
        
        if (movement.magnitude > 0.1f && currentFuel>=0f)
        {
            if(rb.velocity.magnitude < 15)
            {
                rb.AddForce(movement);
            }
            
            else if(rb.velocity.magnitude > 15 && rb.velocity.y > 0)
            {
                rb.AddForce(movement / 2);
            }
            else if(rb.velocity.magnitude > 13 && rb.velocity.y < 0)
            {
                if(movement.y > 0)
                {
                    movement.y *= 1.2f;
                    rb.AddForce(movement);
                } 
                else if(movement.y < 0)
                {
                    movement.y /= 4;
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
            Debug.Log(currentFuel);
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
        
        /*if (photonView.IsMine)
        {
            // Check if the collision is with another player
             otherPlayerCollision = collision.gameObject;
             if (otherPlayerCollision != null)
             {
                // Get the other player's PhotonView
                otherPlayerPhotonView = otherPlayerCollision.GetPhotonView();

                // Get the other player's position
                otherPlayerPosition = new Vector3(otherPlayerPhotonView.transform.position.x, otherPlayerPhotonView.transform.position.y, -12.2f);
                Debug.Log("Other Player Position: " + otherPlayerPosition);
             }
        } */
        else if(collision.gameObject.tag == "fuel")
        {
            Debug.Log("collided");
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
}


