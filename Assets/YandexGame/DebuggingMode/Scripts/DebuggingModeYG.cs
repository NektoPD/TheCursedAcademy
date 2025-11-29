using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
#if PLUGIN_YG_2
using YG.Insides;
#endif

namespace YG
{
    [HelpURL("https://www.notion.so/PluginYG-d457b23eee604b7aa6076116aab647ed#4968547185c2460fb70fd6eceaf101d4")]
    public class DebuggingModeYG : MonoBehaviour
    {
        [Tooltip("?payload=\nЭто значение, которое Вы будете передавать с помощью Deep Linking. Можете написать слово, например, debug и добавить свой пароль, например, 123. Получится debug123.")]
        public string payloadPassword = "debug123";
        [Tooltip("Отображение панели управления в Unity Editor")]
        public bool debuggingInEditor;

#if Leaderboards_yg
        [Serializable]
        public class LeaderboardTest
        {
            public LeaderboardYG leaderboardYG;
            public InputField nameLbInputField;
            public InputField scoreLbInputField;
        }
        public LeaderboardTest leaderboard;
#endif
        public static DebuggingModeYG Instance { get; private set; }

        private Canvas canvas;
        private Transform tr;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);

                if (!canvas)
                    canvas = GetComponent<Canvas>();
                canvas.enabled = false;
#if PLUGIN_YG_2
                if (YG2.isSDKEnabled)
                    GetDataEvent();
#endif
            }
        }

#if PLUGIN_YG_2
        private void OnEnable() => YG2.onGetSDKData += GetDataEvent;
        private void OnDisable() => YG2.onGetSDKData -= GetDataEvent;
#endif
        public void GetDataEvent()
        {
            bool draw = false;
#if UNITY_EDITOR
            if (debuggingInEditor)
                draw = true;
#else
#if EnvirData_yg
            if (YG2.envir.payload == payloadPassword) 
                draw = true;
#endif
#endif
            if (draw)
            {
                if (!canvas)
                    canvas = GetComponent<Canvas>();
                canvas.enabled = true;

                if (!tr)
                    tr = transform;

#if Localization_yg
                tr.Find("Panel").Find("LanguageDebug").GetChild(0).GetComponent<Text>().text = YG2.lang;
#endif
#if Authorization_yg
                string playerId = YG2.player.id;
                if (playerId.Length > 10)
                    playerId = playerId.Remove(10) + "...";
#endif

#if PLUGIN_YG_2
                tr.Find("Panel").Find("DebugData").GetChild(0).GetComponent<Text>().text = "isSDKEnabled - " + YG2.isSDKEnabled
#if Authorization_yg
                     + "\nplayerName - " + YG2.player.name +
                    "\nplayerId - " + playerId +
                    "\nauth - " + YG2.player.auth +
                    "\nphotoSize - " + YG2.infoYG.Authorization.GetPlayerPhotoSize()
#endif
#if EnvirData_yg
                     + "\ndomain - " + YG2.envir.domain +
                    "\ndeviceType - " + YG2.envir.deviceType +
                    "\nisMobile - " + YG2.envir.isMobile +
                    "\nisDesktop - " + YG2.envir.isDesktop +
                    "\nisTablet - " + YG2.envir.isTablet +
                    "\nisTV - " + YG2.envir.isTV +
                    "\nisTablet - " + YG2.envir.isTablet +
                    "\nappID - " + YG2.envir.appID +
                    "\nbrowserLang - " + YG2.envir.browserLang +
                    "\nplatform - " + YG2.envir.platform +
                    "\nbrowser - " + YG2.envir.browser +
                    "\npayload - " + YG2.envir.payload
#endif
                    ;
#endif
            }
        }

        public void GetDataButton()
        {
#if PLUGIN_YG_2
            YG2.onGetSDKData?.Invoke();
#endif
        }

        public void SceneButton(int index)
        {
            SceneManager.LoadScene(index);
        }

        public static Action onRBTRecalculate;
        public void RBTRecalculateButton()
        {
            onRBTRecalculate?.Invoke();
        }

        public static Action onRBTExecuteCode;
        public void RBTExecuteCodeButton()
        {
            onRBTExecuteCode?.Invoke();
        }

        public static Action<bool> onRBTActivity;
        public void RBTActivityButton(bool ativity)
        {
            onRBTActivity?.Invoke(ativity);
        }

        public void AuthCheckButton()
        {
#if Authorization_yg
            YG2.GetAuth();
#endif
        }

        public void AuthDialogButton()
        {
#if Authorization_yg
            YG2.OpenAuthDialog();
#endif
        }

        public void FullAdButton()
        {
#if InterstitialAdv_yg
            YG2.InterstitialAdvShow();
#endif
        }

        public void VideoAdButton()
        {
#if RewardedAdv_yg
            if (!tr) tr = transform;
            string id = tr.Find("Panel").Find("RewardAd").GetChild(0).GetComponent<InputField>().text;
            YG2.RewardedAdvShow(id);
#endif
        }

        public void StickyAdShowButton()
        {
#if StickyAdv_yg
            YG2.StickyAdActivity(true);
#endif
        }

        public void StickyAdHideButton()
        {
#if StickyAdv_yg
            YG2.StickyAdActivity(false);
#endif
        }

        public void RedefineLangButton()
        {
#if Localization_yg
            YG2.GetLanguage();
#endif
        }

        public void SwitchLanguage(Text text)
        {
#if Localization_yg
            YG2.SwitchLanguage(text.text);
#endif
        }

        public void PromptDialogButton()
        {
#if GameLabel_yg
            YG2.GameLabelShowDialog();
#endif
        }

        public void ReviewButton()
        {
#if Review_yg
            YG2.ReviewShow();
#endif
        }

        public void BuyPurchaseButton()
        {
#if Payments_yg
            if (!tr) tr = transform;
            string id = tr.Find("Panel").Find("PurchaseID").GetChild(0).GetComponent<InputField>().text;
            YG2.BuyPayments(id);
#endif
        }

        public void DeletePurchaseButton()
        {
#if Payments_yg
            if (!tr) tr = transform;
            string id = tr.Find("Panel").Find("PurchaseID").GetChild(0).GetComponent<InputField>().text;
            YG2.ConsumePurchaseByID(id);
#endif
        }

        public void DeleteAllPurchasesButton()
        {
#if Payments_yg
            YG2.ConsumePurchases();
#endif
        }

        public void SaveButton()
        {
#if Storage_yg
            YG2.SaveProgress();
#endif
        }

        public void LoadButton()
        {
#if Storage_yg
            YGInsides.LoadProgress();
#endif
        }

        public void ResetSaveButton()
        {
#if Storage_yg
            YG2.SetDefaultSaves();
#endif
        }

        public void NewNameLB()
        {
#if Leaderboards_yg
            leaderboard.leaderboardYG.nameLB = leaderboard.nameLbInputField.text;
            leaderboard.leaderboardYG.UpdateLB();
#endif
        }

        public void NewScoreLB()
        {
#if Leaderboards_yg
            YG2.SetLeaderboard(leaderboard.leaderboardYG.nameLB,
                int.Parse(leaderboard.scoreLbInputField.text));
#endif
        }

        public void NewScoreLBTimeConvert()
        {
#if Leaderboards_yg
            YG2.SetLBTimeConvert(leaderboard.leaderboardYG.nameLB,
                float.Parse(leaderboard.scoreLbInputField.text));
#endif
        }
    }
}
