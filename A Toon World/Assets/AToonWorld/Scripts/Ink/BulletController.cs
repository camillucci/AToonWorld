using System;
using System.Collections;
using System.Collections.Generic;
using Assets.AToonWorld.Scripts;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    private Rigidbody2D _bullet;
    [SerializeField] private float _maxBulletSpped = 50f;

    private void Awake()
    {
        _bullet = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(UnityTag.Enemy))
            other.gameObject.SetActive(false); // Move this into enemy controller?
        if (other.CompareTag(UnityTag.Ground) || other.CompareTag(UnityTag.Enemy))
            gameObject.SetActive(false);
    }

    public void Shoot(Vector2 mouseWorldPosition, Vector2 playerPosition)
    {
        Vector2 direction = mouseWorldPosition - playerPosition, shootingDirection;
        float shootingAngle, bulletVelocity, travelTime;
        bool leftSide = direction.x < 0f;

        if (direction.y > 0f)
        {
            // Calculate tangent versor to parabola with vertex mouseWorldPosition passing for playerPosition
            if (leftSide) direction.x = - direction.x;
            shootingDirection = new Vector2(1, 2 * direction.y / direction.x).normalized;

            // Calculate initial angle of the bullet
            shootingAngle = Mathf.Atan2(shootingDirection.y, shootingDirection.x);

            // Calculate initial velocity of the bullet
            bulletVelocity = Mathf.Sqrt(2 * _bullet.gravityScale * Physics2D.gravity.magnitude * Mathf.Abs(direction.y) / Mathf.Pow(Mathf.Sin(shootingAngle), 2));
            shootingAngle *=  Mathf.Rad2Deg;
            if(leftSide) shootingAngle += 2 * (90f - shootingAngle);
        }
        else
        {
            // Calculate velocity for parabola with vertex playerPosition passing for mouseWorldPosition
            shootingAngle = leftSide ? 180f : 0f;
            travelTime = Mathf.Sqrt(- 2 * direction.y / (_bullet.gravityScale * Physics2D.gravity.magnitude));
            bulletVelocity = Mathf.Abs(direction.x / travelTime);
        }

        transform.position = playerPosition;
        transform.rotation = Quaternion.Euler(0f, 0f, shootingAngle);
        _bullet.velocity = transform.right * Mathf.Min(bulletVelocity, _maxBulletSpped);
    }
}
