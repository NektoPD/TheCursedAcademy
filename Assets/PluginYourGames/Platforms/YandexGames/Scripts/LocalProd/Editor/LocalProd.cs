#if UNITY_EDITOR && YandexGamesPlatform_yg
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEditor;
using YG.EditorScr.BuildModify;
using YG.Insides;
using Debug = UnityEngine.Debug;

namespace YG.EditorScr
{
    [InitializeOnLoad]
    public static class LocalProd
    {
        private const string PACKAGE_NAME = "@yandex-games/sdk-dev-proxy";
        private const string PROCESS_ID_KEY = "YG2.LocalProd.ProcessId";
        private const string PROCESS_START_TIME_KEY = "YG2.LocalProd.ProcessStartTime";
        private const string AUTO_RUN_KEY = "YG2.LocalProd.AutoRunAfterBuild";
        private const string MENU_ROOT = "Tools/YG2/";
        private const string RUN_MENU_PATH = MENU_ROOT + "Run Local Build in Yandex Games";
        private const string AUTO_RUN_MENU_PATH = MENU_ROOT + "Auto Run After Build";
        private const double OPEN_PAGE_DELAY = 2d;
        private static string productionUrl;
        private static double openPageTime;

        static LocalProd()
        {
            ModifyBuild.onModifyComplete -= OnBuildComplete;
            ModifyBuild.onModifyComplete += OnBuildComplete;
        }

        [MenuItem(RUN_MENU_PATH, false, 30)]
        public static void Run()
        {
            try
            {
                PlatformInfo settings = YG2.infoYG.platformInfo;
                string buildPath = BuildLog.ReadProperty("Build path");

                if (string.IsNullOrWhiteSpace(buildPath) || !Directory.Exists(buildPath))
                {
                    Debug.LogError($"{LocalProdLangs.buildFolderNotFound}\n{buildPath}");
                    return;
                }

                buildPath = Path.GetFullPath(buildPath);
                string indexPath = Path.Combine(buildPath, "index.html");

                if (!File.Exists(indexPath))
                {
                    Debug.LogError($"{LocalProdLangs.indexNotFound}\n{indexPath}");
                    return;
                }

                if (!long.TryParse(settings.localProdGameId, out long gameId) || gameId <= 0)
                {
                    Debug.LogError(LocalProdLangs.invalidGameId);
                    return;
                }

                if (settings.localProdPort < 1 || settings.localProdPort > 65535)
                {
                    Debug.LogError(LocalProdLangs.invalidPort);
                    return;
                }

                if (!IsNpxAvailable())
                {
                    Debug.LogError(LocalProdLangs.nodeNotFound);
                    return;
                }

                WarnAboutCompressedBuild(buildPath);
                StopPreviousServer();

                string command = BuildCommand(buildPath, gameId, settings);
                Debug.Log(string.Format(LocalProdLangs.launchInfo,
                    gameId, buildPath, settings.localProdPort, settings.localProdUseCsp, command));

                StartServer(command);
                // With --app-id, sdk-dev-proxy opens the draft page itself.
                if (!settings.localProdUseCsp)
                    ScheduleProductionPage(gameId, settings.localProdPort);
            }
            catch (Exception exception)
            {
                Debug.LogError($"{LocalProdLangs.launchFailed}\n{exception}\n\n{LocalProdLangs.nodeNotFound}");
            }
        }

        [MenuItem(AUTO_RUN_MENU_PATH, false, 31)]
        private static void ToggleAutoRun()
        {
            bool enabled = !EditorPrefs.GetBool(AUTO_RUN_KEY, false);
            EditorPrefs.SetBool(AUTO_RUN_KEY, enabled);
            Menu.SetChecked(AUTO_RUN_MENU_PATH, enabled);
        }

        [MenuItem(AUTO_RUN_MENU_PATH, true)]
        private static bool ValidateAutoRun()
        {
            Menu.SetChecked(AUTO_RUN_MENU_PATH, EditorPrefs.GetBool(AUTO_RUN_KEY, false));
            return true;
        }

        private static void OnBuildComplete()
        {
            if (!UnityEngine.Application.isBatchMode && EditorPrefs.GetBool(AUTO_RUN_KEY, false))
                Run();
        }

        private static string BuildCommand(string buildPath, long gameId, PlatformInfo settings)
        {
            StringBuilder command = new StringBuilder();
            // --yes lets npx download the official package without an installation prompt.
            command.Append($"npx --yes {PACKAGE_NAME} -p \"{buildPath.Replace("\"", "\\\"")}\"");
            command.Append($" --port={settings.localProdPort}");

