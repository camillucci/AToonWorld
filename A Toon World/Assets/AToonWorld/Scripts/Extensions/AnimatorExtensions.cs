using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.Extensions
{
    public static class AnimatorExtensions
    {
        public static void SetProperty(this Animator @this, bool val, [CallerMemberName] string propertyName = "")
            => @this.SetBool(propertyName, val);

        public static bool GetBool(this Animator @this, [CallerMemberName] string propertyName = "")
            => @this.GetBool(propertyName);
    }
}
