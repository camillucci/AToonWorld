using System.Collections;
using System.Collections.Generic;
using Assets.AToonWorld.Scripts.Extensions;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.AToonWorld.Scripts.UI
{
    public class SceneFaderController : MonoBehaviour
    {
        [SerializeField] private Image _image = null;
        [SerializeField] private AnimationCurve _animationCurve = null;

        // At the beginning of a scene do a fade in lasting 1 / fadingSpeed seconds
        public async UniTask FadeIn(float fadingSpeed)
        {
            float time = 1f;
            while(time > 0f)
            {
                time -= Time.unscaledDeltaTime * fadingSpeed;
                float alpha = _animationCurve.Evaluate(time);
                _image.color = new Color(0f, 0f, 0f, alpha);
                await this.WaitForEndOfFrame();
            }
        }

        // At the beginning of a scene do a fade in lasting 1 / fadingSpeed seconds
        public async UniTask FadeOut(float fadingSpeed)
        {
            float time = 0f;
            while(time < 1f)
            {
                time += Time.unscaledDeltaTime * fadingSpeed;
                float alpha = _animationCurve.Evaluate(time);
                _image.color = new Color(0f, 0f, 0f, alpha);
                await UniTask.WaitForEndOfFrame(cancellationToken: this.GetCancellationTokenOnDestroy());
            }
        }

        // At the end of a scene do a fade out lasting 1 / fadingSpeed seconds
        public async UniTask FadeTo(string scene, float fadingSpeed)
        {
            // Start scene fading
            float time = 0f;
            while(time < 1f)
            {
                time += Time.unscaledDeltaTime * fadingSpeed;
                float alpha = _animationCurve.Evaluate(time);
                _image.color = new Color(0f, 0f, 0f, alpha);
                await UniTask.WaitForEndOfFrame();
            }

            // Start scene change, change it only when ready
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(scene);
            asyncOperation.allowSceneActivation = false;
            while(asyncOperation.progress < 0.89f) {
                await UniTask.WaitForEndOfFrame();
            }
            asyncOperation.allowSceneActivation = true;
        }

        // When exiting the game do a fade out lasting 1 / fadingSpeed seconds
        public async UniTask FadeExit(float fadingSpeed)
        {
            float time = 0f;
            while(time < 1f)
            {
                time += Time.unscaledDeltaTime * fadingSpeed;
                float alpha = _animationCurve.Evaluate(time);
                _image.color = new Color(0f, 0f, 0f, alpha);
                await UniTask.WaitForEndOfFrame();
            }

            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }
}
