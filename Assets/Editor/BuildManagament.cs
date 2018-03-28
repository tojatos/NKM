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
		static void BuildWindowsPlayer() => BuildPlayerOnWindows(BuildTarget.StandaloneWindows64, BuildOptions.Development);
		[MenuItem("Developement/Builds/Windows/Build and run")]
		static void BuildAndRunWindowsPlayer() => BuildPlayerOnWindows(BuildTarget.StandaloneWindows64, BuildOptions.AutoRunPlayer | BuildOptions.Development);
		[MenuItem("Developement/Builds/Windows/Build and run (scripts only)")]
		static void BuildScriptsAndRunWindowsPlayer() => BuildPlayerOnWindows(BuildTarget.StandaloneWindows64, BuildOptions.AutoRunPlayer | BuildOptions.Development | BuildOptions.BuildScriptsOnly);

		[MenuItem("Developement/Builds/Linux/Build")]
		static void BuildLinuxPlayer() => BuildPlayerOnLinux(BuildTarget.StandaloneLinux64, BuildOptions.Development);
		[MenuItem("Developement/Builds/Linux/Build and run")]
		static void BuildAndRunLinuxlayer() => BuildPlayerOnLinux(BuildTarget.StandaloneLinux64, BuildOptions.AutoRunPlayer | BuildOptions.Development);
		[MenuItem("Developement/Builds/Linux/Build and run (scripts only)")]
		static void BuildScriptsAndRunLinuxPlayer() => BuildPlayerOnLinux(BuildTarget.StandaloneLinux64, BuildOptions.AutoRunPlayer | BuildOptions.Development | BuildOptions.BuildScriptsOnly);

		private static void BuildPlayerOnWindows(BuildTarget buildTarget, BuildOptions buildOptions)
		{
			var scenes = EditorBuildSettings.scenes.Select(it => it.path).ToArray();

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
			var scenes = EditorBuildSettings.scenes.Select(it => it.path).ToArray();

			var buildPlayerOptions = new BuildPlayerOptions
			{
				scenes = scenes,
				locationPathName = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/NKM/NKM",
				target = buildTarget,
				options = buildOptions
			};
			BuildPipeline.BuildPlayer(buildPlayerOptions);
		}
	}
}
