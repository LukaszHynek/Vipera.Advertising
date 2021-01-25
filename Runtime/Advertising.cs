using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vipera
{
    public class Advertising : MonoBehaviour
    {
        // Remove IDs/ find a way to load from remote config
        public const string admob_ios_appid = "ca-app-pub-6126530391147430~3688734903";
        public const string admob_android_appid = "ca-app-pub-6126530391147430~4511452628";

        public static Advertising instance;
        public static bool IsInitialized = false;

        public static bool adsRemoved = false;
        public static int interstitial_time_between = 30;
        public static int rewarded_video_time_between = 30;

        AdBanner banner;
        AdInterstitial interstitial;
        AdRewardedVideo rewardedVideo;

        // Automatically run function every time, makes sure we have an instance
        [RuntimeInitializeOnLoadMethod]
        public static void Init()
        {
            if (instance != null)
                return;

            instance = new GameObject("Vipera Advertising").AddComponent<Advertising>();
            DontDestroyOnLoad(instance.gameObject);

        }
        private IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();
            Debug.Log("Ads Initializing part 1");
            if (adsRemoved)
            {
                Debug.Log("Ads disabled, initialization ended");
                yield break;
            }

#if UNITY_IOS
        IronSource.Agent.init("8be367c5", IronSourceAdUnits.INTERSTITIAL, IronSourceAdUnits.REWARDED_VIDEO, IronSourceAdUnits.BANNER);
#elif UNITY_ANDROID
        IronSource.Agent.init("8be2f4b5", IronSourceAdUnits.INTERSTITIAL, IronSourceAdUnits.REWARDED_VIDEO, IronSourceAdUnits.BANNER);
#endif
            IsInitialized = true;

            banner = ScriptableObject.CreateInstance<AdBanner>();
            banner.LoadBanner();

            Debug.Log("Ads Initializing part 2");


            interstitial = ScriptableObject.CreateInstance<AdInterstitial>();
            interstitial.LoadInterstitial();

            Debug.Log("Ads Initializing part 3");


            rewardedVideo = ScriptableObject.CreateInstance<AdRewardedVideo>();

            Debug.Log("Ads Initializing DONE");

        }


        // ---- Ad Banner logic ----

        public static void ShowBanner()
        {
            if (!IsInitialized) return;

            instance.banner.ShowBanner();
        }

        public static void HideBanner()
        {
            if (!IsInitialized) return;

            instance.banner.HideBanner();
        }

        // ---- Ad Banner logic ----



        // ---- Ad Interstitial logic ----

        public static bool IsInterstitialReady(bool ignoreTime = false)
        {
            return instance.interstitial.isReady(ignoreTime) && IsInitialized;   
        }

        public static void ShowInterstitial()
        {
            if (!IsInitialized) return;

            instance.interstitial.ShowInterstitial();
        }

        public static void AddInterstitialClosedCallback(Action callback)
        {
            if (!IsInitialized) return;

            instance.interstitial.AddInterstitialClosedCallback(callback);
        }

        public static void RemoveInterstitialClosedCallback(Action callback)
        {
            if (!IsInitialized) return;

            instance.interstitial.RemoveInterstitialClosedCallback(callback);
        }

        public static void AddInterstitialFailedCallback(Action callback)
        {
            if (!IsInitialized) return;

            instance.interstitial.AddInterstitialShowFailedCallback(callback);
        }

        public static void RemoveInterstitialFailedCallback(Action callback)
        {
            if (!IsInitialized) return;

            instance.interstitial.RemoveInterstitialShowFailedCallback(callback);
        }

        // ---- Ad Interstitial logic ----


        // ---- Rewarded Video logic ----

        public static bool IsRewardedVideoReady(bool ignoreTime = false)
        {
            return instance.rewardedVideo.isVideoReady(ignoreTime) && IsInitialized;
        }

        public static void ShowRewardedVideo()
        {
            if (!IsInitialized) return;

            instance.rewardedVideo.ShowVideo();
        }

        public static void AddVideoRewardCallback(Action<bool> callback)
        {
            if (!IsInitialized) return;

            instance.rewardedVideo.AddVideoRewardStatusCallback(callback);
        }

        public static void RemoveVideoRewardCallback(Action<bool> callback)
        {
            if (!IsInitialized) return;

            instance.rewardedVideo.RemoveVideoRewardStatusCallback(callback);
        }

        public static void AddVideoAvailableCallback(Action<bool> callback)
        {
            if (!IsInitialized) return;

            instance.rewardedVideo.AddVideoAvailableStatusCallback(callback);
        }

        public static void RemoveVideoAvailableCallback(Action<bool> callback)
        {
            if (!IsInitialized) return;

            instance.rewardedVideo.RemoveVideoAvailableStatusCallback(callback);
        }


        // ---- Rewarded Video logic ----



        private void OnApplicationPause(bool pause)
        {
            if (!IsInitialized) return;
            IronSource.Agent.onApplicationPause(pause);
        }
    }
}