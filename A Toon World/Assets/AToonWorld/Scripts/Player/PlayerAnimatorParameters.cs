using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.Player
{
    public static class PlayerAnimatorParameters
    {
        public const string VelocityX = nameof(VelocityX);
        public const string VelocityY = nameof(VelocityY);
        public const string Grounded = nameof(Grounded);
        public const string Climbing = nameof(Climbing);
        public const string DeathNormal = nameof(DeathNormal);
        public const string DeathOOB = nameof(DeathOOB);
        public const string Spawning = nameof(Spawning);
        public const string Victory = nameof(Victory);
    }
}
