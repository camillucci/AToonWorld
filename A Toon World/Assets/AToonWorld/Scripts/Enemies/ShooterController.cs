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
    [SerializeField] private GameObject _target = null;
    [SerializeField] private GameObject _bulletPrefab = null;
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
            this.PlaySound(SoundEffects.BulletSounds.RandomOrDefault()).Forget();
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
