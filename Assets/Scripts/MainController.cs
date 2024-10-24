using Photon.Pun;
using Photon.Realtime;
using System.Collections;                    //THIS PROJECT WILL COMPLETE BY 8 NOV 2024
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainController : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    [SerializeField] private GameObject UsernameMenu, settingsMenu, skinSelectionMenu;
    [SerializeField] private GameObject ConnectPanel;
    [SerializeField] private InputField UsernameInput;
    [SerializeField] private Text nameTopText;

    [SerializeField] private InputField CreateInput;
    [SerializeField] private InputField JoinInput;
    [SerializeField] private GameObject StartButton, loadingPanel;

    [SerializeField] private int maxPlayers = 20;
    [SerializeField] private GameObject ccamera;
    [SerializeField] private GameObject leftButton, Rightbutton;
    [SerializeField] private GameObject joinRoomCanvas, rocketSkin;
    public SpriteRenderer spriteRenderer;
    public Sprite[] sprites; // Array of different sprites
    public Sprite btnwhite;
    public Button btnUsername;
    private int currentIndex = 4; // Current index of the sprite
    public float shiftAmount = 0.2f;      // Amount of camera shift based on tilt controls
    public GameObject loadingBar;
    private Vector2 tiltInput;            // Store the tilt input values
    private Vector3 startPosition;
    public float tiltSpeed = 2f;          // Speed of camera movement based on tilt controls
    public float maxHorizontalOffset = 2.8f;  // Maximum horizontal offset from the center of the screen
    public float maxVerticalOffset = 2.8f;    // Maximum vertical offset from the center of the screen

    private void Start()
    {
        UsernameMenu.SetActive(true);
        FindObjectOfType<AudioMnagaer>().Play("start");
        string usernameCatch = PlayerPrefs.GetString("name");
        startPosition = ccamera.transform.position;
        if(usernameCatch != null)
        {
            UsernameMenu.SetActive(false);
            PhotonNetwork.NickName = usernameCatch;
            nameTopText.text = usernameCatch;
            ConnectPanel.SetActive(true);
            rocketSkin.SetActive(true);
        }
        if (sprites.Length > 0)
        {
            spriteRenderer.sprite = sprites[currentIndex];
        }
    }

    private void Awake()
    {
        PhotonNetwork.ConnectUsingSettings();
        //playButton.SetActive(false);
    }

    private void Update()
    {
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster() was called by PUN.");
        //PhotonNetwork.JoinRandomRoom();
    }
    public void changeUserNameInput()
    {
        StartButton.SetActive(true);
    }
    public void SetUsername()
    {
        btnUsername.GetComponent<Image>().sprite = btnwhite;
        FindObjectOfType<AudioMnagaer>().Play("pop");
        PlayerPrefs.SetString("name", UsernameInput.text);
        UsernameMenu.SetActive(false);
        PhotonNetwork.NickName = UsernameInput.text;
        ConnectPanel.SetActive(true);
        rocketSkin.SetActive(true);

    }
    public void OnclickPlay()
    {
        PhotonNetwork.JoinRandomRoom();
        FindObjectOfType<AudioMnagaer>().Play("start");
        ConnectPanel.SetActive(false);
        loadingPanel.SetActive(true);
    }
    public void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = maxPlayers;

        PhotonNetwork.CreateRoom(Random.Range(1f,2f).ToString(), roomOptions, null);
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        ToastMessage toast = new ToastMessage();
        toast.ShowToast("No Room Found");
        ConnectPanel.SetActive(true);
        loadingPanel.SetActive(false);
    }
    public void onclickSettings()
    {// the main menu is connect panel
        settingsMenu.SetActive(true);
        ConnectPanel.SetActive(false);
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("Joined a room!");
        // Start the game or perform any other necessary actions
        Debug.Log(PhotonNetwork.CountOfPlayers);
        StartGame();
    }
    private void StartGame()
    {
        // Add your game start logic here
        Debug.Log("Game started!");
        /*if(PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            
        }*/
        // Set the integer value as a custom property for the local player

        PlayerPrefs.SetInt("MyIntegerData", currentIndex);
        PlayerPrefs.Save();
        PhotonNetwork.LoadLevel("SampleScene");
    }
    public void JoinRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = maxPlayers;
        // will change this function to only joinroom in
        PhotonNetwork.JoinOrCreateRoom(JoinInput.text, roomOptions, TypedLobby.Default);
        loadingPanel.SetActive(true);
        joinRoomCanvas.SetActive(false);
    }
    public void changeSkin()
    {
        skinSelectionMenu.SetActive(true);
        ConnectPanel.SetActive(false);
        FindObjectOfType<AudioMnagaer>().Play("pop");

    }
    public void skin1()
    {
        currentIndex = 0;
        FindObjectOfType<AudioMnagaer>().Play("pop");
    }public void skin2()
    {
        currentIndex = 1;
        FindObjectOfType<AudioMnagaer>().Play("pop");
    }public void skin3()
    {
        currentIndex = 2;
        FindObjectOfType<AudioMnagaer>().Play("pop");
    }public void skin4()
    {
        currentIndex = 3;
        FindObjectOfType<AudioMnagaer>().Play("pop");
    }
    public void cutSkinmenu()
    {
        skinSelectionMenu.SetActive(false);
        FindObjectOfType<AudioMnagaer>().Play("pop");
        ConnectPanel.SetActive(true);
        spriteRenderer.sprite = sprites[currentIndex];
        PlayerPrefs.SetInt("skinIndex", currentIndex);
    }
    public void onClickjoinRoom()
    {
        FindObjectOfType<AudioMnagaer>().Play("pop");

        ConnectPanel.SetActive(false);
        rocketSkin.SetActive(false);
        joinRoomCanvas.SetActive(true);
    }
    public void onClickBack()
    {
        FindObjectOfType<AudioMnagaer>().Play("pop");

        ConnectPanel.SetActive(true);
        rocketSkin.SetActive(true);
        joinRoomCanvas.SetActive(false);
    }
    public void changeSkinScroll()
    {
        currentIndex = (currentIndex + 1) % sprites.Length;
        Debug.Log(currentIndex);
    }

    public static implicit operator MainController(ParticleSystem v)
    {
        throw new System.NotImplementedException();
    }
}
