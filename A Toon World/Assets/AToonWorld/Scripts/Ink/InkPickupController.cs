using System.Collections;
using System.Collections.Generic;
using Assets.AToonWorld.Scripts;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class InkPickupController : MonoBehaviour
{
    //TODO: InkPickup respawn?
    [SerializeField] private float _refillQuantity = 50.0f;
    public float RefillQuantity => _refillQuantity;
    [SerializeField] private bool _refillIsPercentage = false;
    public bool IsPercentage => _refillIsPercentage;
    [SerializeField] private PlayerInkController.InkType _inkType = PlayerInkController.InkType.Construction;
    public PlayerInkController.InkType InkType => _inkType;

    //Director Properties
    [SerializeField] private bool _isDirected = true;
    public bool IsDirected => _isDirected;
    [SerializeField] private float _respawnThreshold = 0.0f;
    public float RespawnThreshold => _respawnThreshold;

    void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.CompareTag(UnityTag.Player))
        {
            //Animations?
            gameObject.SetActive(false);
        }
    }

    public virtual void RequestEnable()
    {
        this.gameObject.SetActive(true);
    }
}
