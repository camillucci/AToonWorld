using Assets.AToonWorld.Scripts.Audio;
using Assets.AToonWorld.Scripts.Extensions;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ParabolicBullet : BulletBehaviour
{
    private float _maxBulletSpeed;
    private float _gravity;

    public ParabolicBullet(Transform bulletTransform, Rigidbody2D bulletRigidBody, float maxBulletSpeed, float gravity) : base(bulletTransform, bulletRigidBody)
    {
        _maxBulletSpeed = maxBulletSpeed;
        _gravity = gravity;
    }

    public override Quaternion CalculateRotation(Vector2 startPosition, Vector2 targetPosition)
    {
        Vector2 direction = targetPosition - startPosition, shootingDirection;
        float shootingAngle, bulletVelocity;
        bool leftSide = direction.x <= 0f, upperside = direction.y > 0f;
        if (leftSide) direction.x = - direction.x;

        // Consider the four quadrants separately
        if (upperside)
        {
            // Calculate tangent to parabola with vertex mouseWorldPosition passing for playerPosition
            shootingDirection = new Vector2(direction.x, 2f * direction.y).normalized;
            shootingAngle = Mathf.Atan2(shootingDirection.y, shootingDirection.x);
            bulletVelocity = VelocityFromPointAndVertex(direction, shootingAngle);
        }
        else
        {
            // Calculate velocity for parabola with vertex playerPosition passing for mouseWorldPosition
            // shootingDirection is always Vector2.right
            shootingAngle = 0f;
            bulletVelocity = VelocityFromHorizontalTangent(direction);
        }

        if (direction.y == 0f || bulletVelocity > _maxBulletSpeed)
        {
            // If too fast, calculate instead tangent versor to parabola with points playerPosition, mouseWorldPosition
            // and (1 / 2 * mouseWorldPosition.x, mouseWorldPosition.y + 1) if on the upper side
            if(upperside) shootingDirection = new Vector2(direction.x, 3f * direction.y + 4f).normalized;
            // or (1 / 2 * mouseWorldPosition.x, 0.5) if on the lower side
            else shootingDirection = new Vector2(direction.x, 2f - direction.y).normalized;
            shootingAngle = Mathf.Atan2(shootingDirection.y, shootingDirection.x);
            bulletVelocity = VelocityFromThreePoints(direction, shootingAngle);
        }
        
        // Adjusting angle
        shootingAngle *=  Mathf.Rad2Deg;
        if(leftSide) shootingAngle += 2 * (90f - shootingAngle);

        _initialBulletPosition = startPosition;
        _bulletVelocity = Mathf.Min(bulletVelocity, _maxBulletSpeed);

        //return Quaternion.Euler(0f, 0f, shootingAngle + 180);
        return LookAt(shootingAngle);
    }

    private float VelocityFromPointAndVertex(Vector2 direction, float shootingAngle) =>
        Mathf.Sqrt(2 * _gravity * Mathf.Abs(direction.y) / Mathf.Pow(Mathf.Sin(shootingAngle), 2));

    private float VelocityFromThreePoints(Vector2 direction, float shootingAngle) =>
        Mathf.Sqrt(Mathf.Pow(direction.x, 2) * _gravity /
            (direction.x * Mathf.Sin(2 * shootingAngle) - 2 * direction.y * Mathf.Pow(Mathf.Cos(shootingAngle), 2)));
    
    private float VelocityFromHorizontalTangent(Vector2 direction) =>
        direction.x / Mathf.Sqrt(- 2 * direction.y / _gravity);
}