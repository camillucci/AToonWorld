using Assets.AToonWorld.Scripts;
using Assets.AToonWorld.Scripts.Audio;
using Assets.AToonWorld.Scripts.Extensions;
using Assets.AToonWorld.Scripts.Player;
using Cysharp.Threading.Tasks;
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
    [SerializeField] private int _jumpDelaySensitivity = 4;
    [SerializeField] private float _frictionWhenIdle = 0.9f;    
    //[SerializeField] private int _verticalAngle = 80;


    // Private fields
    private Dictionary<JumpState, Action> _onJumpHandlers = new Dictionary<JumpState, Action>();
    private Action _fixedUpdateAction; // code scheduled to be executed on FixedUpdate 
    private Func<bool> _jumpHeldCondition; // delegate that says whether jump input is held or not
    private int _drawingPlatformsCollidedCount;
    private int _groundsCollidedCount;
    private int _climbingWallCollidedCount;
    private float _gravityScale;
    private bool _horizontalMovementSoundTaskRunning;
    private static readonly string[] walkableTags = new string[] { UnityTag.Ground, UnityTag.Drawing };
    private readonly Dictionary<Collider2D, Collision2D> _collisionsByCollider = new Dictionary<Collider2D, Collision2D>();
    private ContactPoint2D _lastContact;


    // Initialization
    private void Awake()
    {
        RigidBody = GetComponent<Rigidbody2D>();
        PlayerFeet = GetComponentInChildren<PlayerFeet>();
        PlayerBody = GetComponentInChildren<PlayerBody>();
        AnimatorController = PlayerBody.GetComponent<Animator>();
        _gravityScale = RigidBody.gravityScale;
        InitializeJumpingStates();
        InitializeFeet();
        InitializeBody();
    }

    private void InitializeBody()
    {
        //PlayerBody.Collision.Enter.Subscribe(OnBodyCollisionEnter);
        PlayerBody.Collision.Exit.Subscribe(OnBodyCollisionExit);
        PlayerBody.CollisionStay.Subscribe(OnBodyCollisionStay);
        PlayerBody.ColliderTrigger.Enter.SubscribeWithTag(UnityTag.ClimbingWall, OnClimbingWallEnter);
        PlayerBody.ColliderTrigger.Exit.SubscribeWithTag(UnityTag.ClimbingWall, OnClimbingWallExit);
    }

    private void InitializeFeet()
    {
        PlayerFeet.ColliderTrigger.Enter.SubscribeWithTag
        (
            (UnityTag.Ground, OnGroundEnter),
            (UnityTag.Drawing, OnDrawingEnter)
        );

        PlayerFeet.ColliderTrigger.Exit.SubscribeWithTag
        (
            (UnityTag.Ground, OnGroundExit),
            (UnityTag.Drawing, OnDrawingExit)
        );

        foreach (var walkableTag in walkableTags)
        {
            PlayerFeet.ColliderTrigger.Enter.SubscribeWithTag(walkableTag, OnWalkableEnter);
            PlayerFeet.ColliderTrigger.Exit.SubscribeWithTag(walkableTag, OnWalkableExit);
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
    public Animator AnimatorController { get; private set; }
    public bool IsDoubleJumpEnabled { get => _isDoubleJumpEnabled; set => _isDoubleJumpEnabled = value; }
    public float HorizontalMovementDirection { get; set; }
    public float VerticalMovementDirection { get; set; }
    public bool IsFacingRight { get; private set; } = true;
    public JumpState CurrentJumpState { get; private set; }
    public bool IsClimbing => ClimbingWallCollidedCount > 0;
    public bool IsGrounded => GroundsCollidedCount > 0;
    public bool IsOnDrawingPlatform => DrawingPlatformsCollidedCount > 0;
    public bool CanJump => IsGrounded || IsOnDrawingPlatform || (IsClimbing && CurrentJumpState == JumpState.NoJumping);
    public bool IsMovinghorizontally => Mathf.Abs(HorizontalMovementDirection) > 0 && enabled;
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
    public float CurrentAngleRadians => Mathf.Acos(Mathf.Abs(_lastContact.normal.y)); //  Vector2.Angle(_lastContact.normal, Vector2.up);



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


    // Trigger Collisions
    private void OnGroundEnter(Collider2D collider) => GroundsCollidedCount++;
    private void OnGroundExit(Collider2D collider) => this.InvokeFrameDelayed(() => GroundsCollidedCount--, _jumpDelaySensitivity);
    private void OnDrawingEnter(Collider2D collider) => DrawingPlatformsCollidedCount++;
    private void OnDrawingExit(Collider2D collider) => this.InvokeFrameDelayed(() => DrawingPlatformsCollidedCount--, _jumpDelaySensitivity);




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
        if (!IsClimbing)
            _fixedUpdateAction += () => IsGravityEnabled = true;
    }



    // Real Collisions

    private void OnBodyCollisionStay(Collision2D collision)
    {
        if (!_collisionsByCollider.ContainsKey(collision.otherCollider))
            _collisionsByCollider.Add(collision.otherCollider,collision);
        _collisionsByCollider[collision.otherCollider] = collision;
        _lastContact = collision.GetContact(0);
    }

    private void OnBodyCollisionExit(Collision2D collision)
    {
        _collisionsByCollider.Remove(collision.otherCollider);
    }




    // Unity events   
    private void FixedUpdate()
    {
        SetFriction();
        DoFixedUpdateActions();
        MoveHorizontal();
        MoveVertical();
        PlaySounds();
        UpdateAnimations();
    }






    // JumpState handlers
    private void OnJump_WhileNoJumping()
    {
        if (CanJump)
        {
            CurrentJumpState = JumpState.Jumping;
            VariableJump().Forget();
        }
    }

    private void OnJump_WhileJumping()
    {
        if (IsDoubleJumpEnabled)
            DoubleJump();
    }




    /// <summary>
    /// Implements a variable height jump.
    /// Add a force of <see cref="_jumpStepForce"/> magnitude every <see cref="_jumpHoldStepMs"/> ms if the jump input is held
    /// until <see cref="_maxJumpForce"/> is reached.
    /// </summary>
    /// <returns> The jump coroutine </returns>
    private async UniTaskVoid VariableJump()
    {
        float totForce = 0;
        bool jumpHeld;
        await UniTask.WaitForFixedUpdate();
        RigidBody.velocity = new Vector2(RigidBody.velocity.x, 0);
        IsGravityEnabled = true;
        do
        {
            float forceIncrement = Mathf.Min(_jumpStepForce, _maxJumpForce - totForce);
            await this.WaitForFixedUpdate();
            RigidBody.AddForce(forceIncrement * Vector2.up);
            await this.Delay((int)_jumpHoldStepMs);
            jumpHeld = _jumpHeldCondition?.Invoke() ?? false;
            totForce += forceIncrement;
        }
        while (jumpHeld && totForce < _maxJumpForce && CurrentJumpState == JumpState.Jumping);
    }




    // Private methods     
    private void SetFriction()
    {
        PlayerBody.Friction = (IsGrounded || IsOnDrawingPlatform) && Mathf.Approximately(HorizontalMovementDirection, 0)
                    ? _frictionWhenIdle
                    : 0;        
    }

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
        if (IsGrounded || IsOnDrawingPlatform)
        {
            // Scale velocity according to platforms/drawings angle 
            var descentHorizontalDirection = _lastContact.normal.x;
            if (descentHorizontalDirection * HorizontalMovementDirection < 0) // Not Same direction
                HorizontalMovementDirection *= Mathf.Cos(CurrentAngleRadians); // Apply pseudo-friction
        }


        float xVelocity = HorizontalMovementDirection * _speed;
        RigidBody.velocity = new Vector2(xVelocity, RigidBody.velocity.y);

        // If the player change direction flip the sprite
        if ((xVelocity > 0 && !IsFacingRight) || (xVelocity < 0 && IsFacingRight))
            Flip();

        AnimatorController.SetFloat(PlayerAnimatorParameters.VelocityX, Mathf.Abs(xVelocity));
    }


    /*
    private bool IsForbiddenDirection(float horizontalDirection)
    {
        var boxSize = new Vector2(PlayerBody.ColliderSize.x / 2 * 1.02f, PlayerBody.ColliderSize.y * 0.9f);
        var center = PlayerBody.ColliderCenter + PlayerBody.ColliderSize.x / 4 * Vector2.right;
        var collidersHit = Physics2D.OverlapBoxAll(center, boxSize, LayerMask.NameToLayer(UnityTag.NonWalkable));
        foreach (var collider in collidersHit)
            if (_collisionsByCollider.TryGetValue(collider, out var collision))
            {
                var problemContacts = from contact in collision.contacts
                                      let angle = Vector2.Angle(contact.normal, Vector2.up)
                                      where angle > _verticalAngle
                                      where IsInsideBox(contact.point, center, boxSize)
                                      where Vector2.Dot(contact.point - RigidBody.position, horizontalDirection * Vector2.right) > 0
                                      select contact;
                if (problemContacts.Any())
                    return true;
            }
        return false;
    }
    */

    private bool IsInsideBox(Vector2 point, Vector2 center, Vector2 size)
    {
        var delta = point - center;
        return delta.x < size.x && delta.y < size.y;
    }


    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        IsFacingRight = !IsFacingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private void MoveVertical()
    {

        float yVelocity = VerticalMovementDirection * _climbingSpeed;
        if (IsClimbing && CanJump)
        {
            if(yVelocity > 0)
            {
                var topCollider = PlayerBody.ColliderTrigger.GetInsideWithTag(UnityTag.ClimbingWall)
                                                            .WithMaxOrDefault(coll => coll.bounds.center.y);
                const float delta = 0.03f;
                var yLimit = topCollider.bounds.center.y + topCollider.bounds.extents.y - delta;
                if (RigidBody.position.y > yLimit )
                    yVelocity = 0;
            }
            RigidBody.velocity = new Vector2(RigidBody.velocity.x, yVelocity);
        }

        AnimatorController.SetFloat(PlayerAnimatorParameters.VelocityY, Mathf.Abs(yVelocity));
    }




    // Sounds
    private void PlaySounds()
    {
        if (IsMovinghorizontally && IsGrounded && !_horizontalMovementSoundTaskRunning)
            PlayHorizontalMovementSound().Forget();
    }

    private async UniTask PlayHorizontalMovementSound()
    {
        _horizontalMovementSoundTaskRunning = true;
        while (IsMovinghorizontally && IsGrounded)
            await this.PlaySound(SoundEffects.CharacterMovement.RandomOrDefault());
        _horizontalMovementSoundTaskRunning = false;
    }




    // Enums

    private void HandleJump()
    {
        if (_onJumpHandlers.TryGetValue(CurrentJumpState, out Action jumpStateHandler))
            jumpStateHandler.Invoke();
    }

    private void UpdateAnimations()
    {
        AnimatorController.SetBool(PlayerAnimatorParameters.Grounded, IsGrounded || IsOnDrawingPlatform);
        AnimatorController.SetBool(PlayerAnimatorParameters.Climbing, IsClimbing);
    }

    public enum JumpState
    {
        Jumping,
        DoubleJumping,
        NoJumping,
    }

    private class ForbiddenDirections
    {
        public bool Right { get; set; }
        public bool Left { get; set; }
    }
}
