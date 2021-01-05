using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.AToonWorld.Scripts;
using Assets.AToonWorld.Scripts.UnityAnimations;
using UnityEngine;

public interface IJump
{
    void Jump();
}

public class JumperController : EnemyController
{
    public enum JumpMode { FixedHeight, ToFixedY }

    [Header("Jump Mode")]
    [SerializeField] private JumpMode _jumpMode = JumpMode.ToFixedY;
    [SerializeField] private float _value = 0f;

    [Header("Interleaving seconds")]
    [SerializeField] private float _secondsIntoDarkLake = 2f; //Seconds before next jump
    [SerializeField] private float _secondsBeforeFirstJump = 2f;
    
    private Vector2 _startPosition;
    private float _jumpVelocity;
    private bool _doneFirstJump;

    private Rigidbody2D _rigidBody;
    private Animator _animator;
    private IEnumerator _jumpCoroutine;
    
    void Awake()
    {
        _startPosition = transform.position;
        Events.PlayerEvents.PlayerRespawned.AddListener(ResetInitialState);
        _rigidBody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    private void OnEnable() 
    {
        ResetInitialState();
    }

    private void ResetInitialState()
    {
        _jumpVelocity = 0;
        _doneFirstJump = false;
        this.transform.position = _startPosition;

        if(_jumpCoroutine != null)
            StopCoroutine(_jumpCoroutine);
    }

    private void FixedUpdate()
    {
        _animator.SetFloat("VelocityY", _rigidBody.velocity.y);
    }

    private float CalculateVelocity(float height) => Mathf.Sqrt(2 * Mathf.Abs(Physics.gravity.y) * height);

    private void StartFirstJumpSession()
    {
        _jumpVelocity = CalculateVelocity(_jumpMode == JumpMode.FixedHeight ? _value : _value - transform.position.y);
        StartCoroutine(_jumpCoroutine = Jump(_secondsBeforeFirstJump));
        _doneFirstJump = true;
    }

    IEnumerator Jump(float secondsToWait)
    {
        yield return new WaitForSeconds(secondsToWait);
        _rigidBody.AddForce(_jumpVelocity * Vector2.up, ForceMode2D.Impulse);
        yield return null;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag(UnityTag.DarkLakeFloor))
            if (_doneFirstJump)
                StartCoroutine(_jumpCoroutine = Jump(_secondsIntoDarkLake));
            else
                StartFirstJumpSession();
    }
        
    public override void Kill()
    {
        base.Kill();
        GenericAnimations.InkCloud(transform.position).PlayAndForget();
    }
}
