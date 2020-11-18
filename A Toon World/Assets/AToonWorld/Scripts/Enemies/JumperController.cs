using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.AToonWorld.Scripts;
using UnityEngine;

public interface IJump
{
    void Jump();
}

public class JumperController : MonoBehaviour
{
    public enum JumMode { FixedHeight, ToFixedY }

    [Header("Jump Mode")]
    [SerializeField] private JumMode _jumpMode;
    [SerializeField] private float _value = 0f;

    [Header("Interleaving seconds")]
    [SerializeField] private float _secondsIntoDarkLake = 2f; //Seconds before next jump
    [SerializeField] private float _secondsBeforeFirstJump = 2f;
    
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
        _jumpVelocity = CalculateVelocity(_jumpMode == JumMode.FixedHeight ? _value : _value - transform.position.y);
        StartCoroutine(Jump(_secondsBeforeFirstJump));
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
                StartCoroutine(Jump(_secondsIntoDarkLake));
            else
                StartFirstJumpSession();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(UnityTag.Drawing))
            other.gameObject.SetActive(false);
    }
}
