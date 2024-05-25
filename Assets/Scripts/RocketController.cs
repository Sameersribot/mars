using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using DG.Tweening;
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
    public Vector3 otherPlayerPosition, playerSpwanPosition;
    private ParticleSystem propulsion;
    public GameObject playerCamera, cvCam, otherPlayerCollision;
    public GameObject playerCanvas, audiosrc;
    public GameObject background;
    public Text text;
    private int playerId;
    public CinemachineVirtualCamera virtualCamera;
    public float fuelCapacity = 100f;
    public float fuelConsumptionRate = 4f;
    public float refuelRate = 20f;
    public Slider slider;
    public float currentFuel, fuelpower, missileAdjustment;
    public Button missileBtn, bombBtn;
    private bool isMissileButtonDisabled = false, isbombButtonDisabled = false;
    public GameObject explosionPrefab, missile, bulletMissile, bground; //restartButton
    public float parallaxFactor = 0.05f; // How much slower the background moves compared to the player
    public GameObject[] missiles;
    public GameObject fastMovingParticles;
    private Vector3 initialPosition;
    private Vector3 initialPlayerPosition;
    private Vector3[] previousPositions = new Vector3[30];
    private int positionIndex = 0;


    
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
            bground.SetActive(true);
            audiosrc.SetActive(true);
            text.text = PhotonNetwork.NickName;
            //restartButton.SetActive(false);
        }  
    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        propulsion = GetComponent<ParticleSystem>();
        currentFuel = fuelCapacity;
        playerSpwanPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        // Find the Cinemachine virtual camera by its tag
        virtualCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CinemachineVirtualCamera>();
        initialPosition = bground.transform.position;
        initialPlayerPosition = transform.position;
    }

    void Update()
    {
        slider.value = currentFuel / fuelCapacity;
        Vector3 playerMovement = transform.position - initialPlayerPosition;
        Vector3 backgroundMovement = playerMovement *parallaxFactor;
        bground.transform.position = initialPosition + backgroundMovement;
        Debug.Log(rb.velocity.magnitude);
    }
    void FixedUpdate()
    {
       // bground.transform.position = Vector3.Lerp(bground.transform.position, this.transform.position, 100f);
        rocketMovement();
        if (Input.GetButtonDown("Jump"))
            onClickFireMissile();
        //propulsionVisibility();
    }

    private void rocketMovement()
    {
        if (photonView.IsMine)

        {
            movement = new Vector2(joystick.Horizontal, joystick.Vertical) * speed;
            zoon();

            if (movement.magnitude > 0.1f )// && currentFuel >= 0f
            {
                float cosTheta = Vector3.Dot(rb.velocity.normalized, currentDirection)/ (rb.velocity.normalized.magnitude * currentDirection.magnitude);
                photonView.RPC("propulsionVisibility", RpcTarget.AllViaServer);
                if (cosTheta <= 0.36f && rb.velocity.magnitude < 16.2f)
                {
                    rb.AddForce(movement * 5f);
                }
                else if (cosTheta >= 0.36f)
                {
                    Debug.Log(rb.velocity.magnitude);
                    if (rb.velocity.magnitude > 15)
                    {
                        rb.AddForce(movement / 5);
                        //Instantiate(fastMovingParticles, transform.position, Quaternion.identity);
                    }
                    else
                    {
                        rb.AddForce(movement);
                    }
                }
                else
                {
                    rb.AddForce(movement);
                }

                //rb.velocity =movement;

                currentDirection = Vector2.Lerp(currentDirection, movement.normalized, rotationSpeed * Time.fixedDeltaTime);
                float angle = Mathf.Atan2(currentDirection.y, currentDirection.x) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle - 90);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
                float fuelConsumed = fuelConsumptionRate * Time.deltaTime;
                currentFuel -= fuelConsumed;
                FindObjectOfType<AudioMnagaer>().Play("thrust");
            }
            else
            {
                FindObjectOfType<AudioMnagaer>().Pause("thrust");
                currentDirection = Vector2.zero; // Reset the direction when joystick is in the dead zone
            }

        }

    }
 

    [PunRPC]
    private void propulsionVisibility()
    {
        if (movement.magnitude > 0.1f)
         {
            propulsion.Play(true);
            propulsion.startLifetime = movement.magnitude * 0.182f;
        }
        else
        {
            propulsion.Stop();
        }
        
    }
    void zoon()
    {
        // Adjust the virtual camera's field of view
        virtualCamera.m_Lens.FieldOfView = Mathf.Clamp(60.5f + rb.velocity.magnitude , 55.5f, 74.1f);
        //bground.transform.position = new Vector3(bground.transform.position.x, bground.transform.position.y, Mathf.Clamp(-1.2f - rb.velocity.magnitude / 10, -1.2f, -3.1f));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if(collision.gameObject.tag == "missile")
        {
            Destroy(collision.gameObject);
            FindObjectOfType<AudioMnagaer>().Play("explosion");
            photonView.RPC("collisionWithRocket", RpcTarget.AllViaServer, transform.position, Quaternion.identity);
            //restartButton.SetActive(true);
        } 
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(rb.rotation);
            stream.SendNext(rb.velocity);
            stream.SendNext(rb.angularVelocity);
        }
        else
        { 
            transform.position = (Vector2)stream.ReceiveNext();
            transform.rotation = (Quaternion)stream.ReceiveNext();
            rb.velocity = (Vector3)stream.ReceiveNext();
            rb.angularVelocity = (float)stream.ReceiveNext();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "fuel")
        {
            currentFuel = Mathf.Clamp(currentFuel + fuelpower, 0f, fuelCapacity);
            Destroy(collision.gameObject);
        }
        else if(collision.gameObject.tag == "petrolpump")
        {
            StartRefueling();
        }
        else if(collision.gameObject.tag == "teleporter")
        {
            transform.position = new Vector2(-7.1f, 442.3f);
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
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "petrolpump")
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
    }
    /*private void SpawnPetrol()
    {
        spawnPoint = new Vector2(20f * UnityEngine.Random.Range(-1f, 1f), 20f * UnityEngine.Random.Range(-1f, 1f));
        PhotonNetwork.Instantiate(petrolPrefab.name, spawnPoint, Quaternion.identity);
    }*/
    [PunRPC]
    private void ShootBullet(Vector3 position, Quaternion rotation, Vector2 direction, Vector2 force)
    {
        GameObject bullet = Instantiate(missiles[0], position, rotation);
        Rigidbody2D bulletRigidbody = bullet.GetComponent<Rigidbody2D>();
        //bulletRigidbody.velocity = currentDirection * missileSpeed;
        bulletRigidbody.AddForce(force);
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), bullet.GetComponent<Collider2D>());
    }
    [PunRPC]
    private void ShootBomb(Vector3 position, Quaternion rotation)
    {
        GameObject bullet = Instantiate(missiles[1], position, rotation);
        Rigidbody2D bulletRigidbody = bullet.GetComponent<Rigidbody2D>();
        //bulletRigidbody.velocity = currentDirection * missileSpeed;

        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), bullet.GetComponent<Collider2D>());
    }
    [PunRPC]
    private void ShootMine(Vector3 position, Quaternion rotation)
    {
        GameObject bullet = Instantiate(missiles[3], position, rotation);
        Rigidbody2D bulletRigidbody = bullet.GetComponent<Rigidbody2D>();
        //bulletRigidbody.velocity = currentDirection * missileSpeed;

        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), bullet.GetComponent<Collider2D>());
    }
    [PunRPC]
    private void ShootTriBomb(Vector3 position, Quaternion rotation, Vector2 direction)
    {
        GameObject bullet = Instantiate(missiles[2], position, rotation);
        Rigidbody2D bulletRigidbody = bullet.GetComponent<Rigidbody2D>();
        //bulletRigidbody.velocity = currentDirection * missileSpeed;
        bullet.transform.DOMove(GameObject.FindGameObjectWithTag("Player").transform.position, 4f, true);
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), bullet.GetComponent<Collider2D>());
    }
    public void onClickFireMissile() 
    {
        //if(movement.magnitude > 0) 
        //{
            Vector3 missilePosition = new Vector3(this.transform.position.x + rb.velocity.x * missileAdjustment, this.transform.position.y + rb.velocity.y * missileAdjustment, this.transform.position.z);
            Vector2 missileDirection = new Vector2(transform.rotation.x, transform.rotation.y);
            if (photonView.IsMine)
                FindObjectOfType<AudioMnagaer>().Play("shooting");
            Vector2 randomForce = currentDirection * missileSpeed * 50f;

            //photonView.RPC("ShootTriBomb", RpcTarget.AllViaServer, missilePosition, this.transform.rotation, missileDirection);
            photonView.RPC("ShootBullet", RpcTarget.AllViaServer, missilePosition, this.transform.rotation, missileDirection, randomForce);
 
            missileBtn.interactable = false;

            // Set the flag to true to start the timer
            isMissileButtonDisabled = true;

            // Reset the button after 5 seconds
            Invoke("EnableMissileButton", 5f);
        //}
    }
    public void onClickFireBomb()
    {
        Vector3 bombPosition = new Vector3(this.transform.position.x + rb.velocity.x * missileAdjustment, this.transform.position.y + rb.velocity.y * missileAdjustment, this.transform.position.z);
        if (photonView.IsMine)
            FindObjectOfType<AudioMnagaer>().Play("shooting");
        photonView.RPC("ShootBomb", RpcTarget.AllViaServer, bombPosition, this.transform.rotation);
    }
    public void onClickFireMine()
    {
        Vector3 minePosition = new Vector3(this.transform.position.x + rb.velocity.x * missileAdjustment, this.transform.position.y + rb.velocity.y * missileAdjustment, this.transform.position.z);
        if (photonView.IsMine)
            FindObjectOfType<AudioMnagaer>().Play("shooting");
        photonView.RPC("ShootMine", RpcTarget.AllViaServer, minePosition, this.transform.rotation);
    }

    [PunRPC]
    private void collisionWithRocket(Vector3 position, Quaternion rotation)
    {
        GameObject bullet = Instantiate(explosionPrefab, position, rotation);
        this.gameObject.SetActive(false);
        Invoke("Respawn", 6f);
    }
    
    private void Respawn()
    {
        // Set the position and enable the GameObject
        transform.position = playerSpwanPosition;
        this.gameObject.SetActive(true);
    }
    void EnableMissileButton()
    {
        // Enable the button after 5 seconds
        missileBtn.interactable = true;
        // Reset the flag
        isMissileButtonDisabled = false;
    }
}


