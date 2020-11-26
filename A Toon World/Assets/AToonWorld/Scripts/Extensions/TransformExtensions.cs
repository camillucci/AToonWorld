using Assets.AToonWorld.Scripts.Utils;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.Extensions
{
    public static class TransformExtensions
    {
        public static UniTask MoveToAnimated(this Transform @this, Vector3 position, float speed, bool smooth = true) => Animations.Transition
        (
            from: @this.position,
            to: position,
            callback: val => @this.position = val,
            speed: speed,
            smooth: smooth
        );
    }
}
