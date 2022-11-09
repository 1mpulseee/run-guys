using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public static LobbyManager Instance;
    [System.Serializable] public enum Scenes {Game };
    public Scenes scenes;

    [SerializeField] TMP_Text PlayersText;

    [SerializeField] GameObject ForLoading;
    [SerializeField] GameObject ForLobby;
    [SerializeField] GameObject ForRoom;

    private IEnumerator _AutoConnect;
    private string NewRoomName;
    public TMP_Text RoomName;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        LoadMenu();
        PhotonNetwork.NickName = "Player" + Random.Range(1000, 9999);
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = "1";
        PhotonNetwork.ConnectUsingSettings();
    }

   
    public void JoinToRoomWithName(string RoomName)
    {
        PhotonNetwork.JoinRoom(RoomName);
    }
    public void AutoGame()
    {
        if (_AutoConnect == null)
        {
            _AutoConnect = AutoConnect();
            StartCoroutine(_AutoConnect);
        }
    }
    public IEnumerator AutoConnect()
    {
        JoinRoom();
        yield return new WaitForSeconds(3);
        CreateRoom();
        _AutoConnect = null;
    }
    public void LoadLvl()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(scenes.ToString());
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
        }
        else
        {
            Debug.Log("<color=red>Not Admin</color>");
        }
    }


    void RefreshPlayers()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("ShowPlayers", RpcTarget.All, "players" + "\n" + string.Join("\n", PhotonNetwork.PlayerList.ToStringFull()));
        }
    }
    public void LoadMenu()
    {
        ForLoading.SetActive(true);
        ForLobby.SetActive(false);
        ForRoom.SetActive(false);
    }
    public void LobbyMenu()
    {
        ForLoading.SetActive(false);
        ForLobby.SetActive(true);
        ForRoom.SetActive(false);
    }
    public void RoomMenu()
    {
        ForLoading.SetActive(false);
        ForLobby.SetActive(false);
        ForRoom.SetActive(true);
    }
    public void CreateRoom()
    {
        NewRoomName = Random.Range(10000, 99999).ToString();
        PhotonNetwork.CreateRoom(NewRoomName, new Photon.Realtime.RoomOptions { IsVisible = true, IsOpen = true, MaxPlayers = 16, CleanupCacheOnLeave = false }, Photon.Realtime.TypedLobby.Default);
        Debug.Log("Create");
    }
    public void JoinRoom()
    {
        PhotonNetwork.JoinRandomRoom();
        Debug.Log("Join");
    }




    [PunRPC]
    public void ShowPlayers(string players)
    {
        PlayersText.text = players;
    }


    public override void OnJoinedRoom()
    {
        RoomMenu();
        RefreshPlayers();
        RoomName.text = "Room - " + PhotonNetwork.CurrentRoom.Name;
        if (_AutoConnect != null)
        {
            StopCoroutine(_AutoConnect);
        }
    }
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        RefreshPlayers();
    }
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        RefreshPlayers();
    }
    public override void OnConnectedToMaster()
    {
        LobbyMenu();
    }
    public override void OnLeftRoom()
    {
       LobbyMenu();
    }
}