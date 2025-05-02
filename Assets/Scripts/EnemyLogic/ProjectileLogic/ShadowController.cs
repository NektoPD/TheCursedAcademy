using UnityEngine;

namespace EnemyLogic.ProjectileLogic
{
    public class ShadowController : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer projectile;
        [SerializeField] private SpriteRenderer shadow;


        private void Update()
        {
            shadow.transform.position = new Vector3(
                projectile.transform.position.x,
                projectile.transform.position.y - 0.5f,
                projectile.transform.position.z
            );

            shadow.transform.rotation = Quaternion.identity;
        }
    }
}