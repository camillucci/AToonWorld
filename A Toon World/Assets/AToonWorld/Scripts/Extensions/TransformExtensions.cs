﻿using Assets.AToonWorld.Scripts.Utils;
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
        public static Task MoveToAnimated(this Transform @this, Vector3 position, float speed) => Animations.Transition
        (
            from: @this.position,
            to: position,
            callback: val => @this.position = val,
            speed: speed
        );
    }
}