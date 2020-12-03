using System.Collections;
using System.Collections.Generic;
using Assets.AToonWorld.Scripts.Audio;
using Assets.AToonWorld.Scripts.Extensions;
using Assets.AToonWorld.Scripts.UI;
using Cysharp.Threading.Tasks;
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
            _endLevelMenuController = InGameUIController.PrefabInstance.GetComponent<EndLevelMenuController>();
        }

        // Show the end of level menu
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(UnityTag.Player))
                Victory().Forget();
        }

        private async UniTask Victory()
        {
            #if AnaliticsEnabled
                Events.AnaliticsEvents.LevelEnd.Invoke(new Analitic());
            #endif
            await this.PlaySound(SoundEffects.Victory);
            _endLevelTaken?.Invoke();
            _endLevelMenuController.ShowEndLevelMenu();
        }
    }
}
