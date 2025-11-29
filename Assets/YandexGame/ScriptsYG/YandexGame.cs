using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections.Generic;
#if PLUGIN_YG_2
using YG.Insides;
#endif

namespace YG
{
    [HelpURL("https://ash-message-bf4.notion.site/PluginYG-d457b23eee604b7aa6076116aab647ed")]
    [DefaultExecutionOrder(-100)]
    public partial class YandexGame : MonoBehaviour
    {
        [Tooltip("Объект YandexGame не будет удаляться при смене сцены. При выборе опции singleton, объект YandexGame необходимо поместить только на одну сцену, которая первая загружается при запуске игры.")]
        public bool singleton;

        [Space(10)]
        public UnityEvent ResolvedAuthorization;
        public UnityEvent RejectedAuthorization;
        [Space(30)]
        public UnityEvent OpenFullscreenAd;
        public UnityEvent CloseFullscreenAd;
        public UnityEvent ErrorFullscreenAd;
        [Space(30)]
        public UnityEvent OpenVideoAd;
        public UnityEvent CloseVideoAd;
        public UnityEvent RewardVideoAd;
        public UnityEvent ErrorVideoAd;
        [Space(30)]
        public UnityEvent PurchaseSuccess;
        public UnityEvent PurchaseFailed;
        [Space(30)]
        public UnityEvent PromptDo;
        public UnityEvent PromptFail;
        public UnityEvent ReviewDo;

        public static YandexGame Instance;
        public static Action GetDataEvent;
        public static bool SDKEnabled
        {
#if PLUGIN_YG_2
            get => YG2.isSDKEnabled;
#else
            get => false;
#endif
        }
        public static bool nowAdsShow
        {
#if PLUGIN_YG_2
            get => YG2.nowAdsShow;
#else
            get => false;
#endif
        }
        public static bool nowFullAd
        {
#if PLUGIN_YG_2
            get => YG2.nowInterAdv;
#else
            get => false;
#endif
        }
        public static bool nowVideoAd
        {
#if PLUGIN_YG_2
            get => YG2.nowRewardAdv;
#else
            get => false;
#endif
        }
        public static bool auth
        {
            get
            {
#if Authorization_yg
                return YG2.player.auth;
#else
                return false;
#endif
            }
        }
        public static string playerName
        {
            get
            {
#if Authorization_yg
                return YG2.player.name;
#else
                return "unauthorized";
#endif
            }
        }
        public static string playerId
        {
            get
            {
#if Authorization_yg
                return YG2.player.id;
#else
                return string.Empty;
#endif
            }
        }
        public static string playerPhoto
        {
            get
            {
#if Authorization_yg
                return YG2.player.photo;
#else
                return string.Empty;
#endif
            }
        }

        public class JsonEnvironmentData
        {
            public string language = "ru";
            public string domain = "ru";
            public string deviceType = "desktop";
            public bool isMobile;
            public bool isDesktop = true;
            public bool isTablet;
            public bool isTV;
            public string appID;
            public string browserLang;
            public string payload;
            public string platform = "Win32";
            public string browser = "Other";
        }
        public static JsonEnvironmentData EnvironmentData
        {
            get
            {
#if EnvirData_yg
                return new JsonEnvironmentData
                {
                    language = YG2.envir.language,
                    domain = YG2.envir.domain,
                    deviceType = YG2.envir.deviceType,
                    isMobile = YG2.envir.isMobile,
                    isDesktop = YG2.envir.isDesktop,
                    isTablet = YG2.envir.isTablet,
                    isTV = YG2.envir.isTV,
                    appID = YG2.envir.appID,
                    browserLang = YG2.envir.browserLang,
                    payload = YG2.envir.payload,
                    platform = YG2.envir.platform,
                    browser = YG2.envir.browser
                };
#else
                return new JsonEnvironmentData();
#endif
            }
        }

        public static string lang
        {
            get
            {
#if Localization_yg
                return YG2.lang;
#else
                return "ru";
#endif
            }
        }
        public static Action<string> SwitchLangEvent;

        public static Action OpenFullAdEvent;
        public static Action CloseFullAdEvent;
        public static Action OpenVideoEvent;
        public static Action CloseVideoEvent;
        public static Action<int> RewardVideoEvent;

#if Storage_yg
        public static SavesYG savesData
        {
            get => YG2.saves;
            set => YG2.saves = value;
        }
#else
        public static SavesYG savesData = new SavesYG();
#endif

        private void Awake()
        {
            transform.SetParent(null);
            gameObject.name = "YandexGame";

            if (singleton)
            {
                if (Instance != null)
                {
                    Destroy(gameObject);
                }
                else
                {
                    Instance = this;
                    DontDestroyOnLoad(gameObject);
                }
            }
            else
            {
                Instance = this;
            }
        }