            if (settings.localProdUseCsp)
            {
                // Version 0.0.2 needs the app ID to fetch this draft's CSP rules.
                command.Append($" --app-id={gameId}");
                command.Append(" --csp");
            }

            return command.ToString();
        }

        private static bool IsNpxAvailable()
        {
#if UNITY_EDITOR_WIN
            ProcessStartInfo checkInfo = new ProcessStartInfo("where.exe", "npx")
#else
            ProcessStartInfo checkInfo = new ProcessStartInfo("/usr/bin/env", "sh -lc \"command -v npx\"")
#endif
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            using (Process process = Process.Start(checkInfo))
            {
                if (process == null)
                    return false;

                process.WaitForExit();
                return process.ExitCode == 0;
            }
        }

        private static void StartServer(string command)
        {
#if UNITY_EDITOR_WIN
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/k \"{command}\"",
                UseShellExecute = true,
                CreateNoWindow = false
            };
#else
            string escapedCommand = command.Replace("'", "'\\''");
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "/bin/sh",
                Arguments = $"-lc '{escapedCommand}'",
                UseShellExecute = false,
                CreateNoWindow = false
            };
#endif
            Process process = Process.Start(startInfo);

            if (process == null)
                throw new InvalidOperationException(LocalProdLangs.processNotStarted);

            SessionState.SetInt(PROCESS_ID_KEY, process.Id);
            SessionState.SetString(PROCESS_START_TIME_KEY, process.StartTime.ToUniversalTime().Ticks.ToString());
            process.Dispose();
        }

        private static void StopPreviousServer()
        {
            int processId = SessionState.GetInt(PROCESS_ID_KEY, 0);
            string savedStartTime = SessionState.GetString(PROCESS_START_TIME_KEY, string.Empty);
            ClearSavedProcess();

            if (processId <= 0 || !long.TryParse(savedStartTime, out long startTimeTicks))
                return;

            try
            {
                using (Process process = Process.GetProcessById(processId))
                {
                    if (process.StartTime.ToUniversalTime().Ticks != startTimeTicks)
                        return;

#if UNITY_EDITOR_WIN
                    ProcessStartInfo stopInfo = new ProcessStartInfo
                    {
                        FileName = "taskkill.exe",
                        Arguments = $"/PID {processId} /T /F",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    using (Process stopProcess = Process.Start(stopInfo))
                    {
                        if (stopProcess == null)
                            throw new InvalidOperationException(LocalProdLangs.processNotStopped);

                        stopProcess.WaitForExit();

                        if (stopProcess.ExitCode != 0 && !process.HasExited)
                            throw new InvalidOperationException(LocalProdLangs.processNotStopped);
                    }
#else
                    process.Kill();
                    process.WaitForExit();
#endif
                }
            }
            catch (ArgumentException)
            {
                // The previously started process has already exited.
            }
        }

        private static void ClearSavedProcess()
        {
            SessionState.EraseInt(PROCESS_ID_KEY);
            SessionState.EraseString(PROCESS_START_TIME_KEY);
        }

        private static void WarnAboutCompressedBuild(string buildPath)
        {
            string buildDirectory = Path.Combine(buildPath, "Build");

            if (!Directory.Exists(buildDirectory))
                return;

            bool hasBrotliFiles = Directory.GetFiles(buildDirectory, "*.br").Length > 0;
            bool hasGzipFiles = Directory.GetFiles(buildDirectory, "*.gz").Length > 0;

            if ((hasBrotliFiles || hasGzipFiles) && !UnityEditor.PlayerSettings.WebGL.decompressionFallback)
                Debug.LogWarning(LocalProdLangs.compressionWarning);
        }

        private static void ScheduleProductionPage(long gameId, int port)
        {
            string localGameUrl = Uri.EscapeDataString($"https://localhost:{port}");
            productionUrl = $"https://yandex.ru/games/app/{gameId}?draft=true&game_url={localGameUrl}";
            openPageTime = EditorApplication.timeSinceStartup + OPEN_PAGE_DELAY;

            EditorApplication.update -= OpenProductionPage;
            EditorApplication.update += OpenProductionPage;
        }

        private static void OpenProductionPage()
        {
            if (EditorApplication.timeSinceStartup < openPageTime)
                return;

            EditorApplication.update -= OpenProductionPage;
            UnityEngine.Application.OpenURL(productionUrl);
        }

    }
}
#endif
