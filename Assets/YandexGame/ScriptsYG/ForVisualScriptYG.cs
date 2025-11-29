using UnityEngine;
using UnityEngine.Events;

namespace YG
{
    [DefaultExecutionOrder(-1)]
    public class ForVisualScriptYG : MonoBehaviour
    {
        public YandexGame yandexGame;
        public UnityEvent GetDataEvent;

        [HideInInspector] public bool SDKEnabled;
        [HideInInspector] public bool auth;
        [HideInInspector] public string playerName;
        [HideInInspector] public string playerId;
        [HideInInspector] public string playerPhoto;
        [HideInInspector] public string photoSize;
        [HideInInspector] public string language;
        [HideInInspector] public string domain;
        [HideInInspector] public string deviceType;
        [HideInInspector] public bool isMobile;
        [HideInInspector] public bool isDesktop;
        [HideInInspector] public bool isTablet;
        [HideInInspector] public bool isTV;
        [HideInInspector] public string appID;
        [HideInInspector] public string browserLang;
        [HideInInspector] public string payload;
        [HideInInspector] public string languageSaves;

        private void Awake()
        {
#if PLUGIN_YG_2
            SDKEnabled = YG2.isSDKEnabled;
#endif
#if Authorization_yg
            auth = YG2.player.auth;
            playerName = YG2.player.name;
            playerId = YG2.player.id;
            playerPhoto = YG2.player.photo;
            photoSize = YG2.infoYG.Authorization.GetPlayerPhotoSize();
#endif
#if EnvirData_yg
            language = YG2.envir.language;
            domain = YG2.envir.domain;
            deviceType = YG2.envir.deviceType;
            isMobile = YG2.envir.isMobile;
            isDesktop = YG2.envir.isDesktop;
            isTablet = YG2.envir.isTablet;
            isTV = YG2.envir.isTV;
            appID = YG2.envir.appID;
            browserLang = YG2.envir.browserLang;
            payload = YG2.envir.payload;
#endif
#if Localization_yg
            languageSaves = YG2.lang;
#endif
            GetDataEvent.Invoke();
        }
    }
}
