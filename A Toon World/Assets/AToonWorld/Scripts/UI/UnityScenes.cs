using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.UI
{
    public static class UnityScenes
    {
        public const string ScenesPath = "Scenes/";
        public const string MainMenu = "Scenes/MainMenu";
        public const string LevelsMenu = "Scenes/LevelsMenu";
        public static readonly string[] Levels = {
            "We count levels from number one",
            "Scenes/WallJumpScene",
            "Scenes/BuildLineScene",
            "Scenes/climb-color",
            "Scenes/EnemyJumper",
        };
    }
}
