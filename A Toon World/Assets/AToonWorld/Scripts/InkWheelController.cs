using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InkType = PlayerInkController.InkType;

public class InkWheelController : MonoBehaviour
{
    [SerializeField] private GameObject ConstructionInk;   
    [SerializeField] private GameObject ClimbInk;
    [SerializeField] private GameObject CancelInk;
    [SerializeField] private GameObject DamageInk;

    struct InkPosition
    {
        public InkType inkType;
        public float startAngle;
        public float endAngle;
    }

    private List<InkPosition> _inksPositions;
    
    void Start()
    {
        _inksPositions = new List<InkPosition>()
        {
            new InkPosition() { inkType = InkType.Construction, startAngle = -135, endAngle = 135 },
            new InkPosition() { inkType = InkType.Climb, startAngle = 45, endAngle = 135 },
            new InkPosition() { inkType = InkType.Cancel, startAngle = -135, endAngle = -45 },
            new InkPosition() { inkType = InkType.Damage, startAngle = -45, endAngle = 45 }
        };
    }

    public void Show()
    {
        this.transform.position = Input.mousePosition;
        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    void Update()
    {
        Vector3 mousePositionFromCenter = Input.mousePosition - transform.position;
        float angle = Mathf.Rad2Deg * Mathf.Atan2(mousePositionFromCenter.y, mousePositionFromCenter.x);
        
        _inksPositions.ForEach(position =>
        {
            if (angle > position.startAngle && angle < position.endAngle)
            {
                Events.InterfaceEvents.InkSelected.Invoke(position.inkType);
                return;
            }
        });
    }
}
