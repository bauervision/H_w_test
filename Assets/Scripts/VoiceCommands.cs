using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;

public class VoiceCommands : MonoBehaviour
{

    public MixedRealitySpeechCommandsProfile profile;

    #region Voice Commands
    public void VC_MainMenu()
    {
        Debug.Log("You said: Main Menu...");

    }

    #endregion
}
