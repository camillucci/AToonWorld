using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.Enemies
{
    public static class LinearBallistics
    {
        public static Vector2 FindShootTarget(Vector2 bulletStart, float bulletSpeed, Vector2 targetStart, Vector2 targetVelocity)
        {
            // *** Derivation ***
            // rBullet(t) = bulletStart + bulletSpeed * VELOCITY_DIRECTION * t
            // rTarget(t) = targetStart + targetVelocity * t

            // rBullet(t) = rTarget(t) 
            // VELOCITY_DIRECTION = (targetStart - bulletStart + targetVelocity * t ) / (bulletSpeed * t)
            // |VELOCITY_DIRECTION|^2 = 1   ==>  |targetStart - bulletStart + targetVelocity * t|^2 = |bulletSpeed * t|^2
            // ==> solve for t
            // |targetStart|^2 + |bulletStart|^2 + |targetVelocity|^2 * t^2 - 2<targetStart, bulletStart> - 2<bulletStart, targetVelocity> * t +  2<targetStart, targetVelocity> * t = bulletSpeed^ 2 * t^2
            // ==> a t^2 + bt + c = 0


            var a = targetVelocity.sqrMagnitude - (bulletSpeed * bulletSpeed);
            var b = 2 * Vector2.Dot(targetStart - bulletStart, targetVelocity);
            var c = targetStart.sqrMagnitude + bulletStart.sqrMagnitude - 2 * Vector2.Dot(targetStart, bulletStart);

            var delta = b * b - 4 * a * c;
            if (delta < 0)
                return targetStart;

            var (t1, t2) = ((-b + Mathf.Sqrt(delta)) / (2 * a), (-b - Mathf.Sqrt(delta)) / (2 * a));
            if (t1 < 0 && t2 < 0)
                return targetStart;

            Vector2 RBullet(float time, Vector2 velocityVersor) => bulletStart + bulletSpeed * velocityVersor * time;
            //Vector2 RTarget(float time) => targetStart + targetVelocity * time;


            var t = t1 > 0 ? t1 : t2;


            var velocityDirection = (targetStart - bulletStart + targetVelocity * t) / (bulletSpeed * t);
            return RBullet(t, velocityDirection);
        }
    }
}
