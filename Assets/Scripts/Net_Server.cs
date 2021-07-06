using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UDPCore;
using System.Text;
using System.Net;


public class Net_Server : MonoBehaviour
{
    public static Net_Server instance;
    public bool isRunning;

    private UDPComponent udpClient;
    public Dictionary<string, Client> connectedClients = new Dictionary<string, Client>();

    public class Client
    {
        public string id;
        public double lat;
        public double lng;
        public float heading;
        public IPEndPoint remoteEndPoint;

    }

    // Start is called before the first frame update
    void Start()
    {
        //DontDestroyOnLoad(this.gameObject);
        instance = this;

        udpClient = gameObject.GetComponent<UDPComponent>();
        udpClient.On("PING", OnPing);
        udpClient.On("JOIN", OnJoin);

        //UIManager.instance.ServerText.text = "--- Server Not Running ---";
    }


    public void OnPing(UDPEvent data)
    {
        print("Server: OnPing = " + data.pack[1]);

        byte[] msg = Encoding.ASCII.GetBytes("PONG" + ":" + "sending pong!");
        udpClient.EmitToClient(msg, data.anyIP);
    }

    public void OnJoin(UDPEvent data)
    {
        print("Server: OnJoin = " + data.pack[1]);

        Client newClient = new Client();
        newClient.id = data.pack[1];
        newClient.lat = 00.111111;
        newClient.lng = 11.222222;
        newClient.heading = 0f;
        newClient.remoteEndPoint = data.anyIP;

        // add this newly connected client to our list
        connectedClients.Add(newClient.id.ToString(), newClient);
        print(connectedClients.Count + " connected Clients on the network");
        // and send a response to the client
        byte[] msg = Encoding.ASCII.GetBytes("JOIN_SUCCESS" + ":" + newClient.id);

        // UIManager.instance.ServerText.text = " ---Server Connected to " + connectedClients.Count + " clients---";
        // UIManager.instance.ServerText.color = Color.green;

        udpClient.EmitToClient(msg, newClient.remoteEndPoint);

    }



    public void CreateServer()
    {
        if (udpClient.GetIP() != string.Empty)
        {

            if (Net_Manager.instance.serverFound && !udpClient.serverRunning)
            {
                Interactions.instance.StatusText.text = "Server Found";
            }

        }
    }


}
