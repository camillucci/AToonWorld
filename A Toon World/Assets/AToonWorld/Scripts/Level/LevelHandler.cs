using Assets.AToonWorld.Scripts.Camera;
using Assets.AToonWorld.Scripts.Player;
using Assets.AToonWorld.Scripts.Utils;
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
        private CheckPointsManager _checkPointsManager;
        private PlayerController _playerController;
        private CameraMovementController _cameraMovementController;        


        // Public Properties
        public bool RespawningPlayer { get; private set; }



        // Initialization
        private void Awake()
        {
            _checkPointsManager = GetComponentInChildren<CheckPointsManager>();
            _playerController = FindObjectOfType<PlayerController>();
            _cameraMovementController = FindObjectOfType<CameraMovementController>();
        }


        // Public Methods
        public async Task SpawnFromLastCheckpoint()
        {
            var lastCheckPoint = _checkPointsManager.LastCheckPoint;
            _playerController.DisablePlayer();
            await _playerController.MoveToPosition(lastCheckPoint.Position);
            _playerController.EnablePlayer();
        }


        public async Task KillPlayer()
        {
            RespawningPlayer = true;
            await SpawnFromLastCheckpoint();
            RespawningPlayer = false;
        }

        // Unity events
        private async void Update()
        {
            if (InputUtils.KillPlayer && !RespawningPlayer)
                await KillPlayer();
        }
    }
}
