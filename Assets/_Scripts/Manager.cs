using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Manager : MonoBehaviour
{
    public GameObject Player;
    public Transform SpawnPos;
    public void Awake()
    {
        GameObject NewPlayer = PhotonNetwork.Instantiate(Player.name, SpawnPos.position, SpawnPos.rotation);
    }
}
