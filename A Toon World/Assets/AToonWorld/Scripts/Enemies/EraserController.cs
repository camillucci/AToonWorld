using System.Collections;
using System.Collections.Generic;
using Assets.AToonWorld.Scripts;
using UnityEngine;

public class EraserController : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(UnityTag.Drawing))
            other.gameObject.SetActive(false);
    }
}
