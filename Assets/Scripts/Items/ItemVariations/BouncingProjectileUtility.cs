using UnityEngine;

namespace Items.ItemVariations
{
    public static class BouncingProjectileUtility
    {
        public static void MoveWithViewportBounce(Transform transform, ref Vector2 direction, float speed)
        {
            Vector2 position = transform.position;
            float distance = speed * Time.deltaTime;
            Vector2 nextPosition = position + direction * distance;

            Camera camera = Camera.main;
            if (camera == null)
            {
                transform.position = nextPosition;
                return;
            }

            Vector3 min = camera.ViewportToWorldPoint(Vector3.zero);
            Vector3 max = camera.ViewportToWorldPoint(Vector3.one);

            if (nextPosition.x < min.x || nextPosition.x > max.x)
            {
                direction.x = -direction.x;
                nextPosition.x = Mathf.Clamp(nextPosition.x, min.x, max.x);
            }

            if (nextPosition.y < min.y || nextPosition.y > max.y)
            {
                direction.y = -direction.y;
                nextPosition.y = Mathf.Clamp(nextPosition.y, min.y, max.y);
            }

            transform.position = nextPosition;
        }
    }
}
