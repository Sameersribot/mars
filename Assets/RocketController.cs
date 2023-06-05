using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class RocketController : MonoBehaviour
{
    public float speed = 5f;  // Speed of the rocket movement
    public float rotationSpeed = 200f;  // Speed of the rocket rotation
    public Joystick joystick;
    private Rigidbody2D rb;
    private Vector2 currentDirection;    // Current movement direction of the rocket
    private ParticleSystem propulsion;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        propulsion = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            propulsion.Play(true);
        }
        else 
        {
            propulsion.Stop();
        }
        
    }
    void FixedUpdate()
    {
        Vector2 movement = new Vector2(joystick.Horizontal * speed, joystick.Vertical * speed);
        // Chec k for tou   ch input

        if (Input.touchCount > 0)
        {
            rb.AddForce(movement);
        }
        if (movement.magnitude > 0.1f)
        {
            currentDirection = Vector2.Lerp(currentDirection, movement.normalized, rotationSpeed * Time.fixedDeltaTime);
            float angle = Mathf.Atan2(currentDirection.y, currentDirection.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle-90);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "obstacle")
        {
            SceneManager.LoadScene("SampleScene");
        }
    }
}


