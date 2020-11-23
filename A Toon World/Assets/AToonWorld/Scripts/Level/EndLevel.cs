using System.Collections;
using System.Collections.Generic;
using Assets.AToonWorld.Scripts;
using Assets.AToonWorld.Scripts.UI;
using Assets.AToonWorld.Scripts.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.AToonWorld.Scripts.Level
{
    public class EndLevel : MonoBehaviour
    {
        [SerializeField] private SceneFaderController _sceneFaderController = null;
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(UnityTag.Player))
            {
                // Save player progresses
                PlayerPrefs.SetInt(UnityScenes.ScenesPath + SceneManager.GetActiveScene().name, 2);
                PlayerPrefs.SetInt(UnityScenes.ScenesPath2 + SceneManager.GetActiveScene().name, 2);

                // Deactivate all inks in the scene
                ObjectPoolingManager<PlayerInkController.InkType>.Instance.DeactivateAllObjects();
                
                // Return to the LevelsMenu scene
                _sceneFaderController.FadeTo(UnityScenes.LevelsMenu);
            }
        }
    }
}
