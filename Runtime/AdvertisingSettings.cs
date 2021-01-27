using UnityEngine;

// Example of advertising settings
// You can copy this file and paste it into your Scripts folder.
// After that you will have a new option on right clicking: "Create->Vipera->Advertising->AdvertisingSettings"
// Once you create them you need to put that file into a "Resources" folder and it has to be called "AdvertisingSettings"
// it's what Advertising looks for when loading

// This class contains all the variables that Advertising loads, you can set them manually in your copy or enable Remote Config
// and set them up in firebase.

// ads_removed is meant to be the games way to say the user bought "Remove Ads" 
// ads_disabled is meant to be set through Firebase or manually when you want to disable ads regardless of what ads_removed says

/* Uncomment after copying

[CreateAssetMenu(fileName = "AdvertisingSettings", menuName = "Vipera/Advertising/AdvertisingSettings")]
public class AdvertisingSettings : ScriptableObject
{                              //: Vipera.RemoteSettingsBase
                               // Replace if you want to use Remote Config from Vipera.Firebase


    public bool ads_removed = false; // Advertising waits to the end of the frame before reading
                                     // so you can set this value from the game if ads should be removed
                                     // Alternatively you can use RemoteConfig, in that case Advertising automatically waits
                                     // untill the Config Fetch is complete

    public bool ads_disabled = false; // Gives us the ability to disable ads manually

    public string id_android = "TestIDAndroid";
    public string id_iOS = "TestIDiOS";
    public string admob_ios_appid = "TestIDiOSAdmob";
    public string admob_android_appid = "TestIDAndroidAdmob";

    public int interstitial_time_between = 30;
    public int rewarded_video_time_between = 30;

    // Uncomment if you want to use Remote Config from Vipera.Firebase
    //public override void Setup()
    //{
    //    base.Setup();
    //}
}
*/