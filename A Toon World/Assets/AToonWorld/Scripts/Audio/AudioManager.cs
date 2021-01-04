using Assets.AToonWorld.Scripts.Extensions;
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
    public class AudioManager : Singleton<AudioManager>
    {
        // Editor Fields                
        [Header("Soundtrack Settings")]
        [SerializeField] private List<Sound> _soundtrack = new List<Sound>();
        [SerializeField] private bool _refreshSoundtrack = false;        


        [Header("Sfx settings")]
        [SerializeField] private Transform _sfxTransform = null;
        [SerializeField] private GameObject _soundEffectPrefab = null;
        [SerializeField] private List<GameObject> _sfx = new List<GameObject>();
        [SerializeField] private bool _refreshSfx = false;



        // Private Fields    
        private AudioSourceHandler _musicSource;
        private bool _refreshingSoundtrack;
        private bool _refreshingSFx;
        [Range(0, 1)] private float _globalVolume = 1;





        // Initialization
        protected override void Awake()
        {
            if(Application.isPlaying)
                base.Awake();
            var musicAudioSource = GetComponent<AudioSource>();
            _musicSource = musicAudioSource != null ? new AudioSourceHandler(musicAudioSource) : null;
            SetupMusicSource();
        }

        private void Start()
        {          
        }

        private void SetupMusicSource()
        {
            if (_musicSource == null)
                return;
            _musicSource.MusicEnd += MusicSource_MusicEnd;
        }



        // Private Properties 
        private static string ResourcesPath => $"{Application.dataPath}/Resources/";
        private static string SoundTrackRelativePath => "Audio/Soundtrack/";
        private static string SfxRelativePath => "Audio/Sfx/";

        
        // Public Properties        

        public float GlobalVolume
        {
            get => _globalVolume;
            set
            {
                _globalVolume = value;
                _musicSource.MusicSource.volume = value;
            }
        }



        // Public Properties        
        public Sound CurrentMusic => _musicSource.CurrentMusic;




        // Public Methods
        public IEnumerable<SoundEffect> GetAllSfx()
            => from soundObject in _sfx select soundObject.GetComponent<SoundEffect>();

      
        

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

        public void PlayPreviousMusic()
        {
            var currentMusicIndex = _soundtrack.IndexOf(CurrentMusic ?? _soundtrack.Last());
            var previous = currentMusicIndex - 1 < 0 ? _soundtrack[_soundtrack.Count - 1] : _soundtrack[currentMusicIndex - 1];
            PlayMusic(previous);
        }

        public void PauseMusic() => _musicSource.Pause();
        public void StopMusic() => _musicSource.Stop();



        // Sfx
        public async UniTask PlaySound(SoundEffect soundEffectModel, Vector2 position)
        {
            if (soundEffectModel == null)
            {
                // to handle border case like while(true) PlaySound(sound, _) when sound is null without blocking main thread
                await this.NextFrame();
                return;
            }

            var newSound = Instantiate(soundEffectModel.gameObject);
            newSound.transform.position = position;
            var soundEffect = newSound.GetComponent<SoundEffect>();
            await PlaySound(soundEffect);
        }

        public UniTask PlaySound(string name, Transform transform)
        {
            //TODO add object pooling
            var soundEffectModel = _sfx
                .Where(sound => name.Equals(sound.name, StringComparison.InvariantCultureIgnoreCase))
                .FirstOrDefault();
            if (soundEffectModel == default)
                throw new InvalidOperationException($"The sound named {name} does not exist");

            return PlaySound(soundEffectModel.GetComponent<SoundEffect>(), transform);
        }

        public async UniTask PlaySound(SoundEffect soundEffectModel, Transform transform)
        {
            if (soundEffectModel == null)
            {
                // to handle border case like while(true) PlaySound(sound, _) when sound is null without blocking main thread
                await this.NextFrame();
                return;
            }

            var newSound = Instantiate(soundEffectModel.gameObject);
            newSound.transform.parent = transform;
            newSound.transform.position = transform.position;
            var soundEffect = newSound.GetComponent<SoundEffect>();
            await PlaySound(soundEffect);
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
        
        // Sounds loading
        private async UniTask LoadSoundtrack()
        {
            var resourcesMusicTasks = GetClips(SoundTrackRelativePath, (relPath, clip) => clip);
            foreach (var clipTask in resourcesMusicTasks)
                AddMusicToSoundtrack(await clipTask);

            var toRemoveMusics = from music in _soundtrack where music.Clip == default select music;
            foreach (var music in toRemoveMusics.ToList())
                _soundtrack.Remove(music);
        }


        private async UniTask LoadSfx()
        {
            #if UNITY_EDITOR
            var resourcesSfxTasks = GetClips(SfxRelativePath, (relPath, clip) => new { clip, SfxRelativePath = relPath == SfxRelativePath ? "" : relPath.Substring(SfxRelativePath.Length) });
            foreach (var soundEffectTask in resourcesSfxTasks)
            {
                var soundEffectInfo = await soundEffectTask;
                AddSoundEffectToSfx(soundEffectInfo.clip, soundEffectInfo.SfxRelativePath);
            }

            var toRemoveSfx = from soundEffectObj in _sfx
                              let soundEffect = soundEffectObj.GetComponent<SoundEffect>()
                              where soundEffect.Clip == null
                              select soundEffectObj;
            UniTaskCompletionSource<bool> tcs = new UniTaskCompletionSource<bool>();
            
                UnityEditor.EditorApplication.delayCall += () =>
                {
                    try
                    {
                        foreach (var toRemove in toRemoveSfx.ToList())
                        {
                            _sfx.Remove(toRemove);
                            DestroyImmediate(toRemove);
                        }
                    }
                    finally
                    {
                        tcs.TrySetResult(true);
                    }
                };

            await tcs.Task;
            #endif
        }




        // Soundtrack helpers
        private void AddMusicToSoundtrack(AudioClip clip)
        {
            if (clip != null && !_soundtrack.Any(s => AreClipsEquals(clip, s.Clip)))
                _soundtrack.Add(new Sound(clip.name, clip));
        }

        private void PlayMusic(Sound music)
        {
            if (music is null)
                return;

            _musicSource.Stop();
            _musicSource.Play(music);
        }




        //  Sfx Helpers
        private async UniTask PlaySound(SoundEffect instance)
        {
            instance.AudioSource.volume *= _globalVolume;
            await instance.Play();
            Destroy(instance.gameObject);
        }

        private void AddSoundEffectToSfx(AudioClip clip, string relativePath)
        {
            if (clip != null && !_sfx.Any(s => AreClipsEquals(clip, s.GetComponent<SoundEffect>().Clip)))
                _sfx.Add(BuildSoundEffect(clip, relativePath));
        }

        private GameObject BuildSoundEffect(AudioClip clip, string relativePath)
        {
            if (_sfxTransform == null)
                throw new InvalidOperationException($"{nameof(_sfxTransform)} must be assigned from the editor");

            var soundEffectModel = Instantiate(_soundEffectPrefab);
            soundEffectModel.transform.parent = _sfxTransform;
            soundEffectModel.name = relativePath + clip.name;
            var soundEffect = soundEffectModel.GetComponent<SoundEffect>();
            soundEffect.Clip = clip;
            soundEffect.RelativePath = relativePath;
            return soundEffectModel;
        }




        // Clips-Sounds helpers        
        private IEnumerable<UniTask<T>> GetClips<T>(string relativePath, Func<string, AudioClip, T> selectFunction)
        {            
            var dirInfo = new DirectoryInfo(ResourcesPath + relativePath);
            var ret = new List<Sound> { };
            var clipsInCurrentDirectory = dirInfo.GetFiles().Select(async file => selectFunction(relativePath, await LoadClipFromFile(file, relativePath)));
            var clipsInOtherDirectories = dirInfo.GetDirectories().SelectMany(folder =>  GetClips(relativePath + $"{folder.Name}/", selectFunction));
            return clipsInCurrentDirectory.Union(clipsInOtherDirectories);
        }

        private Sound SoundByName(string name, IEnumerable<Sound> sounds)
            => sounds.FirstOrDefault(sound => name.Equals(sound.Name, StringComparison.InvariantCultureIgnoreCase))
            ?? throw new InvalidOperationException($"Can't find a sound named {name}");

        private async UniTask<AudioClip> LoadClipFromFile(FileInfo file, string relativePath)
        {
            var soundName = file.Name;
            soundName = soundName.Substring(0, soundName.LastIndexOf('.'));
            var clip = await Resources.LoadAsync(relativePath + soundName) as AudioClip;
            return clip;
        }               

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
