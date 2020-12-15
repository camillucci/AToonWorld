using UnityEngine;

public abstract class BulletBehaviour
{
    protected Transform _transform;
    protected Rigidbody2D _rigidBody;

    protected Vector2 _initialBulletPosition;
    protected float _bulletVelocity;

    protected BulletBehaviour(Transform bulletTransform, Rigidbody2D bulletRigidBody)
    {
        _transform = bulletTransform;
        _rigidBody = bulletRigidBody;
    }

    public abstract Quaternion CalculateRotation(Vector2 startPosition, Vector2 targetPosition);

    public void Shoot(Vector2 startPosition, Vector2 targetPosition)
    {
        _transform.position = _initialBulletPosition;
        _transform.rotation = CalculateRotation(startPosition, targetPosition);
        _rigidBody.velocity = _transform.right * _bulletVelocity;
    }

    protected Quaternion LookAt(Vector2 me, Vector2 target)
    {
        Vector2 direction = target - me;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        return Quaternion.AngleAxis(angle, Vector3.forward);
    }

    protected Quaternion LookAt(float angle)
    {
        return Quaternion.AngleAxis(angle, Vector3.forward);
    }
}