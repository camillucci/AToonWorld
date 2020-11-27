using Assets.AToonWorld.Scripts;
using Assets.AToonWorld.Scripts.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private bool _isDoubleJumpEnabled;

    // Private fields
    private readonly WaitForFixedUpdate _waitForFixedUpdate = new WaitForFixedUpdate();
    private Dictionary<JumpState, Action> _onJumpHandlers = new Dictionary<JumpState, Action>(); 
    private Action _fixedUpdateAction; // code scheduled to be executed on FixedUpdate 
    private Func<bool> _jumpHeldCondition; // delegate that says whether jump input is held or not
    private int _drawingPlatformsCollidedCount;
    private int _groundsCollidedCount;
    private int _climbingWallCollidedCount;
    private float _gravityScale;
    

    // Initialization
    private void Awake()
    {        
        RigidBody = GetComponent<Rigidbody2D>();
        PlayerFeet = GetComponentInChildren<PlayerFeet>();
        PlayerBody = GetComponentInChildren<PlayerBody>();
        _gravityScale = RigidBody.gravityScale;
        InitializeJumpingStates();
        InitializeFeet();
        InitializeBody();
    }

    private void InitializeBody()
    {
        PlayerBody.TriggerEnter.SubscribeWithTag(UnityTag.ClimbingWall, OnClimbingWallEnter);
        PlayerBody.TriggerExit.SubscribeWithTag(UnityTag.ClimbingWall, OnClimbingWallExit);        
    }

    private void InitializeFeet()
    {
        var walkableTags = new string[] { UnityTag.Ground, UnityTag.Drawing };

        PlayerFeet.TriggerEnter.SubscribeWithTag
        (
            (UnityTag.Ground, OnGroundEnter),
            (UnityTag.Drawing, OnDrawingEnter)
        );

        PlayerFeet.TriggerExit.SubscribeWithTag
        (
            (UnityTag.Ground, OnGroundExit),
            (UnityTag.Drawing, OnDrawingExit)
        );
      
        foreach(var walkableTag in walkableTags)
        {
            PlayerFeet.TriggerEnter.SubscribeWithTag(walkableTag, OnWalkableEnter);
            PlayerFeet.TriggerExit.SubscribeWithTag(walkableTag, OnWalkableExit);
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



    // Events
    public event Action AllGroundsExit;
    public event Action AllClimbingExit;
    public event Action AllDrawingExit;    




    // Public Properties    
    public PlayerBody PlayerBody { get; private set; }
    public PlayerFeet PlayerFeet { get; private set; }
    public bool IsDoubleJumpEnabled { get => _isDoubleJumpEnabled; set => _isDoubleJumpEnabled = value; }
    public float HorizontalMovementDirection { get; set; }
    public float VerticalMovementDirection { get; set; }
    public JumpState CurrentJumpState { get; private set; }
    public bool IsClimbing => ClimbingWallCollidedCount > 0;
    public bool IsGrounded => GroundsCollidedCount > 0;
    public bool IsOnDrawingPlatform => DrawingPlatformsCollidedCount > 0;   
    public bool CanJump => IsGrounded || IsOnDrawingPlatform || (IsClimbing && CurrentJumpState == JumpState.NoJumping);    
    public bool IsGravityEnabled 
    {
        get => Math.Abs(RigidBody.gravityScale) > float.Epsilon;
        private set => RigidBody.gravityScale = value ? _gravityScale : 0;
    }
    public int GroundsCollidedCount
    {
        get => _groundsCollidedCount;
        set
        {
            var oldValue = _groundsCollidedCount;
            _groundsCollidedCount = value;
            if (value == 0 && oldValue > 0)
                AllGroundsExit?.Invoke();
        }
    }
    private int DrawingPlatformsCollidedCount
    {
        get => _drawingPlatformsCollidedCount;
        set
        {
            var oldValue = _drawingPlatformsCollidedCount;
            _drawingPlatformsCollidedCount = value;
            if (value == 0 && oldValue > 0)
                AllDrawingExit?.Invoke();
        }
    }
    private int ClimbingWallCollidedCount
    {
        get => _climbingWallCollidedCount;
        set
        {
            var oldValue = _climbingWallCollidedCount;
            _climbingWallCollidedCount = value;
            if (value == 0 && oldValue > 0)
                AllClimbingExit?.Invoke();
        }
    }
    public bool IsInTheAir => !IsClimbing && !IsGrounded && !IsOnDrawingPlatform;
    public Rigidbody2D RigidBody { get; private set; }





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
  
    private void OnGroundEnter(Collider2D collider) => GroundsCollidedCount++;        
    private void OnGroundExit(Collider2D collider) => GroundsCollidedCount--;    
    private void OnDrawingEnter(Collider2D collider) => DrawingPlatformsCollidedCount++;
    private void OnDrawingExit(Collider2D collider) =>DrawingPlatformsCollidedCount--;
     

    

    // Both Ground and Drawing. After the specific events
    private void OnWalkableEnter(Collider2D collider)
    {
        CurrentJumpState = JumpState.NoJumping;
        if (IsClimbing)
            _fixedUpdateAction += () => IsGravityEnabled = false;
    }

    private void OnWalkableExit(Collider2D collider)
    {

    }



    private void OnClimbingWallEnter(Collider2D collider)
    {
        ClimbingWallCollidedCount++;
        CurrentJumpState = JumpState.NoJumping;
        _fixedUpdateAction += () => IsGravityEnabled = false;
    }

    private void OnClimbingWallExit(Collider2D collider)
    {        
        ClimbingWallCollidedCount--;
        if(!IsClimbing)
            _fixedUpdateAction += () => IsGravityEnabled = true;
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
        RigidBody.velocity = new Vector2(RigidBody.velocity.x, 0);
        IsGravityEnabled = true;
        do
        {
            float forceIncrement = Mathf.Min(_jumpStepForce, _maxJumpForce - totForce);
            yield return _waitForFixedUpdate;
            RigidBody.AddForce(forceIncrement * Vector2.up);
            yield return new WaitForSeconds(_jumpHoldStepMs / 1000);
            jumpHeld = _jumpHeldCondition?.Invoke() ?? false;
            totForce += forceIncrement;
        }
        while (jumpHeld && totForce < _maxJumpForce && CurrentJumpState == JumpState.Jumping);
    }




    // Private methods 

    private void DoFixedUpdateActions()
    {
        if (_fixedUpdateAction is null)
            return;

        Action action = _fixedUpdateAction;
        _fixedUpdateAction = null;
        action.Invoke();        
    }


    private void DoubleJump()
    {        
        CurrentJumpState = JumpState.DoubleJumping;        
        var velocity = new Vector2(RigidBody.velocity.x, _doubleJumpSpeed);
        _fixedUpdateAction += () => RigidBody.velocity = velocity;
    }

    private void MoveHorizontal()
    {
        float xVelocity = HorizontalMovementDirection * _speed;        
        RigidBody.velocity = new Vector2(xVelocity, RigidBody.velocity.y);
    }

    private void MoveVertical()
    {
        if(IsClimbing && CanJump)
        {
            float yVelocity = VerticalMovementDirection * _climbingSpeed;        
            RigidBody.velocity = new Vector2(RigidBody.velocity.x, yVelocity);
        }
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
