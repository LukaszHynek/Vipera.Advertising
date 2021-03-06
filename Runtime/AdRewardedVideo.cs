﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vipera
{
    // Rewarded video control class
    // TODO: Add cooldown like interstitials to stop infinite videos 
    public class AdRewardedVideo : ScriptableObject
    {
        Action<bool> videoRewardStatusCallback;
        Action<bool> videoAvailableStatusCallback;

        float lastShowTime = 0;

        // Checks if rewarded video is ready with the possibility to ignore time
        public bool isVideoReady(bool ignoreTime)
        {
            if (ignoreTime)
            {
                return IronSource.Agent.isRewardedVideoAvailable();
            }
            else
            {
                return IronSource.Agent.isRewardedVideoAvailable() &&
                       (Time.time > lastShowTime + Advertising.rewarded_video_time_between || lastShowTime == 0);
            }
        }

        public void ShowVideo()
        {
            if (isVideoReady(true))
            {
                lastShowTime = Time.time;
                IronSource.Agent.showRewardedVideo();
            }
        }

        public void AddVideoRewardStatusCallback(Action<bool> callback)
        {
            videoRewardStatusCallback += callback;
        }

        public void RemoveVideoRewardStatusCallback(Action<bool> callback)
        {
            videoRewardStatusCallback -= callback;
        }

        public void AddVideoAvailableStatusCallback(Action<bool> callback)
        {
            videoAvailableStatusCallback += callback;
        }

        public void RemoveVideoAvailableStatusCallback(Action<bool> callback)
        {
            videoAvailableStatusCallback -= callback;
        }


        private void Awake()
        {
            BindEvents();
        }

        // Binding all used events
        void BindEvents()
        {
            IronSourceEvents.onRewardedVideoAdOpenedEvent += RewardedVideoOpened;
            IronSourceEvents.onRewardedVideoAdClickedEvent += RewardedVideoClicked;
            IronSourceEvents.onRewardedVideoAdClosedEvent += RewardedVideoClosed;
            IronSourceEvents.onRewardedVideoAvailabilityChangedEvent += RewardedVideoAvailabilityChanged;
            IronSourceEvents.onRewardedVideoAdStartedEvent += RewardedVideoStarted;
            IronSourceEvents.onRewardedVideoAdEndedEvent += RewardedVideoEnded;
            IronSourceEvents.onRewardedVideoAdRewardedEvent += RewardedVideoRewarded;
            IronSourceEvents.onRewardedVideoAdShowFailedEvent += RewardedVideoShowFailed;
        }

        private void RewardedVideoShowFailed(IronSourceError error)
        {
            Debug.Log("Rewarded Video load failed\n Error code: " + error.getErrorCode() + "  Description: " + error.getDescription());

#if VIPERA_CORE
            MainThreadQueue.Enqueue(() =>
            {
                try
                {
                    videoRewardStatusCallback.Invoke(false);
                }
                catch
                {
                    Debug.Log("Nothing listening for \"videoRewardStatusCallback\"");
                }
            });
#else
            
            try
            {
                videoRewardStatusCallback.Invoke(false);
            }
            catch
            {
                Debug.Log("Nothing listening for \"videoRewardStatusCallback\"");
            }
#endif

#if VIPERA_FIREBASE
            Analytics.LogEvent("rewarded_video_show_failed", "error_code", error.getErrorCode());
#else
            Firebase.Analytics.FirebaseAnalytics.LogEvent("rewarded_video_show_failed", "error_code", error.getErrorCode());
#endif
        }

        private void RewardedVideoRewarded(IronSourcePlacement reward)
        {
#if VIPERA_CORE
            MainThreadQueue.Enqueue(() =>
            {
                try
                {
                    videoRewardStatusCallback.Invoke(true);
                }
                catch
                {
                    Debug.Log("Nothing listening for \"videoRewardStatusCallback\"");
                }
            });
#else
            Debug.Log("Rewarded video rewarded");
            try
            {
                videoRewardStatusCallback.Invoke(true);
            }
            catch
            {
                Debug.Log("Nothing listening for \"videoRewardStatusCallback\"");
            }
#endif
        }

        private void RewardedVideoEnded()
        {
            Debug.Log("Rewarded video ended");
        }

        private void RewardedVideoStarted()
        {
            Debug.Log("Rewarded video started");
        }

        private void RewardedVideoAvailabilityChanged(bool available)
        {
            Debug.Log("Rewarded video availability changed, now available: " + available);
#if VIPERA_CORE
            MainThreadQueue.Enqueue(() =>
            {
                try
                {
                    videoAvailableStatusCallback.Invoke(available);
                }
                catch
                {
                    Debug.Log("Nothing listening for RewardedVideoAvailabilityChaged");
                }
            });
#else
            try
            {
                videoAvailableStatusCallback.Invoke(available);
            }
            catch
            {
                Debug.Log("Nothing listening for RewardedVideoAvailabilityChaged");
            }
#endif
        }

        private void RewardedVideoClosed()
        {
            Debug.Log("Rewarded video closed");

            // If another video is available, manually call the event with true as it's not called by IronSource
            if (IronSource.Agent.isRewardedVideoAvailable())
                RewardedVideoAvailabilityChanged(true);
        }

        private void RewardedVideoClicked(IronSourcePlacement obj)
        {
            Debug.Log("Rewarded video clicked");
        }

        private void RewardedVideoOpened()
        {
            Debug.Log("Rewarded video opened");
        }
    }
}