using Timelines;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace EnemyLogic.BossArenaLogic
{
    public class BossArenaCutscenesActivator : MonoBehaviour
    {
        private const string BossTrackName = "Boss";

        [SerializeField] private TimelineController _timelineController;
        [SerializeField] private PlayableDirector _spawnCutscene;
        [SerializeField] private PlayableDirector _deadCutscene;

        private Enemy _boss;

        public void SpawnCutsceneActivate(Enemy boss)
        {
            _boss = boss;
            BindTrack(BossTrackName, _boss.EnemyAnimator, _spawnCutscene);

            _timelineController.StartCutscene(_spawnCutscene.name);
        }

        public void DeadCutsceneActivate()
        {
            BindTrack(BossTrackName, _boss.EnemyAnimator, _deadCutscene);
            _timelineController.StartCutscene(_deadCutscene.name);
        }

        private void BindTrack(string trackName, Animator animator, PlayableDirector cutscene)
        {
            var timeline = cutscene.playableAsset as TimelineAsset;

            foreach (var track in timeline.GetOutputTracks())
            {
                if (track.name == trackName)
                    cutscene.SetGenericBinding(track, animator);
            }
        }
    }
}