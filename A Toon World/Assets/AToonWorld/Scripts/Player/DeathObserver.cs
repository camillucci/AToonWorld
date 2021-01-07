using Assets.AToonWorld.Scripts.Audio;
using Assets.AToonWorld.Scripts.Extensions;
using Assets.AToonWorld.Scripts.Level;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Camera = UnityEngine.Camera;

namespace Assets.AToonWorld.Scripts.Player
{
    public class DeathObserver : MonoBehaviour
    {
        // Editor Fields
        [SerializeField] private GameObject _tombstonePrefab = null;

        // Private Fields
        private PlayerMovementController _playerMovementController;
        private Transform _playerTransform;
        private Vector3 _previousGroundedPosition;
        private MapBorders _mapBorders;
        private bool _wasInTheAir;
        private GameObject _tombstone;
        private UnityEngine.Camera _mainCamera;
        
        
        // Initialization
        private void Awake()
        {
            _playerMovementController = FindObjectOfType<PlayerMovementController>();
            _mapBorders = FindObjectOfType<MapBorders>();
            _mainCamera = UnityEngine.Camera.main;
            if (_mapBorders != null)
                InitializeMapBorders();

            _tombstone = Instantiate(_tombstonePrefab);
            _tombstone.SetActive(false);

            Events.PlayerEvents.PlayerRespawning.AddListener(EnableTombstone);
        }

        private void InitializeMapBorders()
        {
            _mapBorders.ColliderTrigger.Enter.SubscribeWithTag(UnityTag.Player, OnPlayerOutOfMapBorders);
        }

        private void Start()
        {
            _playerTransform = _playerMovementController.transform;
            ResetStatus();
            SubscribeToFallDeathEvents();
            SubscribeToEnemyDeathEvents();
        }

        private void OnDestroy() 
        {
            Events.PlayerEvents.PlayerRespawning.RemoveListener(EnableTombstone);
        }

        private void SubscribeToFallDeathEvents()
        {
            _playerMovementController.AllDrawingExit += OnWalkableOrDrawingExit;
            _playerMovementController.AllClimbingExit += OnWalkableOrDrawingExit;
            _playerMovementController.AllGroundsExit += OnWalkableOrDrawingExit;
            _playerMovementController.PlayerFeet.ColliderTrigger.Enter.SubscribeWithTag(UnityTag.Ground, _ => OnGroundEnter());
            _playerMovementController.PlayerFeet.ColliderTrigger.Enter.SubscribeWithTag(UnityTag.ClimbingWall, _ => OnClimbingWallEnter());
            _playerMovementController.PlayerFeet.ColliderTrigger.Enter.SubscribeWithTag(UnityTag.Drawing, _ => OnDrawingEnter());
        }

        private void SubscribeToEnemyDeathEvents()
        {
            var enemyDeathTagsToCheck = new string[] { UnityTag.Enemy, UnityTag.DarkLake };

            foreach (var tag in enemyDeathTagsToCheck)
            {
                _playerMovementController.PlayerBody.ColliderTrigger.Enter.SubscribeWithTag(tag, collider => InvokeDeathEvent(DeathType.Enemy));
            }
        }


        // Public Properties
        public float MaxFallDistanceBeforeDeath => _mainCamera.orthographicSize * 2f;



        // Public Methods
        public void ResetStatus()
        {
            _previousGroundedPosition = _playerTransform.position;
        }


        // DeathObserver Events
        private void OnPlayerOutOfMapBorders(Collider2D collision)
        {
            InvokeDeathEvent(DeathType.OutOfBound);
        }

        private void OnWalkableOrDrawingExit()
        {
            if (!_playerMovementController.IsInTheAir)
                return;

            _wasInTheAir = true;
            UpdateFallDistance();
        }

        private void OnGroundEnter()
        {
            CheckFallDeath();
            UpdateFallDistance();
        }

        private void OnClimbingWallEnter()
        {            
            UpdateFallDistance();
        }

        private void OnDrawingEnter()
        {
            CheckFallDeath();
            UpdateFallDistance();
        }
       
        private void UpdateFallDistance()
        {
            _previousGroundedPosition = _playerTransform.position;
        }

        private void CheckFallDeath()
        {
            if (!_wasInTheAir && !_playerMovementController.IsClimbing)
                return;
            _wasInTheAir = false;
            var (previousPos, currentPos) = (_previousGroundedPosition, _playerTransform.position);
            if (IsFallDeath(previousPos, currentPos))
                InvokeDeathEvent(DeathType.Fall);
        }

        private bool IsFallDeath(Vector3 start, Vector3 end) 
            => start.y - end.y > MaxFallDistanceBeforeDeath;

        
        private void InvokeDeathEvent(DeathType deathType)
        {
            Vector2 playerPosition = _playerMovementController.PlayerBody.transform.parent.position;
            _tombstone.SetActive(false);
            UpdateTombstone(playerPosition);
            #if AnaliticsEnabled
                Events.AnaliticsEvents.PlayerDeath.Invoke(new Analitic(playerPosition.x, playerPosition.y));
            #endif
            

            // Play animation
            _playerMovementController.AnimatorController.SetBool(PlayerAnimatorParameters.Spawning, true);
            if (deathType == DeathType.OutOfBound)
                _playerMovementController.AnimatorController.SetTrigger(PlayerAnimatorParameters.DeathOOB);
            else
                _playerMovementController.AnimatorController.SetTrigger(PlayerAnimatorParameters.DeathNormal);

            Events.PlayerEvents.Death.Invoke();
        }

        private void UpdateTombstone(Vector2 position)
        {
            _tombstone.transform.position = position;
        }

        private void EnableTombstone()
        {
            if (!_tombstone.activeSelf)
                _tombstone.SetActive(true);
        }
    }

    public enum DeathType {
        Enemy,
        Fall,
        OutOfBound
    }
}
