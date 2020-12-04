using Assets.AToonWorld.Scripts.Utils;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

        public static UniTask RotateTowardsAnimated(this Transform @this, Quaternion rotation, float speed, bool smooth = true) => Animations.Transition
        (
            from: @this.rotation,
            to: rotation,
            callback: val => @this.rotation = val,
            speed: speed,
            smooth: smooth
        );

        public static UniTask RotateTowardsAnimatedWithCancellation(this Transform @this, Quaternion rotation, CancellationToken cancellationToken, float speed, bool smooth = true) => Animations.Transition
        (
            from: @this.rotation,
            to: rotation,
            callback: val => @this.rotation = val,
            speed: speed,
            smooth: smooth,
            cancelCondition: () => cancellationToken.IsCancellationRequested
        );

        public static async UniTask FollowPathAnimatd(this Transform @this, IEnumerable<Vector3> positions, float speed, bool smooth = true)
        {
            foreach (var position in positions)
                await @this.MoveToAnimated(position, speed, smooth);
        }
    }
}
