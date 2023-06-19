using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cinemachine;
using UnityEngine.Audio;

public class RocketController : MonoBehaviour, IPunObservable
{
    public PhotonView photonView;
    public float speed = 5f;  // Speed of the rocket movement
    public float rotationSpeed = 200f;  // Speed of the rocket rotation
    public Joystick joystick;
    private Rigidbody2D rb;
    private Vector2 currentDirection;    // Current movement direction of the rocket
    private ParticleSystem propulsion;
    public GameObject playerCamera, cvCam;
    public GameObject playerCanvas;
    public GameObject background;
    public Text text;
    private AudioSource audio;
    public CinemachineVirtualCamera virtualCamera;
    public float fuelCapacity = 100f;
    public float fuelConsumptionRate = 4f;
    public float refuelRate = 20f;
    public Slider slider;
    private float currentFuel;

    // public AudioSource audio;
    private void Awake()
    {
        PhotonNetwork.SendRate = 60; //Default is 30
        PhotonNetwork.SerializationRate = 60;
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
            background.transform.position = new Vector2(transform.position.x, transform.position.y);
        }
    }

    private void rocketMovement()
    {
        Vector2 movement = new Vector2(joystick.Horizontal * speed, joystick.Vertical * speed);
        zoon();
        
        if (movement.magnitude > 0.1f && currentFuel>=0f)
        {
            rb.AddForce(movement);
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
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(rb.position);
            stream.SendNext(rb.rotation);
            stream.SendNext(rb.velocity);
        }
        else
        {
            rb.position = (Vector3)stream.ReceiveNext();
            transform.rotation = (Quaternion)stream.ReceiveNext();
            rb.velocity = (Vector3)stream.ReceiveNext();

            float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.timestamp));
            rb.position += rb.velocity * lag;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "petrolpump")
        {
            StartRefueling();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {

        if (collision.gameObject.tag == "petrolpump")
        {
            StopRefueling();
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
        Debug.Log(currentFuel);
    }
}


