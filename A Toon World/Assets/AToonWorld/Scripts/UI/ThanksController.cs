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
            PlayerPrefs.SetInt(UnityScenes.ScenesPath + SceneManager.GetActiveScene().name, 3);
            PlayerPrefs.SetInt(UnityScenes.ScenesPath2 + SceneManager.GetActiveScene().name, 3);

            InGameUIController.PrefabInstance.FadeTo(UnityScenes.LevelsMenu);
        }
    }
}
