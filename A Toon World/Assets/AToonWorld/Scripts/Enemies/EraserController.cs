using System.Collections;
using System.Collections.Generic;
using Assets.AToonWorld.Scripts;
using UnityEngine;

public class EraserController : MonoBehaviour, IKillable
{
    [SerializeField] private MonoBehaviour _behaviourParent;
    private IKillable _killableParent;

    private void Start() {
        if(_behaviourParent is IKillable)
            _killableParent = (IKillable)_behaviourParent;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(UnityTag.Drawing))
            other.gameObject.SetActive(false);
    }

    public void Kill()
    {
        _killableParent?.Kill();
    }
}
