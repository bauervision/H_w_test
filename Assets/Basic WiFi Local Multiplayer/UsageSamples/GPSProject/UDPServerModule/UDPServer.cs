using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using  UDPCore;


namespace UDPServerModule
{

	public class UDPServer : MonoBehaviour {

	    public static UDPServer instance;

		private UDPComponent udpServer;

		public int onlineClients;

		public float maxTimeOut;
		
		//store all players in game
		public Dictionary<string, Client> connectedClients = new Dictionary<string, Client>();

		public float cont;
		
		public string id;
		

	 public class Client
    {
        public string id;
        public double lat;
        public double lng;
        public float heading;
        public IPEndPoint remoteEndPoint;

    }

	


		// Use this for initialization
		void Start () {

			// if don't exist an instance of this class
			if (instance == null) {

				//it doesn't destroy the object, if other scene be loaded
				DontDestroyOnLoad (this.gameObject);

				instance = this;// define the class as a static variable

				udpServer = gameObject.GetComponent<UDPComponent>();
				//receives a "PING" notification from client
			    udpServer.On ("PING", OnReceivePing);
				udpServer.On ("JOIN", OnReceiveJoinGame);
				udpServer.On ("SEND_STRING",  OnSendString);
				udpServer.On ("disconnect",  OnReceiveDisconnect);

			}
			else
			{
				//it destroys the class if already other class exists
				Destroy(this.gameObject);
			}

		}

		/// <summary>
		/// Creates a UDP Server in in the associated client
		/// called method when the button "start" on HUDCanvas is pressed
		/// </summary>
		public void CreateServer()
		{
		  
			if(!udpServer.serverRunning)
			{
				 //start server
			    udpServer.StartServer();
				Debug.Log ("UDP Server listening on IP " + udpServer.GetServerIP () + " and port " + udpServer.serverPort);
			    Debug.Log("------- server is running -------");
				
				NetworkManager.instance.ConnectToUDPServer ();
				
				//open server panel
				CanvasManager.instance.OpenScreen (1);
						
				NetworkManager.instance.SetUserType ("server");
			}
			else
			{
				Debug.Log("server already running");
			}
			
				

		}




		void OnReceivePing(UDPEvent data  )
		{
			/*
		       * pack[0]= CALLBACK_NAME: "PONG"
		       * pack[1]= "ping"
		    */

		     Debug.Log ("[INFO] receice PING message ");

			Dictionary<string, string> send_pack = new Dictionary<string, string> ();
					  	
		    //JSON package
		    send_pack ["callback_name"] = "PONG";
			    	
		    //store  player info in msg field
		    send_pack ["msg"] = "pong!!!";
				
		    var response = string.Empty;

		    byte[] msg = null;
	
		    response = send_pack ["callback_name"] + ':' + send_pack ["msg"];

		     msg = Encoding.ASCII.GetBytes (response);

		     //send answer to client that called server
	        udpServer.EmitToClient( msg, data.anyIP);

			Debug.Log ("[INFO] PONG message sended to connected player");

		
		}


	public void OnReceiveJoinGame(UDPEvent data)
    {
        print("Server: OnJoin = " + data.pack[1]);


		Client newClient = new Client();
        newClient.id = data.pack [1];
        newClient.lat = 00.111111;
        newClient.lng = 11.222222;
        newClient.heading = 0f;
        newClient.remoteEndPoint = data.anyIP;

		 // add this newly connected client to our list
		connectedClients.Add ( newClient.id.ToString (), newClient);

		onlineClients = connectedClients.Count;

		print(connectedClients.Count + " connected Clients on the network");

	
		UIManager.instance.ServerText.text = " ---Server Connected to " + connectedClients.Count + " clients---";
        UIManager.instance.ServerText.color = Color.green;

        // and send a response to the client
        byte[] msg = Encoding.ASCII.GetBytes("JOIN_SUCCESS" + ":" + newClient.id);

        udpServer.EmitToClient(msg, newClient.remoteEndPoint);
        EmitCurrentPlayersToNewPlayer(newClient);
    }

	public void EmitCurrentPlayersToNewPlayer(Client client)
    {

    }

	//this method will be responsible for broadcasting the command to open an application to the clients' applications.
	void OnSendString(UDPEvent data)
	{
			/*
		      * data.pack[0] = CALLBACK_NAME: "SEND_STRING"
		      * data.pack[1] = _app
		    
		              
		    */


			Dictionary<string, string> send_pack = new Dictionary<string, string>();
		
			//JSON package
			send_pack ["callback_name"] = "RECEIVE_STRING";

		    send_pack  ["msg"] =  data.pack [1];
		
		    var response = string.Empty;

		    byte[] msg = null;
		
		
		    //sends the client sender to all clients in game
		    foreach (KeyValuePair<string, Client> entry in connectedClients) {

			  
			   if (!entry.Value.id.Equals(NetworkManager.instance.serverID)) 
			   {
				    //format the data with the ':' for they be send from turn to udp client
			        response = send_pack  ["callback_name"] + ':' + send_pack  ["msg"];

			        msg = Encoding.ASCII.GetBytes (response);

			        //send answer to all clients in connectClients list
			        udpServer.EmitToClient( msg,entry.Value.remoteEndPoint);
				  
			   }
			      
		    }//END_FOREACH
               
		  
		}



			 /// <summary>
	/// Receive the player's disconnection.
	/// </summary>
	/// <param name="data">received package from client.</param>
	void OnReceiveDisconnect(UDPEvent data )
	{
		/*
		     * data.pack[0]= CALLBACK_NAME: "disconnect"
		     * data.pack[1]= player_id
		     * data.pack[2]= isMasterServer (true or false)
		    */

	   
	   if (connectedClients.ContainsKey (data.pack [1])) {
	   
	    Dictionary<string, string> send_pack = new Dictionary<string, string> ();
			
	   //JSON package
		send_pack ["callback_name"] = "USER_DISCONNECTED";
			
		send_pack ["msg"] = connectedClients [data.pack [1]].id.ToString ();

		send_pack ["isMasterServer"] = data.pack [2];

		var response = string.Empty;

		byte[] msg = null;
		
		//sends the client sender to all clients in game
		foreach (KeyValuePair<string, Client> entry in connectedClients) {

			if (entry.Value.id != connectedClients [data.pack [1]].id ) {

			  
				//format the data with the sifter comma for they be send from turn to udp client
			
		       response = send_pack ["callback_name"] + ':' + send_pack ["msg"]+':'+send_pack ["isMasterServer"] ;

				msg = Encoding.ASCII.GetBytes (response);

				//send answer to all clients in connectClients list
				udpServer.EmitToClient( msg,entry.Value.remoteEndPoint);

			}//END_IF

		}//END_FOREACH
		
		
		connectedClients.Remove (data.pack [1]);
		
		
	   }//END_IF
		
	}



	}

}
