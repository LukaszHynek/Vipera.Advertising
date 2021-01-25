using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vipera
{
    // Ad banner control class
    public class AdBanner : ScriptableObject
    {
        readonly IronSourceBannerSize bannerSize = IronSourceBannerSize.BANNER; // Read IronSrc docs for sizes
        readonly IronSourceBannerPosition bannerPosition = IronSourceBannerPosition.BOTTOM; // or .TOP

        public void LoadBanner()
        {
            IronSource.Agent.loadBanner(bannerSize, bannerPosition);
        }

        public void ShowBanner()
        {
            IronSource.Agent.displayBanner();
        }

        public void HideBanner()
        {
            IronSource.Agent.hideBanner();
        }

        private void Awake()
        {
            BindEvents();
        }

        void BindEvents()
        {
            IronSourceEvents.onBannerAdLoadedEvent += OnBannerLoaded;
            IronSourceEvents.onBannerAdLoadFailedEvent += OnBannerLoadFailed;
            // Add remaining if needed
        }

        void OnBannerLoaded()
        {
            Debug.Log("Banner loaded");
        }

        void OnBannerLoadFailed(IronSourceError error)
        {
            Debug.Log("Banner failed to load\n Error code: " + error.getErrorCode() + "  Description: " + error.getDescription());
            Firebase.Analytics.FirebaseAnalytics.LogEvent("banner_load_failed", "error_code", error.getErrorCode());
        }
    }
}