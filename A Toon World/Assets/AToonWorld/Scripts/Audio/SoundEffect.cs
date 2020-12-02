using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.Audio
{
    public class SoundEffect :MonoBehaviour
    {
        [SerializeField] private AudioClip _clip;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private bool _testPlay;


        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        // Public Properties
        public AudioClip Clip { get => _clip; set => _clip = value; }
        public AudioSource AudioSource => _audioSource;



        // Public Methods
        public UniTask Play()
        {
            _audioSource.Stop();
            _audioSource.clip = Clip;            
            _audioSource.Play();
            return UniTask.Delay((int)(Clip.length + 1) * 1000);
        }

        
        
        // Unity Events
        private void OnValidate()
        {
            OnValidateAsync().Forget();
        }

        private async UniTask OnValidateAsync()
        {
            if(_testPlay)
            {
                _audioSource = GetComponent<AudioSource>();
                await Play();
                _testPlay = false;
            }
        }
    }
}
