using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WavingSprite : MonoBehaviour
{
    protected Material _imageMaterial;
    private Image _imageComponent;
    [SerializeField] private Canvas _canvas = null;
    private CanvasController _canvasController;

    protected virtual void Awake() 
    {
        //Copy material
        _imageComponent = gameObject.GetComponent<Image>();
        _imageMaterial = new Material(_imageComponent.material);
        _imageComponent.material = _imageMaterial;

        _canvasController = _canvas.GetComponent<CanvasController>();
        _canvasController.CanvasSizeChanged.AddListener(UpdateShaderProperties);
        
        UpdateShaderProperties();
    }

    private void OnDestroy() {
        _canvasController.CanvasSizeChanged.RemoveListener(UpdateShaderProperties);

        #if UNITY_EDITOR
            //Clean Up
            _imageMaterial.SetVector("_UIElementOrigin", Vector4.zero);
            _imageMaterial.SetVector("_UIElementSize", Vector4.zero);
            _imageMaterial.SetVector("_UIElementUpQuaternion", Vector4.zero);
            _imageMaterial.SetVector("_UIElementUpDirection", Vector3.up);
        #endif

        DestroyInherited();
    }

    protected virtual void DestroyInherited() {}

    public void UpdateShaderProperties()
    {
        Quaternion rotationQuaternion = Quaternion.FromToRotation(Vector3.up, transform.up);
        _imageMaterial.SetVector("_UIElementOrigin", transform.position - _canvas.transform.position);
        _imageMaterial.SetVector("_UIElementSize", new Vector4(
            _imageComponent.rectTransform.rect.width *  _imageComponent.rectTransform.lossyScale.x,
            _imageComponent.rectTransform.rect.height *  _imageComponent.rectTransform.lossyScale.y,
            0,
            0
        ));
        _imageMaterial.SetVector("_UIElementUpQuaternion", new Vector4(
            rotationQuaternion.x,
            rotationQuaternion.y,
            rotationQuaternion.z,
            rotationQuaternion.w
        ));
        _imageMaterial.SetVector("_UIElementUpDirection", transform.up);
    }
}
