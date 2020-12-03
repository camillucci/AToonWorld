using System;
using System.Collections;
using System.Collections.Generic;
using Assets.AToonWorld.Scripts;
using UnityEngine;

public abstract class BulletController : MonoBehaviour
{
    private Rigidbody2D _bullet;
    private float _gravity;

    [SerializeField] protected GameObject _explosion = null;
    [SerializeField] private float _maxBulletSpeed = 40f;

    private void Awake()
    {
        _bullet = GetComponent<Rigidbody2D>();
        _gravity = _bullet.gravityScale * Physics2D.gravity.magnitude;
    }

    protected abstract void OnTriggerEnter2D(Collider2D other);

    public float Shoot(Vector2 startPosition, Vector2 targetPosition)
    {
        Vector2 direction = targetPosition - startPosition, shootingDirection;
        float shootingAngle, bulletVelocity;
        bool leftSide = direction.x <= 0f, upperside = direction.y >= 0f;
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

        if (bulletVelocity > _maxBulletSpeed)
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

        // Instantiating bullet
        transform.position = startPosition;
        transform.rotation = Quaternion.Euler(0f, 0f, shootingAngle);
        _bullet.velocity = transform.right * Mathf.Min(bulletVelocity, _maxBulletSpeed);

        return shootingAngle;
    }

    private float VelocityFromPointAndVertex(Vector2 direction, float shootingAngle) =>
        Mathf.Sqrt(2 * _gravity * Mathf.Abs(direction.y) / Mathf.Pow(Mathf.Sin(shootingAngle), 2));

    private float VelocityFromThreePoints(Vector2 direction, float shootingAngle) =>
        Mathf.Sqrt(Mathf.Pow(direction.x, 2) * _gravity /
            (direction.x * Mathf.Sin(2 * shootingAngle) - 2 * direction.y * Mathf.Pow(Mathf.Cos(shootingAngle), 2)));
    
    private float VelocityFromHorizontalTangent(Vector2 direction) =>
        direction.x / Mathf.Sqrt(- 2 * direction.y / _gravity);
}
