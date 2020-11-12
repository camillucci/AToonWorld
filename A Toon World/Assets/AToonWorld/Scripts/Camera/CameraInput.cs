using Assets.AToonWorld.Scripts.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.Camera
{
    public class CameraInput : MonoBehaviour
    {
        private CameraMovementController _cameraMovementController;

        private void Awake()
        {
            _cameraMovementController = GetComponent<CameraMovementController>();
        }


        private void Update()
        {
            _cameraMovementController.ZoomIncrementFactor = InputUtils.Zoom;
        }
    }
}
