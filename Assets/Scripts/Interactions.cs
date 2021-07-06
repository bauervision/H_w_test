using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Interactions : MonoBehaviour
{
    public static Interactions instance;
    public TextMeshPro IPText;
    public TextMeshPro StatusText;
    public string foundIP = " 000.000.000.000";

    private void Start()
    {
        instance = this;

        // IPText.text = $"Available Server IP: {foundIP}";
        // StatusText.text = "Connection Status: Not Connected";
    }
    public void ServerSearch()
    {
        Debug.Log("Searching for servers....");
        IPText.text = $"Available Server IP: .........";
        StatusText.text = "Connection Status: Searching...";
    }

    public void ServerConnect()
    {
        Debug.Log("Connection to server....");
        IPText.text = $"Available Server IP: .........";
        StatusText.text = "Connection Status: Connecting...";
    }
}
