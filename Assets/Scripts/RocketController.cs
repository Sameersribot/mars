using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using DG.Tweening;
using UnityEngine.Audio;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;

public class RocketController : MonoBehaviourPunCallbacks
{
    public PhotonView photonView, otherPlayerPhotonView;
    public float speed = 5f;  // Speed of the rocket movement
    public buttonPressedListener buttonPressed;
    public float rotationSpeed = 200f, missileSpeed, starTimer;  // Speed of the rocket rotation
    private bool isrunning = false;
    public Joystick joystick, weaponJoystick;
    private Rigidbody2D rb;
    public Vector2 currentDirection, movement;    // Current movement direction of the rocket
    public Vector3 otherPlayerPosition, playerSpwanPosition;
    public ParticleSystem propulsion, electricPwr, bulletTrail;
    public GameObject playerCamera, cvCam, otherPlayerCollision;
    public GameObject playerCanvas, audiosrc, mainGun;
    public GameObject background;
    public GameObject redDot, whiteDot, greenPowerEfct;
    public Text text, killsText, leaderBordText, whokilldwho, respawnTimer;//who killed whom text
    private int playerId;
    public CinemachineVirtualCamera virtualCamera;
    private float shakeIntensity = 1.5f;
    private float shakeTime = 0.1f;
    private float timerShake;
    private CinemachineBasicMultiChannelPerlin _cameraChannelPerlin;
    public float fuelCapacity = 100f;
    public float fuelConsumptionRate = 4f;
    public float refuelRate = 20f;
    public Slider fuelSlider, lifeSlider;
    public float currentFuel, fuelpower, missileAdjustment;
    public Button missileBtn, bombBtn;
    public Button nitroButton;
    private bool isMissileButtonDisabled = false, isbombButtonDisabled = false;
    public GameObject explosionPrefab, missile, bulletMissile, bground; //restartButton
    public float parallaxFactor = 0.05f; // How much slower the background moves compared to the player
    public GameObject[] missiles;
    public GameObject fastMovingParticles, arrowTrget, redRocketPowerEffect;
    private GameObject blackholeTarget, blackholeParent;
    private Vector3 initialPosition;
    private Vector3 initialPlayerPosition;
    private int kills, seconds = 6;
    private int weapon = 0;
    private float nextFireTime, proplsnValue;
    private bool isInblackhole, electricPwrOn, sparksPwrOn, playThrust;
    private int[] killsData;
    public GameObject dulmissile, pointerMissile, lifeUi, respawningUI;
    public AudioSource[] voices;
    public Color[] colorsForNicknames;
    public Sprite[] weaponIcons;
    public Image weaponIconImage;
    private bool[] whichWeapon;
    private int x=0;
    public float frictionForce = 0.5f; // Adjust this value to change the strength of the friction
    public string nameOfShooter, nameofGotShot;

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
            lifeUi.SetActive(true);
            bground.SetActive(true);
            audiosrc.SetActive(true);
            propulsion = gameObject.GetComponent<ParticleSystem>();
            //restartButton.SetActive(false);
        }
    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (photonView.IsMine) whiteDot.SetActive(true);
        else redDot.SetActive(true);
        naming();
        currentFuel = fuelCapacity;
        missiles[3].SetActive(false);
        playerSpwanPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        blackholeParent = GameObject.FindGameObjectWithTag("blackhole");
        //nitroButton.onClick.AddListener(ToggleNitro);

        // Find the Cinemachine virtual camera by its tag
        virtualCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CinemachineVirtualCamera>();
        initialPosition = bground.transform.position;
        initialPlayerPosition = transform.position;
    }


    void Update()
    {
        fuelSlider.value = currentFuel / fuelCapacity;
        Vector3 playerMovement = transform.position - initialPlayerPosition;
        Vector3 backgroundMovement = playerMovement * parallaxFactor;
        bground.transform.position = initialPosition + backgroundMovement;
        refresh();
        if(photonView.IsMine) statusOfWeapons();
        blackholePhysics();
        if (playThrust && !voices[2].isPlaying) voices[2].Play();
        else voices[2].Stop();
        photonView.RPC("propulsionVisibility", RpcTarget.AllViaServer);
    }
    
    void FixedUpdate()
    {
        // bground.transform.position = Vector3.Lerp(bground.transform.position, this.transform.position, 100f);
        rocketMovement();
        shootJoystick();
        breakForce();
        leaderBoard();

        //if (Input.GetButtonDown("Jump"))
        // onClickFireMissile();
        propulsionVisibility();
    }

    private void rocketMovement()
    {
        if (photonView.IsMine)
        {
            movement = new Vector2(joystick.Horizontal, joystick.Vertical) * speed;
            zoon();
            if (movement.magnitude > 0.1f)// && currentFuel >= 0f
            {
                playThrust = true;
                //rb.velocity = movement*1.5f;
                float cosTheta = Vector3.Dot(rb.velocity.normalized, currentDirection) / (rb.velocity.normalized.magnitude * currentDirection.magnitude);
                //Debug.Log(movement.x);
                //Debug.Log(movement.y);

                //rb.AddForce(movement);
                /*if (!buttonPressed.usingNox)
                {
                    rb.velocity = movement * 1.9f;
                    FindObjectOfType<AudioMnagaer>().Pause("thrust");
                    //nitroParticles.SetActive(false);
                }
                else if (buttonPressed.usingNox)
                {
                    rb.velocity = movement * 3f;
                    //nitroParticles.SetActive(true);
                    FindObjectOfType<AudioMnagaer>().Play("thrust");
                }*/
                if (cosTheta <= 0.6f) //&& rb.velocity.magnitude < 16.2f)
                {
                    rb.AddForce(movement * 2f);
                }
                else 
                {
                    if (rb.velocity.magnitude > 15)
                    {
                        rb.AddForce(movement / 10);
                        //Instantiate(fastMovingParticles, transform.position, Quaternion.identity);
                    }
                    else
                    {
                        rb.AddForce(movement * 0.55f);
                    }
                }                      // ________________________________________
                                      //|SAMEER SRIVASTAVA WILL CHANGE THE WORLD|......
                                         // ------------------------------------------
                //this function is for for movement
                //rb.velocity =movement;
                //FindObjectOfType<AudioMnagaer>().Play("thrust");
                currentDirection = Vector2.Lerp(currentDirection, movement.normalized, rotationSpeed * Time.fixedDeltaTime);
                float angle = Mathf.Atan2(currentDirection.y, currentDirection.x) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle - 90);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
                float fuelConsumed = fuelConsumptionRate * Time.deltaTime;
                currentFuel -= fuelConsumed;
                
            }
            else
            {
                currentDirection = Vector2.zero; // Reset the direction when joystick is in the dead zone
                playThrust = false;
            }
        }
    }


    [PunRPC]
    private void propulsionVisibility()
    {
        if (photonView.IsMine)
        {
            proplsnValue = new Vector2(joystick.Horizontal, joystick.Vertical).magnitude;
            this.propulsion.startLifetime = proplsnValue;
        }
    }

    private void naming()
    {
        if (photonView.IsMine)
        {
            text.text = PhotonNetwork.NickName;
        }
        else text.text = photonView.Owner.NickName;
    }

    void zoon()
    {
        // Adjust the virtual camera's field of view
        virtualCamera.m_Lens.FieldOfView = Mathf.Clamp(60.5f + rb.velocity.magnitude, 55.5f, 74.1f);
        //bground.transform.position = new Vector3(bground.transform.position.x, bground.transform.position.y, Mathf.Clamp(-1.2f - rb.velocity.magnitude / 10, -1.2f, -3.1f));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("missile") && !electricPwrOn)
        {
            Destroy(collision.gameObject);
            Instantiate(explosionPrefab, gameObject.transform.position, Quaternion.identity);
            if (photonView.IsMine)
            {
                lifeSlider.value -= 0.2f;
            }
            if (lifeSlider.value <= 0.01f)
            {
                voices[0].Play();
                if (photonView.IsMine)
                {
                    respawningUI.SetActive(true);
                    starTimer = 0f;
                    isrunning = true;
                    InvokeRepeating("setTimer", 0.5f, 1f);
                }
                photonView.RPC("collisionWithRocket",RpcTarget.AllViaServer);
            }

            //restartButton.SetActive(true);
            if (collision.gameObject.GetComponent<missile>().ownerId != this)
            {
                collision.gameObject.GetComponent<missile>().ownerId.AddKill();
            }
        }
        else if (collision.gameObject.CompareTag("hammer") && !electricPwrOn)
        {
            Instantiate(explosionPrefab, gameObject.transform.position, Quaternion.identity);
            Destroy(gameObject);

            voices[0].Play();
            photonView.RPC("collisionWithRocket", RpcTarget.AllViaServer);
            if (collision.gameObject.GetComponent<missile>().ownerId != this)
            {
                collision.gameObject.GetComponent<missile>().ownerId.AddKill();
            }
        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(rb.rotation);
            stream.SendNext(rb.velocity);
            stream.SendNext(rb.angularVelocity);
        }
        else
        {
            transform.rotation = (Quaternion)stream.ReceiveNext();
            rb.velocity = (Vector3)stream.ReceiveNext();
            rb.angularVelocity = (float)stream.ReceiveNext();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("fuel"))
        {
            currentFuel = Mathf.Clamp(currentFuel + fuelpower, 0f, fuelCapacity);
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("petrolpump")) 
        {
            StartRefueling();
        }
        else if (collision.gameObject.CompareTag("teleporter"))
        {
            int ft = (int)Random.Range(1f, 6f);
            FindObjectOfType<AudioMnagaer>().Play("teleport");
            switch (ft)
            {
                case (1): 
                    transform.position = new Vector2(63f, 653f);
                    break;
                case 2:
                    transform.position = new Vector2(-183f, 360f);
                    break;
                case 3:
                    transform.position = new Vector2(-40f, 324.2f);
                    break;
                case 4:
                    transform.position = new Vector2(276f, 339.2f);
                    break;
                case 5:
                    transform.position = new Vector2(63f, 653f);
                    break;
            }
        }
       
        else if (collision.gameObject.CompareTag("redPower"))
        {
            redRocketPowerEffect.SetActive(true);
            redRocketPowerEffect.GetComponent<ParticleSystem>().Play(true);
            sparksPwrOn = true;
            Invoke("sparksPwrOff", 10f);
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("bluePower"))
        {
            if(photonView.IsMine)FindObjectOfType<AudioMnagaer>().Play("electricpwr");
            electricPwr.gameObject.SetActive(true);
            electricPwr.Play(true);
            electricPwrOn = true;
            Invoke("electricPowerOff", 10f);
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("blackhole"))
        {
            blackholeTarget = collision.gameObject;
            isInblackhole = true;
        }
        else if (collision.gameObject.CompareTag("greenPower") && photonView.IsMine)
        {
            Instantiate(greenPowerEfct, gameObject.transform.position, Quaternion.identity);
            FindObjectOfType<AudioMnagaer>().Play("healthPwr");
            lifeSlider.value = 1f;
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("bulletGain"))
        {
            Destroy(collision.gameObject);
            //whichWeapon[0] = true;
        }
        else if (collision.gameObject.CompareTag("bombGain"))
        {
            Destroy(collision.gameObject);
            //whichWeapon[1] = true;
        }
        else if (collision.gameObject.CompareTag("redmissileGain")) 
        {
            Destroy(collision.gameObject);
            //whichWeapon[2] = true;
        }
        else if (collision.gameObject.CompareTag("hammerGain")) 
        {
            Destroy(collision.gameObject);
            //whichWeapon[3] = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("petrolpump"))
        {
            StopRefueling();
        }
        else if (collision.gameObject.CompareTag("blackhole"))
        {
            isInblackhole = false;
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
    private void ShootBullet(Vector3 position, Quaternion rotation, Vector2 force)
    {
        GameObject bullet = Instantiate(missiles[0], position, rotation);
        Rigidbody2D bulletRigidbody = bullet.GetComponent<Rigidbody2D>();
        
        //bulletRigidbody.velocity = currentDirection * missileSpeed;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, transform.rotation.z + 90f);
        bullet.transform.rotation = Quaternion.Slerp(bullet.transform.rotation, targetRotation, Time.deltaTime);
        if (photonView.IsMine) voices[1].Play();
        bulletRigidbody.AddForce(force);
        //bullet.transform.Translate(force*100f);
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), bullet.GetComponent<Collider2D>());
        bullet.GetComponent<missile>().ownerId = this;
    }
    private void setTimer()
    {
        if (isrunning && photonView.IsMine)
        {
            respawnTimer.text = $"Respwaning in {--seconds}";
        }
    }
    [PunRPC]
    private void ShootDualMissile(Vector3 position, Quaternion rotation, Vector2 force)
    {
        for (int k = 0; k < 2; k++)
        {
            GameObject bullet = Instantiate(missiles[4], position * (1 + k * 0.002f), rotation);
            bullet.transform.Rotate(bullet.transform.rotation.x, bullet.transform.rotation.y, bullet.transform.rotation.z + 180f, Space.Self);

            Rigidbody2D bulletRigidbody = bullet.GetComponent<Rigidbody2D>();
            //bulletRigidbody.velocity = currentDirection * missileSpeed;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, bullet.transform.rotation.z);
            bullet.transform.rotation = Quaternion.Slerp(bullet.transform.rotation, targetRotation, Time.deltaTime);

            bulletRigidbody.AddForce(force);
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), bullet.GetComponent<Collider2D>());

            bullet.GetComponent<missile>().ownerId = this;
            //bullet.GetComponent<missile>().shooterName = photonView.Owner.NickName;
        }
    }
    [PunRPC]
    private void ShootBomb(Vector3 position, Quaternion rotation)
    {
        GameObject bullet = Instantiate(missiles[2], position, rotation);
        Rigidbody2D bulletRigidbody = bullet.GetComponent<Rigidbody2D>();
        //bulletRigidbody.velocity = currentDirection * missileSpeed;

        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), bullet.GetComponent<Collider2D>());

        bullet.GetComponent<missile>().ownerId = this;
        //bullet.GetComponent<missile>().shooterName = photonView.Owner.NickName;

    }
    [PunRPC]
    private void ShootMine(Vector3 position, Quaternion rotation)
    {
        GameObject bullet = Instantiate(missiles[1], position, rotation);
        Rigidbody2D bulletRigidbody = bullet.GetComponent<Rigidbody2D>();
        //bulletRigidbody.velocity = currentDirection * missileSpeed;

        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), bullet.GetComponent<Collider2D>());

        bullet.GetComponent<missile>().ownerId = this;
        //bullet.GetComponent<missile>().shooterName = photonView.Owner.NickName;

    }
    [PunRPC]
    private void ShootTriBomb(Vector3 position, Quaternion rotation, Vector2 direction)
    {
        GameObject bullet = Instantiate(missiles[2], position, rotation);
        Rigidbody2D bulletRigidbody = bullet.GetComponent<Rigidbody2D>();
        //bulletRigidbody.velocity = currentDirection * missileSpeed;
        bullet.transform.DOMove(GameObject.FindGameObjectWithTag("Player").transform.position, 4f, true);
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), bullet.GetComponent<Collider2D>());

        bullet.GetComponent<missile>().ownerId = this;
        //bullet.GetComponent<missile>().shooterName = photonView.Owner.NickName;

    }
    [PunRPC]
    private void ShootPointerMissile(Vector3 position, Quaternion rotation, Vector2 force)
    {
        GameObject bullet = Instantiate(missiles[5], position, new Quaternion(rotation.x, rotation.y, rotation.z, rotation.w));
        bullet.transform.Rotate(bullet.transform.rotation.x, bullet.transform.rotation.y, bullet.transform.rotation.z + 180f, Space.Self);
        Rigidbody2D bulletRigidbody = bullet.GetComponent<Rigidbody2D>();
        //bulletRigidbody.velocity = currentDirection * missileSpeed;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, bullet.transform.rotation.z);
        bullet.transform.rotation = Quaternion.Slerp(bullet.transform.rotation, targetRotation, Time.deltaTime);
        if(photonView.IsMine) voices[1].Play();
        bulletRigidbody.AddForce(force);
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), bullet.GetComponent<Collider2D>());

        bullet.GetComponent<missile>().ownerId = this;
        //bullet.GetComponent<missile>().shooterName = PhotonNetwork.NickName;

    }
    public void onClickFireMissile()
    {
        weapon++;
        if(photonView.IsMine) FindObjectOfType<AudioMnagaer>().Play("weaponChange");
        weaponIconImage.sprite = weaponIcons[weapon%7];
    }
    public void onClickFireBomb()
    {
        Vector3 bombPosition = new Vector3(this.transform.position.x, this.transform.position.y + rb.velocity.y * missileAdjustment, this.transform.position.z);
        if (photonView.IsMine)
            voices[1].Play();
        photonView.RPC("ShootBomb", RpcTarget.AllViaServer, bombPosition, this.transform.rotation);
    }
    public void onClickFireMine()
    {
        Vector3 minePosition = new Vector3(this.transform.position.x, this.transform.position.y + rb.velocity.y * missileAdjustment, this.transform.position.z);
        if (photonView.IsMine)
            voices[1].Play();
        photonView.RPC("ShootMine", RpcTarget.AllViaServer, minePosition, this.transform.rotation);
    }

    [PunRPC]
    private void collisionWithRocket()
    {
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        gameObject.SetActive(false);
        Invoke("Respawn", 6f);
    }

    private void Respawn()
    {
        // Set the position and enable the GameObject
        if(photonView.IsMine) respawningUI.SetActive(false);
        transform.position = playerSpwanPosition;
        lifeSlider.value = 1f;
        isrunning = false;
        CancelInvoke("setTimer");
        seconds = 6;
        gameObject.SetActive(true);
    }

    public void AddKill()
    {
        kills++;
        leaderBoard();
        killsText.text = "kills -" + kills.ToString();
    }
   
    public void shootJoystick()
    {
        Vector2 dirToShoot = new Vector2(weaponJoystick.Horizontal, weaponJoystick.Vertical) * 10f;
        
        float angle = Mathf.Atan2(dirToShoot.y, dirToShoot.x) * Mathf.Rad2Deg;
        if (dirToShoot.magnitude > 0f)
        {
            arrowTrget.SetActive(true);
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle + 90f);
            Quaternion targetRotation2 = Quaternion.Euler(0f, 0f, angle);
            arrowTrget.transform.rotation = Quaternion.Slerp(arrowTrget.transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
            mainGun.transform.rotation = Quaternion.Slerp(mainGun.transform.rotation, targetRotation2, rotationSpeed * Time.fixedDeltaTime);
        }
        else
        {
            arrowTrget.SetActive(false);
        }
        if (dirToShoot.magnitude == 10f && Time.time >= nextFireTime)
        {

            x= weapon % 7;
            Vector3 missilePosition = new Vector3(this.transform.position.x , this.transform.position.y + rb.velocity.y * missileAdjustment, this.transform.position.z);

            Vector2 randomForce = currentDirection * missileSpeed * 50f;

            //photonView.RPC("ShootTriBomb", RpcTarget.AllViaServer, missilePosition, this.transform.rotation, missileDirection);
            switch (x)
            {
                case 0:
                    photonView.RPC("ShootBullet", RpcTarget.AllViaServer, missilePosition, arrowTrget.transform.rotation, dirToShoot * 30f);
                    if (photonView.IsMine)
                    {
                        shakeCamera();
                        Invoke("stopShake", timerShake);
                    }

                    nextFireTime = Time.time + 0.15f;
                    break;
                case 1:
                    onClickFireMine();
                    nextFireTime = Time.time + 0.8f;
                    break;
                case 2:
                    onClickFireBomb();
                    nextFireTime = Time.time + 0.5f;
                    break;//this si the emain functions and the best part of it is that we can make a new option out of it and the options change..
                case 3:
                    if (photonView.IsMine)
                    {
                        missiles[3].SetActive(true);
                        Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle - 90f);
                        nextFireTime = Time.time + 0.1f;
                        missiles[3].transform.rotation = Quaternion.Slerp(missiles[3].transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime * 3f);
                    }
                    break;
                case 4:
                    dulmissile.SetActive(true);
                    photonView.RPC("ShootDualMissile", RpcTarget.AllViaServer, missilePosition, arrowTrget.transform.rotation, dirToShoot * 160f);
                   
                    nextFireTime = Time.time + 1f;
                    break;
                case 5:
                    pointerMissile.SetActive(true);
                    photonView.RPC("ShootPointerMissile", RpcTarget.AllViaServer, missilePosition, arrowTrget.transform.rotation, dirToShoot * 160f);
                    nextFireTime = Time.time + 0.8f;
                    break;
            }
        }        
    }

    private void shakeCamera()
    {
        _cameraChannelPerlin = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _cameraChannelPerlin.m_AmplitudeGain = shakeIntensity;
        timerShake = shakeTime;
    }
    void stopShake()
    {
        _cameraChannelPerlin.m_AmplitudeGain = 0f;
        timerShake = 0f;
    }
    void statusOfWeapons()
    {
        if (x != 3) missiles[3].SetActive(false);
        if (x != 0) mainGun.SetActive(false);
        if (x == 0) mainGun.SetActive(true);
    }
    void breakForce()
    {
        if (rb.velocity.magnitude > 0f && movement.magnitude < 0.1f)
        {
            // Calculate the friction force in the opposite direction of the last movement
            Vector3 frictionForceDirection = -rb.velocity;
            frictionForceDirection.Normalize();

            // Apply the friction force
            rb.AddForce(frictionForceDirection * frictionForce);
        }
    }
    void leaderBoard()
    {
        try
        {
            Hashtable hash = PhotonNetwork.LocalPlayer.CustomProperties;

            if (photonView.IsMine)
            {
                hash["kills"] = kills;
                //hash["life"] = lifeSlider.value;
                //hash["propvalue"] = proplsnValue;
            }

            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
        catch
        {
            //do nothing
        }
    }
    
    public void refresh()
    {
        int i = 0;
        leaderBordText.text = "";
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.TryGetValue("kills", out object killsObj))
            {
                if (killsObj != null)
                {
                    int kills = (int)killsObj;
                    leaderBordText.text = leaderBordText.text + player.NickName + "-  " + kills + "\n";
                    //leaderBordText.color = colorsForNicknames[i++];
                }
            }
        }
    }
   
    // Called when disconnected from Photon

    private void blackholePhysics()
    {
        //blackholeParent.transform.DOMove(new Vector2(-2f, 511f), 100f, false);
        if (isInblackhole)
        {
            rb.AddForce((blackholeTarget.transform.position - transform.position) * 2f);
        }
    }
    private void electricPowerOff()
    {
        electricPwrOn = false;
        electricPwr.Stop();
        electricPwr.gameObject.SetActive(false);
    }
    private void sparksPwrOff()
    {
        sparksPwrOn = false;
        redRocketPowerEffect.GetComponent<ParticleSystem>().Stop();
        redRocketPowerEffect.SetActive(false);
    }
    public void OnJoystickClicked()
    {
        Vector2 dirToShoot = new Vector2(joystick.Horizontal, joystick.Vertical) * 10f;
        float angle = Mathf.Atan2(dirToShoot.y, dirToShoot.x) * Mathf.Rad2Deg;
        if (movement.magnitude > 0.01f && Time.time >= nextFireTime)
        {
            int x = weapon % 7;
            Vector3 missilePosition = new Vector3(this.transform.position.x, this.transform.position.y + rb.velocity.y * missileAdjustment, this.transform.position.z);

            Vector2 randomForce = currentDirection * missileSpeed * 50f;

            //photonView.RPC("ShootTriBomb", RpcTarget.AllViaServer, missilePosition, this.transform.rotation, missileDirection);
            switch (x)
            {
                case 0:
                    photonView.RPC("ShootBullet", RpcTarget.AllViaServer, missilePosition, arrowTrget.transform.rotation, dirToShoot * 50f);
                    nextFireTime = Time.time + 0.12f;

                    break;
                case 1:
                    onClickFireMine();
                    nextFireTime = Time.time + 0.8f;
                    break;
                case 2:
                    onClickFireBomb();
                    nextFireTime = Time.time + 0.5f;
                    break;
                case 3:
                    if (photonView.IsMine)
                    {
                        missiles[3].SetActive(true);
                        Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle - 90f);
                        nextFireTime = Time.time + 0.1f;
                        missiles[3].transform.rotation = Quaternion.Slerp(missiles[3].transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime * 3f);
                    }
                    break;
                case 4:
                    dulmissile.SetActive(true);
                    photonView.RPC("ShootDualMissile", RpcTarget.AllViaServer, missilePosition, dirToShoot, dirToShoot * 100f);

                    nextFireTime = Time.time + 1f;
                    break;
                case 5:
                    pointerMissile.SetActive(true);
                    photonView.RPC("ShootPointerMissile", RpcTarget.AllViaServer, missilePosition, arrowTrget.transform.rotation, dirToShoot * 120f);
                    nextFireTime = Time.time + 0.8f;
                    break;
            }
        }
    }
}


