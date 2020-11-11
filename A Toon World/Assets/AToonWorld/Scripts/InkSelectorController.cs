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
    
    private Dictionary<InkType, GameObject> inks;

    void Start()
    {
        inks = new Dictionary<InkType, GameObject>()
        {
            [InkType.Construction] = ConstructorInk,
            [InkType.Climb] = ClimbInk,
            [InkType.Cancel] = ClearInk,
            [InkType.Damage] = DamageInk
        };

        InterfaceEvents.InkSelected.AddListener(selectedInk => 
        {
            foreach(InkType ink in Enum.GetValues(typeof(InkType)))
                if (ink.Equals(selectedInk))
                    Select(ink);
                else
                    UnSelect(ink);
        });
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
