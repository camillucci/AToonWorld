using Assets.AToonWorld.Scripts.Audio;
using Assets.AToonWorld.Scripts.Extensions;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class LinearBullet : BulletBehaviour
{
    private float _speed;

    public LinearBullet(Transform bulletTransform, Rigidbody2D bulletRigidBody, float maxBulletSpeed) : base(bulletTransform, bulletRigidBody)
    {
        _transform = bulletTransform;
        _rigidBody = bulletRigidBody;
        _speed = maxBulletSpeed;
    }

    public override Quaternion CalculateRotation(Vector2 startPosition, Vector2 targetPosition)
    {
        _initialBulletPosition = startPosition;
        _bulletVelocity = _speed;
        return LookAt(startPosition, targetPosition);
    }
}