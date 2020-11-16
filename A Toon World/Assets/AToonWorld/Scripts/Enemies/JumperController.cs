using System.Collections;
using System.Collections.Generic;
using Assets.AToonWorld.Scripts;
using UnityEngine;

public interface IJump
{
    void Jump();
}

public class JumperController : MonoBehaviour
{
    [SerializeField] private float JumpHeight = 1f;
    [SerializeField] private float JumpToY = 1f;
    [SerializeField] private bool UseJumpHeight = false;
    [SerializeField] private float SecondsIntoDarkLake = 2f; //Seconds before next jump
    [SerializeField] private float SecondsBeforeFirstJump = 2f;
    
    private float _jumpVelocity;
    private bool _doneFirstJump;
    private Rigidbody2D _rigidBody;
    
    void Start()
    {
        _doneFirstJump = false;
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    private float CalculateVelocity(float height) => Mathf.Sqrt(2 * Mathf.Abs(Physics.gravity.y) * height);

    private void StartFirstJumpSession()
    {
        _jumpVelocity = CalculateVelocity(UseJumpHeight ? JumpHeight : JumpToY - transform.position.y);
        StartCoroutine(Jump(SecondsBeforeFirstJump));
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
        {
            if (_doneFirstJump)
                StartCoroutine(Jump(SecondsIntoDarkLake));
            else
                StartFirstJumpSession();
        }
    }
}
