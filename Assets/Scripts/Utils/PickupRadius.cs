using UnityEngine;

namespace Utils
{
    public static class PickupRadius
    {
        public static float Scale { get; private set; } = 1f;

        public static void Set(float multiplier) => Scale = Mathf.Max(1f, multiplier);
    }
}
