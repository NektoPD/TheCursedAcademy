using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Timelines.TextSwitcher
{
    [Serializable]
    public class TextSwitcherClipAsset : PlayableAsset, ITimelineClipAsset
    {
        [SerializeField] private TextSwitcherBehaviour _template = new();

        public ClipCaps clipCaps => ClipCaps.None;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<TextSwitcherBehaviour>.Create(graph, _template);
            return playable;
        }
    }
}