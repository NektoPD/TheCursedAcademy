using UnityEngine;
using YG;

namespace Utils
{
    public class Ads : MonoBehaviour
    {
        public void OpenRewardAd() => YandexGame.RewVideoShow(0);
    }
}