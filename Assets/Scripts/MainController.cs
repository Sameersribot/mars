using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainController : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    [SerializeField] private GameObject UsernameMenu;
    [SerializeField] private GameObject ConnectPanel;
    [SerializeField] private InputField UsernameInput;

    [SerializeField] private InputField CreateInput;
    [SerializeField] private InputField JoinInput;
    [SerializeField] private GameObject StartButton;

    [SerializeField]  private int maxPlayers = 4;
    [SerializeField] private GameObject ccamera;
    public float shiftAmount = 0.2f;      // Amount of camera shift based on tilt controls

    private Vector2 tiltInput;            // Store the tilt input values
    private Vector3 startPosition;
    public float tiltSpeed = 2f;          // Speed of camera movement based on tilt controls
    public float maxHorizontalOffset = 2.8f;  // Maximum horizontal offset from the center of the screen
    public float maxVerticalOffset = 2.8f;    // Maximum vertical offset from the center of the screen

    private void Start()
    {
        UsernameMenu.SetActive(true);
        startPosition = ccamera.transform.position;
    }

    private void Awake()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    private void Update()
    {
        // Read the tilt input
        tiltInput.x = Input.acceleration.x;
        tiltInput.y = Input.acceleration.y;
    }
    private void LateUpdate()
    {
        // Calculate the movement vector based on tilt input
        Vector3 movement = new Vector3(tiltInput.x * tiltSpeed, tiltInput.y * tiltSpeed, 0f);

        // Calculate the target position after applying movement
        Vector3 targetPosition = new Vector3(startPosition.x + movement.x, startPosition.y + movement.y, startPosition.z);

        // Clamp the target position within the limits of the screen
        float clampedX = Mathf.Clamp(targetPosition.x, startPosition.x - maxHorizontalOffset, startPosition.x + maxHorizontalOffset);
        float clampedY = Mathf.Clamp(targetPosition.y, startPosition.y - maxVerticalOffset, startPosition.y + maxVerticalOffset);

        // Set the clamped target position
        targetPosition = new Vector2(clampedX, clampedY);

        // Move the camera towards the target position
        ccamera.transform.position = Vector3.Lerp(startPosition, targetPosition, Time.deltaTime);
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster() was called by PUN.");
        PhotonNetwork.JoinRandomRoom();
    }
    public void changeUserNameInput()
    {
        if(UsernameInput.text.Length >= 3)
        {
            StartButton.SetActive(true);
        }
        else
        {
            StartButton.SetActive(false);
        }
    }
    public void SetUsername()
    {
        UsernameMenu.SetActive(false);
        PhotonNetwork.NickName = UsernameInput.text;
    }
    public void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = maxPlayers;
        PhotonNetwork.CreateRoom(CreateInput.text, roomOptions, null);
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to join a random room. Creating a new room...");
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("Joined a room!");
        // Start the game or perform any other necessary actions
        StartGame();
    }
    private void StartGame()
    {
        // Add your game start logic here
        Debug.Log("Game started!");
        PhotonNetwork.LoadLevel("SampleScene");
    }
    public void JoinRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = maxPlayers;
        // will change this function to only joinroom in
        PhotonNetwork.JoinOrCreateRoom(JoinInput.text, roomOptions, TypedLobby.Default);
    }
}
