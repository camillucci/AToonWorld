using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Componenet used by the interpolator to get points that need special logic (for example that need a coordinate conversion)
/// </summary>
public abstract class InterpolatorDynamicPointComponent : MonoBehaviour
{
    public abstract Vector3 GetDynamicPoint();
}
