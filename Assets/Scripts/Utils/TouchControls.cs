using System.Runtime.InteropServices;
using YG;

namespace Utils
{
    public static class TouchControls
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern int CursedAcademy_PrefersTouchControls();
#endif

        public static bool IsEnabled(bool forceInEditor)
        {
#if UNITY_EDITOR
            return forceInEditor;
#else
            var device = YandexGame.EnvironmentData;
            if (!device.isDesktop || device.isMobile || device.isTablet)
                return true;

#if UNITY_WEBGL
            return CursedAcademy_PrefersTouchControls() != 0;
#else
            return false;
#endif
#endif
        }
    }
}
