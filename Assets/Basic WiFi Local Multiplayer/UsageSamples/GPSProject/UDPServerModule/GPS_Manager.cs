using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Android;
using UnityEngine.Events;

public class GPS_Manager : MonoBehaviour
{
    public static GPS_Manager instance;
    public bool GPS_ready;
    public double deviceLAT;
    public double deviceLNG;
    public float deviceHEADING;


    // Gyro
    public Gyroscope gyro;
    private Quaternion rotation;

    private void Awake()
    {
        if (!Application.isEditor)
            if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
                Permission.RequestUserPermission(Permission.FineLocation);

        if (!Permission.HasUserAuthorizedPermission(Permission.CoarseLocation))
            Permission.RequestUserPermission(Permission.CoarseLocation);
    }


    IEnumerator Start()
    {
        instance = this;

        // First, check if user has location service enabled
        if (!Input.location.isEnabledByUser)
            yield break;

        // Start service before querying location
        Input.location.Start();

        // Wait until service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Service didn't initialize in 20 seconds
        if (maxWait < 1)
        {
            yield break;
        }

        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            yield break;
        }

        // check if we support Gyro
        if (!SystemInfo.supportsGyroscope)
            yield break;
        else
        {
            // enable gyro
            gyro = Input.gyro;
            gyro.enabled = true;
            rotation = new Quaternion(0, 0, 1, 0);
        }

        if (!Application.isEditor)
            GPS_ready = true;


    }

    // Update is called once per frame
    void Update()
    {
        if (Application.isEditor)
        {
            deviceLAT = 00.000000;
            deviceLNG = 11.111111;
            deviceHEADING = 90;
        }
        else
        {
            if (Input.location.status != LocationServiceStatus.Failed)
            {
                deviceLAT = Input.location.lastData.latitude;
                deviceLNG = Input.location.lastData.longitude;
                deviceHEADING = Input.compass.trueHeading;

                // if compass isnt yet enabled, enable it
                if (!Input.compass.enabled)
                    Input.compass.enabled = true;

            }

        }

    }


    public static string DegreesToCardinalDetailed(double degrees)
    {
        string[] caridnals = { "N", "NNE", "NE", "ENE", "E", "ESE", "SE", "SSE", "S", "SSW", "SW", "WSW", "W", "WNW", "NW", "NNW", "N" };
        return caridnals[(int)System.Math.Round(((double)degrees * 10 % 3600) / 225)];
    }





}
