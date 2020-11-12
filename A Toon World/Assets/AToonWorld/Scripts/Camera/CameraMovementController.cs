using Assets.AToonWorld.Scripts.Extensions;
using Assets.AToonWorld.Scripts.Player;
using Assets.AToonWorld.Scripts.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityAsync;
using UnityEngine;
using WaitForSeconds = UnityAsync.WaitForSeconds;

namespace Assets.AToonWorld.Scripts.Camera
{
    public class CameraMovementController : MonoBehaviour
    {
        // Editor Fields        
        [SerializeField] private float _cameraSpeed = 5;
        [SerializeField] private float _minimumZoom = 1;
        [SerializeField] private float _zoomStep = 7;
        [SerializeField] private float _zoomAnimationSpeed = 5;
        [SerializeField] private bool _followPlayerWhenDrawing;

        // Private fields
        private Transform _playerTransform;
        private UnityEngine.Camera _camera;
        private bool _manuallyMovingCamera;
        private Transform _transform;
        private ZoomTaskCancellation _zoomTaskCancellation = new ZoomTaskCancellation();
        private Task _currentZoomTask = Task.CompletedTask;
        private PlayerController _playerController;


        // Initialization
        private void Awake()
        {
            _transform = transform;
            _playerController = FindObjectOfType<PlayerController>();
            _camera = GetComponent<UnityEngine.Camera>();
            _playerTransform = _playerController.transform;
        }


        private bool CanFollowPlayer => ShouldFollowPlayer && !_manuallyMovingCamera
                                       && (_followPlayerWhenDrawing || !_playerController.PlayerInkController.IsDrawing);

        //Public Properties
        public float ZoomIncrementFactor { get; set; }
        public bool ShouldFollowPlayer { get; set; } = true;
        public bool IsZoomEnabled { get; set; } = true;
        public float CameraSpeed => _cameraSpeed;
        public bool FollowPlayerWhenDrawing { get => _followPlayerWhenDrawing; set => _followPlayerWhenDrawing = value; }
                                       


        public async Task MoveCameraToTarget()
        {
            _manuallyMovingCamera = true;
            await _camera.MoveTo(_playerTransform.position, _cameraSpeed);
            _manuallyMovingCamera = false;
        }


        public async Task MoveCameraToPosition(Vector3 position)
        {            
            _manuallyMovingCamera = true;
            await _camera.MoveTo(position, _cameraSpeed);
            _manuallyMovingCamera = false;
        }


        // Unity events

        private void Update()
        {
            FollowTarget();
            UpdateZoom();
        }


        // Private Methods
        private void FollowTarget()
        {
            if (!CanFollowPlayer)
                return;

            var targetPos = new Vector3(_playerTransform.position.x, _playerTransform.position.y, transform.position.z);
            _transform.position = Vector3.Lerp(_transform.position, targetPos, _cameraSpeed * Time.deltaTime);
        }

        private void UpdateZoom()
        {
            if (!IsZoomEnabled)
                return;

            var zoomIncrement = ZoomIncrementFactor * _zoomStep;
            if (Mathf.Abs(zoomIncrement) < float.Epsilon)
                return;

            _zoomTaskCancellation.Cancel();
            _zoomTaskCancellation = new ZoomTaskCancellation();
            var zoomToValue = Mathf.Max(_camera.orthographicSize + zoomIncrement, _minimumZoom);
            _currentZoomTask = ZoomTo(zoomToValue, _zoomTaskCancellation);
        }



        private Task ZoomTo(float to, ZoomTaskCancellation zoomCancellation) => Animations.Transition
        (
            from: _camera.orthographicSize,
            to: to,
            callback: val => _camera.orthographicSize = val,
            speed: _zoomAnimationSpeed,
            smooth: true,
            frameSensitivity: 1,
            cancelCondition: () => zoomCancellation.IsCancellationRequested
        );
        

        private class ZoomTaskCancellation
        {
            public bool IsCancellationRequested { get; private set; }
            public void Cancel() => IsCancellationRequested = true;           
        }
    }
}
