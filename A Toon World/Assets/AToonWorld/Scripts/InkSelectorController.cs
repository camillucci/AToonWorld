using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using InkType = PlayerInkController.InkType;
using EventMessaging;

public class InkSelectorController : MonoBehaviour
{
    [SerializeField] private GameObject ConstructorInk;   
    [SerializeField] private GameObject ClearInk;
    [SerializeField] private GameObject DamageInk;
    [SerializeField] private GameObject ClimbInk;
    
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

        EventMessenger.Subscribe(Events.InkSelected, selectedInk => 
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
        EventMessenger.Invoke(Events.InkSelected, inkType);
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
