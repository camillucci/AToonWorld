using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.AToonWorld.Scripts.Utils;
using Assets.AToonWorld.Scripts.Player;
using Assets.AToonWorld.Scripts;
using Events;
using System;

public class PlayerInkController : MonoBehaviour
{
    //TODO: Logica selezione ink
    //TODO: Interfaccia e gestione ink
    [SerializeField] private GameObject _constructionInkPrefab = null;
    [SerializeField] private GameObject _climbingInkPrefab = null;
    [SerializeField] private GameObject _damageInkPrefab = null;
    [SerializeField] private GameObject _cancelInkPrefab = null;
    private PlayerBody _playerBody;
    private InkType _selectedInk = InkType.Construction;
    private bool _isDrawing = false;
    private Vector2 _mouseWorldPosition;
    private Dictionary<InkType, IInkHandler> _inkHandlers;
    public InkType SelectedInk => _selectedInk;

    void Awake()
    {
        //TODO: Handler assegnati da codice? Forse è meglio usare qualcosa di assegnabile da Editor? (da Editor non possiamo usare le interfacce)
        _inkHandlers = new Dictionary<InkType, IInkHandler>()
        {
            [InkType.Construction] = new ConstructionInkHandler(this), //Spline Ink
            [InkType.Climb] = new ClimbingInkHandler(this),
            [InkType.Damage] = new DamageInkHandler(this),
            [InkType.Cancel] = new CancelInkHandler(this), //Spline Ink (for now?)
            //TODO: Others
        };

        _playerBody = GetComponentInChildren<PlayerBody>();
        _playerBody.TriggerEnter.SubscribeWithTag(UnityTag.InkPickup, OnInkPickup);

        ObjectPoolingManager<InkType>.Instance.CreatePool(InkType.Construction, _constructionInkPrefab, 50, 200, true);
        ObjectPoolingManager<InkType>.Instance.CreatePool(InkType.Climb, _climbingInkPrefab, 20, 50, true);
        ObjectPoolingManager<InkType>.Instance.CreatePool(InkType.Damage, _damageInkPrefab, 20, 50, true);
        ObjectPoolingManager<InkType>.Instance.CreatePool(InkType.Cancel, _cancelInkPrefab, 1, 2, true);
    }
    
    void Start()
    {
        OnInkSelected(InkSelection.Forward);
        LoadInkState(new List<(InkType, float)> { (InkType.Construction, 0)});
    }

    public void LoadInkState(List<(InkType, float)> savedState)
    {
        savedState.ForEach(savedInk => 
        {
            if (_inkHandlers[savedInk.Item1] is ExpendableResource expendable)
                expendable.SetCapacity(savedInk.Item2);
        });
    }

    public void OnInkSelected(InkType newInk)
    {
        if(!_isDrawing)
        {
            if (IsAvailableInk(newInk))
            {
                _selectedInk = newInk;
                InterfaceEvents.InkSelected.Invoke(_selectedInk);
            }
        }
    }

    public void OnInkSelected(InkSelection inkSelection)
    {
        if(!_isDrawing)
        {
            int totalInks = Enum.GetValues(typeof(InkType)).Length;
            int nextInk = ((int)_selectedInk + (int)inkSelection + totalInks) % totalInks;
            for (int i = 0; i < totalInks; i++)
            {
                if (IsAvailableInk((InkType)nextInk))
                    break;
                nextInk = ((int)nextInk + (int)inkSelection + totalInks) % totalInks;
            }

            _selectedInk = (InkType)nextInk;
            InterfaceEvents.InkSelected.Invoke(_selectedInk);
        }
    }

    private bool IsAvailableInk(InkType ink) => _inkHandlers[ink] is ExpendableResource expendable ? expendable.Capacity > 0 : true;

    private void OnInkPickup(Collider2D collider)
    {
        InkPickupController pickupController = collider.GetComponent<InkPickupController>();
        if(pickupController != null)
        {
            if(pickupController.IsPercentage && pickupController.RefillQuantity > 100)
                throw new System.Exception("Picked up refill with percentage higher than 100%");

            if(_inkHandlers[pickupController.InkType] is ExpendableResource expendableInk)
                expendableInk.Refill(pickupController.IsPercentage ? expendableInk.MaxCapacity * (pickupController.RefillQuantity / 100) 
                                                                   : pickupController.RefillQuantity);
            else
                throw new System.Exception("Picked up refill for non expendable ink!");
        }
    }

    public void OnDrawDown()
    {
        if(_isDrawing)
            return;

        _isDrawing = true;
        _mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        GameObject pooledSpline = ObjectPoolingManager<InkType>.Instance.GetObject(_selectedInk);
        
        if(_inkHandlers[_selectedInk] is ISplineInk _selectedSplineInk)
            _selectedSplineInk?.BindSpline(pooledSpline.GetComponent<DrawSplineController>());
        else if (_inkHandlers[_selectedInk] is IBulletInk _selectedBulletInk)
            _selectedBulletInk?.BindBulletAndPosition(pooledSpline.GetComponent<BulletController>(), transform.position);

        _inkHandlers[_selectedInk].OnDrawDown(_mouseWorldPosition);
    }

    public void WhileDrawHeld()
    {
        if(_isDrawing)
        {
            _mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if(!_inkHandlers[_selectedInk].OnDrawHeld(_mouseWorldPosition))
                OnDrawReleased();
        }
    }

    public void OnDrawReleased()
    {
        if(_isDrawing)
        {
            _inkHandlers[_selectedInk].OnDrawReleased(_mouseWorldPosition);
            _isDrawing = false;
        }
    }

    public enum InkType {
        Construction = 0,
        Climb,
        Damage,
        Cancel
    }

    public enum InkSelection {
        Forward = 1,
        Backward = -1
    }
}
