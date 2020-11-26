using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.AToonWorld.Scripts;

public abstract class ScriptableInkHandler : ScriptableObject, IInkHandler
{
    protected PlayerInkController _playerInkController;
    
    #region Scriptable IInkHandler
    [SerializeField] private GameObject _inkPrefab;
    public GameObject InkPrefab =>_inkPrefab;

    [SerializeField] private PlayerInkController.InkType _inkType;
    public PlayerInkController.InkType InkType => _inkType;

    [SerializeField] private Color _inkColor;
    public Color InkColor =>_inkColor;

    [SerializeField] private int _minPoolSize;
    public int MinPoolSize => _minPoolSize;

    [SerializeField] private int _maxPoolSize;
    public int MaxPoolSize => _maxPoolSize;

    #endregion

    public void BindInkController(PlayerInkController playerInkController)
    {
        _playerInkController = playerInkController;
    }

    public abstract void OnDrawDown(Vector2 mouseWorldPosition);

    public abstract bool OnDrawHeld(Vector2 mouseWorldPosition);

    public abstract void OnDrawReleased(Vector2 mouseWorldPosition);
}