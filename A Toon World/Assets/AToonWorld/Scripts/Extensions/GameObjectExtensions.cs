using Assets.AToonWorld.Scripts.Audio;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.Extensions
{
    public static class GameObjectExtensions
    {
        public static UniTask PlaySound(this GameObject @this, SoundEffect soundEffect)
            => AudioManager.PrefabInstance?.PlaySound(soundEffect, @this.transform) ?? UniTask.CompletedTask;


    }
}
