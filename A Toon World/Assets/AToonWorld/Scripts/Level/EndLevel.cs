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
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(UnityTag.Player))
            {
                PlayerPrefs.SetInt(UnityScenes.ScenesPath + SceneManager.GetActiveScene().name, 2);
                SceneManager.LoadScene(UnityScenes.LevelsMenu);
            }
        }
    }
}
