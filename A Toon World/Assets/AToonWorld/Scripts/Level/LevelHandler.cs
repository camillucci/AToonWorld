using Assets.AToonWorld.Scripts.Camera;
using Assets.AToonWorld.Scripts.Player;
using Assets.AToonWorld.Scripts.UI;
using Assets.AToonWorld.Scripts.Utils;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.Level
{
    public class LevelHandler : MonoBehaviour
    {
        // Editor Fields
        [SerializeField] private int _maxDeathsForAchievement = 5;
        [SerializeField] private InkPaletteSO _inkPalette = null;


        // Editor Fields
        private CheckPointsManager _checkPointsManager;
        public CollectiblesManager _collectiblesManager { get; private set; }
        public TimeManager _timeManager { get; private set; }
        private PlayerController _playerController;
        private PlayerMovementController _playerMovementController;
        private CameraMovementController _cameraMovementController;
        private DeathObserver _deathObserver;
        private MapBorders _mapBorders;
        public int _deathCounter { get; private set; }

        //Reset tables, we use IDs to keep track of duplicates
        private Dictionary<int, GameObject> _enabledObjectsSinceCheckpoint;
        private Dictionary<int, GameObject> _disabledObjectsSinceCheckpoint;
        private Dictionary<PlayerInkController.InkType, float> _savedInkCapacity;
        private PlayerInkController.InkType _savedSelectedInk;

        // Public Properties
        public bool RespawningPlayer { get; private set; }



        // Initialization
        private void Awake()
        {
            _checkPointsManager = GetComponentInChildren<CheckPointsManager>();
            _collectiblesManager = GetComponentInChildren<CollectiblesManager>();
            _timeManager = GetComponentInChildren<TimeManager>();
            _enabledObjectsSinceCheckpoint = new Dictionary<int, GameObject>();
            _disabledObjectsSinceCheckpoint = new Dictionary<int, GameObject>();
            _savedInkCapacity = new Dictionary<PlayerInkController.InkType, float>();
            
            Events.PlayerEvents.Death.AddListener(() => OnPlayerDead().WithCancellation(this.GetCancellationTokenOnDestroy()).Forget());
            Events.LevelEvents.CheckpointReached.AddListener(checkpointNumber => CheckpointReached());
            Events.LevelEvents.SplineDrawn.AddListener(DrawingCreated);
            Events.LevelEvents.EnemyKilled.AddListener(EnemyKilled);
            Events.InterfaceEvents.CursorChangeRequest.Invoke(CursorController.CursorType.Game);
            _deathCounter = 0;
            Time.timeScale = 1f;

            #if AnaliticsEnabled
                Events.AnaliticsEvents.LevelStart.Invoke(new Analitic());
            #endif
        }

        private void Start()
        {
            _playerController = FindObjectOfType<PlayerController>();
            _playerMovementController = FindObjectOfType<PlayerMovementController>();
            _cameraMovementController = FindObjectOfType<CameraMovementController>();
            _deathObserver = FindObjectOfType<DeathObserver>();
            _mapBorders = FindObjectOfType<MapBorders>();
            InGameUIController.PrefabInstance.FadeInLevel();
        }


        // Public Methods
        public async UniTask SpawnFromLastCheckpoint()
        {
            // Disable player
            RespawningPlayer = true;
            var lastCheckPoint = _checkPointsManager.LastCheckPoint;
            _playerController.DisablePlayer();

            await UniTask.Delay(1000); // Wait for death animation to be played

            // Respawn: reset stuff done since last checkpoint and move player to it
            _collectiblesManager.OnPlayerRespawn();
            lastCheckPoint.OnPlayerRespawnStart();     
            ResetLevelStateFromCheckpoint(); 
            InGameUIController.PrefabInstance.FadeOutAndIn(2f, 500, 2f).Forget();
            Events.PlayerEvents.PlayerRespawning.Invoke();
            await _playerController.MoveToPosition(lastCheckPoint.Position, _cameraMovementController.CameraSpeed);

            // Re-enable player
            _playerController.EnablePlayer();
            lastCheckPoint.OnPlayerRespawnEnd();  
            _playerMovementController.AnimatorController.SetBool(PlayerAnimatorParameters.Spawning, false);
            RespawningPlayer = false;
            Events.PlayerEvents.PlayerRespawned.Invoke();
        }

        // Private Methods
        void ResetLevelStateFromCheckpoint()
        {
            //Disables all the drawings
            foreach(GameObject enabledItem in _enabledObjectsSinceCheckpoint.Values)
                enabledItem.gameObject.SetActive(false);

            //Enables all killed enemies
            foreach(GameObject disabledItem in _disabledObjectsSinceCheckpoint.Values)
                disabledItem.SetActive(true);

            //Restores ink state
            _inkPalette?.InkPalette.ForEach(inkHandler => {
                if(inkHandler is ScriptableExpendableInkHandler expendableInkHandler &&
                    _savedInkCapacity.ContainsKey(expendableInkHandler.InkType))
                    expendableInkHandler.Expendable.SetCapacity(_savedInkCapacity[expendableInkHandler.InkType]);
            });

            if(_inkPalette != null)
                Events.InterfaceEvents.InkSelectionRequested.Invoke(_savedSelectedInk);

            //Clears the tracking list
            _enabledObjectsSinceCheckpoint.Clear();
            _disabledObjectsSinceCheckpoint.Clear();
        }
      

        // Unity events
        private void Update()
        {
            if (InputUtils.KillPlayer && !RespawningPlayer)
                OnPlayerDead().WithCancellation(this.GetCancellationTokenOnDestroy()).Forget();
        }


        // Level Events
        private async UniTask OnPlayerDead()
        {
            if (!_playerController.IsImmortal)
            {
                _playerController.IsImmortal = true;
                await SpawnFromLastCheckpoint();
                _deathObserver.ResetStatus();
                _playerController.IsImmortal = false;
                _deathCounter += 1;
            }
        }
        private void CheckpointReached()
        {
            _enabledObjectsSinceCheckpoint.Clear();

            _inkPalette?.InkPalette.ForEach(inkHandler => {
                if(inkHandler is ScriptableExpendableInkHandler expendableInkHandler)
                    _savedInkCapacity[expendableInkHandler.InkType] = expendableInkHandler.CurrentCapacity;
            });

            if(_inkPalette != null)
                _savedSelectedInk = _inkPalette.SelectedInk;
        }

        private void DrawingCreated(DrawSplineController splineController)
        {
            if(! _enabledObjectsSinceCheckpoint.ContainsKey(splineController.gameObject.GetInstanceID()))
            {
                _enabledObjectsSinceCheckpoint.Add(splineController.gameObject.GetInstanceID(), splineController.gameObject);
            }
        }

        private void EnemyKilled(GameObject enemy)
        {
            _disabledObjectsSinceCheckpoint.Add(enemy.gameObject.GetInstanceID(), enemy);
        }

        public bool GotDeathsAchievement => _deathCounter <= _maxDeathsForAchievement;
    }
}
