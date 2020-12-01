using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.Audio
{
    public class AudioManager : MonoBehaviour
    {
        // Editor Fields        
        [SerializeField] private List<Sound> _soundtrack = new List<Sound>();
        [SerializeField] private bool _refresh = true;
        [SerializeField] private GameObject _soundEffectPrefab;



        // Private Fields                
        private bool _resfreshing;
        private MusicSourceHandler _musicSource;




        // Initialization
        private void Awake()
        {
            var musicAudioSource = GetComponent<AudioSource>();
            _musicSource = new MusicSourceHandler(musicAudioSource);
            Test().Forget();
        }

        private void SetupMusicSource()
        {
            _musicSource.MusicEnd += MusicSource_MusicEnd;
        }


        private async UniTask Test()
        {
            PlayNextMusic();
            await UniTask.Delay(2000);
            PlayNextMusic();
            await UniTask.Delay(2000);
            PlayNextMusic();
            await UniTask.Delay(2000);
            PlayNextMusic();
        }



        // Private Properties 
        private static string ResourcesPath => $"{Application.dataPath}/Resources/";
        private static string SoundTrackRelativePath => "Audio/Soundtrack/";
        private List<Sound> _resourcesSoundtrack = new List<Sound>();





        // Public Properties        
        public Sound CurrentMusic => _musicSource.CurrentMusic;




        // Public Methods
        public async UniTask LoadSoundtrack()
        {
            _resfreshing = true;

            var sounds = await UpdateSoundsAsync(SoundTrackRelativePath, _soundtrack);
            var removedSounds = from sound in _resourcesSoundtrack where !sounds.Any(s => AreClipsEquals(s.Clip, sound.Clip)) select sound;
            foreach (var sound in removedSounds)
                _soundtrack.Remove(sound);
            _resourcesSoundtrack = sounds;

            _resfreshing = false;
            _refresh = false;
        }


        public void PlayMusic(string musicName)
        {
            var music = SoundByName(musicName, _soundtrack);
            PlayMusic(music);
        }

        public void PlayMusic() => _musicSource.Play();

        public void PlayNextMusic()
        {
            var currentMusicIndex = _soundtrack.IndexOf(CurrentMusic ?? _soundtrack.Last());
            var next = currentMusicIndex + 1 < _soundtrack.Count ? _soundtrack[currentMusicIndex + 1] : _soundtrack?[0];
            PlayMusic(next);
        }

        public void PauseMusic() => _musicSource.Pause();

        public void StopMusic() => _musicSource.Stop();



        public void PlaySound(Sound sound)
        {
            var tmpGameObject = new GameObject();
            tmpGameObject.transform.parent = gameObject.transform;
            var audioSource = tmpGameObject.AddComponent<AudioSource>();
            SetupAudioSource(sound, audioSource);
            var clipDuration = sound.Clip.length;

            audioSource.Play();
            Destroy(tmpGameObject);
        }





        // Unity Events
        private void OnValidate()
        {
            if (!_refresh || _resfreshing)
                return;
            Debug.Log("Loading soundtracks...");            
            LoadSoundtrack().Forget();
        }


        
        
        // AudioManager events
        private void MusicSource_MusicEnd(Sound music)
        {

        }




        // Private Methods
        private void PlayMusic(Sound music)
        {
            if (music is null)
                return;

            _musicSource.Stop();
            _musicSource.Play(music);
        }        

        private void SetupAudioSource(Sound sound, AudioSource audioSource)
        {
            audioSource.clip = sound.Clip;
        }

        private async UniTask<List<Sound>> UpdateSoundsAsync(string relativePath, IList<Sound> toUpdateSounds)
        {
            var dirInfo = new DirectoryInfo(ResourcesPath + relativePath);
            var ret = new List<Sound> { };
            foreach (var file in dirInfo.GetFiles())
            {
                var sound = await LoadSoundFromFile(file, relativePath);
                if (sound != null)
                {
                    ret.Add(sound);
                    if (!toUpdateSounds.Any(s => AreClipsEquals(s.Clip, sound.Clip)))
                        toUpdateSounds.Add(sound);
                }
            }

            return ret;
        }

        private Sound SoundByName(string name, IEnumerable<Sound> sounds)
        {
            var possibleSounds = from sound in sounds
                                 where EqualsSoundName(name, sound)
                                 select sound;

            return possibleSounds.FirstOrDefault()
                ?? throw new InvalidOperationException($"Can't find a sound named {name}");
        }

        private async UniTask<Sound> LoadSoundFromFile(FileInfo file, string relativePath)
        {
            var soundName = file.Name;
            soundName = soundName.Substring(0, soundName.LastIndexOf('.'));
            var clip = await Resources.LoadAsync(relativePath + soundName) as AudioClip;
            return clip == null ? null : new Sound(soundName, clip);       
        }        

        private string SoundPoolKeyBySoundName(string name)
            => $"{nameof(AudioManager)}.Sfx_{name}_pool";

        private bool EqualsSoundName(string name, Sound clip)
            => name.Equals(clip.Name, StringComparison.InvariantCultureIgnoreCase);

        private bool AreClipsEquals(AudioClip clipA, AudioClip clipB)
        {
            var lengthA = clipA.samples + clipA.channels;
            var lengthB = clipB.samples + clipB.channels;

            if (lengthA != lengthB)
                return false;

            var dataA = new float[lengthA];
            clipA.GetData(dataA, 0);

            var dataB = new float[lengthB];
            clipB.GetData(dataB, 0);

            for (var i = 0; i < dataA.Length; i++)
                if (!Mathf.Approximately(dataA[i], dataB[i]))
                    return false;
            return true;
        }
    }
}
