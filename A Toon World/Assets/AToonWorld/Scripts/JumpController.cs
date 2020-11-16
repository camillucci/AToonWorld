using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpController : MonoBehaviour, IJump
{
    public void Jump()
    {
        Debug.Log("Jump!");
    }
}