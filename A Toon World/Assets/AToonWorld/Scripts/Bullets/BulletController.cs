using System;
using System.Collections;
using System.Collections.Generic;
using Assets.AToonWorld.Scripts;
using Assets.AToonWorld.Scripts.Audio;
using Assets.AToonWorld.Scripts.Extensions;
using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class BulletController : MonoBehaviour
{
    public enum BulletBehaviourType { Parabolic, Linear}

    private float _gravity;
    [SerializeField] protected GameObject _explosion = null;
    [SerializeField] private float _maxBulletSpeed = 40f;
    [SerializeField] private BulletBehaviourType _bulletBehaviourType;
    private BulletBehaviour _bulletBehaviour;

    public BulletBehaviourType BehaviourType => _bulletBehaviourType;

    private void Awake()
    {
        _gravity = GetComponent<Rigidbody2D>().gravityScale * Physics2D.gravity.magnitude;

        if (_bulletBehaviourType == BulletBehaviourType.Parabolic)
            _bulletBehaviour = new ParabolicBullet(this.transform, GetComponent<Rigidbody2D>(), _maxBulletSpeed, _gravity);
        
        if (_bulletBehaviourType == BulletBehaviourType.Linear)
            _bulletBehaviour = new LinearBullet(this.transform, GetComponent<Rigidbody2D>(), _maxBulletSpeed);
    }

    public Quaternion CalculateRotation(Vector2 startPosition, Vector2 targetPosition) => _bulletBehaviour.CalculateRotation(startPosition, targetPosition);

    public void Shoot(Vector2 startPosition, Vector2 targetPosition)
    {
        this.PlaySound(SoundEffects.BulletSounds.RandomOrDefault()).Forget();
        _bulletBehaviour.Shoot(startPosition, targetPosition);
    } 

    protected abstract void OnTriggerEnter2D(Collider2D other);
}
