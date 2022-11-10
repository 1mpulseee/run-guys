using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Manager : MonoBehaviourPunCallbacks
{
    public Transform[] Ways;

    public GameObject PlayerPrefab;
    public Transform SpawnPos;
    public Transform SpawnPosBot;
    private List<GameObject> Bots = new List<GameObject>();
    private GameObject Player;

    public GameObject BotPrefab;
    public GameObject Bot;

    public static Manager Instance { get; private set; }
    public void Awake()
    {
        Instance = this;
        Player = PhotonNetwork.Instantiate(PlayerPrefab.name, SpawnPos.position, SpawnPos.rotation);
        Bot = PhotonNetwork.Instantiate(BotPrefab.name, SpawnPosBot.position, SpawnPosBot.rotation);
    }
    public override void OnLeftRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("HostQuit");
            Photon.Realtime.Player NewHost = null;
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                if (PhotonNetwork.PlayerList[i] != PhotonNetwork.LocalPlayer)
                {
                    NewHost = PhotonNetwork.PlayerList[i];
                    Debug.Log("New - " + NewHost.NickName);
                    break;
                }
            }
            if (NewHost == null)
            {
                return;
            }
            Bot.GetComponent<PhotonView>().TransferOwnership(NewHost);
            PhotonNetwork.SetMasterClient(NewHost);
            photonView.RPC("GiveBots", NewHost, Bot);
            PhotonNetwork.Destroy(Player);
        }
    }
    [PunRPC]
    public void GiveBots(GameObject NewBots)
    {
        Bot = NewBots;
        Debug.Log("BotChange");
    }
    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        Debug.Log("Master - " + newMasterClient.NickName);
    }
}