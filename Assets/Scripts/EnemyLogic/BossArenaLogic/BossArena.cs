using Cinemachine;
using Difficulties;
using UnityEngine;

namespace EnemyLogic.BossArenaLogic
{
    [RequireComponent(typeof(BossArenaCutscenesActivator))]
    public class BossArena : MonoBehaviour
    {
        [SerializeField] private float _teleportDistanceFromCenter = 3f;
        [SerializeField] private CinemachineConfiner2D _confinerCamera;
        [SerializeField] private PolygonCollider2D _cameraBounds;
        [SerializeField] private Difficulty _difficulty;

        private BossArenaCutscenesActivator _cutscensActivator;
        private Enemy _boss = null;

        private void Awake()
        {
            _cutscensActivator = GetComponent<BossArenaCutscenesActivator>();
        }

        private void OnEnable()
        {
            _difficulty.BossSpawned += Activate;
        }

        private void OnDisable()
        {
            _difficulty.BossSpawned -= Activate;
        }

        private void Activate(Enemy boss)
        {
            Vector3 center = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, Camera.main.nearClipPlane));

            if (_boss == null)
                transform.position = center;
            else
               _boss.Died -= _cutscensActivator.DeadCutsceneActivate;

            _boss = boss;
            _boss.Died += _cutscensActivator.DeadCutsceneActivate;
            SetupCameraBounds();

            _cutscensActivator.SpawnCutsceneActivate(_boss);
        }

        private void Deactivate()
        {
            _confinerCamera.m_BoundingShape2D = null;
            _confinerCamera.InvalidateCache();
            _cameraBounds.gameObject.SetActive(false);

            _boss.Died -= _cutscensActivator.DeadCutsceneActivate;
            _boss = null;
        }

        private void SetupCameraBounds()
        {
            _cameraBounds.gameObject.SetActive(true);

            _confinerCamera.m_BoundingShape2D = _cameraBounds;
        }
    }
}