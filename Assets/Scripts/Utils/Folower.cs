using UnityEngine;


namespace Untils
{
    public class Folower : MonoBehaviour
    {
        [SerializeField] private Transform _target;

        private Transform _transform;
        private bool _isFollowing = true;

        private void Awake()
        {
            _transform = transform;
        }

        private void LateUpdate()
        {
            if (_target == null || _isFollowing == false)
                return;

            _transform.position = _target.position;
        }

        public void StartFollowing() => _isFollowing = true;

        public void StopFollowing() => _isFollowing = false;
    }
}