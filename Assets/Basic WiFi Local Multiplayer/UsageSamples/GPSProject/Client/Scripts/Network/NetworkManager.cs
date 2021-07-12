using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UDPCore;



public class NetworkManager : MonoBehaviour
{

    //from UDP Socket API
    private UDPComponent udpClient;


    //useful for any gameObject to access this class without the need of instances her or you declare her
    public static NetworkManager instance;

    //flag which is determined the player is logged in the arena
    public bool onLogged = false;

    public int serverPort = 3310;

    public int clientPort = 3000;

    public bool serverFound;

    public bool joined;

    public bool isMasterServer;

    public bool waitingSearch;

    public string clientID;

    public string serverID;

    public int maxReconnectTimes = 10;

    public int contTimes;

    public float maxTimeOut;

    public float timeOut;

    public bool waitingConnection;

    public enum UserType { SERVER, CLIENT };


    public UserType userType; // tipo da celula, definido pela enumeracao 


    // Use this for initialization
    void Start()
    {

        // if don't exist an instance of this class
        if (instance == null)
        {

            //it doesn't destroy the object, if other scene be loaded
            DontDestroyOnLoad(this.gameObject);

            instance = this;// define the class as a static variable


            udpClient = gameObject.GetComponent<UDPComponent>();

        }
        else
        {
            //it destroys the class if already other class exists
            Destroy(this.gameObject);
        }

    }




    /// <summary>
    /// Connect client to UDP server.
    /// </summary>
    public void ConnectToUDPServer()
    {


        if (udpClient.GetServerIP() != string.Empty)
        {


            int randomPort = UnityEngine.Random.Range(3001, 3310);

            //connect to udp server
            udpClient.connect(udpClient.GetServerIP(), serverPort, randomPort);

            //map the OnPrintPongMsg callback.
            udpClient.On("PONG", OnPrintPongMsg);

            //map the OnReceiveString callback.
            udpClient.On("RECEIVE_STRING", OnReceiveString);

            udpClient.On("JOIN_SUCCESS", OnJoinSuccess);

            //map the OnUserDisconnected callback.
            udpClient.On("USER_DISCONNECTED", OnUserDisconnected);


        }


    }

    void Update()
    {


        if (udpClient.noNetwork)
        {


            Interactions.instance.StatusText.text = "Please Connect to Wifi Hotspot";

            serverFound = false;
        }


        //if it was not found a server
        if (!serverFound)
        {


            Interactions.instance.StatusText.text = string.Empty;

            if (!udpClient.noNetwork)
            {
                Interactions.instance.StatusText.text = "Please start the server ";

            }
            else
            {

                Interactions.instance.StatusText.text = "Please Connect to Wifi Hotspot ";
            }


            StartCoroutine("PingPong");
        }
        //found server
        else
        {

            StartCoroutine("ConnectionTest");


        }

    }


    //executa um testes de conexão com o servidor
    private IEnumerator ConnectionTest()
    {

        if (waitingConnection)
        {
            yield break;
        }

        waitingConnection = true;

        //sends a ping to server
        EmitPing();

        contTimes++;
        if (contTimes >= 4)
        {

            serverFound = false;
            contTimes = 0;
            waitingConnection = false;
            waitingSearch = false;
            yield break;

        }

        // wait 1 seconds and continue
        yield return new WaitForSeconds(3);



        waitingConnection = false;

    }

    /// <summary>
    /// corroutine called  of times in times to send a ping to the server
    /// </summary>
    /// <returns>The pong.</returns>
    private IEnumerator PingPong()
    {

        if (waitingSearch)
        {
            yield break;
        }

        waitingSearch = true;

        //sends a ping to server
        EmitPing();



        // wait 1 seconds and continue
        yield return new WaitForSeconds(1);



        waitingSearch = false;

    }



    //it generates a random id for the local player
    public string generateID()
    {
        string id = Guid.NewGuid().ToString("N");

        //reduces the size of the id
        id = id.Remove(id.Length - 15);

        return id;
    }

