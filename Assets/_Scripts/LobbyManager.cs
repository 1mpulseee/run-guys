using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    private List<RoomInfo> roomList = new List<RoomInfo>();



    private void Start()
    {
        PhotonNetwork.NickName = "Player" + Random.Range(1000, 9999);
        PhotonNetwork.AutomaticallySyncScene = true; //???????????????? ?????
        PhotonNetwork.GameVersion = "1"; //?????? ????
        PhotonNetwork.ConnectUsingSettings();
    }


    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Game");
    }




    public void StartGame()
    {
        if (CheckRooms())
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            string RoomName = "room" + Random.Range(0, 10000);
            PhotonNetwork.CreateRoom(RoomName, new Photon.Realtime.RoomOptions { IsVisible = true, MaxPlayers = 16, CleanupCacheOnLeave = false }, Photon.Realtime.TypedLobby.Default);
        }
    }




    public bool CheckRooms()
    {
        if (roomList.Count > 0)
        {
            for (int i = 0; i < roomList.Count; i++)
            {
                if (roomList[i].PlayerCount < roomList[i].MaxPlayers)
                {
                    return true;
                }
            }
            return false;
        }
        else
        {
            return false;
        }
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("Server loaded");
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomListUpd)
    {
        roomList = roomListUpd;
    }
}
