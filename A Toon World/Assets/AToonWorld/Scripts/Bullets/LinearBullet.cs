using Assets.AToonWorld.Scripts.Audio;
using Assets.AToonWorld.Scripts.Extensions;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class LinearBullet : BulletBehaviour
{
    public LinearBullet(Transform bulletTransform, Rigidbody2D bulletRigidBody) : base(bulletTransform, bulletRigidBody)
    {
        _transform = bulletTransform;
        _rigidBody = bulletRigidBody;
    }

    public override Quaternion CalculateRotation(Vector2 startPosition, Vector2 targetPosition)
    {
        _initialBulletPosition = startPosition;
        _bulletVelocity = targetPosition - startPosition;
        return Quaternion.identity;
    }
}