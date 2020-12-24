using System;
using Assets.AToonWorld.Scripts.Level;
using UnityEngine;
using UnityEngine.Events;
[CreateAssetMenu(fileName = "ExpendableResource", menuName = "Inkverse/Resources/Expendable Resource", order = 1)]
public class ExpendableResource : ScriptableObject
{
    public event Action OnCapacityChanged;

    private float _capacity;
    
    [SerializeField] private float _maxCapacity = 25;
    public float MaxCapacity => _maxCapacity;

    public float Capacity => _capacity;

    private float _splineInkMultiplier = 1f;
    private float _bulletInKMultiplier = 5f;

    public ExpendableResource()
    {
        this._capacity = 0.0f;
    }

    public virtual void SetCapacity(float newCapacity)
    {
        if(newCapacity < 0.0f)
            throw new System.Exception("Tried to set a negative capacity");
        if(newCapacity > this.MaxCapacity)
            throw new System.Exception("Tried to set more capacity than allowed");

        this._capacity = newCapacity;
        OnCapacityChanged?.Invoke();
    }

    /// <summary>
    /// Tries to consume a quantity of this resource
    /// </summary>
    /// <param name="quantity">Quantity to consume</param>
    /// <returns>Returns the quantity consumed</returns>
    public virtual float Consume(float quantity)
    {
        if(quantity < 0.0f)
            throw new System.Exception("Tried to consume a negative quantity");

        float consumedCapacity = this._capacity - quantity;
        this.SetCapacity(consumedCapacity > 0 ? consumedCapacity : 0.0f);
        InkUsageManager.InkQuantityChanged.Invoke(quantity * _splineInkMultiplier);
        return consumedCapacity > 0.0f ? quantity : quantity + consumedCapacity;
    }

    /// <summary>
    /// Tries to consume a quantity of this resource, don't consume if the requested quantity is too much
    /// </summary>
    /// <param name="quantity">Quantity to consume</param>
    /// <returns>Returns if there's enough capacity to consume</returns>
    public virtual bool CanConsume(float quantity) => this._capacity - quantity > 0.0f;

    /// <summary>
    /// Consumes the requested quantity only if there's enough capacity
    /// </summary>
    /// <param name="quantity">Quantity to consume</param>
    /// <returns>Returns if the requested quantity was consumed</returns>
    public virtual bool ConsumeOrFail(float quantity)
    {
        if(quantity < 0.0f)
            throw new System.Exception("Tried to consume a negative quantity");

        float consumedCapacity = this._capacity - quantity;
        if(consumedCapacity >= 0.0f)
        {
            this.SetCapacity(consumedCapacity);
            InkUsageManager.InkQuantityChanged.Invoke(quantity * _bulletInKMultiplier);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Refills the current resource for a given quantity to MaxCapacity
    /// </summary>
    /// <param name="quantity">Maximum quantity to refill</param>
    public virtual void Refill(float quantity)
    {
        if(quantity < 0.0f)
            throw new System.Exception("Tried to refill a negative quantity");

        float newQuantity = this._capacity + quantity;
        this.SetCapacity(newQuantity > this.MaxCapacity ? this.MaxCapacity : newQuantity);
    }

    public virtual void Refill()
    {
        this.SetCapacity(this.MaxCapacity);
    }
}