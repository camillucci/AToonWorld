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
    [SerializeField] private BulletBehaviourType _bulletBehaviourType = BulletBehaviourType.Linear;
    private BulletBehaviour _bulletBehaviour;

    public BulletBehaviourType BehaviourType => _bulletBehaviourType;
    public BulletBehaviour BulletBehaviour { set { _bulletBehaviour = value; }}
    public float Speed => _maxBulletSpeed;

    private Transform _transform;
    private Rigidbody2D _rigidBody;

    private void Awake()
    {
        _transform = this.transform;
        _rigidBody = GetComponent<Rigidbody2D>();
        _gravity = _rigidBody.gravityScale * Physics2D.gravity.magnitude;
    }

    public Quaternion CalculateRotation(Vector2 startPosition, Vector2 targetPosition) => _bulletBehaviour.CalculateRotation(_rigidBody, startPosition, targetPosition, _maxBulletSpeed).rotation;

    public void Shoot(Vector2 startPosition, Vector2 targetPosition)
    {
        AudioManager.PrefabInstance.PlaySound(SoundEffects.BulletSounds.RandomOrDefault(), startPosition).Forget();
        _bulletBehaviour.Shoot( _transform, _rigidBody, startPosition, targetPosition, _maxBulletSpeed);
    } 

    protected abstract void OnTriggerEnter2D(Collider2D other);
}
