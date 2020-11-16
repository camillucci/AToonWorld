using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.AToonWorld.Scripts;
using UnityEngine;

public class DirectedAreaComponent : MonoBehaviour
{
    private List<InkPickupController> _childPickups;

    void Start()
    {
        //At the start of the levels finds it's childs
        _childPickups = new List<InkPickupController>(GetComponentsInChildren<InkPickupController>().Where(pickup => pickup.IsDirected));
    }

    public void ProcessChilds(Action<InkPickupController> processAction) => _childPickups.ForEach(processAction);

    void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.CompareTag(UnityTag.Player))
            InkDirector.Instance.OnDirectedAreaActivated(this);
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if(collider.CompareTag(UnityTag.Player))
            InkDirector.Instance.OnDirectedAreaDeactivated(this);
    }
}
