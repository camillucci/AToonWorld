using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using InkType = PlayerInkController.InkType;

public class InkSelectorController : MonoBehaviour
{
    [SerializeField] private GameObject ConstructorInk;   
    [SerializeField] private GameObject ClearInk;
    [SerializeField] private GameObject DamageInk;
    [SerializeField] private GameObject ClimbInk;
    
    private UnityEvent<InkType> InkSelect;
    private Dictionary<InkType, GameObject> inks;

    void Start()
    {
        inks = new Dictionary<InkType, GameObject>()
        {
            [InkType.Construction] = ConstructorInk,
            [InkType.Cancel] = ClearInk,
            [InkType.Damage] = DamageInk,
            [InkType.Climb] = ClimbInk,
        };

        this.InkSelect.AddListener(selectedInk => 
        {
            foreach(InkType ink in Enum.GetValues(typeof(InkType)))
                if (ink.Equals(selectedInk))
                    Select(ink);
                else
                    UnSelect(ink);
        });
    }

    public void InvokeInkSelection(InkType inkType)
    {
        this.InkSelect.Invoke(inkType);
    }

    private void Select(InkType inkType)
    {
        inks[inkType].GetComponent<Image>().color = Color.red;
    }

    private void UnSelect(InkType inkType)
    {
        inks[inkType].GetComponent<Image>().color = Color.white;
    }
}
