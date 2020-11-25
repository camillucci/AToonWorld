using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CanvasController : MonoBehaviour
{
    public UnityEvent CanvasSizeChanged;
    
    private void OnRectTransformDimensionsChange()
    {
        CanvasSizeChanged.Invoke();
    }
}
