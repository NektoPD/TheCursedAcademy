using Enums;
using UnityEngine;

namespace Utils
{
    public static class OffscreenPositionGenerator
    {
        public static Vector3 GetRandomPositionOutsideCamera(float offset)
        {
            Camera camera = Camera.main;

            Side[] pool = new Side[] { Side.Top, Side.Bottom, Side.Right, Side.Left };
            Side side = (Side)pool.GetValue(Random.Range(0, pool.Length));

            Vector3 viewportPosition = Vector3.zero;

            switch (side)
            {
                case Side.Top:
                    viewportPosition = new Vector3(Random.Range(0f, 1f), 1 + offset, camera.nearClipPlane);
                    break;

                case Side.Bottom:
                    viewportPosition = new Vector3(Random.Range(0f, 1f), -offset, camera.nearClipPlane);
                    break;

                case Side.Left:
                    viewportPosition = new Vector3(-offset, Random.Range(0f, 1f), camera.nearClipPlane);
                    break;

                case Side.Right:
                    viewportPosition = new Vector3(1 + offset, Random.Range(0f, 1f), camera.nearClipPlane);
                    break;
            }

            return camera.ViewportToWorldPoint(viewportPosition);
        }
    }
}
