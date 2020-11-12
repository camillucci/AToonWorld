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
    [SerializeField] private GameObject ConstructorInk;   
    [SerializeField] private GameObject ClimbInk;
    [SerializeField] private GameObject ClearInk;
    [SerializeField] private GameObject DamageInk;
    
    //TODO: se ancora inutilizzato rimuovere
    private Dictionary<InkType, GameObject> _inks;
    private Dictionary<InkType, Image> _inksImages;

    void Awake()
    {
        _inks = new Dictionary<InkType, GameObject>()
        {
            [InkType.Construction] = ConstructorInk,
            [InkType.Climb] = ClimbInk,
            [InkType.Cancel] = ClearInk,
            [InkType.Damage] = DamageInk
        };

        _inksImages = new Dictionary<InkType, Image>()
        {
            [InkType.Construction] = ConstructorInk.GetComponent<Image>(),
            [InkType.Climb] = ClimbInk.GetComponent<Image>(),
            [InkType.Cancel] = ClearInk.GetComponent<Image>(),
            [InkType.Damage] = DamageInk.GetComponent<Image>()
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
        Color newColor = Color.red;
        newColor.a = _inksImages[inkType].color.a;
        _inksImages[inkType].color = newColor;
    }

    private void UnSelect(InkType inkType)
    {
        Color newColor = Color.white;
        newColor.a = _inksImages[inkType].color.a;
        _inksImages[inkType].color = newColor;
    }
    
    private void ChangeInkCapacity(InkType inkType, float capacity)
    {
        #if UNITY_EDITOR
        if (capacity < 0 || capacity > 1)
            throw new Exception("Ink capacity not normalized, it should be in [0, 1]");
        #endif

        // TODO: Se manteniamo la gestione con l'alpha allora cachare il getComponent
        Color selectedInkColor = _inksImages[inkType].color;
        _inksImages[inkType].color = new Color(selectedInkColor.r, selectedInkColor.g, selectedInkColor.b, capacity);
    }
}
