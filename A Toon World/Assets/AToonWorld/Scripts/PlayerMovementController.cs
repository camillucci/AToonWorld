using Assets.AToonWorld.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerMovementController : MonoBehaviour
{
    // Editor fields
    [SerializeField] private float _speed = 8;
    [SerializeField] private float _jumpStepForce = 450;
    [SerializeField] private float _maxJumpForce = 1100;
    [SerializeField] private float _doubleJumpSpeed = 15;
    [SerializeField] [Range(1, 200)] private float _jumpHoldStepMs = 39.5f;


    
    // Private fields
    private Rigidbody2D _rigidBody;   
    private readonly WaitForFixedUpdate _waitForFixedUpdate = new WaitForFixedUpdate();
    private Dictionary<JumpState, Action> _onJumpHandlers = new Dictionary<JumpState, Action>(); 
    private Action _fixedUpdateActions; // code scheduled to be executed on FixedUpdate 
    private Func<bool> _jumpHeldCondition; // delegate that says whether jump input is held or not

    

    // Initialization
    private void Awake()
    {        
        _rigidBody = GetComponent<Rigidbody2D>();        
        InitializeJumpingStates();
    }

    private void InitializeJumpingStates()
    {
        _onJumpHandlers = new Dictionary<JumpState, Action>
        {
            { JumpState.NoJumping, OnJump_WhileNoJumping },
            { JumpState.Jumping, OnJump_WhileJumping },
        };
    }



    
    // Public Properties
    public bool IsDoubleJumpEnabled { get; set; } = true;
    public float HorizontalMovementDirection { get; set; }
    public bool IsGrounded { get; private set; }
    public JumpState CurrentJumpState { get; private set; }




    // Public Methods
    public void JumpWhile(Func<bool> jumpHeldCondition)
    {
        _jumpHeldCondition = jumpHeldCondition ??
                             throw new InvalidOperationException($"{nameof(jumpHeldCondition)} cannot be null");
        HandleJump();
    }

    public void Jump()
    {
        _jumpHeldCondition = null;
        HandleJump();
    }




    
    // Unity events   
    private void FixedUpdate()
    {
        DoFixedUpdateActions();
        MoveHorizontal();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(UnityTag.Ground))
        {
            IsGrounded = true;
            CurrentJumpState = JumpState.NoJumping;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(UnityTag.Ground))
            IsGrounded = false;
    }   



    // JumpState handlers
    private void OnJump_WhileNoJumping()
    {
        if (IsGrounded)
            StartCoroutine(JumpCoroutine());
    }

    private void OnJump_WhileJumping()
    {
        if (IsDoubleJumpEnabled)
            DoubleJump();
    }




    // Coroutine
    

    /// <summary>
    /// Implements a variable height jump.
    /// Add a force of <see cref="_jumpStepForce"/> magnitude every <see cref="_jumpHoldStepMs"/> ms if the jump input is held
    /// until <see cref="_maxJumpForce"/> is reached.
    /// </summary>
    /// <returns> The jump coroutine </returns>
    private IEnumerator JumpCoroutine()
    {
        CurrentJumpState = JumpState.Jumping;
        float totForce = 0;
        bool jumpHeld;
        do
        {
            float forceIncrement = Mathf.Min(_jumpStepForce, _maxJumpForce - totForce);
            yield return _waitForFixedUpdate;
            _rigidBody.AddForce(forceIncrement * Vector2.up);
            yield return new WaitForSeconds(_jumpHoldStepMs / 1000);
            jumpHeld = _jumpHeldCondition?.Invoke() ?? false;
            totForce += forceIncrement;
        }
        while (jumpHeld && totForce < _maxJumpForce && CurrentJumpState == JumpState.Jumping);
    }




    // Private methods
    private void DoFixedUpdateActions()
    {
        if (_fixedUpdateActions is null)
            return;

        Action actions = _fixedUpdateActions;
        _fixedUpdateActions = null;
        actions.Invoke();        
    }


    private void DoubleJump()
    {        
        CurrentJumpState = JumpState.DoubleJumping;        
        var velocity = new Vector2(_rigidBody.velocity.x, _doubleJumpSpeed);
        _fixedUpdateActions += () => _rigidBody.velocity = velocity;
    }

    private void MoveHorizontal()
    {
        float xVelocity = HorizontalMovementDirection * _speed;        
        _rigidBody.velocity = new Vector2(xVelocity, _rigidBody.velocity.y);
    }


    private void HandleJump()
    {
        if (_onJumpHandlers.TryGetValue(CurrentJumpState, out Action jumpStateHandler))
            jumpStateHandler.Invoke();
    }


    public enum JumpState
    {
        Jumping,
        DoubleJumping,
        NoJumping,
    }
}
