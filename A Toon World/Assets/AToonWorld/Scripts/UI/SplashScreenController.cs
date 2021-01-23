using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.UI
{
    public class SplashScreenController : MonoBehaviour
    {
        private void Start()
        {
            InGameUIController.PrefabInstance.FadeInMenu(1f);
            UniTask.Delay(2000).ContinueWith(() =>
                InGameUIController.PrefabInstance.FadeTo(UnityScenes.MainMenu, 1f)).Forget();
        }
    }
}
