using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.UI
{
    public static class UnityScenes
    {
        public const string LevelsPath = "AToonWorld/Scenes/Tutorial/";
        public const string MenusPath = "AToonWorld/Scenes/Menus/";
        public const string AchievementsPath = "/Achievements";
        public const string CollectiblesPath = "/Collectibles";
        
        #region StandaloneScenes

        public const string MainMenu = "AToonWorld/Scenes/Menus/MainMenu";
        public const string LevelsMenu = "AToonWorld/Scenes/Menus/LevelsMenu";

        #endregion

        #region Levels

        // List of all levels with paths, valid index start from 1
        public static readonly string[] Levels = {
            "We count levels from number one",
            "AToonWorld/Scenes/Tutorial/build_tutorial_000",
            "AToonWorld/Scenes/Tutorial/damage_tutorial_002",
            "AToonWorld/Scenes/Tutorial/climb_tutorial_001",
            "AToonWorld/Scenes/Menus/ThanksForPlaying",
        };

        #endregion
    }
}
