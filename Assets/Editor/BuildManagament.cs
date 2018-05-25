using System;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;

namespace Editor
{
	[UsedImplicitly]
	public class BuildManagament
	{
		[MenuItem("Developement/Builds/Windows/Build")]
		private static void BuildWindowsPlayer() => BuildPlayerOnWindows(BuildTarget.StandaloneWindows64, BuildOptions.Development);
		[MenuItem("Developement/Builds/Windows/Build and run")]
		private static void BuildAndRunWindowsPlayer() => BuildPlayerOnWindows(BuildTarget.StandaloneWindows64, BuildOptions.AutoRunPlayer | BuildOptions.Development);
		[MenuItem("Developement/Builds/Windows/Build and run (scripts only)")]
		private static void BuildScriptsAndRunWindowsPlayer() => BuildPlayerOnWindows(BuildTarget.StandaloneWindows64, BuildOptions.AutoRunPlayer | BuildOptions.Development | BuildOptions.BuildScriptsOnly);

		[MenuItem("Developement/Builds/Linux/Build")]
		private static void BuildLinuxPlayer() => BuildPlayerOnLinux(BuildTarget.StandaloneLinux64, BuildOptions.Development);
		[MenuItem("Developement/Builds/Linux/Build and run")]
		private static void BuildAndRunLinuxlayer() => BuildPlayerOnLinux(BuildTarget.StandaloneLinux64, BuildOptions.AutoRunPlayer | BuildOptions.Development);
		[MenuItem("Developement/Builds/Linux/Build and run (scripts only)")]
		private static void BuildScriptsAndRunLinuxPlayer() => BuildPlayerOnLinux(BuildTarget.StandaloneLinux64, BuildOptions.AutoRunPlayer | BuildOptions.Development | BuildOptions.BuildScriptsOnly);

		private static void BuildPlayerOnWindows(BuildTarget buildTarget, BuildOptions buildOptions)
		{
			string[] scenes = EditorBuildSettings.scenes.Select(it => it.path).ToArray();

			var buildPlayerOptions = new BuildPlayerOptions
			{
				scenes = scenes,
				locationPathName = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/NKM/NKM.exe",
				target = buildTarget,
				options = buildOptions
			};
			BuildPipeline.BuildPlayer(buildPlayerOptions);
		}
		private static void BuildPlayerOnLinux(BuildTarget buildTarget, BuildOptions buildOptions)
		{
			string[] scenes = EditorBuildSettings.scenes.Select(it => it.path).ToArray();

			var buildPlayerOptions = new BuildPlayerOptions
			{
				scenes = scenes,
				locationPathName = "Builds/Linux/Automatic/NKM",
				target = buildTarget,
				options = buildOptions
			};
			BuildPipeline.BuildPlayer(buildPlayerOptions);
		}
	}
}
