using UnityEngine;

namespace CharacterLogic.Abilities
{
    public class PoisonThrowAbility : AbilityBase
    {
        private Vector2 _lastFacingDirection = Vector2.right;
        private CharacterLogic.InputHandler.CharacterMovementHandler _movementHandler;

        public void SetMovementHandler(CharacterLogic.InputHandler.CharacterMovementHandler handler)
        {
            _movementHandler = handler;
        }

        private void Update()
        {
            if (_movementHandler != null && _movementHandler.IsMoving())
                _lastFacingDirection = _movementHandler.GetMoveDirection();
        }

        protected override void Execute()
        {
            IsActive = true;

            Vector2 direction = _movementHandler != null && _movementHandler.IsMoving()
                ? _movementHandler.GetMoveDirection()
                : _lastFacingDirection;

            AbilityProjectile projectile = Instantiate(Config.ProjectilePrefab, OwnerTransform.position, Quaternion.identity);
            projectile.transform.SetParent(null);

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            projectile.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            projectile.Launch(direction, Config.ProjectileSpeed, Config.Damage, Config.Duration);

            IsActive = false;
        }
    }
}
