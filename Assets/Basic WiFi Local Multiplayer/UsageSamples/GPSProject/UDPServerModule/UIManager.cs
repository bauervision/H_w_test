using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public static UIManager instance;

    public Text AppText;
    public Text ServerText;
    public Text ClientText;


    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        AppText.text = GetPlatform();

    }

    private string GetPlatform()
    {
        string currentPlatform = string.Empty;
#if UNITY_EDITOR
        currentPlatform = "Editor";
#endif

#if UNITY_ANDROID
        currentPlatform = "Android";
#endif

#if UNITY_WSA_10_0
        currentPlatform = "Windows";
#endif

        return currentPlatform;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
