using System;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;

namespace Editor
{
	[UsedImplicitly]
	public class BuildManagement
	{
		[MenuItem("Developement/Builds/Windows/Build")]
		private static void BuildWindowsPlayer() => BuildWindowsPlayer(BuildOptions.None);
		[MenuItem("Developement/Builds/Windows/Build and run")]
		private static void BuildAndRunWindowsPlayer() => BuildWindowsPlayer(BuildOptions.AutoRunPlayer);
		[MenuItem("Developement/Builds/Windows/Build (scripts only)")]
		private static void BuildScriptsWindowsPlayer() => BuildWindowsPlayer(BuildOptions.BuildScriptsOnly);
		[MenuItem("Developement/Builds/Windows/Build and run (scripts only)")]
		private static void BuildScriptsAndRunWindowsPlayer() => BuildWindowsPlayer(BuildOptions.AutoRunPlayer | BuildOptions.BuildScriptsOnly);

		[MenuItem("Developement/Builds/Linux/Build")]
		private static void BuildLinuxPlayer() => BuildLinuxPlayer(BuildOptions.None);
		[MenuItem("Developement/Builds/Linux/Build and run")]
		private static void BuildAndRunLinuxPlayer() => BuildLinuxPlayer(BuildOptions.AutoRunPlayer);
		[MenuItem("Developement/Builds/Linux/Build (scripts only)")]
		private static void BuildScriptsLinuxPlayer() => BuildLinuxPlayer(BuildOptions.BuildScriptsOnly);
		[MenuItem("Developement/Builds/Linux/Build and run (scripts only)")]
		private static void BuildScriptsAndRunLinuxPlayer() => BuildLinuxPlayer( BuildOptions.AutoRunPlayer | BuildOptions.BuildScriptsOnly);

		[MenuItem("Developement/Builds/Android Build")]
		private static void BuildAndroidPlayer()
			=> BuildPlayer(BuildTarget.Android, BuildOptions.None, "Builds/Linux/Automatic/NKM");

		private static void BuildWindowsPlayer(BuildOptions buildOptions)
			=> BuildPlayer(BuildTarget.StandaloneWindows64, buildOptions, Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/NKM/NKM.exe");

		private static void BuildLinuxPlayer(BuildOptions buildOptions)
			=> BuildPlayer(BuildTarget.StandaloneLinux64, buildOptions, "Builds/Linux/Automatic/NKM");


		private static void BuildPlayer(BuildTarget buildTarget, BuildOptions buildOptions, string locationPathName)
		{
			buildOptions |= BuildOptions.Development | BuildOptions.AllowDebugging;
			string[] scenes = EditorBuildSettings.scenes.Select(it => it.path).ToArray();

			var buildPlayerOptions = new BuildPlayerOptions
			{
				scenes = scenes,
				locationPathName = locationPathName,
				target = buildTarget,
				options = buildOptions
			};
			BuildPipeline.BuildPlayer(buildPlayerOptions);
		}
	}
}
