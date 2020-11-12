using System;
using System.Collections;
using System.Collections.Generic;
using Assets.AToonWorld.Scripts;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    private Rigidbody2D _bullet;
    [SerializeField] private float _distanceFromPlayer = 0.75f;
    [SerializeField] private float _minBulletSpeed = 5f;
    [SerializeField] private float _incrementBulletSpeed = 2.5f;

    private void Awake()
    {
        _bullet = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(UnityTag.Enemy))
            other.gameObject.SetActive(false); //TODO: Move this into enemy controller?
        if (other.CompareTag(UnityTag.Ground) || other.CompareTag(UnityTag.Enemy))
            gameObject.SetActive(false);
    }

    public void Shoot(Vector2 mouseWorldPosition, Vector2 playerPosition)
    {
        Vector2 lookDirection = mouseWorldPosition - playerPosition;
        Vector2 normalizedDirection = lookDirection.normalized;
        float lookAngle = Mathf.Atan2(normalizedDirection.y, normalizedDirection.x) * Mathf.Rad2Deg;
        transform.position = playerPosition + normalizedDirection * _distanceFromPlayer;
        transform.rotation = Quaternion.Euler(0f, 0f, lookAngle);
        float bulletSpeed = _minBulletSpeed + _incrementBulletSpeed * lookDirection.magnitude;
        _bullet.velocity = transform.right * bulletSpeed;
    }
}
