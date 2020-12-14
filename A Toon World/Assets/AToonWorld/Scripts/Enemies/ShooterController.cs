using System.Collections;
using System.Collections.Generic;
using Assets.AToonWorld.Scripts;
using Assets.AToonWorld.Scripts.Audio;
using Assets.AToonWorld.Scripts.Extensions;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ShooterController : MonoBehaviour
{
    [SerializeField] private GameObject _bulletSpawner = null;
    [SerializeField] private bool _fixedTarget = true;
    [SerializeField] private Transform _target = null;
    [SerializeField] private GameObject _bulletPrefab = null;
    [SerializeField] private float _bulletsInterleavingSeconds = 1;
    private bool _canFire;
    private IEnumerator _enableFire;
    private Transform _transform;

    void Start()
    {
        ObjectPoolingManager<string>.Instance.CreatePool(_bulletPrefab.name, _bulletPrefab, 5, 10, true, true);
        _transform = this.transform;
    }

    void OnEnable()
    {
       if (_enableFire != null)
            StopCoroutine(_enableFire);
        _canFire = true;
    }

    void FixedUpdate()
    {
        if (_canFire)
        {
            _canFire = false;
            GameObject bullet = ObjectPoolingManager<string>.Instance.GetObject(_bulletPrefab.name);

            if (_fixedTarget) ShootAFixedObject(bullet);
            else ShootAMovingObject(bullet);

            StartCoroutine(_enableFire = EnableFire());
        }
    }

    private void ShootAMovingObject(GameObject bullet)
    {
        BulletController bulletController = bullet.GetComponent<EnemyBulletController>();
        _transform.rotation = bulletController.CalculateParabola(_bulletSpawner.transform.position, _target.position);
        bulletController.Shoot();
    }

    private void ShootAFixedObject(GameObject bullet)
    {
        BulletController bulletController = bullet.GetComponent<EnemyBulletController>();
        bulletController.Shoot(_bulletSpawner.transform.position, _target.position);
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
