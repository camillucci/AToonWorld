using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.Utils
{
    public static class InputUtils
    {
        public static float HorizontalRawAxis => Input.GetAxisRaw("Horizontal");
        public static float VerticalRawAxis => Input.GetAxisRaw("Vertical");
        public static bool JumpDown => Input.GetButtonDown("Jump");
        public static bool JumpHeld => Input.GetButton("Jump");

        #region Ink

        public static bool DrawDown => Input.GetButtonDown("Draw");
        public static bool DrawUp => Input.GetButtonUp("Draw");
        public static bool DrawHeld => Input.GetButton("Draw");

        public static bool ConstructionInkSelect => Input.GetButtonDown("ConstructionInk");
        public static bool ClimbInkSelect => Input.GetButtonDown("ClimbInk");
        public static bool DamageInkSelect => Input.GetButtonDown("DamageInk");
        public static bool CancelInkSelect => Input.GetButtonDown("CancelInk");

        public static float RotateInks => Input.GetAxis("RotateInks");

        public static bool WheelOpened => Input.GetButtonDown("InkWheel");
        public static bool WheelClosed => Input.GetButtonUp("InkWheel");

        #endregion

        public static float Zoom => Input.GetKey(KeyCode.LeftControl) ? -Input.GetAxis("Mouse ScrollWheel") : 0;

        public static bool EnterButton => Input.GetButtonDown("Submit");

        public static bool TogglePauseMenu => Input.GetButtonDown("PauseMenu");

        //DEBUG
        public static bool KillPlayer => Input.GetKey(KeyCode.K);
    }
}
