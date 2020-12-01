using System;
using System.Collections;
using System.Collections.Generic;
using Assets.AToonWorld.Scripts.UI;
using UnityEngine;
using UnityEngine.UI;
using InkType = PlayerInkController.InkType;

public class InkWheelController : MonoBehaviour
{
    struct InkPosition
    {
        public float startAngle;
        public float endAngle;
    }

    [SerializeField] private Color _selectionRequestTint = Color.red;
    [SerializeField] private Color _selectedTint = Color.white;
    [SerializeField] private Color _emptyTint = Color.grey;
    [SerializeField] private InkPaletteSO _inkPalette = null;
    [SerializeField] private GameObject ConstructionInkContainer = null;   
    [SerializeField] private GameObject ClimbInkContainer = null;
    [SerializeField] private GameObject CancelInkContainer = null;
    [SerializeField] private GameObject DamageInkContainer = null;
    [SerializeField] private UIInkGauge ConstructionInkGauge = null;   
    [SerializeField] private UIInkGauge ClimbInkGauge = null;
    [SerializeField] private UIInkGauge DamageInkGauge = null;

    private Dictionary<InkType, Image> _inksImages;
    private Dictionary<InkType, InkPosition> _inksPositions;
    private Dictionary<InkType, UIInkGauge> _inkGauges;
    private Vector2 _centerPosition;
    private InkType? _wantToSelect;
    private InkType? _currentlySelected;
    [SerializeField] private float _mouseDeadzone = 15f;
    
    void Awake()
    {
        _inksPositions = new Dictionary<InkType, InkPosition>()
        {
            [InkType.Construction] = new InkPosition() { startAngle = 0, endAngle = 90 },
            [InkType.Cancel] = new InkPosition() { startAngle = 90, endAngle = 180 },
            [InkType.Damage] = new InkPosition() { startAngle = 180, endAngle = 270 },
            [InkType.Climb] = new InkPosition() { startAngle = 270, endAngle = 360 }
        };

        _inksImages = new Dictionary<InkType, Image>()
        {
            [InkType.Construction] = ConstructionInkContainer.GetComponent<Image>(),
            [InkType.Climb] = ClimbInkContainer.GetComponent<Image>(),
            [InkType.Cancel] = CancelInkContainer.GetComponent<Image>(),
            [InkType.Damage] = DamageInkContainer.GetComponent<Image>()
        };

        _inkGauges = new Dictionary<InkType, UIInkGauge>()
        {
            [InkType.Construction] = ConstructionInkGauge,
            [InkType.Climb] = ClimbInkGauge,
            [InkType.Damage] = DamageInkGauge,
        };

        Events.InterfaceEvents.InkSelected.AddListener(NewInkSelected);

        _mouseDeadzone /= transform.localScale.x;
    }

    public void Show()
    {
        this._centerPosition = Input.mousePosition;
        this.transform.position = _centerPosition;
        this.gameObject.SetActive(true);
        UnSelectCurrent();
        UpdateShaders();
        this._wantToSelect = null;
        NewInkSelected(_inkPalette.SelectedInk);
        Events.InterfaceEvents.CursorChangeRequest.Invoke(CursorController.CursorType.Menu);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
        if (_wantToSelect.HasValue)
            Events.InterfaceEvents.InkSelectionRequested.Invoke(_wantToSelect.Value);
        Events.InterfaceEvents.CursorChangeRequest.Invoke(CursorController.CursorType.Game);
    }

    void Update()
    {
        Vector2 mousePositionFromCenter = transform.InverseTransformPoint(Input.mousePosition);
        float angle = Mathf.Rad2Deg * Mathf.Atan2(mousePositionFromCenter.y, mousePositionFromCenter.x) + 180;
        float distance = mousePositionFromCenter.magnitude;

        if (distance > _mouseDeadzone)
        {
            foreach(InkType inkType in _inksPositions.Keys)
            {
                InkPosition position = _inksPositions[inkType];
                bool mouseOverArea = angle > position.startAngle && angle < position.endAngle;
                bool otherInkSelected = !_wantToSelect.HasValue || inkType != _wantToSelect.Value;
                if (otherInkSelected && mouseOverArea)
                {
                    UnSelectCurrent();
                    _wantToSelect = inkType;
                    SelectCurrent();
                    return;
                }
            }
        }
    }

    private void OnDestroy() 
    {
        Events.InterfaceEvents.InkSelected.RemoveListener(NewInkSelected);
    }

    private void NewInkSelected(InkType newInk)
    {
        _currentlySelected = newInk;

        if(_currentlySelected.HasValue && _currentlySelected != _wantToSelect)
            _inksImages[_currentlySelected.Value].color = _selectedTint;

        foreach(InkType ink in _inksImages.Keys)
        {
            if(ink != newInk && ink != _wantToSelect)
                _inksImages[ink].color = _emptyTint;
        }
    }

    private void UpdateShaders()
    {
        _inkPalette.InkPalette.ForEach(inkHandler => {
            if(inkHandler is ScriptableExpendableInkHandler expendableInk && 
                _inkGauges.ContainsKey(expendableInk.InkType))
            {
                _inkGauges[expendableInk.InkType].UpdateShaderProperties();
                _inkGauges[expendableInk.InkType].SetFillAmount(expendableInk.CurrentCapacity/expendableInk.MaxCapacity);
            }
        });
    }

    private void SelectCurrent()
    {
        if (_wantToSelect.HasValue)
             _inksImages[_wantToSelect.Value].color = _selectionRequestTint;
    }

    private void UnSelectCurrent()
    {
        if (_wantToSelect.HasValue)
        {
            if(_wantToSelect.Value != _currentlySelected)
                _inksImages[_wantToSelect.Value].color = _emptyTint;
            else
                _inksImages[_wantToSelect.Value].color = _selectedTint;
        }
    }
}
