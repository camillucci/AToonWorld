using UnityEngine;

public class TrajectoryResult
{
    public Vector2 initialBulletPosition;
    public float bulletVelocity;
    public Quaternion rotation;
}

public abstract class BulletBehaviour
{
    public abstract TrajectoryResult CalculateRotation(Rigidbody2D rigidBody, Vector2 startPosition, Vector2 targetPosition, float maxBulletSpeed);

    public void Shoot(Transform bulletTransform, Rigidbody2D bulletRigidBody, Vector2 startPosition, Vector2 targetPosition, float maxBulletSpeed)
    {
        TrajectoryResult result = CalculateRotation(bulletRigidBody, startPosition, targetPosition, maxBulletSpeed);
        bulletTransform.position = result.initialBulletPosition;
        bulletTransform.rotation = result.rotation;
        bulletRigidBody.velocity = bulletTransform.right * result.bulletVelocity;
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