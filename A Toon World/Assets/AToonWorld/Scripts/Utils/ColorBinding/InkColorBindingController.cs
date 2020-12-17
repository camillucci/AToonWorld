using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InkColorBindingController : MonoBehaviour
{
    [SerializeField] private ScriptableInkHandler _boundInkHandler = null;
    public Color BoundColor => _boundInkHandler.InkColor;

    void Awake()
    {
        Renderer genericRenderer = GetComponent<Renderer>();
        if(genericRenderer != null)
        {
            if(genericRenderer is LineRenderer lineRenderer)
            {
                lineRenderer.startColor = _boundInkHandler.InkColor;
                lineRenderer.endColor = _boundInkHandler.InkColor;
            }
            if(genericRenderer is SpriteRenderer spriteRenderer)
            {
                spriteRenderer.color = _boundInkHandler.InkColor;
            }
            else
                genericRenderer.material?.SetColor("_Color", _boundInkHandler.InkColor);
        }
        else
        {
            //UI Elements
            Image uiImage = GetComponent<Image>();
            if(uiImage != null)
                uiImage.color = _boundInkHandler.InkColor;
        }
    }
}
