using System;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;

namespace Editor
{
	[UsedImplicitly]
	public class BuildManagament
	{
		[MenuItem("Developement/Builds/Build Linux player")]
		static void BuildLinuxPlayer() => BuildPlayer(BuildTarget.StandaloneLinux64, BuildOptions.Development);
		[MenuItem("Developement/Builds/Build Windows player")]
		static void BuildWindowsPlayer() => BuildPlayer(BuildTarget.StandaloneWindows64, BuildOptions.Development);
		[MenuItem("Developement/Builds/Build and run Windows player")]
		static void BuildAndRunPlayer() => BuildPlayer(BuildTarget.StandaloneWindows64, BuildOptions.AutoRunPlayer | BuildOptions.Development);
		[MenuItem("Developement/Builds/Build and run Windows player (scripts only)")]
		static void BuildScriptsAndRunPlayer() => BuildPlayer(BuildTarget.StandaloneWindows64, BuildOptions.AutoRunPlayer | BuildOptions.Development | BuildOptions.BuildScriptsOnly);


		private static void BuildPlayer(BuildTarget buildTarget, BuildOptions buildOptions)
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
	}
}
