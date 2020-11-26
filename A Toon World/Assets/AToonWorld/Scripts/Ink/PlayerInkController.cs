﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.AToonWorld.Scripts.Utils;
using Assets.AToonWorld.Scripts.Player;
using Assets.AToonWorld.Scripts;
using Events;
using System;

public class PlayerInkController : MonoBehaviour
{
    [SerializeField] private InkPaletteSO _inkPaletteSettings;

    private PlayerBody _playerBody;
    private Vector2 _mouseWorldPosition;
    private Dictionary<InkType, IInkHandler> _inkHandlers;
    public InkType SelectedInk => _inkPaletteSettings.SelectedInk;
    public bool IsDrawing { get; private set; } = false;

    void Awake()
    {
        //Parse degli InkHandler caricati da ScriptableObject
        _inkHandlers = new Dictionary<InkType, IInkHandler>();
        _inkPaletteSettings.InkPalette.ForEach(inkHandler => {
            _inkHandlers.Add(inkHandler.InkType, inkHandler);
            ObjectPoolingManager<InkType>.Instance.CreatePool(inkHandler.InkType, inkHandler.InkPrefab, inkHandler.MinPoolSize, inkHandler.MaxPoolSize, true);
        });

        _playerBody = GetComponentInChildren<PlayerBody>();
        _playerBody.TriggerEnter.SubscribeWithTag(UnityTag.InkPickup, OnInkPickup);

        //ObjectPoolingManager<InkType>.Instance.CreatePool(InkType.Construction, _constructionInkPrefab, 50, 200, true);
        //ObjectPoolingManager<InkType>.Instance.CreatePool(InkType.Climb, _climbingInkPrefab, 20, 50, true);
        //ObjectPoolingManager<InkType>.Instance.CreatePool(InkType.Damage, _damageInkPrefab, 20, 50, true);
        //ObjectPoolingManager<InkType>.Instance.CreatePool(InkType.Cancel, _cancelInkPrefab, 1, 2, true);

        Events.InterfaceEvents.InkSelectionRequested.AddListener(OnInkSelected);
    }

    private void OnDestroy() 
    {
        _playerBody.TriggerEnter.UnSubscribeWithTag(UnityTag.InkPickup, OnInkPickup);
        Events.InterfaceEvents.InkSelectionRequested.RemoveListener(OnInkSelected);
    }
    
    void Start()
    {
        OnInkSelected(InkSelection.Forward);
        LoadInkState(new List<(InkType, float)> { (InkType.Construction, 0),
                                                  (InkType.Climb, 0),
                                                  (InkType.Damage, 0)});
    }

    public void LoadInkState(List<(InkType, float)> savedState)
    {
        savedState.ForEach(savedInk => 
        {
            if (_inkHandlers[savedInk.Item1] is ScriptableExpendableInkHandler expendableInk)
                expendableInk.Expendable.SetCapacity(savedInk.Item2);
        });
    }

    public void OnInkSelected(InkType newInk)
    {
        if(!IsDrawing)
        {
            if (IsAvailableInk(newInk))
            {
                _inkPaletteSettings.SelectedInk = newInk;
                InterfaceEvents.InkSelected.Invoke(_inkPaletteSettings.SelectedInk);
            }
        }
    }

    public void OnInkSelected(InkSelection inkSelection)
    {
        if(!IsDrawing)
        {
            int totalInks = Enum.GetValues(typeof(InkType)).Length;
            int nextInk = ((int)_inkPaletteSettings.SelectedInk + (int)inkSelection + totalInks) % totalInks;
            for (int i = 0; i < totalInks; i++)
            {
                if (IsAvailableInk((InkType)nextInk))
                    break;
                nextInk = ((int)nextInk + (int)inkSelection + totalInks) % totalInks;
            }

            _inkPaletteSettings.SelectedInk = (InkType)nextInk;
            InterfaceEvents.InkSelected.Invoke(_inkPaletteSettings.SelectedInk);
        }
    }

    private bool IsAvailableInk(InkType ink) => _inkHandlers[ink] is ScriptableExpendableInkHandler expendable ? expendable.CurrentCapacity > 0 : true;

    private void OnInkPickup(Collider2D collider)
    {
        InkPickupController pickupController = collider.GetComponent<InkPickupController>();
        if(pickupController != null)
        {
            if(pickupController.IsPercentage && pickupController.RefillQuantity > 100)
                throw new System.Exception("Picked up refill with percentage higher than 100%");

            if(_inkHandlers[pickupController.InkType] is ScriptableExpendableInkHandler expendableInk)
                expendableInk.Expendable.Refill(pickupController.IsPercentage ? expendableInk.MaxCapacity * (pickupController.RefillQuantity / 100) 
                                                                   : pickupController.RefillQuantity);
            else
                throw new System.Exception("Picked up refill for non expendable ink!");
        }
    }

    public void OnDrawDown()
    {
        if(IsDrawing)
            return;

        IsDrawing = true;
        _mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        GameObject pooledSpline = ObjectPoolingManager<InkType>.Instance.GetObject(_inkPaletteSettings.SelectedInk);
        
        if(_inkHandlers[_inkPaletteSettings.SelectedInk] is ISplineInk _selectedSplineInk)
            _selectedSplineInk?.BindSpline(pooledSpline.GetComponent<DrawSplineController>());
        else if (_inkHandlers[_inkPaletteSettings.SelectedInk] is IBulletInk _selectedBulletInk)
            _selectedBulletInk?.BindBulletAndPosition(pooledSpline.GetComponent<BulletController>(), transform.position);

        _inkHandlers[_inkPaletteSettings.SelectedInk].OnDrawDown(_mouseWorldPosition);
    }

    public void WhileDrawHeld()
    {
        if(IsDrawing)
        {
            _mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if(!_inkHandlers[_inkPaletteSettings.SelectedInk].OnDrawHeld(_mouseWorldPosition))
                OnDrawReleased();
        }
    }

    public void OnDrawReleased()
    {
        if(IsDrawing)
        {
            _inkHandlers[_inkPaletteSettings.SelectedInk].OnDrawReleased(_mouseWorldPosition);
            IsDrawing = false;
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
