using UnityEngine;
using UnityEngine.Events;

namespace YG
{
#if PLUGIN_YG_2 && GameLabel_yg
    public class PromptYG : GameLabelYG
    {
    }
#else
    public class PromptYG : MonoBehaviour
    {
        [Header("Buttons serialize")]
        [Tooltip("Объект (отключённая кнопка или текст), который будет сообщать о том, что ярлык не поддерживается. Данный объект можно не указывать, тогда, если ярлык не будет поддерживаться - ничего не будет отображаться.")]
        public GameObject notSupported;
        [Tooltip("Объект (отключённая кнопка или текст), который будет сообщать о том, что ярлык уже установлен. Данный объект можно не указывать, тогда, если ярлык уже установлен - ничего не будет отображаться.")]
        public GameObject done;
        [Tooltip("Объект с кнопкой, которая будет предлагать установить ярлык на рабочий стол (возможно, за вознаграждение). При клике на кнопку необходимо запускать метод PromptShow через данный скрипт или через YandexGame скрипт.")]
        public GameObject showDialog;
        [Header("Events")]
        [Space(5)]
        public UnityEvent onPromptSuccess;
        public UnityEvent onPromptFail;
    }
#endif
}