using Assets.AToonWorld.Scripts.Audio;
using Assets.AToonWorld.Scripts.Extensions;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class LinearBullet : BulletBehaviour
{
    public override TrajectoryResult CalculateRotation(Rigidbody2D rigidBody, Vector2 startPosition, Vector2 targetPosition, float speed)
    {
        return new TrajectoryResult() 
        { 
            initialBulletPosition = startPosition,
            bulletVelocity = speed,
            rotation = LookAt(startPosition, targetPosition)
        };
    }
}