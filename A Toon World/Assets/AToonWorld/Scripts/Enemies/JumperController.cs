using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IJump
{
    void Jump();
}

public class JumperController : MonoBehaviour
{
    [SerializeField] private float SecondsIntoDarkLake = 2f; //Seconds before next jump
    [SerializeField] private float SecondsBeforeFirstJump = 2f;
    private bool _jump;
    private IJump _jumpController;
    
    void Start()
    {
        _jump = false;
        _jumpController = GetComponent<IJump>();
        StartCoroutine(EnableFirstJump());
    }

    void FixedUpdate()
    {
        if (_jump)
        {
            _jump = false;
            _jumpController.Jump();
            StartCoroutine(EnableNextJump());
        }
    }

    IEnumerator EnableFirstJump()
    {
        yield return new WaitForSeconds(SecondsBeforeFirstJump);
        _jump = true;
        yield return null;
    }

    IEnumerator EnableNextJump()
    {
        yield return new WaitForSeconds(SecondsIntoDarkLake);
        _jump = true;
        yield return null;
    }
}
