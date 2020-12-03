using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.Audio
{
    public class AudioSourceHandler
    {
        // Private Fields
        private CancellationTokenSource _cts = new CancellationTokenSource();


        // Initialization
        public AudioSourceHandler(AudioSource audioSource)
        {
            MusicSource = audioSource;
            MusicSource?.Stop();
        }


        // Public Properties
        public Sound CurrentMusic { get; private set; }
        public AudioSource MusicSource { get; private set; }


        // Public Events
        public event Action<Sound> MusicPlayed;
        public event Action<Sound> MusicPaused;
        public event Action<Sound> MusicStopped;
        public event Action<Sound> MusicEnd;


        // Public Methods
        public void Play(Sound music)
        {
            CurrentMusic = music;
            MusicSource.Stop();
            MusicSource.clip = music.Clip;
            Play();
        }

        public void Play()
        {
            if (CurrentMusic != null && Mathf.Approximately(MusicSource.time, CurrentMusic.Clip.length))
                return;
            MusicSource.Play();
            WaitForMusicToEnd().Forget();
            MusicPlayed?.Invoke(CurrentMusic);
        }

        public void Pause()
        {
            if (!MusicSource.isPlaying)
                return;

            CancelWaitForMusicToEnd();
            MusicSource.Pause();
            MusicPaused?.Invoke(CurrentMusic);
        }

        public void Stop()
        {
            if (!MusicSource.isPlaying)
                return;

            CancelWaitForMusicToEnd();
            MusicSource.Stop();
            MusicSource.time = 0;
            MusicStopped?.Invoke(CurrentMusic);
        }


        // Private Methods
        private void CancelWaitForMusicToEnd()
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = new CancellationTokenSource();
        }

        public async UniTask WaitForMusicToEnd()
        {
            var waitTime = (int) ( MusicSource.clip.length - MusicSource.time) + 1;
            var isCanceled = await UniTask.Delay(waitTime * 1000, cancellationToken:_cts.Token).SuppressCancellationThrow();
            if (!isCanceled)
                MusicEnd?.Invoke(CurrentMusic);
        }
    }
}
