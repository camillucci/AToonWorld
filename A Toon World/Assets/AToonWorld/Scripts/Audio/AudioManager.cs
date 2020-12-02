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
    [ExecuteInEditMode]
    public class AudioManager : MonoBehaviour
    {
        // Editor Fields                
        [Header("Soundtrack Settings")]
        [SerializeField] private List<Sound> _soundtrack = new List<Sound>();
        [SerializeField] private bool _refreshSoundtrack;        


        [Header("Sfx settings")]
        [SerializeField] private Transform _sfxTransform;
        [SerializeField] private Transform _playingSfxTransform;
        [SerializeField] private GameObject _soundEffectPrefab;
        [SerializeField] private List<GameObject> _sfx = new List<GameObject>();
        [SerializeField] private bool _refreshSfx;



        // Private Fields    
        private AudioSourceHandler _musicSource;
        private List<Sound> _resourcesSoundtrack = new List<Sound>();
        private bool _refreshingSoundtrack;
        private List<GameObject> _resourcesSfx = new List<GameObject>();
        private bool _refreshingSFx;





        // Initialization
        private void Awake()
        {
            var musicAudioSource = GetComponent<AudioSource>();
            _musicSource = new AudioSourceHandler(musicAudioSource);
        }

        private void Start()
        {
            if(Application.isPlaying)
                Test().Forget();            
        }

        private void SetupMusicSource()
        {
            _musicSource.MusicEnd += MusicSource_MusicEnd;
        }


        private async UniTask Test()
        {            
            while(true)
            {
                Debug.Log("Playing");
                await PlaySound(SoundEffects.Pop);
                await UniTask.Delay(2000);
                Debug.Log(" Stop Playing");
            }
        }



        // Private Properties 
        private static string ResourcesPath => $"{Application.dataPath}/Resources/";
        private static string SoundTrackRelativePath => "Audio/Soundtrack/";
        private static string SoundEffectsRelativePath => "Audio/Sfx/";





        // Public Properties        
        public Sound CurrentMusic => _musicSource.CurrentMusic;




        // Public Methods
        public async UniTask LoadSoundtrack()
        {
            var clipsTasks = GetClips(SoundTrackRelativePath);
            foreach(var clipTask in clipsTasks)
            {
                var clip = await clipTask;
                if(clip != null)
                {                    
                    if (!_soundtrack.Any(s => AreClipsEquals(clip, s.Clip)))
                        _soundtrack.Add(new Sound(clip.name,clip));
                    var toRemoveSounds = (from sound in _resourcesSoundtrack where AreClipsEquals(sound.Clip, clip) select sound).ToList();
                    foreach (var sound in toRemoveSounds)
                        _resourcesSoundtrack.Remove(sound);
                }
            }
        }

        public async UniTask LoadSfx()
        {
            _refreshingSFx = true;
            var clipsTasks = GetClips(SoundEffectsRelativePath);
            foreach(var clipTask in clipsTasks)
            {
                var clip = await clipTask;
                if(clip != null)
                {
                    if (!_sfx.Any(s => AreClipsEquals(clip, s.GetComponent<SoundEffect>().Clip)))
                        _sfx.Add(BuildSoundEffect(clip));
                    var toRemoveSounds = (from sound in _resourcesSfx where AreClipsEquals(sound.GetComponent<SoundEffect>().Clip, clip) select sound).ToList();
                    foreach (var sound in toRemoveSounds)
                        _resourcesSfx.Remove(sound);
                }
            }
        }

        

        // Music
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



        // Sfx
        public async UniTask PlaySound(string name)
        {
            //TODO add object pooling
            var soundEffectModel = (from sound in _sfx
                               where name.Equals(sound.name, StringComparison.InvariantCultureIgnoreCase)
                               select sound).FirstOrDefault();
            if (soundEffectModel == default)
                throw new InvalidOperationException($"The sound named {name} does not exist");

            var newSound = Instantiate(soundEffectModel);
            newSound.transform.parent = _playingSfxTransform;
            var soundEffect = newSound.GetComponent<SoundEffect>();
            await soundEffect.Play();
            Destroy(newSound);
        }





        // Unity Events
        private void OnValidate()
        {
            OnValidateAsync().Forget();
        }

        private async UniTask OnValidateAsync()
        {
            try
            {
                if (_refreshSoundtrack && !_refreshingSoundtrack)
                {
                    Debug.Log("Loading soundtracks...");
                    _refreshingSoundtrack = true;
                    await LoadSoundtrack();
                    Debug.Log("Soundtrack loaded");
                }
                if (_refreshSfx && !_refreshingSFx)
                {
                    Debug.Log("Loading Sfx");
                    _refreshingSFx = true;
                    await LoadSfx();
                    Debug.Log("Sfx loaded");
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            finally
            {
                _refreshSoundtrack = false;
                _refreshSfx = false;
                _refreshSfx = _refreshSoundtrack = false;
                _refreshingSoundtrack = _refreshingSFx = false;
            }
        }


        
        
        // AudioManager events
        private void MusicSource_MusicEnd(Sound music)
        {
            PlayNextMusic();
        }




        // Private Methods
        private GameObject BuildSoundEffect(AudioClip clip)
        {
            if (_sfxTransform == null)
                throw new InvalidOperationException($"{nameof(_sfxTransform)} must be assigned from the editor");

            var soundEffectModel = Instantiate(_soundEffectPrefab);
            soundEffectModel.transform.parent = _sfxTransform;
            soundEffectModel.name = clip.name;
            var soundEffect = soundEffectModel.GetComponent<SoundEffect>();
            soundEffect.Clip = clip;            
            return soundEffectModel;
        }

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

        private IEnumerable<UniTask<AudioClip>> GetClips(string relativePath)
        {
            var dirInfo = new DirectoryInfo(ResourcesPath + relativePath);
            var ret = new List<Sound> { };
            return from file in dirInfo.GetFiles() select LoadClipFromFile(file, relativePath);
        }

        private Sound SoundByName(string name, IEnumerable<Sound> sounds)
        {
            var possibleSounds = from sound in sounds
                                 where EqualsSoundName(name, sound)
                                 select sound;

            return possibleSounds.FirstOrDefault()
                ?? throw new InvalidOperationException($"Can't find a sound named {name}");
        }

        private async UniTask<AudioClip> LoadClipFromFile(FileInfo file, string relativePath)
        {
            var soundName = file.Name;
            soundName = soundName.Substring(0, soundName.LastIndexOf('.'));
            var clip = await Resources.LoadAsync(relativePath + soundName) as AudioClip;
            return clip;
        }        

        private string SoundPoolKeyBySoundName(string name)
            => $"{nameof(AudioManager)}.Sfx_{name}_pool";

        private bool EqualsSoundName(string name, Sound clip)
            => name.Equals(clip.Name, StringComparison.InvariantCultureIgnoreCase);

        private bool AreClipsEquals(AudioClip clipA, AudioClip clipB)
        {
            if (clipA == null || clipB == null)
                return false;

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
