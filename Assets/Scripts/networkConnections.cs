using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class networkConnections : MonoBehaviour
{
    public GameObject playr;
    public GameObject errorCanvas;
    private PhotonView playerView;
    // Update is called once per frame
    private void Start()
    {
        playerView = playr.gameObject.GetPhotonView();
    }
    void Update()
    {
        if(playr == null && playerView.IsMine)
        {
            Debug.Log("playr died");
            errorCanvas.SetActive(true);
        }
    }
    public void clickRetry()
    {
        PhotonNetwork.LoadLevel("lobbyscene");
    }
}
