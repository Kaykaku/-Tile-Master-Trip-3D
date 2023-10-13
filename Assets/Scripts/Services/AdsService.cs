using UnityEngine.Events;
using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

namespace Services
{
    public class AdsService
    {
        private readonly TimeSpan APPOPEN_TIMEOUT = TimeSpan.FromHours(4);
        private string bannerId = "unused";
        private string interstitialId = "unused";
        private string interstitialRewardId = "unused";
        private string rewardId = "unused";
        private string appOpenAdId = "unused";

        private DateTime appOpenExpireTime;
        private AppOpenAd appOpenAd;
        private BannerView bannerView;
        private InterstitialAd interstitialAd;
        private RewardedAd rewardedAd;
        private RewardedInterstitialAd rewardedInterstitialAd;
        private bool isShowingAppOpenAd;

        public Action<BannerState> OnBannerState;
        public Action<InterstitialState> OnInterState;
        public Action<RewardState> OnRewardState;
        public Action<RewardState> OnInterstitialRewardState;
        public Action<AppOpenState> OnAppOpenState;

        #region CONSTRUCTOR METHODS

        public AdsService(string bannerId = null, string interstitialId = null, string rewardId = null, string interstitialRewardId = null, string appOpenAdId = null)
        {
            this.bannerId = bannerId == null || bannerId.Length == 0 ? this.bannerId : bannerId;
            this.interstitialId = interstitialId == null || interstitialId.Length == 0 ? this.interstitialId : interstitialId;
            this.interstitialRewardId = interstitialRewardId == null || interstitialRewardId.Length == 0 ? this.interstitialRewardId : interstitialRewardId;
            this.rewardId = rewardId == null || rewardId.Length == 0 ? this.rewardId : rewardId;
            this.appOpenAdId = appOpenAdId == null || appOpenAdId.Length == 0 ? this.appOpenAdId : appOpenAdId;
            MobileAds.SetiOSAppPauseOnBackground(true);
            MobileAds.RaiseAdEventsOnUnityMainThread = true;

            List<String> deviceIds = new List<String>() { AdRequest.TestDeviceSimulator };

            // Add some test device IDs (replace with your own device IDs).
            deviceIds.Add(SystemInfo.deviceUniqueIdentifier);

            // Configure TagForChildDirectedTreatment and test device IDs.
            RequestConfiguration requestConfiguration =
                new RequestConfiguration.Builder()
                .SetTagForChildDirectedTreatment(TagForChildDirectedTreatment.Unspecified)
                .SetTestDeviceIds(deviceIds).build();
            MobileAds.SetRequestConfiguration(requestConfiguration);
            // Initialize the Google Mobile Ads SDK.
            MobileAds.Initialize(HandleInitCompleteAction);
        }
        public void SetAppStateChanged()
        {
            // Listen to application foreground / background events.
            AppStateEventNotifier.AppStateChanged += OnAppStateChanged;
        }
        private void HandleInitCompleteAction(InitializationStatus initstatus)
        {
            Logger.Debug("Initialization complete.");

            // Callbacks from GoogleMobileAds are not guaranteed to be called on
            // the main thread.
            // In this example we use MobileAdsEventExecutor to schedule these calls on
            // the next Update() loop.
            MobileAdsEventExecutor.ExecuteInUpdate(() =>
            {
                //statusText.text = "Initialization complete.";
            });
        }

        #endregion
        #region HELPER METHODS

        private AdRequest CreateAdRequest(string key)
        {
            return new AdRequest.Builder()
                .AddKeyword(key)
                .Build();
        }

        #endregion
        #region BANNER ADS

