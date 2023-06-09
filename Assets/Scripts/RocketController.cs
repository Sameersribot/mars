using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class RocketController : MonoBehaviour
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

    private void Awake()
    {
        if (photonView.IsMine)
        {
            playerCanvas.SetActive(true);
            playerCamera.SetActive(true);
            cvCam.SetActive(true);
        }
    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        propulsion = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        // Check for touch input
    }
    void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            rocketMovement();
        }
        
    }
   
    private void rocketMovement()
    {
        Vector2 movement = new Vector2(joystick.Horizontal * speed, joystick.Vertical * speed);
        
        if (movement.magnitude > 0.1f && photonView.IsMine)
        {
            rb.AddForce(movement);
            propulsion.Play(true);
            currentDirection = Vector2.Lerp(currentDirection, movement.normalized, rotationSpeed * Time.fixedDeltaTime);
            float angle = Mathf.Atan2(currentDirection.y, currentDirection.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle - 90);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
        else
        {
            propulsion.Stop();
        }
    }
}


