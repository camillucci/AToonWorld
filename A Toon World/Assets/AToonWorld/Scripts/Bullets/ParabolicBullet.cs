using Assets.AToonWorld.Scripts.Audio;
using Assets.AToonWorld.Scripts.Extensions;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ParabolicBullet : BulletBehaviour
{
    public override TrajectoryResult CalculateRotation(Rigidbody2D rigidBody, Vector2 startPosition, Vector2 targetPosition, float maxBulletSpeed)
    {
        float gravity = rigidBody.gravityScale * Physics2D.gravity.magnitude;
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
            bulletVelocity = VelocityFromPointAndVertex(direction, shootingAngle, gravity);
        }
        else
        {
            // Calculate velocity for parabola with vertex playerPosition passing for mouseWorldPosition
            // shootingDirection is always Vector2.right
            shootingAngle = 0f;
            bulletVelocity = VelocityFromHorizontalTangent(direction, gravity);
        }

        if (direction.y == 0f || bulletVelocity > maxBulletSpeed)
        {
            // If too fast, calculate instead tangent versor to parabola with points playerPosition, mouseWorldPosition
            // and (1 / 2 * mouseWorldPosition.x, mouseWorldPosition.y + 1) if on the upper side
            if(upperside) shootingDirection = new Vector2(direction.x, 3f * direction.y + 4f).normalized;
            // or (1 / 2 * mouseWorldPosition.x, 0.5) if on the lower side
            else shootingDirection = new Vector2(direction.x, 2f - direction.y).normalized;
            shootingAngle = Mathf.Atan2(shootingDirection.y, shootingDirection.x);
            bulletVelocity = VelocityFromThreePoints(direction, shootingAngle, gravity);
        }
        
        // Adjusting angle
        shootingAngle *=  Mathf.Rad2Deg;
        if(leftSide) shootingAngle += 2 * (90f - shootingAngle);
        
        return new TrajectoryResult() 
        { 
            initialBulletPosition = startPosition,
            bulletVelocity = Mathf.Min(bulletVelocity, maxBulletSpeed),
            rotation = LookAt(shootingAngle)
        };
    }

    private float VelocityFromPointAndVertex(Vector2 direction, float shootingAngle, float gravity) =>
        Mathf.Sqrt(2 * gravity * Mathf.Abs(direction.y) / Mathf.Pow(Mathf.Sin(shootingAngle), 2));

    private float VelocityFromThreePoints(Vector2 direction, float shootingAngle, float gravity) =>
        Mathf.Sqrt(Mathf.Pow(direction.x, 2) * gravity /
            (direction.x * Mathf.Sin(2 * shootingAngle) - 2 * direction.y * Mathf.Pow(Mathf.Cos(shootingAngle), 2)));
    
    private float VelocityFromHorizontalTangent(Vector2 direction, float gravity) =>
        direction.x / Mathf.Sqrt(- 2 * direction.y / gravity);
}