using Assets.AToonWorld.Scripts;
using Assets.AToonWorld.Scripts.Player;
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
    [SerializeField] private float _climbingSpeed = 5;
    [SerializeField] private float _gravityScale = 5;


    
    // Private fields
    private Rigidbody2D _rigidBody;
    private PlayerFeet _playerFeet;
    private PlayerBody _playerBody;
    private readonly WaitForFixedUpdate _waitForFixedUpdate = new WaitForFixedUpdate();
    private Dictionary<JumpState, Action> _onJumpHandlers = new Dictionary<JumpState, Action>(); 
    private Action _fixedUpdateActions; // code scheduled to be executed on FixedUpdate 
    private Func<bool> _jumpHeldCondition; // delegate that says whether jump input is held or not
    private int _drawingPlatformsCollidedCounter;
    private int _groundsCollidedCounter;
    private int _climbingWallCollidedCounter;
    

    // Initialization
    private void Awake()
    {        
        _rigidBody = GetComponent<Rigidbody2D>();
        _playerFeet = GetComponentInChildren<PlayerFeet>();
        _playerBody = GetComponentInChildren<PlayerBody>();
        InitializeJumpingStates();
        InitializeFeet();
        InitializeBody();
    }

    private void InitializeBody()
    {
        _playerBody.TriggerEnter.SubscribeWithTag(UnityTag.ClimbingWall, OnClimbingWallEnter);
        _playerBody.TriggerExit.SubscribeWithTag(UnityTag.ClimbingWall, OnClimbingWallExit);        
    }

    private void InitializeFeet()
    {
        var walkableTags = new string[] { UnityTag.Ground, UnityTag.Drawing };
        _playerFeet.TriggerEnter.SubscribeWithTag(UnityTag.Ground, OnGroundEnter);
        _playerFeet.TriggerExit.SubscribeWithTag(UnityTag.Ground, OnGroundExit);
        _playerFeet.TriggerEnter.SubscribeWithTag(UnityTag.Drawing, OnDrawingEnter);
        _playerFeet.TriggerExit.SubscribeWithTag(UnityTag.Drawing, OnDrawingExit);        
        foreach(var walkableTag in walkableTags)
        {
            _playerFeet.TriggerEnter.SubscribeWithTag(walkableTag, OnWalkableEnter);
            _playerFeet.TriggerExit.SubscribeWithTag(walkableTag, OnWalkableExit);
        }
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
    public float VerticalMovementDirection { get; set; }
    public JumpState CurrentJumpState { get; private set; }
    public bool IsClimbing => _climbingWallCollidedCounter > 0;
    public bool IsGrounded => _groundsCollidedCounter > 0;
    public bool IsOnDrawingPlatform => _drawingPlatformsCollidedCounter > 0;   
    public bool CanJump => IsGrounded || IsOnDrawingPlatform || (IsClimbing && CurrentJumpState == JumpState.NoJumping);




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



    // Player events
  
    private void OnGroundEnter(Collider2D collider)
    {
        _groundsCollidedCounter++;        
    }

    private void OnGroundExit(Collider2D collider)
    {
        _groundsCollidedCounter--;
    }


    private void OnDrawingEnter(Collider2D collider)
    {        
        _drawingPlatformsCollidedCounter++;
    }

    private void OnDrawingExit(Collider2D collider)
    {
        _drawingPlatformsCollidedCounter--;
    }

    
    
    // Both Ground and Drawing. After the specific events
    private void OnWalkableEnter(Collider2D collider)
    {
        CurrentJumpState = JumpState.NoJumping;
        if (IsClimbing)
            SetGravity(false);
    }

    private void OnWalkableExit(Collider2D collider)
    {

    }



    private void OnClimbingWallEnter(Collider2D collider)
    {
        _climbingWallCollidedCounter++;
        CurrentJumpState = JumpState.NoJumping;
        _fixedUpdateActions += () => SetGravity(false);
    }

    private void OnClimbingWallExit(Collider2D collider)
    {        
        _climbingWallCollidedCounter--;
        _fixedUpdateActions += () => SetGravity(true);
    }

    


    // Unity events   
    private void FixedUpdate()
    {
        DoFixedUpdateActions();
        MoveHorizontal();
        MoveVertical();
    }
  



    // JumpState handlers
    private void OnJump_WhileNoJumping()
    {
        if (CanJump)
        {
            CurrentJumpState = JumpState.Jumping;
            StartCoroutine(JumpCoroutine());
        }
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

        float totForce = 0;
        bool jumpHeld;
        yield return _waitForFixedUpdate;
        _rigidBody.velocity = new Vector2(_rigidBody.velocity.x, 0);
        SetGravity(true);
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

    private void MoveVertical()
    {
        if(IsClimbing && CanJump)
        {
            float yVelocity = VerticalMovementDirection * _climbingSpeed;        
            _rigidBody.velocity = new Vector2(_rigidBody.velocity.x, yVelocity);
        }
    }

    private void HandleJump()
    {
        if (_onJumpHandlers.TryGetValue(CurrentJumpState, out Action jumpStateHandler))
            jumpStateHandler.Invoke();
    }


    private void SetGravity(bool isGravityEnabled)
    {
        _rigidBody.gravityScale = isGravityEnabled ? _gravityScale : 0;
    }

    public enum JumpState
    {
        Jumping,
        DoubleJumping,
        NoJumping,
    }
}
