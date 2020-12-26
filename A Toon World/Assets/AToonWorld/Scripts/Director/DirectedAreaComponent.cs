using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.AToonWorld.Scripts;
using UnityEngine;

public class DirectedAreaComponent : MonoBehaviour
{
    private InkDirector _inkDirector;
    private List<InkPickupController> _childPickups;

    // Instruct the GameDirector to remove pickups if the player has enough ink
    [SerializeField] private bool _limitedResourcesArea = true;

    void Start()
    {
        //At the start of the levels finds it's children
        _childPickups = new List<InkPickupController>(GetComponentsInChildren<InkPickupController>().Where(pickup => pickup.IsDirected));
        _inkDirector = FindObjectOfType<InkDirector>();
    }

    public void ProcessChildren(Action<InkPickupController, bool> processAction) => _childPickups.ForEach(pickup => processAction.Invoke(pickup, _limitedResourcesArea));

    void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.CompareTag(UnityTag.Player))
            _inkDirector.OnDirectedAreaActivated(this);
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if(collider.CompareTag(UnityTag.Player))
            _inkDirector.OnDirectedAreaDeactivated(this);
    }
}
