using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.AToonWorld.Scripts.UI
{
    public class ThanksController : MonoBehaviour
    {
        private void Start()
        {
            InGameUIController.PrefabInstance.FadeInMenu();
        }

    // Return to the LevelsMenu scene
        public void BackButton()
        {
            PlayerPrefs.SetInt(UnityScenes.MenusPath + SceneManager.GetActiveScene().name + UnityScenes.AchievementsPath, 3);
            for (int i = 0; i < UnityScenes.AchievementPaths.Length; i++)
            {
                PlayerPrefs.SetInt(UnityScenes.MenusPath + SceneManager.GetActiveScene().name + UnityScenes.AchievementPaths[i], 1);
            }

            InGameUIController.PrefabInstance.FadeTo(UnityScenes.LevelsMenu);
        }
    }
}
