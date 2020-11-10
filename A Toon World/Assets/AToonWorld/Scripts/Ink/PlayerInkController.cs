using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.AToonWorld.Scripts.Utils;

public class PlayerInkController : MonoBehaviour
{
    //TODO: Logica selezione ink
    //TODO: Interfaccia e gestione ink
    [SerializeField] private GameObject _constructionInkPrefab;
    [SerializeField] private GameObject _cancelInkPrefab;
    private InkType _selectedInk = InkType.Construction;
    private bool _isDrawing = false;
    private Vector2 _mouseWorldPosition;
    private Dictionary<InkType, IInkHandler> _inkHandlers;
    public InkType SelectedInk => _selectedInk;

    void Awake()
    {
        //TODO: Handler assegnati da codice? Forse è meglio usare qualcosa di assegnabile da Editor? (da Editor non possiamo usare le interfacce)
        _inkHandlers = new Dictionary<InkType, IInkHandler>()
        {
            [InkType.Construction] = new ConstructionInkHandler(this), //Spline Ink
            [InkType.Cancel] = new CancelInkHandler(this), //Spline Ink (for now?)
            //TODO: Others
        };

        ObjectPoolingManager<InkType>.Instance.CreatePool(InkType.Construction, _constructionInkPrefab, 50, 200, true);
        ObjectPoolingManager<InkType>.Instance.CreatePool(InkType.Cancel, _cancelInkPrefab, 1, 2, true);
    }

    public void OnInkSelected(InkType newInk)
    {
        if(!_isDrawing)
            _selectedInk = newInk;
    }

    public void OnDrawDown()
    {
        _isDrawing = true;
        _mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        GameObject pooledSpline = ObjectPoolingManager<InkType>.Instance.GetObject(_selectedInk);
        
        if(_inkHandlers[_selectedInk] is ISplineInk _selectedSplineInk)
            _selectedSplineInk?.BindSpline(pooledSpline.GetComponent<DrawSplineController>());

        _inkHandlers[_selectedInk].OnDrawDown(_mouseWorldPosition);
    }

    public void WhileDrawHeld()
    {
        _mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _inkHandlers[_selectedInk].OnDrawHeld(_mouseWorldPosition);
    }

    public void OnDrawReleased()
    {
        _inkHandlers[_selectedInk].OnDrawReleased(_mouseWorldPosition);
        _isDrawing = false;
    }

    public enum InkType {
        Construction = 0,
        Climb,
        Damage,
        Cancel
    }
}
