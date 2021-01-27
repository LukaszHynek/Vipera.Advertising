using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vipera
{
    // Interstitial ad control class
    public class AdInterstitial : ScriptableObject
    {
        private static float lastShowTime;

        Action interstitialClosed;
        Action interstitialFailed;

        public void LoadInterstitial()
        {
            if (!IronSource.Agent.isInterstitialReady()) IronSource.Agent.loadInterstitial();
        }

        // Check if interstitial is ready with the possiblity to ignore time limits
        public bool isReady(bool ignoreTime)
        {
            if (ignoreTime)
            {
                return (IronSource.Agent.isInterstitialReady());
            }
            else
            {
                return (IronSource.Agent.isInterstitialReady() && (Time.time > lastShowTime + Advertising.interstitial_time_between) || lastShowTime == 0);
            }
        }

        public void ShowInterstitial()
        {
            if (isReady(true))
            {
                IronSource.Agent.showInterstitial();
                lastShowTime = Time.time;
            }
            else
            {
                Debug.Log("Interstitial not ready");
                try
                {
                    interstitialFailed.Invoke();
                }
                catch
                {
                    Debug.Log("Nothing listening for \"interstitialFailed\"");
                }
            }
        }

        public void AddInterstitialClosedCallback(Action callback)
        {
            interstitialClosed += callback;
        }

        public void RemoveInterstitialClosedCallback(Action callback)
        {
            interstitialClosed -= callback;
        }

        public void AddInterstitialShowFailedCallback(Action callback)
        {
            interstitialFailed += callback;
        }

        public void RemoveInterstitialShowFailedCallback(Action callback)
        {
            interstitialFailed -= callback;
        }

        private void Awake()
        {
            BindEvents();
        }

        // Binding all used events
        void BindEvents()
        {
            IronSourceEvents.onInterstitialAdReadyEvent += OnInterstitialReady;
            IronSourceEvents.onInterstitialAdLoadFailedEvent += OnInterstitialLoadFailed;
            IronSourceEvents.onInterstitialAdShowSucceededEvent += OnInterstitialShowSucceeded;
            IronSourceEvents.onInterstitialAdShowFailedEvent += OnInterstitialShowFailed;
            IronSourceEvents.onInterstitialAdClickedEvent += OnInterstitialClicked;
            IronSourceEvents.onInterstitialAdOpenedEvent += OnInterstitialOpened;
            IronSourceEvents.onInterstitialAdClosedEvent += OnInterstitialClosed;
        }

        private void OnInterstitialLoadFailed(IronSourceError error)
        {
            Debug.Log("Interstitial load failed\n Error code: " + error.getErrorCode() + "  Description: " + error.getDescription());
#if VIPERA_FIREBASE
            Analytics.LogEvent("interstitial_load_failed", "error_code", error.getErrorCode());
#else
            Firebase.Analytics.FirebaseAnalytics.LogEvent("interstitial_load_failed", "error_code", error.getErrorCode());
#endif
        }

        private void OnInterstitialReady()
        {
            Debug.Log("Interstitial ready");
        }


        private void OnInterstitialShowFailed(IronSourceError error)
        {
            Debug.Log("Interstitial show failed\n Error code: " + error.getErrorCode() + "  Description: " + error.getDescription());
            try
            {
                interstitialFailed.Invoke();
            }
            catch
            {
                Debug.Log("Nothing listening for \"interstitialFailed\"");
            }
#if VIPERA_FIREBASE
            Analytics.LogEvent("interstitial_show_failed", "error_code", error.getErrorCode());
#else
            Firebase.Analytics.FirebaseAnalytics.LogEvent("interstitial_show_failed", "error_code", error.getErrorCode());
#endif
        }

        private void OnInterstitialShowSucceeded()
        {
            Debug.Log("Interstitial show succeeded");
#if VIPERA_FIREBASE
            Analytics.LogEvent("interstitial_show");
#else
            Firebase.Analytics.FirebaseAnalytics.LogEvent("interstitial_show");
#endif
        }


        private void OnInterstitialClosed()
        {
            Debug.Log("Interstitial closed");
#if VIPERA_CORE
            MainThreadQueue.Enqueue(() => 
            {
                try
                {
                    interstitialClosed.Invoke();
                }
                catch
                {
                    Debug.Log("Nothing listening for \"interstitialClosed\"");
                }
            });
#else
            try
            {
                interstitialClosed.Invoke();
            }
            catch
            {
                Debug.Log("Nothing listening for \"interstitialClosed\"");
            }
#endif
            LoadInterstitial();
        }

        private void OnInterstitialOpened()
        {
            Debug.Log("Interstitial opened");
        }

        private void OnInterstitialClicked()
        {
            Debug.Log("Interstitial clicked");
#if VIPERA_FIREBASE
            Analytics.LogEvent("interstitial_clicked");
#else
            Firebase.Analytics.FirebaseAnalytics.LogEvent("interstitial_clicked");
#endif
        }
    }
}