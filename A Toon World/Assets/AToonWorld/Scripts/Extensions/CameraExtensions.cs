using Assets.AToonWorld.Scripts.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.Extensions
{
    public static class CameraExtensions
    {
        public static Task MoveTo(this UnityEngine.Camera @this, Vector3 position, float speed = 10f)
        {
            var transform = @this.transform;
            return Animations.Transition
            (
                from: transform.position,
                to: position,
                callback: val => transform.position = val,
                speed: speed
            );            
        }
    }
}
