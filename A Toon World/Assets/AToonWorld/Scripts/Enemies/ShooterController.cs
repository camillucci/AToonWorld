using System.Collections;
using System.Collections.Generic;
using Assets.AToonWorld.Scripts;
using Assets.AToonWorld.Scripts.Audio;
using Assets.AToonWorld.Scripts.Extensions;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ShooterController : MonoBehaviour
{
    [SerializeField] private bool _fixedTarget = true;
    [SerializeField] private Transform _target = null;
    [SerializeField] private Transform _bulletSpawner = null;
    [SerializeField] private GameObject _bulletPrefab = null;
    [SerializeField] private float _bulletsInterleavingSeconds = 1;

    private bool _canFire;
    private IEnumerator _enableFire;
    private Transform _transform;
    private GameObject _bullet;
    private BulletController _bulletController;
    private AreaController _areaController;

    void Start()
    {
        _areaController = transform.parent.GetComponent<AreaController>();
        ObjectPoolingManager<int>.Instance.CreatePool(this.GetInstanceID(), _bulletPrefab, 5, 10, true, true);
        _transform = this.transform;
        GetBulletFromPool();
        LookAtTarget();
    }

    void OnEnable()
    {
       if (_enableFire != null)
            StopCoroutine(_enableFire);
        _canFire = true;
    }

    void Update()
    {
        if (!_fixedTarget)
            LookAtTarget();
    }

    void FixedUpdate()
    {
        if (_canFire && _areaController.InSights)
        {
            _bullet.SetActive(true);
            _bulletController.Shoot(_bulletSpawner.position, _target.position);
            _canFire = false;
            GetBulletFromPool();
            StartCoroutine(_enableFire = EnableFire());
        }
    }

    private void GetBulletFromPool()
    {
        _bullet = ObjectPoolingManager<int>.Instance.GetObject(this.GetInstanceID());
        _bullet.SetActive(false);
        _bulletController = _bullet.GetComponent<BulletController>();
    }

    private void LookAtTarget()
    {
        _transform.rotation = _bulletController.CalculateRotation(_bulletSpawner.position, _target.position);
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
