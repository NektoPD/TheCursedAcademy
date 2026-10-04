using System;
using Difficulties;
using Timelines;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace EnemyLogic.BossArenaLogic
{
    public class BossArena : MonoBehaviour
    {
        private const string BossTrackName = "Boss";

        [SerializeField] private Difficulty _difficulty;
        [SerializeField] private PlayableDirector _spawnCutscene;
        [SerializeField] private PlayableDirector _deadCutscene;
        [SerializeField] private Transform _placeholderBoss;
        [SerializeField] private Transform _bossCamera;

        private Enemy _boss = null;
        private Action _deadCutsceneHandler = null;

        private void OnEnable()
        {
            _difficulty.BossSpawned += Activate;
        }

        private void OnDisable()
        {
            _difficulty.BossSpawned -= Activate;
        }

        public void Deactivate()
        {
            if (_boss != null && _deadCutsceneHandler != null)
                _boss.Died -= _deadCutsceneHandler;

            _deadCutsceneHandler = null;
            _boss = null;
        }

        private void Activate(Enemy boss)
        {
            bool needActivateCutscene = _boss == null;

            if (_boss != null && _deadCutsceneHandler != null)
                _boss.Died -= _deadCutsceneHandler;

            _boss = boss;

            if (needActivateCutscene)
            {
                _boss.transform.SetParent(transform);
                _boss.transform.localPosition = _placeholderBoss.localPosition;
                CutsceneStart(_spawnCutscene);
            }

            _boss.Died += _deadCutsceneHandler = () => 
            {        
                _bossCamera.position = new Vector3(_boss.transform.position.x, _boss.transform.position.y, _bossCamera.position.z);
                CutsceneStart(_deadCutscene);
            };
        }

        public void CutsceneStart(PlayableDirector cutscene)
        {
            cutscene.Stop();
            cutscene.time = 0;
            cutscene.Evaluate();
            BindTrack(BossTrackName, _boss.EnemyAnimator, cutscene);
            cutscene.Play();
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