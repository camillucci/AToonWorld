using Assets.AToonWorld.Scripts.Extensions;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.Generics
{
    public class AutomaticDisable : MonoBehaviour
    {
        [SerializeField] private int _startDelay;
        [SerializeField] private int _timeActiveSeconds;
        [SerializeField] private float _fadeOutSpeed;
        [SerializeField] private int _timeInactiveSeconds;        
        [SerializeField] private float _fadeInSpeed;

                
        // Private Fields
        private bool _isDestroyed;
        private SpriteRenderer _spriteRenderer;


        // Initialization
        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }


        private void Start()
        {
            HideAndAppear().WithCancellation(this.GetCancellationTokenOnDestroy()).Forget();
        }

        

        // Private Methods
        private async UniTask HideAndAppear()
        {
            await UniTask.Delay(_startDelay * 1000);
            while(true)
            {
                await UniTask.Delay(_timeActiveSeconds * 1000);
                await _spriteRenderer.FadeTo(0, _fadeOutSpeed, false);
                gameObject.SetActive(false);
                await UniTask.Delay(_timeInactiveSeconds * 1000);
                gameObject.SetActive(true);
                await _spriteRenderer.FadeTo(1, _fadeInSpeed, false);                
            }
        }
    }
}
