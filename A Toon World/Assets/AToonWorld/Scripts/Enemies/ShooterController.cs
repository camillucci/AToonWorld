using System.Collections;
using System.Collections.Generic;
using Assets.AToonWorld.Scripts;
using Assets.AToonWorld.Scripts.Audio;
using Assets.AToonWorld.Scripts.Enemies;
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
    [SerializeField] private bool _ballisticsEnabled = false;


    private bool _canFire;
    private IEnumerator _enableFire;
    private Transform _transform;
    private GameObject _bullet;
    private Rigidbody2D _prefabBulletRigidBody;
    private BulletController _prefabBulletController;
    private BulletBehaviour _bulletBehaviour;
    private AreaController _areaController;
    private Vector2 _currentTargetVelocity;
    private Vector2 _previousTargetPosition;

    public virtual Vector2 CurrentShootingTarget
    {
        get
        {
            if (!_ballisticsEnabled)
                return _target.position;

            // if "Jumping"
            if (_currentTargetVelocity.y / _currentTargetVelocity.x > 1.5) 
                return (Vector2)_target.position + _currentTargetVelocity.normalized * 1;

            var linearPosition = LinearBallistics.FindShootTarget(_bulletSpawner.position, _prefabBulletController.Speed, _target.position, _currentTargetVelocity);
            if (_prefabBulletController.BehaviourType == BulletBehaviourType.Linear)
                return linearPosition + Vector2.Distance(linearPosition, _target.position)* 0.2f * _currentTargetVelocity.normalized;
            return linearPosition;
        }
    }

    void Start()
    {
        _previousTargetPosition = _target.position;
        _areaController = transform.parent.GetComponent<AreaController>();
        ObjectPoolingManager<int>.Instance.CreatePool(this.GetInstanceID(), _bulletPrefab, 5, 10, true, true);
        _transform = this.transform;
        _prefabBulletRigidBody = _bulletPrefab.GetComponent<Rigidbody2D>();
        _prefabBulletController = _bulletPrefab.GetComponent<BulletController>();
        BulletBehaviourType bulletType = _prefabBulletController.BehaviourType;
        if (bulletType == BulletBehaviourType.Linear) _bulletBehaviour = new LinearBullet();
        if (bulletType == BulletBehaviourType.Parabolic) _bulletBehaviour = new ParabolicBullet();
        LookAtTarget();

        if(_ballisticsEnabled)
            UpdateTargetVelocity().Forget();
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
            _bulletController.Shoot(_bulletSpawner.position, CurrentShootingTarget /*_target.position*/);
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

    // Only if ballistics is enabled
    private async UniTask UpdateTargetVelocity()
    {
        var deltaTimeMs = 100;
        while (true)
        {
            await this.Delay(deltaTimeMs);
            var currentTargetPosition = (Vector2)_target.position;
            _currentTargetVelocity = (currentTargetPosition - _previousTargetPosition) * 1000 / deltaTimeMs;
            _previousTargetPosition = currentTargetPosition;
        }
    }
}
