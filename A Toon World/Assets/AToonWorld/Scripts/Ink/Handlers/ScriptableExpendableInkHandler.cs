using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.AToonWorld.Scripts;

public abstract class ScriptableExpendableInkHandler : ScriptableInkHandler
{
    [SerializeField] protected ExpendableResource _expendableResource;

    public float MaxCapacity => _expendableResource.MaxCapacity;
    public float CurrentCapacity => _expendableResource.Capacity;
    public ExpendableResource Expendable => _expendableResource;

    //Called on Editor/Game start
    private void OnEnable() 
    {
        if(_expendableResource != null)
            _expendableResource.OnCapacityChanged += CapacityChanged;
    }

    //Called on scene changed when the SO is unreferenced
    private void OnDisable() 
    {
        if(_expendableResource != null)
            _expendableResource.OnCapacityChanged -= CapacityChanged;
    }

    private void CapacityChanged()
    {
        Events.InterfaceEvents.InkCapacityChanged.Invoke((InkType, CurrentCapacity/MaxCapacity));
        Events.InterfaceEvents.RawInkCapacityChanged.Invoke((InkType, CurrentCapacity));
    }
}