        public void RequestBannerAd(AdSize adSize, AdPosition adPosition, Action<BannerState> onState = null)
        {
            PrintStatus("Requesting Banner ad.");

            // Clean up banner before reusing
            if (bannerView != null)
            {
                bannerView.Destroy();
                bannerView = null;
            }
            if (onState != null) OnBannerState = onState;
            // Create a 320x50 banner at top of the screen
            bannerView = new BannerView(bannerId, adSize, adPosition);
            // Add Event Handlers
            bannerView.OnBannerAdLoaded += () =>
            {
                PrintStatus("Banner ad loaded.");
                OnBannerState?.Invoke(BannerState.OnLoaded);
            };
            bannerView.OnBannerAdLoadFailed += (args) =>
            {
                PrintStatus("Banner ad failed to load with error: " + args.GetMessage());
                OnBannerState?.Invoke(BannerState.OnLoadFailed);
            };
            bannerView.OnAdFullScreenContentOpened += () =>
            {
                PrintStatus("Banner ad opened.");
                OnBannerState?.Invoke(BannerState.OnOpened);
            };
            bannerView.OnAdFullScreenContentClosed += () =>
            {
                PrintStatus("Banner ad closed.");
                OnBannerState?.Invoke(BannerState.OnClosed);
            };
            // Load a banner ad
            bannerView.LoadAd(CreateAdRequest("banner"));
        }
        public void HandleBannerAdEvent(Action<BannerState> onState = null)
        {
            OnBannerState = onState;
        }
        public void DestroyBannerAd()
        {
            if (bannerView != null)
            {
                bannerView.Destroy();
            }
        }

        #endregion
        #region INTERSTITIAL ADS

        public void RequestAndLoadInterstitialAd(Action<InterstitialState> onState = null)
        {
            PrintStatus("Requesting Interstitial ad.");
            // Clean up interstitial before using it
            if (interstitialAd != null)
            {
                interstitialAd.Destroy();
                interstitialAd = null;
            }

            if (onState != null) OnInterState = onState;
            InterstitialAd.Load(interstitialId, CreateAdRequest("interstitial"), (InterstitialAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Logger.Debug("Interstitial ad failed to load" + error);
                    OnInterState?.Invoke(InterstitialState.OnLoadFailed);
                    return;
                }
                else
                {
                    Logger.Debug("Interstitial load success");
                    OnInterState?.Invoke(InterstitialState.OnLoadSuccess);
                }

                Logger.Debug("Interstitial ad loaded !!" + ad.GetResponseInfo());

                interstitialAd = ad;

                // Add Event Handlers
                interstitialAd.OnAdFullScreenContentFailed += (args) =>
                {
                    PrintStatus("Interstitial ad failed to load with error: " + args.GetMessage());
                    OnInterState?.Invoke(InterstitialState.OnLoadFailed);
                };
                interstitialAd.OnAdFullScreenContentOpened += () =>
                {
                    PrintStatus("Interstitial ad opened.");
                    OnInterState?.Invoke(InterstitialState.OnOpened);
                };
                interstitialAd.OnAdFullScreenContentClosed += () =>
                {
                    PrintStatus("Interstitial ad closed.");
                    RequestAndLoadInterstitialAd();
                    OnInterState?.Invoke(InterstitialState.OnClosed);
                };
                interstitialAd.OnAdClicked += () =>
                {
                    PrintStatus("Interstitial ad clicked.");
                    OnInterState?.Invoke(InterstitialState.OnClicked);
                };
                interstitialAd.OnAdImpressionRecorded += () =>
                {
                    PrintStatus("Interstitial ad impression recorded.");
                    OnInterState?.Invoke(InterstitialState.OnImpressionRecorded);
                };
            });
        }
        public void HandleInterstitialAdEvent(Action<InterstitialState> onState = null)
        {
            OnInterState = onState;
            if (!IsInterstitialReady())
            {
                RequestAndLoadInterstitialAd();
            }
        }
        public bool IsInterstitialReady() => interstitialAd != null && interstitialAd.CanShowAd();
        public void ShowInterstitialAd(Action<bool> OnShow = null)
        {
            if (IsInterstitialReady())
            {
                OnShow?.Invoke(true);
                interstitialAd.Show();
            }
            else
            {
                PrintStatus("Interstitial ad is not ready yet.");
                OnShow?.Invoke(false);
            }
        }
        public void DestroyInterstitialAd()
        {
            if (interstitialAd != null)
            {
                interstitialAd.Destroy();
            }
        }

        #endregion
        #region REWARDED ADS
        
