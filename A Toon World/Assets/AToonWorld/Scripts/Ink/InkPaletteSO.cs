using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerInkController;

[CreateAssetMenu(fileName = "InkPaletteSO", menuName = "Inkverse/Inks/Palette Settings", order = 1)]
public class InkPaletteSO : ScriptableObject
{
    public InkType SelectedInk { get; set; }
    [SerializeField] private List<ScriptableInkHandler> _inkPalette;

    public List<ScriptableInkHandler> InkPalette => _inkPalette;
}
