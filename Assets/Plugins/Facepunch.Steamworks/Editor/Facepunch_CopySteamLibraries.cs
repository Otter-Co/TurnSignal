using UnityEditor;
using UnityEditor.Callbacks;

namespace Facepunch.Editor
{
    public class CopySteamLibraries
    {
        [PostProcessBuild(1)]
        public static void Copy(BuildTarget target, string pathToBuiltProject)
        {
            //
            // Only steam
            //
            if (!target.ToString().StartsWith("Standalone"))
                return;

            //
            // You only need a steam_appid.txt if you're launching outside of Steam, you don't need to ship with it
            // but most games do anyway.
            //
            FileUtil.ReplaceFile("steam_appid.txt", System.IO.Path.GetDirectoryName(pathToBuiltProject) + "/steam_appid.txt");

            //
            // Put these dlls next to the exe
            //
            if (target == BuildTarget.StandaloneWindows)
                FileUtil.ReplaceFile("steam_api.dll", System.IO.Path.GetDirectoryName(pathToBuiltProject) + "/steam_api.dll");

            if (target == BuildTarget.StandaloneWindows64)
                FileUtil.ReplaceFile("steam_api64.dll", System.IO.Path.GetDirectoryName(pathToBuiltProject) + "/steam_api64.dll");

            if (target == BuildTarget.StandaloneOSX)
                FileUtil.ReplaceFile("libsteam_api.dylib", System.IO.Path.GetDirectoryName(pathToBuiltProject) + "/libsteam_api.dylib");

            if (target == BuildTarget.StandaloneLinux64 || target == BuildTarget.StandaloneLinuxUniversal)
                FileUtil.ReplaceFile("libsteam_api64.so", System.IO.Path.GetDirectoryName(pathToBuiltProject) + "/libsteam_api64.so");

            if (target == BuildTarget.StandaloneLinux || target == BuildTarget.StandaloneLinuxUniversal)
                FileUtil.ReplaceFile("libsteam_api.so", System.IO.Path.GetDirectoryName(pathToBuiltProject) + "/libsteam_api.so");
        }

    }
}