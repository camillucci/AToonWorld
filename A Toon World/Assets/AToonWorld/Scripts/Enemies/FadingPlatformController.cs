using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadingPlatformController : MonoBehaviour, ILinkedObjectManager<DrawSplineController>
{
    private List<DrawSplineController> _linkedPlatforms;

    private void Awake() 
    {
        _linkedPlatforms = new List<DrawSplineController>();
    }

    public void Link(DrawSplineController linkableObject)
    {
        _linkedPlatforms.Add(linkableObject);
        linkableObject.BindManager(this);

        if(!this.enabled)
            linkableObject.Disable();
    }

    public void Unlink(DrawSplineController linkableObject)
    {
        _linkedPlatforms.Remove(linkableObject);
    }

    private void OnFadeOutExit()
    {
        _linkedPlatforms.ForEach(linkedInk => linkedInk.Disable());
    }

    private void OnFadeInEnter()
    {
        _linkedPlatforms.ForEach(linkedInk => linkedInk.Enable());
    }
}
