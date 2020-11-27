using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using InkType = PlayerInkController.InkType;
using Events;

public class InkSelectorController : MonoBehaviour
{
    [SerializeField] private Color _selectedTint = Color.red;
    [SerializeField] private Color _emptyTint = Color.grey;
    [SerializeField] private GameObject ConstructorInkContainer = null;   
    [SerializeField] private GameObject ClimbInkContainer = null;  
    [SerializeField] private GameObject ClearInkContainer = null;
    [SerializeField] private GameObject DamageInkContainer = null;
    [SerializeField] private UIInkGauge ConstructionInkGauge = null;   
    [SerializeField] private UIInkGauge ClimbInkGauge = null; 
    [SerializeField] private UIInkGauge DamageInkGauge = null;   
    private Dictionary<InkType, Image> _inksImages;
    private Dictionary<InkType, UIInkGauge> _inkGauges;

    void Awake()
    {
        _inksImages = new Dictionary<InkType, Image>()
        {
            [InkType.Construction] = ConstructorInkContainer.GetComponent<Image>(),
            [InkType.Climb] = ClimbInkContainer.GetComponent<Image>(),
            [InkType.Cancel] = ClearInkContainer.GetComponent<Image>(),
            [InkType.Damage] = DamageInkContainer.GetComponent<Image>()
        };

        _inkGauges = new Dictionary<InkType, UIInkGauge>()
        {
            [InkType.Construction] = ConstructionInkGauge,
            [InkType.Climb] = ClimbInkGauge,
            [InkType.Damage] = DamageInkGauge,
        };

        InterfaceEvents.InkSelected.AddListener(selectedInk => ChangeSelectedInk(selectedInk));
        InterfaceEvents.InkCapacityChanged.AddListener(tuple => ChangeInkCapacity(tuple.Item1, tuple.Item2));
    }

    private void ChangeSelectedInk(InkType selectedInk)
    {
        foreach(InkType ink in Enum.GetValues(typeof(InkType)))
            if (ink.Equals(selectedInk))
                Select(ink);
            else
                UnSelect(ink);
    }

    private void Select(InkType inkType)
    {
        _inksImages[inkType].color = _selectedTint;
    }

    private void UnSelect(InkType inkType)
    {
        Empty(inkType);
    }

    private void Empty(InkType inkType)
    {
        _inksImages[inkType].color = _emptyTint;
    }
    
    private void ChangeInkCapacity(InkType inkType, float capacity)
    {
        #if UNITY_EDITOR
        if (capacity < 0 || capacity > 1)
            throw new Exception("Ink capacity not normalized, it should be in [0, 1]");
        #endif

        _inkGauges[inkType].SetFillAmmount(capacity);

        if(capacity == 0)
            Empty(inkType);
    }
}