        public void RequestAndLoadRewardedAd(Action<RewardState> onState = null)
        {
            PrintStatus("Requesting Rewarded ad.");
            if (rewardedAd != null)
            {
                rewardedAd.Destroy();
                rewardedAd = null;
            }
            if (onState != null) OnRewardState = onState;

            RewardedAd.Load(rewardId, CreateAdRequest("Rewarded"), (RewardedAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Logger.Debug("Rewarded failed to load" + error);
                    OnRewardState?.Invoke(RewardState.OnLoadFailed);
                    return;
                }
                else
                {
                    Logger.Debug("Rewarded load success");
                    OnRewardState?.Invoke(RewardState.OnLoadSuccess);
                }

                Logger.Debug("Rewarded ad loaded !!");
                rewardedAd = ad;
                // Add Event Handlers
                rewardedAd.OnAdFullScreenContentFailed += (args) =>
                {
                    PrintStatus("Rewarded ad failed to load with error: " + args.GetMessage());
                    OnRewardState?.Invoke(RewardState.OnLoadFailed);
                };
                rewardedAd.OnAdFullScreenContentOpened += () =>
                {
                    PrintStatus("Rewarded ad opened.");
                    OnRewardState?.Invoke(RewardState.OnOpened);
                };
                rewardedAd.OnAdFullScreenContentClosed += () =>
                {
                    PrintStatus("Rewarded ad closed.");
                    RequestAndLoadRewardedAd();
                    OnRewardState?.Invoke(RewardState.OnClosed);
                };
                rewardedAd.OnAdClicked += () =>
                {
                    PrintStatus("Rewarded ad clicked.");
                    OnRewardState?.Invoke(RewardState.OnClicked);
                };
                rewardedAd.OnAdImpressionRecorded += () =>
                {
                    PrintStatus("Rewarded ad impression recorded.");
                    OnRewardState?.Invoke(RewardState.OnImpressionRecorded);
                };
            });
        }
        public bool IsRewardReady() => rewardedAd != null && rewardedAd.CanShowAd();
        public void HandleRewardAdEvent(Action<RewardState> onState = null)
        {
            OnRewardState = onState;
            if (!IsRewardReady())
            {
                RequestAndLoadRewardedAd();
            }
        }
        public void ShowRewardedAd(Action<bool> OnReward = null)
        {
            if (IsRewardReady())
            {
                rewardedAd.Show((reward) => {
                    OnReward?.Invoke(true);
                });
            }
            else
            {
                OnReward?.Invoke(false);
                PrintStatus("Rewarded ad is not ready yet.");
            }

        }

        public void RequestAndLoadRewardedInterstitialAd(Action<RewardState> onState = null)
        {
            PrintStatus("Requesting Rewarded Interstitial ad.");

            // Create an interstitial.
            if (onState != null) OnInterstitialRewardState = onState;
            RewardedInterstitialAd.Load(interstitialRewardId, CreateAdRequest("RewardedInterstitial"), (rewardedInterstitialAd, error) =>
            {
                if (rewardedInterstitialAd==null || error != null)
                {
                    PrintStatus("Rewarded Interstitial ad load failed with error: " + error);
                    OnInterstitialRewardState?.Invoke(RewardState.OnLoadFailed);
                    return;
                }
                else
                {
                    Logger.Debug("Rewarded Interstitial load success");
                    OnInterstitialRewardState?.Invoke(RewardState.OnLoadSuccess);
                }

                this.rewardedInterstitialAd = rewardedInterstitialAd;
                PrintStatus("Rewarded Interstitial ad loaded.");

                // Add Event Handlers
                rewardedInterstitialAd.OnAdFullScreenContentFailed += (args) =>
                {
                    PrintStatus("Rewarded Interstitial ad failed to load with error: " + args.GetMessage());
                    OnInterstitialRewardState?.Invoke(RewardState.OnLoadFailed);
                };
                rewardedInterstitialAd.OnAdFullScreenContentOpened += () =>
                {
                    PrintStatus("Rewarded Interstitial ad opened.");
                    OnInterstitialRewardState?.Invoke(RewardState.OnOpened);
                };
                rewardedInterstitialAd.OnAdFullScreenContentClosed += () =>
                {
                    PrintStatus("Rewarded Interstitial ad closed.");
                    RequestAndLoadRewardedInterstitialAd();
                    OnInterstitialRewardState?.Invoke(RewardState.OnClosed);
                };
                rewardedInterstitialAd.OnAdClicked += () =>
                {
                    PrintStatus("Rewarded Interstitial ad clicked.");
                    OnInterstitialRewardState?.Invoke(RewardState.OnClicked);
                };
                rewardedInterstitialAd.OnAdImpressionRecorded += () =>
                {
                    PrintStatus("Rewarded Interstitial ad impression recorded.");
                    OnInterstitialRewardState?.Invoke(RewardState.OnImpressionRecorded);
                };
            });
        }
        public bool IsRewardInterstitialReady() => rewardedInterstitialAd != null && rewardedInterstitialAd.CanShowAd();
        public void HandleRewardInterstitialAdEvent(Action<RewardState> onState = null)
        {
            OnInterstitialRewardState = onState;
            if (!IsRewardInterstitialReady())
            {
                RequestAndLoadRewardedInterstitialAd();
            }
        }
        public void ShowRewardedInterstitialAd(Action<bool> OnReward = null)
        {
            if (IsRewardInterstitialReady())
            {
                rewardedInterstitialAd.Show((reward) =>
                {
                    OnReward?.Invoke(true);
                });
            }
            else
            {
                OnReward?.Invoke(false);
            }
        }

        #endregion
        #region APPOPEN ADS

        public bool IsAppOpenAdAvailable() => (!isShowingAppOpenAd && appOpenAd != null && DateTime.Now < appOpenExpireTime);
        public void OnAppStateChanged(AppState state)
        {
            // Display the app open ad when the app is foregrounded.
            Logger.Debug("App State is " + state);

            // OnAppStateChanged is not guaranteed to execute on the Unity UI thread.

            MobileAdsEventExecutor.ExecuteInUpdate(() =>
            {
                if (state == AppState.Foreground)
                {
                    if (appOpenAd == null) RequestAndLoadAppOpenAd();
                    ShowAppOpenAd();
                }
            });
        }

        public void RequestAndLoadAppOpenAd(Action<AppOpenState> onState = null)
        {
            PrintStatus("Requesting App Open ad.");

            // create new app open ad instance
            AppOpenAd.Load(appOpenAdId, ScreenOrientation.Portrait, CreateAdRequest("App open ad"), (AppOpenAd ad, LoadAdError error) => {
                if (error != null || ad == null)
                {
                    PrintStatus("App Open ad failed to load with error: " + error);
                    onState?.Invoke(AppOpenState.OnLoadFailed);
                    return;
                }else
                {
                    Logger.Debug("App Open ad load success");
                    onState?.Invoke(AppOpenState.OnLoadSuccess);
                }

                PrintStatus("App Open ad loaded. Please background the app and return.");
                this.appOpenAd = ad;
                this.appOpenExpireTime = DateTime.Now + APPOPEN_TIMEOUT;
                if (onState == null) OnAppOpenState = onState;
                // Register for ad events.
                this.appOpenAd.OnAdFullScreenContentFailed += (args) =>
                {
                    PrintStatus("App Open ad failed to present with error: " + args.GetMessage());

                    isShowingAppOpenAd = false;
                    if (this.appOpenAd != null)
                    {
                        this.appOpenAd.Destroy();
                        this.appOpenAd = null;
                    }
                    onState?.Invoke(AppOpenState.OnLoadFailed);
                };
                this.appOpenAd.OnAdFullScreenContentOpened += () =>
                {
                    PrintStatus("App Open ad opened.");
                    OnAppOpenState?.Invoke(AppOpenState.OnOpened);
                };
                this.appOpenAd.OnAdFullScreenContentClosed += () =>
                {
                    PrintStatus("App Open ad close.");
                    isShowingAppOpenAd = false;
                    this.appOpenAd.Destroy();
                    this.appOpenAd = null;
                    RequestAndLoadAppOpenAd();
                    OnAppOpenState?.Invoke(AppOpenState.OnClosed);
                };
            });
        }
        public void HandleAppOpenAdEvent(Action<AppOpenState> onState = null)
        {
            OnAppOpenState = onState;
            if (!IsAppOpenAdAvailable())
            {
                RequestAndLoadAppOpenAd();
            }
        }
        public void ShowAppOpenAd()
        {
            if (!IsAppOpenAdAvailable())
            {
                return;
            }
            isShowingAppOpenAd = true;
            appOpenAd.Show();
        }

        #endregion
        #region Utility

        ///<summary>
        /// Log the message and update the status text on the main thread.
        ///<summary>
        private void PrintStatus(string message)
        {
            Logger.Debug(message);
        }

        #endregion
    }
}
