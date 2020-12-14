using UnityEngine;

public abstract class BulletBehaviour
{
    protected Transform _transform;
    protected Rigidbody2D _rigidBody;

    protected Vector2 _initialBulletPosition;
    protected Vector2 _bulletVelocity;

    protected BulletBehaviour(Transform bulletTransform, Rigidbody2D bulletRigidBody)
    {
        _transform = bulletTransform;
        _rigidBody = bulletRigidBody;
    }

    public abstract Quaternion CalculateRotation(Vector2 startPosition, Vector2 targetPosition);

    /// <summary>
    /// Rotation performed on parent object by caller
    /// </summary>
    public void Shoot()
    {
        _transform.transform.position = _initialBulletPosition;
        _rigidBody.velocity = -_transform.right * _bulletVelocity;
    }

    /// <summary>
    /// Rotation applied on the bullet (don't rotate the caller object)
    /// </summary>
    public void Shoot(Vector2 startPosition, Vector2 targetPosition)
    {
        _transform.position = _initialBulletPosition;
        _transform.rotation = CalculateRotation(startPosition, targetPosition);
        _rigidBody.velocity = _bulletVelocity;
    }
}