        private void OnEnable()
        {
#if PLUGIN_YG_2
            YG2.onGetSDKData += OnGetSDKData;
#endif
#if InterstitialAdv_yg
            YG2.onOpenInterAdv += OnOpenInterAdv;
            YG2.onCloseInterAdv += OnCloseInterAdv;
            YG2.onErrorInterAdv += OnErrorInterAdv;
#endif
#if RewardedAdv_yg
            YG2.onOpenRewardedAdv += OnOpenRewardedAdv;
            YG2.onCloseRewardedAdv += OnCloseRewardedAdv;
            YG2.onRewardAdv += OnRewardAdv;
            YG2.onErrorRewardedAdv += OnErrorRewardedAdv;
#endif
#if Payments_yg
            YG2.onPurchaseSuccess += OnPurchaseSuccess;
            YG2.onPurchaseFailed += OnPurchaseFailed;
#endif
#if Review_yg
            YG2.onReviewSent += OnReviewSent;
#endif
#if GameLabel_yg
            YG2.onGameLabelSuccess += OnGameLabelSuccess;
            YG2.onGameLabelFail += OnGameLabelFail;
#endif
#if Localization_yg
            YG2.onSwitchLang += OnSwitchLang;
#endif
        }

        private void OnDisable()
        {
#if PLUGIN_YG_2
            YG2.onGetSDKData -= OnGetSDKData;
#endif
#if InterstitialAdv_yg
            YG2.onOpenInterAdv -= OnOpenInterAdv;
            YG2.onCloseInterAdv -= OnCloseInterAdv;
            YG2.onErrorInterAdv -= OnErrorInterAdv;
#endif
#if RewardedAdv_yg
            YG2.onOpenRewardedAdv -= OnOpenRewardedAdv;
            YG2.onCloseRewardedAdv -= OnCloseRewardedAdv;
            YG2.onRewardAdv -= OnRewardAdv;
            YG2.onErrorRewardedAdv -= OnErrorRewardedAdv;
#endif
#if Payments_yg
            YG2.onPurchaseSuccess -= OnPurchaseSuccess;
            YG2.onPurchaseFailed -= OnPurchaseFailed;
#endif
#if Review_yg
            YG2.onReviewSent -= OnReviewSent;
#endif
#if GameLabel_yg
            YG2.onGameLabelSuccess -= OnGameLabelSuccess;
            YG2.onGameLabelFail -= OnGameLabelFail;
#endif
#if Localization_yg
            YG2.onSwitchLang -= OnSwitchLang;
#endif
        }

        private void OnGetSDKData()
        {
            GetDataEvent?.Invoke();
#if Authorization_yg
            if (YG2.player.auth)
                ResolvedAuthorization?.Invoke();
            else
                RejectedAuthorization?.Invoke();
#endif
        }
        public void _GameReadyAPI()
        {
#if PLUGIN_YG_2
            YG2.GameReadyAPI();
#endif
        }
        public void _GameplayStart()
        {
#if PLUGIN_YG_2
            YG2.GameplayStart();
#endif
        }
        public void _GameplayStop()
        {
#if PLUGIN_YG_2
            YG2.GameplayStop();
#endif
        }
        public static void GameReadyAPI()
        {
#if PLUGIN_YG_2
            YG2.GameReadyAPI();
#endif
        }
        public static void GameplayStart()
        {
#if PLUGIN_YG_2
            YG2.GameplayStart();
#endif
        }
        public static void GameplayStop()
        {
#if PLUGIN_YG_2
            YG2.GameplayStop();
#endif
        }

