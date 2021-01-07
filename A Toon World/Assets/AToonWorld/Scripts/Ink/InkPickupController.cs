using System.Collections;
using System.Collections.Generic;
using Assets.AToonWorld.Scripts;
using Assets.AToonWorld.Scripts.UI;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class InkPickupController : MonoBehaviour
{
    [SerializeField] private float _refillQuantity = 50.0f;
    public float RefillQuantity => _refillQuantity;
    [SerializeField] private bool _refillIsPercentage = false;
    public bool IsPercentage => _refillIsPercentage;
    [SerializeField] private PlayerInkController.InkType _inkType = PlayerInkController.InkType.Construction;
    public PlayerInkController.InkType InkType => _inkType;

    //Effects Properties
    [SerializeField] private int _pickupInkSparks = 5;
    [SerializeField] private Sprite _inkSparkSprite = null;
    [SerializeField] private Vector3 _inkSparkScale = Vector3.one;

    //Director Properties
    [SerializeField] private bool _isDirected = true;
    public bool IsDirected => _isDirected;
    [SerializeField] private float _respawnThreshold = 0.0f;
    public float RespawnThreshold => _respawnThreshold;

    private InkColorBindingController _colorBinder;

    private void Awake() 
    {
        _colorBinder = GetComponent<InkColorBindingController>();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.CompareTag(UnityTag.Player))
        {
            InGameUIController.PrefabInstance.WorldToUIEffects.SpawnEffects(_pickupInkSparks, _inkSparkSprite, _inkSparkScale, _colorBinder.BoundColor, this.transform, InGameUIController.PrefabInstance.InkSelector.GetDynamicPointConverter(_inkType));
            gameObject.SetActive(false);
        }
    }

    public virtual void RequestEnable()
    {
        this.gameObject.SetActive(true);
    }
}