    /// <summary>
    ///  receives an answer of the server.
    /// from  void OnReceivePing(string [] pack,IPEndPoint anyIP ) in server
    /// </summary>
    public void OnPrintPongMsg(UDPEvent data)
    {

        /*
		 * data.pack[0]= CALLBACK_NAME: "PONG"
		 * data.pack[1]= "pong!!!!"
		*/

        if (!joined)
        {

            EmitJoinServer();
        }

        serverFound = true;

        contTimes = 0;

        //arrow the located text in the inferior part of the game screen
        Interactions.instance.StatusText.text = "Connected to server!";

    }




    // <summary>
    /// sends ping message to UDPServer.
    ///     case "PING":
    ///     OnReceivePing(pack,anyIP);
    ///     break;
    /// take a look in UDPServer.cs script
    /// </summary>
    public void EmitPing()
    {

        //hash table <key, value>	
        Dictionary<string, string> data = new Dictionary<string, string>();

        //JSON package
        data["callback_name"] = "PING";

        //store "ping!!!" message in msg field
        data["msg"] = "ping!!!!";

        //Interactions.instance.ShowAlertDialog ("try emit ping");
        //The Emit method sends the mapped callback name to  the server
        udpClient.EmitToServer(data["callback_name"], data["msg"]);
        //Interactions.instance.ShowAlertDialog ("ping sended: "+serverFound);
        Debug.Log("ping sended: " + serverFound);

    }




    /// <summary>
    /// recebe do aplicativo do professor uma ordem para abrir um aplicativo na aplicação dos alunos
    /// </summary>
    /// <param name="_msg">Message.</param>
    void OnReceiveString(UDPEvent data)
    {

        /*
		 * data.pack[0] = RECEIVE_STRING
		 * data.pack[1] = heading
         * data.pack[2] = lat
         * data.pack[3] = lng
		*/

        print("Received: " + data.pack.Length + " packets of data");
        Interactions.instance.HeadingText.text = $"Heading: {(data.pack[1])}";
        Interactions.instance.LatText.text = $"Lat: {(data.pack[2])}";
        Interactions.instance.LngText.text = $"Lg: {(data.pack[3])}";
    }




    public void EmitJoinServer()
    {
        if (serverFound)
        {
            clientID = generateID();
            if (udpClient.serverRunning)
            {
                isMasterServer = true;
                serverID = clientID;

            }
            udpClient.EmitToServer("JOIN", clientID);
        }
    }


    public void OnJoinSuccess(UDPEvent data)
    {
        joined = true;
        print("NetworkManager Join Success: " + data.pack[1]);
        UIManager.instance.ClientText.text = "--- Joined ---";
        UIManager.instance.ClientText.color = Color.green;
    }




    /// <summary>
    /// inform the local player to destroy offline network player
    /// </summary>
    /// <param name="_msg">Message.</param>
    //desconnect network player
    void OnUserDisconnected(UDPEvent data)
    {

        /*
		 * data.pack[0]  = USER_DISCONNECTED
		 * data.pack[1] = id (network player id)
		*/
        Debug.Log("disconnect!");



    }

    void CloseApplication()
    {

        if (udpClient != null)
        {


            udpClient.disconnect();

        }
    }


    void OnApplicationQuit()
    {

        Debug.Log("Application ending after " + Time.time + " seconds");

        CloseApplication();

    }


    public string GetUserType()
    {
        switch (userType)
        {

            case UserType.CLIENT:
                return "client";
            case UserType.SERVER:
                return "server";

        }
        return string.Empty;
    }


    /// <summary>
    /// Sets the type of the user.
    /// </summary>
    /// <param name="_userType">User type.</param>
    public void SetUserType(string _userType)
    {
        switch (_userType)
        {

            case "client":
                userType = UserType.CLIENT;
                break;
            case "server":
                userType = UserType.SERVER;
                break;
        }
    }

}
