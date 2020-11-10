using Assets.AToonWorld.Scripts.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AToonWorld.Scripts
{
    [RequireComponent(typeof(PlayerMovementController))]
    public class PlayerInput : MonoBehaviour
    {
        private PlayerMovementController _playerMovementController;



        // Initialization
        private void Awake()
        {
            _playerMovementController = GetComponent<PlayerMovementController>();
        }

                                      

        // Unity events
        private void Update()
        {
            if (InputUtils.JumpDown)
                _playerMovementController.JumpWhile(IsJumpHeld);
            _playerMovementController.HorizontalMovementDirection = InputUtils.HorizontalRawAxis;
            _playerMovementController.VerticalMovementDirection = InputUtils.VerticalRawAxis;
        }


        // Private methods
        private bool IsJumpHeld() => InputUtils.JumpHeld;
    }
}
