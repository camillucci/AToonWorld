using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.AToonWorld.Scripts.UI
{
    public class SceneFaderController : MonoBehaviour
    {
        [SerializeField] private Image _image = null;
        [SerializeField] private float _fadingSpeed = 1f;
        [SerializeField] private AnimationCurve _animationCurve = null;

        void Start()
        {
            FadeIn();
        }

        public void FadeTo(string scene)
        {
            FadeOut(scene);
        }

        // At the beginning of a scene do a fade in lasting 1/_fadingSpeed seconds
        private async void FadeIn()
        {
            float time = 1f;
            while(time > 0f)
            {
                time -= Time.unscaledDeltaTime * _fadingSpeed;
                float alpha = _animationCurve.Evaluate(time);
                _image.color = new Color(0f, 0f, 0f, alpha);
                await UniTask.WaitForEndOfFrame();
            }
        }

        // At the end of a scene do a fade out lasting 1/_fadingSpeed seconds
        private async void FadeOut(string scene)
        {
            float time = 0f;
            while(time < 1f)
            {
                time += Time.unscaledDeltaTime * _fadingSpeed;
                float alpha = _animationCurve.Evaluate(time);
                _image.color = new Color(0f, 0f, 0f, alpha);
                await UniTask.WaitForEndOfFrame();
            }

            SceneManager.LoadScene(scene);
        }
    }
}
