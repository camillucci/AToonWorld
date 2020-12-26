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
    public static class SpriteRendererExtensions
    {
        public static UniTask FadeTo(this SpriteRenderer @this, float to, float speed = 5, bool smooth = true) => @this.Transition
        (
            from: @this.color.a,
            to: to,
            callback: val => @this.SetAlpha(val),
            speed: speed,
            smooth: smooth
        );

        public static void SetAlpha(this SpriteRenderer @this, float alpha) 
            => @this.color = @this.color = new Color(@this.color.r, @this.color.g, @this.color.b, alpha);
    }
}