        public static void FullscreenShow()
        {
#if InterstitialAdv_yg
            YG2.InterstitialAdvShow();
#endif
        }
        public void _FullscreenShow()
        {
#if InterstitialAdv_yg
            YG2.InterstitialAdvShow();
#endif
        }

#if InterstitialAdv_yg
        private void OnOpenInterAdv()
        {
            OpenFullscreenAd?.Invoke();
            OpenFullAdEvent?.Invoke();
        }
        private void OnCloseInterAdv()
        {
            CloseFullscreenAd?.Invoke();
            CloseFullAdEvent?.Invoke();
        }
        private void OnErrorInterAdv() => ErrorFullscreenAd?.Invoke();
#endif
        public static void RewVideoShow(int id)
        {
#if RewardedAdv_yg
            YG2.RewardedAdvShow(id.ToString());
#endif
        }
        public void _RewardedShow(int id)
        {
#if RewardedAdv_yg
            YG2.RewardedAdvShow(id.ToString());
#endif
        }
#if RewardedAdv_yg
        private void OnOpenRewardedAdv()
        {
            OpenVideoAd?.Invoke();
            OpenVideoEvent?.Invoke();
        }
        private void OnCloseRewardedAdv()
        {
            CloseVideoAd?.Invoke();
            CloseVideoEvent?.Invoke();
        }
        private void OnRewardAdv(string id)
        {
            RewardVideoAd?.Invoke();
            RewardVideoEvent?.Invoke(int.Parse(id));
        }
        private void OnErrorRewardedAdv() => ErrorVideoAd?.Invoke();
#endif
        public static void StickyAdActivity(bool activity)
        {
#if StickyAdv_yg
            YG2.StickyAdActivity(activity);
#endif
        }
        public void _StickyAdActivity(bool activity)
        {
#if StickyAdv_yg
            YG2.StickyAdActivity(activity);
#endif
        }
        public static void OpenAuthDialog()
        {
#if Authorization_yg
            YG2.OpenAuthDialog();
#endif
        }
        public void _RequestAuth()
        {
#if Authorization_yg
            YG2.GetAuth();
#endif
        }
        public void _OpenAuthDialog()
        {
#if Authorization_yg
            YG2.OpenAuthDialog();
#endif
        }
        public void _RequesEnvirData()
        {
#if EnvirData_yg
            YG2.GetEnvirData();
#endif
        }
        public static void SwitchLanguage(string language)
        {
#if Localization_yg
            YG2.SwitchLanguage(language);
#endif
        }
        public void _LanguageRequest()
        {
#if Localization_yg
            YG2.GetLanguage();
#endif
        }
        public void _SwitchLanguage(string language)
        {
#if Localization_yg
            YG2.SwitchLanguage(language);
#endif
        }

#if Localization_yg
        private void OnSwitchLang(string lang) => SwitchLangEvent?.Invoke(lang);
#endif
        public static void NewLeaderboardScores(string nameLB, int score)
        {
#if Leaderboards_yg
            YG2.SetLeaderboard(nameLB, score);
#endif
        }
        public static void NewLBScoreTimeConvert(string nameLB, float secondsScore)
        {
#if Leaderboards_yg
            YG2.SetLBTimeConvert(nameLB, secondsScore);
#endif
        }
        public static void BuyPayments(string id)
        {
#if Payments_yg
            YG2.BuyPayments(id);
#endif
        }
        public static void ConsumePurchases()
        {
#if Payments_yg
            YG2.ConsumePurchases();
#endif
        }
        public void _BuyPayments(string id)
        {
#if Payments_yg
            YG2.BuyPayments(id);
#endif
        }
        public void _ConsumePurchases()
        {
#if Payments_yg
            YG2.ConsumePurchases();
#endif
        }

#if Payments_yg
        private void OnPurchaseSuccess(string id) => PurchaseSuccess?.Invoke();
        private void OnPurchaseFailed(string id) => PurchaseFailed?.Invoke();
#endif
        public static void ResetSaveProgress()
        {
#if Storage_yg
            YG2.SetDefaultSaves();
#endif
        }
        public static void SaveProgress()
        {
#if Storage_yg
            YG2.SaveProgress();
#endif
        }
        public static void LoadProgress()
        {
#if Storage_yg
            YGInsides.LoadProgress();
#endif
        }
        public void _ResetSaveProgress()
        {
#if Storage_yg
            YG2.SetDefaultSaves();
#endif
        }
        public void _SaveProgress()
        {
#if Storage_yg
            YG2.SaveProgress();
#endif
        }
        public void _LoadProgress()
        {
#if Storage_yg
            YGInsides.LoadProgress();
#endif
        }
        public static void SetFullscreen(bool fullscreen)
        {
#if Fullscreen_yg
            YG2.SetFullscreen(fullscreen);
#endif
        }
        public void _SetFullscreen(bool fullscreen)
        {
#if Fullscreen_yg
            YG2.SetFullscreen(fullscreen);
#endif
        }
        public static void OnURL_Yandex_DefineDomain(string url)
        {
#if OpenURL_yg
            YG2.OnURLDefineDomain(url);
#endif
        }
        public static void OnAnyURL(string url)
        {
#if OpenURL_yg
            YG2.OnURL(url);
#endif
        }
        public void _OnURL_Yandex_DefineDomain(string url)
        {
#if OpenURL_yg
            YG2.OnURLDefineDomain(url);
#endif
        }
        public void _OnAnyURL(string url)
        {
#if OpenURL_yg
            YG2.OnURL(url);
#endif
        }
        public void _ReviewShow(bool authDialog)
        {
#if Review_yg
            YG2.ReviewShow();
#endif
        }
#if Review_yg
        private void OnReviewSent(bool b) => ReviewDo?.Invoke();
#endif
        public void _PromptShow()
        {
#if GameLabel_yg
            YG2.GameLabelShowDialog();
#endif
        }
#if GameLabel_yg
        private void OnGameLabelSuccess() => PromptDo?.Invoke();
        private void OnGameLabelFail() => PromptFail?.Invoke();
#endif
    }
    public static class YandexMetrica
    {
        public static void Send(string eventName)
        {
#if Metrica_yg
            YG2.MetricaSend(eventName);
#endif
        }
        public static void Send(string eventName, Dictionary<string, string> eventParams)
        {
#if Metrica_yg
            YG2.MetricaSend(eventName, eventParams);
#endif
        }
    }

#if !Storage_yg
    public partial class SavesYG { public int idSave; }
#endif
    public partial class SavesYG
    {
        public string language
        {
#if Localization_yg
            get => YG2.lang;
#else
            get => "ru";
#endif
        }
    }
}

#if !Leaderboards_yg
namespace YG.Utils.LB
{
    public static class LBMethods
    {
        public static string TimeTypeConvertStatic(int value)
        {
            return string.Empty;
        }
    }
}
#endif
