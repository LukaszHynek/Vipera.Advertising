using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System;

public class iOSPostProcessBuild : MonoBehaviour
{
    private const string AppLovinSdkKey = "Jld9g_qC9k-1d8zM1QcZf4G15Tm1bQcfiqpqMDmiN2hHbwkWPVTbqbnysR_OhmKsNyzmob6NQ5C6dJEcz2ebVv";

    private static string[] SKAdNetworkIdentifiers = new string[]
    {
        "SU67R6K2V3.skadnetwork",//ironSource
        "cstr6suwn9.skadnetwork",//Admob
        "4DZT52R2T5.skadnetwork",//UnityAds
        "v9wttpbfk9.skadnetwork",//Facebook
        "n38lu8286q.skadnetwork",//Facebook
        "ludvb6z3bs.skadnetwork",//Applovin all to the bottom
        "2u9pt9hc89.skadnetwork",//Applovin
        "4468km3ulz.skadnetwork",//Applovin
        "4fzdc2evr5.skadnetwork",//Applovin
        "7ug5zh24hu.skadnetwork",//Applovin
        "8s468mfl3y.skadnetwork",//Applovin
        "9rd848q2bz.skadnetwork",//Applovin
        "9t245vhmpl.skadnetwork",//Applovin
        "av6w8kgt66.skadnetwork",//Applovin
        "f38h382jlk.skadnetwork",//Applovin
        "hs6bdukanm.skadnetwork",//Applovin
        "kbd757ywx3.skadnetwork",//Applovin
        "m8dbw4sv7c.skadnetwork",//Applovin
        "mlmmfzh3r3.skadnetwork",//Applovin
        "prcb7njmu6.skadnetwork",//Applovin
        "t38b2kh725.skadnetwork",//Applovin
        "tl55sbb4fm.skadnetwork",//Applovin
        "wzmmz9fp6w.skadnetwork",//Applovin
        "yclnxrl5pm.skadnetwork",//Applovin
        "ydx93a7ass.skadnetwork" //Applovin
    };

    [PostProcessBuild]
    public static void ChangeXCodeInfoPlist(BuildTarget target, string pathToBuildProject)
    {
        if (target == BuildTarget.iOS)
        {
            string GADApplicationIdentifier = Vipera.Advertising.admob_ios_appid;
            string plistPath = System.IO.Path.Combine(pathToBuildProject, "Info.plist");

            PlistDocument plistDocument = new PlistDocument();
            plistDocument.ReadFromFile(plistPath);

            PlistElementDict root = plistDocument.root;

            SetATSSettings(root);

            SetSDKNetworkID(root);

            root.SetString("NSUserTrackingUsageDescription", "This identifier will be used to deliver personalized ads to you.");
            root.SetString("GADApplicationIdentifier", GADApplicationIdentifier);
            root.SetString("AppLovinSdkKey", AppLovinSdkKey);
            root.SetString("LSApplicationCategoryType", "public.app-category.games");

            plistDocument.WriteToFile(plistPath);
        }
    }

    private static void SetATSSettings(PlistElementDict root)
    {
        if (root.values.ContainsKey("NSAppTransportSecurity"))
            root.values.Remove("NSAppTransportSecurity");

        PlistElementDict atsDict = null;
        if (!root.values.ContainsKey("NSAppTransportSecurity"))
            atsDict = root.CreateDict("NSAppTransportSecurity");

        if (atsDict == null)
            atsDict = root.values["NSAppTransportSecurity"].AsDict();

        atsDict.SetBoolean("NSAllowsArbitraryLoads", true);
        atsDict.SetBoolean("NSAllowsArbitraryLoadsInWebContent", true);

        root.values["NSAppTransportSecurity"] = atsDict;
    }

    private static void SetSDKNetworkID(PlistElementDict root)
    {
        //list of SKAdNetwork Ids
        //https://developers.ironsrc.com/ironsource-mobile/unity/ios-14-network-support/
        if (root.values.ContainsKey("SKAdNetworkItems"))
            root.values.Remove("SKAdNetworkItems");

        PlistElementArray array = root.CreateArray("SKAdNetworkItems");

        foreach (var identifier in SKAdNetworkIdentifiers)
        {
            PlistElementDict dict = array.AddDict();
            dict.SetString("SKAdNetworkIdentifier", identifier);
        }


        //PlistElementDict dictIron = array.AddDict();
        //dictIron.SetString("SKAdNetworkIdentifier", "SU67R6K2V3.skadnetwork");

        //PlistElementDict dictAdmob = array.AddDict();
        //dictAdmob.SetString("SKAdNetworkIdentifier", "cstr6suwn9.skadnetwork");

        //PlistElementDict dictApplovin = array.AddDict();
        //dictApplovin.SetString("SKAdNetworkIdentifier", "ludvb6z3bs.skadnetwork");

        //PlistElementDict dictUnityAds = array.AddDict();
        //dictUnityAds.SetString("SKAdNetworkIdentifier", "4DZT52R2T5.skadnetwork");

        //https://dash.applovin.com/o/account#skadnetwork_info

    }
}