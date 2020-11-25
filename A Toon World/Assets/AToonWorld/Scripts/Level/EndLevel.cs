using System.Collections;
using System.Collections.Generic;
using Assets.AToonWorld.Scripts.UI;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.Level
{
    public class EndLevel : MonoBehaviour
    {
        [SerializeField] private EndLevelMenuController _endLevelMenuController = null;

        // Show the end of level menu
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(UnityTag.Player))
            {
                _endLevelMenuController.gameObject.SetActive(true);
            }
        }
    }
}
