using Vipera; // Use to not have to type "Vipera." every time 

public class ViperaAdExample
{
    void Start()
    {
        Advertising.ads_removed = true; // If you want to remove ads
        Advertising.interstitial_time_between = 30;   // If you want to adjust the minimum time between intersticials
        Advertising.rewarded_video_time_between = 30; // Same for video


        Advertising.ShowBanner();
        Advertising.HideBanner();

        // Check is interstitial is ready, has a bool parameter that can bypass time when = true and defaults to false
        if (Advertising.IsInterstitialReady(false))
        {
            // Always bind events before showing 
            Advertising.AddInterstitialClosedCallback(Callback); // Bind interstitial closing callback
            Advertising.AddInterstitialFailedCallback(Callback); // Also bind failed because some can fail

            Advertising.ShowInterstitial();
        }

        if (Advertising.IsRewardedVideoReady()) // Check if rewarded video is ready (includes time between)
        {
            Advertising.AddVideoRewardCallback(CallbackVideo); // Bind callback (this one requires a bool parameter)

            Advertising.ShowRewardedVideo();
        }
    }

    // Interstitial callback
    void Callback() // No parameters requierd for interstitial
    {
        Advertising.RemoveInterstitialClosedCallback(Callback);
        Advertising.RemoveInterstitialFailedCallback(Callback); // Unbinding callbacks - remove if you want to keep them

        // Do what must be done when interstitial is over
    }

    // Rewarded video Callback 
    void CallbackVideo(bool reward) // This one requires a bool to tell you if someone gets a reward or not 
    {
        Advertising.RemoveVideoRewardCallback(CallbackVideo); // Unbinding callbacks - remove if you want to keep them

        if (reward)
        {
            // Give reward
        }
        else
        {
            // Don give reward
        }
    }
}