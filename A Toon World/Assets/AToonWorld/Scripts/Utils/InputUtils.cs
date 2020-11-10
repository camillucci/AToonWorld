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
        public static bool WDown => Input.GetKeyDown("w");
    }
}
