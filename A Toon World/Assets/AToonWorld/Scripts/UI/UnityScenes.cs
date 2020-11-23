using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.UI
{
    public static class UnityScenes
    {
        //TODO: Only keep one scene path
        public const string ScenesPath = "Scenes/";
        public const string ScenesPath2 = "AToonWorld/Scenes/Tutorial/";
        
        #region StandaloneScenes

        public const string MainMenu = "Scenes/MainMenu";
        public const string LevelsMenu = "Scenes/LevelsMenu";

        #endregion

        #region Levels

        // List of all levels with paths, valid index start from 1
        public static readonly string[] Levels = {
            "We count levels from number one",
            "AToonWorld/Scenes/Tutorial/build_tutorial_000",
            "Scenes/WallJumpScene",
            "Scenes/BuildLineScene",
            "Scenes/climb-color",
            "Scenes/EnemyJumper",
            "Scenes/PathFinding",
            "Scenes/GameDirectorExample",
        };

        #endregion
    }
}
