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
    [SerializeField] private float mouseDeadzone = 50f;
    
    void Start()
    {
        _inksPositions = new Dictionary<InkType, InkPosition>()
        {
            [InkType.Construction] = new InkPosition() { startAngle = 135, endAngle = 180 },
            [InkType.Construction] = new InkPosition() { startAngle = -180, endAngle = -135 },
            [InkType.Climb] = new InkPosition() { startAngle = 45, endAngle = 135 },
            [InkType.Cancel] = new InkPosition() { startAngle = -135, endAngle = -45 },
            [InkType.Damage] = new InkPosition() { startAngle = -45, endAngle = 45 }
        };

        _inksImages = new Dictionary<InkType, Image>()
        {
            [InkType.Construction] = ConstructionInk.GetComponent<Image>(),
            [InkType.Climb] = ClimbInk.GetComponent<Image>(),
            [InkType.Cancel] = CancelInk.GetComponent<Image>(),
            [InkType.Damage] = DamageInk.GetComponent<Image>()
        };
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
        Vector2 mousePositionFromCenter = (Vector2)Input.mousePosition - _centerPosition;
        float angle = Mathf.Rad2Deg * Mathf.Atan2(mousePositionFromCenter.y, mousePositionFromCenter.x);
        float distance = mousePositionFromCenter.magnitude;
        
        if (distance > mouseDeadzone)
        {
            foreach(InkType inkType in _inksPositions.Keys)
            {
                InkPosition position = _inksPositions[inkType];
                if (inkType != _selected && angle > position.startAngle && angle < position.endAngle)
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
