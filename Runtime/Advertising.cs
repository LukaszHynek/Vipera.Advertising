using System;
using System.Collections;
using UnityEngine;

namespace Vipera
{
    public class Advertising : MonoBehaviour
    {
        static string settingsFileName = "AdvertisingSettings";

        public static Advertising instance;
        public static bool IsInitialized = false;

        static bool usingFirebase = false;
        static bool firebaseFetchComplete = false;

        public static bool ads_disabled = false;
        public static bool ads_removed = false;

        public static string ironsource_id_android = "";
        public static string ironsource_id_iOS = "";
        
        public static string admob_android_appid = "";
        public static string admob_ios_appid = "";

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

#if VIPERA_FIREBASE
            usingFirebase = true;
            RemoteConfig.OnFetchComplete += OnFechRemoteConfigComplete;
#endif

            instance = new GameObject("Vipera Advertising").AddComponent<Advertising>();
            DontDestroyOnLoad(instance.gameObject);
        }

#if VIPERA_FIREBASE
        static void OnFechRemoteConfigComplete()
        {
            RemoteConfig.OnFetchComplete -= OnFechRemoteConfigComplete;
            firebaseFetchComplete = true;
        }
#endif

        bool GetSettings()
        {
            try
            {
                string json = JsonUtility.ToJson(Resources.Load(settingsFileName));

                AdvertisingSettingsObject loadedSettings = JsonUtility.FromJson<AdvertisingSettingsObject>(json);

                ads_disabled = loadedSettings.ads_disabled;
                ads_removed = loadedSettings.ads_removed;
                ironsource_id_android = loadedSettings.ironsource_id_android;
                ironsource_id_iOS = loadedSettings.ironsource_id_iOS;
                admob_android_appid = loadedSettings.admob_android_appid;
                admob_ios_appid = loadedSettings.admob_ios_appid;

                Debug.Log(JsonUtility.ToJson(loadedSettings));

                return true;
            }
            catch
            {
                return false;
            }
        }

        private IEnumerator Start()
        {
            if (usingFirebase)
            {
                while (!firebaseFetchComplete)
                {
                    yield return new WaitForSeconds(0.5f);
                }
            }

            yield return new WaitForEndOfFrame();

            if (GetSettings() == false)
            {
                Debug.LogError("No AdvertisingSettings found or could not load.\n" +
                               "Make sure you created a file named \"AdvertisingSettings\" and put the variables you want to change there\n" +
                               "For the variables to work they need to have the same name and be public non-static\n" +
                               "Class name does not make a difference as it's converted to JSON and back into a settings class");
                yield break;
            }
            

            Debug.Log("Ads Initializing part 1");
            if (ads_removed || ads_disabled)
            {
                Debug.Log("Ads removed or disabled, initialization ended");
                yield break;
            }

#if UNITY_IOS
        IronSource.Agent.init(id_iOS, IronSourceAdUnits.INTERSTITIAL, IronSourceAdUnits.REWARDED_VIDEO, IronSourceAdUnits.BANNER);
#elif UNITY_ANDROID
        IronSource.Agent.init(id_android, IronSourceAdUnits.INTERSTITIAL, IronSourceAdUnits.REWARDED_VIDEO, IronSourceAdUnits.BANNER);
#endif

            banner = ScriptableObject.CreateInstance<AdBanner>();
            banner.LoadBanner();

            Debug.Log("Ads Initializing part 2");


            interstitial = ScriptableObject.CreateInstance<AdInterstitial>();
            interstitial.LoadInterstitial();

            Debug.Log("Ads Initializing part 3");


            rewardedVideo = ScriptableObject.CreateInstance<AdRewardedVideo>();

            IsInitialized = true;
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