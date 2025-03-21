using Enums;
using UnityEngine;

namespace Utils
{
    public class OffscreenPositionGenerator : MonoBehaviour
    {
        [SerializeField] private float _offset = 0.1f;

        public Vector3 GetRandomPositionOutsideCamera()
        {
            Camera camera = Camera.main;

            Side[] pool = new Side[] { Side.Top, Side.Bottom, Side.Right, Side.Left };
            Side side = (Side)pool.GetValue(Random.Range(0, pool.Length));

            Vector3 viewportPosition = Vector3.zero;

            switch (side)
            {
                case Side.Top:
                    viewportPosition = new Vector3(Random.Range(0f, 1f), 1 + _offset, camera.nearClipPlane);
                    break;

                case Side.Bottom:
                    viewportPosition = new Vector3(Random.Range(0f, 1f), -_offset, camera.nearClipPlane);
                    break;

                case Side.Left:
                    viewportPosition = new Vector3(-_offset, Random.Range(0f, 1f), camera.nearClipPlane);
                    break;

                case Side.Right:
                    viewportPosition = new Vector3(1 + _offset, Random.Range(0f, 1f), camera.nearClipPlane);
                    break;
            }

            return camera.ViewportToWorldPoint(viewportPosition);
        }
    }
}
