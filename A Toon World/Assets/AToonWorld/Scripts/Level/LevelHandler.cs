using Assets.AToonWorld.Scripts.Camera;
using Assets.AToonWorld.Scripts.Player;
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
        [SerializeField] private float _respawnSpeed;
        [SerializeField] private int _maxDeathsForAchievement = 5;


        // Editor Fields
        private CheckPointsManager _checkPointsManager;
        public CollectiblesManager _collectiblesManager { get; private set; }
        public TimeManager _timeManager { get; private set; }
        private PlayerController _playerController;
        private CameraMovementController _cameraMovementController;
        private DeathObserver _deathObserver;
        private MapBorders _mapBorders;
        public int _deathCounter { get; private set; }

        //Reset tables, we use IDs to keep track of duplicates
        private Dictionary<int, GameObject> _enabledObjectsSinceCheckpoint;
        private Dictionary<int, GameObject> _disabledObjectsSinceCheckpoint;


        // Public Properties
        public bool RespawningPlayer { get; private set; }



        // Initialization
        private void Awake()
        {
            _checkPointsManager = GetComponentInChildren<CheckPointsManager>();
            _collectiblesManager = GetComponentInChildren<CollectiblesManager>();
            _timeManager = GetComponentInChildren<TimeManager>();
            _playerController = FindObjectOfType<PlayerController>();
            _cameraMovementController = FindObjectOfType<CameraMovementController>();
            _deathObserver = FindObjectOfType<DeathObserver>();
            _mapBorders = FindObjectOfType<MapBorders>();
            _enabledObjectsSinceCheckpoint = new Dictionary<int, GameObject>();
            _disabledObjectsSinceCheckpoint = new Dictionary<int, GameObject>();
            Events.PlayerEvents.Death.AddListener(() => OnPlayerDead().WithCancellation(this.GetCancellationTokenOnDestroy()).Forget());
            Events.LevelEvents.CheckpointReached.AddListener(CheckpointReached);
            Events.LevelEvents.SplineDrawn.AddListener(DrawingCreated);
            Events.LevelEvents.EnemyKilled.AddListener(EnemyKilled);
            Events.InterfaceEvents.CursorChangeRequest.Invoke(CursorController.CursorType.Game);
            _deathCounter = 0;
            Time.timeScale = 1f;
        }      


        // Public Methods
        public async UniTask SpawnFromLastCheckpoint()
        {
            RespawningPlayer = true;

            var lastCheckPoint = _checkPointsManager.LastCheckPoint;
            _playerController.DisablePlayer();
            _collectiblesManager.OnPlayerRespawn();
            lastCheckPoint.OnPlayerRespawnStart();     
            ResetLevelStateFromCheckpoint(); 
            await _playerController.MoveToPosition(lastCheckPoint.Position, _cameraMovementController.CameraSpeed);
            _playerController.EnablePlayer();
            lastCheckPoint.OnPlayerRespawnEnd();  
            RespawningPlayer = false;
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
