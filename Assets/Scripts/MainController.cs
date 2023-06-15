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



    private void Start()
    {
        UsernameMenu.SetActive(true); 
    }

    private void Awake()
    {
        PhotonNetwork.ConnectUsingSettings();
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
