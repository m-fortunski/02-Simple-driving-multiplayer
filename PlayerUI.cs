using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using TMPro;


public class PlayerUI : MonoBehaviour
{
    public GameObject JoinButton;
    public TMP_InputField IPInput;
    public GameObject HostButton;
    
    void JoinGame()
    {
        GameObject.Find("NetworkManager").GetComponent<UnityTransport>().ConnectionData.Address = IPInput.text;
    }

    public void AddClient()
    {
        NetworkManager.Singleton.StartClient();
    }
    public void AddHost()
    {
        NetworkManager.Singleton.StartHost();
    }
    public void AddServer()
    {
        NetworkManager.Singleton.StartServer();
    }



}
