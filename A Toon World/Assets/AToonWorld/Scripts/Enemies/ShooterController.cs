using System.Collections;
using System.Collections.Generic;
using Assets.AToonWorld.Scripts;
using Assets.AToonWorld.Scripts.Audio;
using Assets.AToonWorld.Scripts.Extensions;
using Cysharp.Threading.Tasks;
using UnityEngine;
using static BulletController;

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
    private Rigidbody2D _prefabBulletRigidBody;
    private BulletController _prefabBulletController;
    private BulletBehaviour _bulletBehaviour;
    private AreaController _areaController;

    void Start()
    {
        _areaController = transform.parent.GetComponent<AreaController>();
        ObjectPoolingManager<int>.Instance.CreatePool(this.GetInstanceID(), _bulletPrefab, 5, 10, true, true);
        _transform = this.transform;
        _prefabBulletRigidBody = _bulletPrefab.GetComponent<Rigidbody2D>();
        _prefabBulletController = _bulletPrefab.GetComponent<BulletController>();
        BulletBehaviourType bulletType = _prefabBulletController.BehaviourType;
        if (bulletType == BulletBehaviourType.Linear) _bulletBehaviour = new LinearBullet();
        if (bulletType == BulletBehaviourType.Parabolic) _bulletBehaviour = new ParabolicBullet();
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
        if (!_fixedTarget && _areaController.InSights)
            LookAtTarget();
    }

    void FixedUpdate()
    {
        if (_canFire && _areaController.InSights)
        {
            _bullet = ObjectPoolingManager<int>.Instance.GetObject(this.GetInstanceID());
            BulletController _bulletController = _bullet.GetComponent<BulletController>();
            _bulletController.BulletBehaviour = _bulletBehaviour;
            _bulletController.Shoot(_bulletSpawner.position, _target.position);
            _canFire = false;
            StartCoroutine(_enableFire = EnableFire());
        }
    }

    private void LookAtTarget()
    {
        _transform.rotation = _bulletBehaviour.CalculateRotation(_prefabBulletRigidBody, _bulletSpawner.position, _target.position, _prefabBulletController.Speed).rotation;
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
