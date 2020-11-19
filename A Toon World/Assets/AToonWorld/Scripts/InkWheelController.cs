using System;
using System.Collections;
using System.Collections.Generic;
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

    [SerializeField] private GameObject ConstructionInk;   
    [SerializeField] private GameObject ClimbInk;
    [SerializeField] private GameObject CancelInk;
    [SerializeField] private GameObject DamageInk;

    private Dictionary<InkType, Image> _inksImages;
    private Dictionary<InkType, InkPosition> _inksPositions;
    private Vector2 _centerPosition;
    private InkType? _selected;
    [SerializeField] private float _mouseDeadzone = 15f;
    
    void Start()
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
            [InkType.Construction] = ConstructionInk.GetComponent<Image>(),
            [InkType.Climb] = ClimbInk.GetComponent<Image>(),
            [InkType.Cancel] = CancelInk.GetComponent<Image>(),
            [InkType.Damage] = DamageInk.GetComponent<Image>()
        };

        _mouseDeadzone /= transform.localScale.x;
    }

    public void Show()
    {
        this._centerPosition = Input.mousePosition;
        this.transform.position = _centerPosition;
        this.gameObject.SetActive(true);
        UnSelectCurrent();
        this._selected = null;
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
        if (_selected.HasValue)
            Events.InterfaceEvents.InkSelectionRequested.Invoke(_selected.Value);
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
                bool otherInkSelected = !_selected.HasValue || inkType != _selected.Value;
                if (otherInkSelected && mouseOverArea)
                {
                    UnSelectCurrent();
                    _selected = inkType;
                    SelectCurrent();
                    return;
                }
            }
        }
    }

    private void SelectCurrent()
    {
        if (_selected.HasValue)
        {
            Color newColor = Color.red;
            newColor.a = _inksImages[_selected.Value].color.a;
            _inksImages[_selected.Value].color = newColor;
        }
    }

    private void UnSelectCurrent()
    {
        if (_selected.HasValue)
        {
           Color newColor = Color.white;
           newColor.a = _inksImages[_selected.Value].color.a;
           _inksImages[_selected.Value].color = newColor;
        }
    }
}
