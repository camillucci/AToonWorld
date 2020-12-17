using System.Collections;
using System.Collections.Generic;
using Assets.AToonWorld.Scripts;
using UnityEngine;

public class AreaController : MonoBehaviour
{
    [SerializeField] private bool _enabled = false;
    public bool InSights { get; set; }

    void Start()
    {
        InSights = !_enabled;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(UnityTag.Player))
            InSights = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(UnityTag.Player))
            InSights = !_enabled;
    }
}
