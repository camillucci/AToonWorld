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
        private PlayerInkController _playerInkController;
        private InkWheelController _inkWheelController;

        // Initialization
        private void Awake()
        {
            _playerMovementController = GetComponent<PlayerMovementController>();
            _playerInkController = GetComponent<PlayerInkController>();

            var userInterface = FindObjectOfType<Canvas>();
            _inkWheelController = userInterface.GetComponentInChildren<InkWheelController>(true);
        }

                                      

        // Unity events
        private void Update()
        {
            if (InputUtils.JumpDown)
                _playerMovementController.JumpWhile(IsJumpHeld);
            _playerMovementController.HorizontalMovementDirection = InputUtils.HorizontalRawAxis;
            _playerMovementController.VerticalMovementDirection = InputUtils.VerticalRawAxis;

            if(InputUtils.DrawDown) _playerInkController.OnDrawDown();
            else if(InputUtils.DrawUp) _playerInkController.OnDrawReleased();
            else if(InputUtils.DrawHeld) _playerInkController.WhileDrawHeld();

            if(InputUtils.ConstructionInkSelect) _playerInkController.OnInkSelected(PlayerInkController.InkType.Construction);
            if(InputUtils.ClimbInkSelect) _playerInkController.OnInkSelected(PlayerInkController.InkType.Climb);
            if(InputUtils.DamageInkSelect) _playerInkController.OnInkSelected(PlayerInkController.InkType.Damage);
            if(InputUtils.CancelInkSelect) _playerInkController.OnInkSelected(PlayerInkController.InkType.Cancel);

            if (InputUtils.RotateInks > 0) _playerInkController.OnInkSelected(PlayerInkController.InkSelection.Forward);
            if (InputUtils.RotateInks < 0) _playerInkController.OnInkSelected(PlayerInkController.InkSelection.Backward);

            if (InputUtils.WheelOpened) _inkWheelController.Show();
            if (InputUtils.WheelClosed) _inkWheelController.Hide();
        }

        // Private methods
        private bool IsJumpHeld() => InputUtils.JumpHeld;
    }
}
