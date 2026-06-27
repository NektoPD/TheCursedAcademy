using UnityEngine;

namespace CharacterLogic.Abilities
{
    public class PoisonThrowAbility : AbilityBase
    {
        private bool _facingLeft;
        private CharacterLogic.InputHandler.CharacterMovementHandler _movementHandler;

        public void SetMovementHandler(CharacterLogic.InputHandler.CharacterMovementHandler handler)
        {
            _movementHandler = handler;
        }

        private void Update()
        {
            if (_movementHandler == null || !_movementHandler.IsMoving()) return;

            if (_movementHandler.IsMovingLeft())
                _facingLeft = true;
            else if (_movementHandler.IsMovingRight())
                _facingLeft = false;
        }

        protected override void Execute()
        {
            IsActive = true;

            Vector2 direction = _facingLeft ? Vector2.left : Vector2.right;

            AbilityProjectile projectile = Instantiate(Config.ProjectilePrefab, OwnerTransform.position, Quaternion.identity);
            projectile.transform.SetParent(null);

            float angle = _facingLeft ? 180f : 0f;
            projectile.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            if (projectile is PoisonProjectile poison)
                poison.SetFacing(_facingLeft);

            projectile.Launch(direction, Config.ProjectileSpeed, Config.Damage, Config.Duration);

            IsActive = false;
        }
    }
}
