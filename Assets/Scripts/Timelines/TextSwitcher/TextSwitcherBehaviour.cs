using System;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using Utils;

namespace Timelines.TextSwitcher
{
    [Serializable]
    public class TextSwitcherBehaviour : PlayableBehaviour
    {
        [SerializeField] private string _textRu;
        [SerializeField] private string _textEn;
        [SerializeField] private string _textTr;

        private TextMeshProUGUI _textObject;

        private bool _firstFrameHappend;
        private string _defaultText = string.Empty;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            _textObject = playerData as TextMeshProUGUI;

            if (_textObject == null) 
                return;

            if (_firstFrameHappend == false)
            {
                _defaultText = _textObject.text;
                _firstFrameHappend = true;
            }

            _textObject.text = Translator.Translate(_textRu, _textEn, _textTr);
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            _firstFrameHappend = false;

            if (_textObject == null)
                return;

            _textObject.text = _defaultText;

            base.OnBehaviourPause(playable, info);
        }
    }
}
