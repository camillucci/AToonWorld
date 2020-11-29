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
    public static class CanvasGroupExtensions
    {
        public static UniTask FadeTo(this CanvasGroup @this, float to, float speed = 5, bool smooth = true) => Animations.Transition
        (
            from: @this.alpha,
            to: to,
            callback: val => @this.alpha = val,
            speed: speed,
            smooth: smooth
        );
    }
}
