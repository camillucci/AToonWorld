using System.Collections;
using System.Collections.Generic;
using Assets.AToonWorld.Scripts.UI;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.AToonWorld.Scripts.Level
{
    public class EndLevel : MonoBehaviour
    {
        private EndLevelMenuController _endLevelMenuController;
        [SerializeField] private UnityEvent _endLevelTaken = null;

        private void Start()
        {
            _endLevelMenuController = FindObjectOfType<EndLevelMenuController>();
        }

        // Show the end of level menu
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(UnityTag.Player))
            {
                #if AnaliticsEnabled
                    Events.AnaliticsEvents.LevelEnd.Invoke(new Analitic());
                #endif
                _endLevelTaken?.Invoke();
                _endLevelMenuController.ShowEndLevelMenu();
            }
        }
    }
}
