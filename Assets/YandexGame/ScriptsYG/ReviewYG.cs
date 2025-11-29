using UnityEngine;
using UnityEngine.Events;

namespace YG
{
    public class ReviewYG : MonoBehaviour
    {
        [Tooltip("Открывать окно авторизации, если пользователь не авторизован.")]
        public enum ForUnauthorized { OpenAuthDialog, ReviewNotAvailable, Ignore };
        public ForUnauthorized forUnauthorized;

        [Tooltip("Активировать оценку игры на мобильных устройствах?")]
        public bool showOnMobileDevice;

        [Tooltip("Обновлять информацию при каждой активации объекта (в OnEnable)?")]
        public bool updateDataOnEnable;
        [Space(15)]
        public UnityEvent ReviewAvailable;
        public UnityEvent ReviewNotAvailable;
        public UnityEvent LeftReview;
        public UnityEvent NotLeftReview;
    }
}
