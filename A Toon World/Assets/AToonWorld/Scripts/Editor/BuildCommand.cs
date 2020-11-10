using UnityEditor;
using System.Linq;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor.Build.Reporting;

static class BuildCommand
{
    static string[] GetBuildScenes()
	{
		List<string> names = new List<string>();

		foreach (EditorBuildSettingsScene e in EditorBuildSettings.scenes) {
			if (e == null) {
				continue;
			}

			if (e.enabled) {
				names.Add(e.path);
			}
		}

		return names.ToArray();
	}

    static void PerformBuild ()
    {
        var buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = GetBuildScenes();
        buildPlayerOptions.locationPathName = "Build";
        buildPlayerOptions.target = BuildTarget.WebGL;
        buildPlayerOptions.options = BuildOptions.None;
        PlayerSettings.WebGL.analyzeBuildSize = false;
        PlayerSettings.WebGL.memorySize = 256;
        PlayerSettings.WebGL.dataCaching = true;
        PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Gzip;
        PlayerSettings.runInBackground = true;
        PlayerSettings.defaultScreenHeight = 480;
        PlayerSettings.defaultScreenWidth = 640;
        PlayerSettings.WebGL.linkerTarget = WebGLLinkerTarget.Wasm;
        PlayerSettings.WebGL.debugSymbols = true;
        PlayerSettings.WebGL.exceptionSupport = WebGLExceptionSupport.FullWithStacktrace;
        EditorUserBuildSettings.development = false;

        var buildReport = BuildPipeline.BuildPlayer(buildPlayerOptions);
        if (buildReport.summary.result == BuildResult.Succeeded)
            EditorApplication.Exit(0);
        else
            EditorApplication.Exit(1);
    }
}