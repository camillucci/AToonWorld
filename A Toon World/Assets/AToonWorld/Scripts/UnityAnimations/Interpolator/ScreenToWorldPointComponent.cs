using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Translates current transform from screen to worldposition
/// </summary>
public class ScreenToWorldPointComponent : InterpolatorDynamicPointComponent
{
    private Transform _transform;

    private void Awake() 
    {
        _transform = this.transform;
    }

    public override Vector3 GetDynamicPoint() => Camera.main.ScreenToWorldPoint(_transform.position);
}
