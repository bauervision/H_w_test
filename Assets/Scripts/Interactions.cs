using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Interactions : MonoBehaviour
{
    public static Interactions instance;

    public GameObject ConnectToServerButton;
    public TextMeshPro LatText;
    public TextMeshPro LngText;
    public TextMeshPro HeadingText;
    public TextMeshPro StatusText;


    private void Start()
    {
        instance = this;

        LatText.text = "Lat: ";
        LngText.text = "Lng: ";
        HeadingText.text = "Heading: ";
        StatusText.text = "";
    }

    public void ServerConnect()
    {
        NetworkManager.instance.ConnectToUDPServer();
        //set user type
        NetworkManager.instance.SetUserType("client");
        ConnectToServerButton.SetActive(false);




    }
}
