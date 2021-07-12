using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour {

	public static  CanvasManager instance;

	public Canvas  HUDHome;

	public Canvas HUDServer;

	public Canvas HUDClient;

	public Canvas alertgameDialog;

	public Text alertDialogText;

	public Text txtSearchServerStatus;

	public GameObject lobbyCamera;
	
	public InputField inputText;

	public int currentMenu;

	public Canvas loadingImg;


	public float delay = 0f;

	public Text txtReceivedString;

	// Use this for initialization
	void Start () {

		if (instance == null) {

			DontDestroyOnLoad (this.gameObject);

			instance = this;

			OpenScreen(0);

			CloseAlertDialog ();
		}
		else
		{
			Destroy(this.gameObject);
		}



	}

	void Update()
	{
		delay += Time.deltaTime;

		if (Input.GetKey ("escape") && delay > 1f) {

		  switch (currentMenu) {

			case 0:
			 Application.Quit ();
			break;

			case 1:
			Application.Quit ();
			 delay = 0f;
			break;
			
			case 2:
			Application.Quit ();
			 delay = 0f;
			break;
			
			case 3:
			 Application.Quit ();
			break;

		 }//END_SWITCH

	 }//END_IF
}
	/// <summary>
	/// Opens the screen.
	/// </summary>
	/// <param name="_current">Current.</param>
	public void  OpenScreen(int _current)
	{
		switch (_current)
		{
		    //lobby menu
		case 0:
			currentMenu = _current;
			HUDHome.enabled = true;
			HUDServer.enabled = false;
			HUDClient.enabled = false;
			break;


		    case 1:
			currentMenu = _current;
			HUDHome.enabled = false;
			HUDServer.enabled = true;
			HUDClient.enabled = false;
			break;

		    case 2:
			HUDHome.enabled = false;
			HUDServer.enabled = false;
			HUDClient.enabled = true;

			break;
	
	
		}

	}


	//open HUD cleint panel
	public void OpenHUDClient()
	{
		//connect client to the server
		NetworkManager.instance.ConnectToUDPServer ();

		//set user type
		NetworkManager.instance.SetUserType ("client");

		//open client panel
		OpenScreen (2);
	}


	public void ShowString(string _message)
	{
		txtReceivedString.text = _message;

	}

	

	

	/// <summary>
	/// Shows the alert dialog.
	/// </summary>
	/// <param name="_message">Message.</param>
	public void ShowAlertDialog(string _message)
	{
		alertDialogText.text = _message;
		alertgameDialog.enabled = true;
	}

	public void ShowLoadingImg()
	{
		loadingImg.enabled = true;


	}
	public void CloseLoadingImg()
	{
		loadingImg.enabled = false;

	}
	/// <summary>
	/// Closes the alert dialog.
	/// </summary>
	public void CloseAlertDialog()
	{
		alertgameDialog.enabled = false;
	}
}
