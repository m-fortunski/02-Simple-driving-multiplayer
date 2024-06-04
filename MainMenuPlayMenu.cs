using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.Netcode.Transports.UTP;

public class MainMenuPlayMenu : NetworkBehaviour
{
    public TMP_InputField IPInput;
    public TMP_InputField PortInput;
    string defaultIP = "127.0.0.1";

    public void JoinGame()
    {
        if(IPInput.text == "") { IPInput.text = "127.0.0.1"; }
        if (PortInput.text == "") { PortInput.text = "7777"; }
        NetworkManager.GetComponent<UnityTransport>().SetConnectionData(IPInput.text, ushort.Parse(PortInput.text));
        GameObject.Find("NetworkData").GetComponent<PlayerNetworkData>().IsClient = true;
        GameObject.Find("NetworkData").GetComponent<PlayerNetworkData>().IsHost = false;
    }
    public void HostGame()
    {
        if (IPInput.text == "") { IPInput.text = "127.0.0.1"; }
        if (PortInput.text == "") { PortInput.text = "7777"; }
        NetworkManager.GetComponent<UnityTransport>().SetConnectionData(IPInput.text, ushort.Parse(PortInput.text));
        GameObject.Find("NetworkData").GetComponent<PlayerNetworkData>().IsHost = true;
        GameObject.Find("NetworkData").GetComponent<PlayerNetworkData>().IsClient = false;
    
    }
}
