﻿using System.Collections;
using System.Collections.Generic;
using Assets.AToonWorld.Scripts;
using UnityEngine;

public class ShooterController : MonoBehaviour
{
    [SerializeField] private GameObject _bulletSpawner;
    [SerializeField] private GameObject _target;
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private float _bulletsInterleavingSeconds = 1;
    private bool _canFire;

    void Start()
    {
        ObjectPoolingManager<string>.Instance.CreatePool(nameof(_bulletPrefab), _bulletPrefab, 5, 10, true);
        _canFire = true;
    }

    void FixedUpdate()
    {
        if (_canFire)
        {
            _canFire = false;
            GameObject bullet = ObjectPoolingManager<string>.Instance.GetObject(nameof(_bulletPrefab));
            bullet.GetComponent<EnemyBulletController>().Shoot(_bulletSpawner.transform.position, _target.transform.position);
            StartCoroutine(EnableFire());
        }
    }

    IEnumerator EnableFire()
    {
        yield return new WaitForSeconds(_bulletsInterleavingSeconds);
        _canFire = true;
        yield return null;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(UnityTag.Drawing))
            other.gameObject.SetActive(false);
    }
}