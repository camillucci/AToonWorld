using Assets.AToonWorld.Scripts.Audio;
using Assets.AToonWorld.Scripts.Extensions;
using Assets.AToonWorld.Scripts.Level;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.Player
{
    public class DeathObserver : MonoBehaviour
    {
        // Editor Fields
        [SerializeField] private float _maxFallDistanceBeforeDeath = 3;
        [SerializeField] private GameObject _tombstonePrefab = null;

        // Private Fields
        private PlayerMovementController _playerMovementController;
        private Transform _playerTransform;
        private Vector3 _previousGroundedPosition;
        private MapBorders _mapBorders;
        private bool _isImmortal;
        private bool _wasInTheAir;
        private GameObject _tombstone;

        // Initialization
        private void Awake()
        {
            _playerMovementController = FindObjectOfType<PlayerMovementController>();
            _mapBorders = FindObjectOfType<MapBorders>();
            if(_mapBorders != null)
                InitializeMapBorders();

            _tombstone = Instantiate(_tombstonePrefab);
            _tombstone.SetActive(false);
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
                _playerMovementController.PlayerBody.ColliderTrigger.Enter.SubscribeWithTag(tag, collider => InvokeDeathEvent());
            }
        }


        // Public Methods
        public void ResetStatus()
        {
            _previousGroundedPosition = _playerTransform.position;
        }


        // DeathObserver Events
        private void OnPlayerOutOfMapBorders(Collider2D collision)
        {
            InvokeDeathEvent();
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


        // Private Methods
        /*
          FIXME: Sicuramente qualcosa si è rotto dopo il merge, dato che ora immortal è un flag del player bisogna vedere come gestire
          public bool IsImmortal
          {
              get => _isImmortal;
              set
              {
                  if (value == _isImmortal)
                      return;
                  _isImmortal = value;
                  if (!value)
                      ResetStatus();
              }
          }
        */


       
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
                InvokeDeathEvent();
        }

        private bool IsFallDeath(Vector3 start, Vector3 end) 
            => start.y - end.y > _maxFallDistanceBeforeDeath;

        
        private void InvokeDeathEvent()
        {
            Vector2 playerPosition = _playerMovementController.PlayerBody.transform.parent.position;
            UpdateTombstone(playerPosition);
            #if AnaliticsEnabled
                Events.AnaliticsEvents.PlayerDeath.Invoke(new Analitic(playerPosition.x, playerPosition.y));
#endif
            this.PlaySound(SoundEffects.DeathSounds.RandomOrDefault());
            Events.PlayerEvents.Death.Invoke();
        }

        private void UpdateTombstone(Vector2 position)
        {
            _tombstone.transform.position = position;
            if (!_tombstone.activeSelf)
                _tombstone.SetActive(true);
        }
    }
}
