using UnityEngine;
using Adverty;

public class InitializeAdvertySDK : MonoBehaviour
{
    public Camera GameCamera;

    private void Start()
    {
        //Set up correct GameCamera for Adverty SDK
        if(GameCamera == null)
        {
            GameCamera = Camera.main;
        }
        Adverty.AdvertySettings.SetMainCamera(GameCamera);

        //Define data and initialize Adverty SDK

        /*
        string apiKey = "YOUR_KEY"; //put key there
        AdvertySettings.Mode platform = AdvertySettings.Mode.Mobile; //define target platform (Mobile, VR, AR)
        bool restrictUserDataCollection = false; //do you disallow collect extra user data?
        UserData userData = new UserData(AgeSegment.Adult, Gender.Male); // define user as adult male
        AdvertySDK.Init(apiKey, platform, restrictUserDataCollection, userData);
        */

        //or use predefined data from Adverty settings window and define user data (here we define user as adult male)
        UserData userData = new UserData(AgeSegment.Unknown, Adverty.Gender.Unknown);
        AdvertySDK.Init(userData);
    }
}
