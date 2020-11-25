using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WavingSprite : MonoBehaviour
{
    protected Material _imageMaterial;
    [SerializeField] private Canvas _canvas;
    private CanvasController _canvasController;

    void Start() 
    {
        _imageMaterial = gameObject.GetComponent<Image>().material;
        UpdateShaderProperties();
        _canvasController = _canvas.GetComponent<CanvasController>();
        _canvasController.CanvasSizeChanged.AddListener(UpdateShaderProperties);
    }

    private void OnDestroy() {
        _canvasController.CanvasSizeChanged.RemoveListener(UpdateShaderProperties);

        #if UNITY_EDITOR
            //Clean Up
            _imageMaterial.SetVector("_UIElementOrigin", Vector4.zero);
            _imageMaterial.SetVector("_UIElementUpQuaternion", Vector4.zero);
            _imageMaterial.SetVector("_UIElementUpDirection", Vector3.up);
        #endif

        DestroyInherited();
    }

    protected virtual void DestroyInherited() {}

    void UpdateShaderProperties()
    {
        Quaternion rotationQuaternion = Quaternion.FromToRotation(Vector3.up, transform.up);
        _imageMaterial.SetVector("_UIElementOrigin", transform.position - _canvas.transform.position);
        _imageMaterial.SetVector("_UIElementUpQuaternion", new Vector4(
            rotationQuaternion.x,
            rotationQuaternion.y,
            rotationQuaternion.z,
            rotationQuaternion.w
        ));
        _imageMaterial.SetVector("_UIElementUpDirection", transform.up);
    }
}
