using System.Collections;
using System.Collections.Generic;
using Events;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using static PlayerInkController;
using System.Linq;

/// <summary>
/// The director directs the player experience by monitoring his ink reserves
/// Some InkPickups are "directed" which means they will respawn under certain circumstances
/// Some Pickups can be "partially-directed" which means that the director is only part of the logic which determines if they respawn
/// </summary>
public class InkDirector : Singleton<InkDirector>
{
    [SerializeField] private int _directorUpdateTime = 1000;
    private List<DirectedAreaComponent> _directedAreas;
    private List<InkPickupController> _globalPickpups;
    private Dictionary<InkType, float> _cachedInkCapacities;
    
    private bool _capacitiesInvalidated = false;

    void OnEnable()
    {
        _globalPickpups = new List<InkPickupController>();
        _directedAreas = new List<DirectedAreaComponent>();

        _cachedInkCapacities = new Dictionary<InkType, float>(3)
        {
            [InkType.Climb] = 0,
            [InkType.Construction] = 0,
            [InkType.Damage] = 0,
        };

        SceneManager.sceneLoaded += OnSceneLoaded;
        InterfaceEvents.RawInkCapacityChanged.AddListener(OnPlayerInkCapacityUpdate);
    }

    void OnDisable()
    {
        //TODO: Ho notato che molte altre classi si dimenticano di de-referenziare gli eventi, bisognerebbe fare il giro di ogni evento e assicurarsi che questo accada.
        SceneManager.sceneLoaded -= OnSceneLoaded;
        InterfaceEvents.RawInkCapacityChanged.RemoveListener(OnPlayerInkCapacityUpdate);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _directedAreas.Clear();
        _globalPickpups.Clear();

        //Very Expensive, only done on scene loads
        _globalPickpups.AddRange(GetComponentsInChildren<InkPickupController>().Where(pickup => pickup.IsDirected));
    }

    void OnPlayerInkCapacityUpdate((InkType, float) inkUpdate)
    {
        //Update capacities buffers
        _cachedInkCapacities[inkUpdate.Item1] = inkUpdate.Item2;

        // Throttle capacity update frequency down to max one each second
        // keep a capacity buffer always updated
        if(!_capacitiesInvalidated)
        {
            _capacitiesInvalidated = true;
            ThrottleUpdateRate().Forget();
        }
    }

    public void OnDirectedAreaActivated(DirectedAreaComponent areaComponent)
    {
        _directedAreas.Add(areaComponent);
        areaComponent.ProcessChilds(ProcessOnePickup);
    }

    public void OnDirectedAreaDeactivated(DirectedAreaComponent areaComponent)
    {
        _directedAreas.Remove(areaComponent);
    }

    private async UniTask ThrottleUpdateRate()
    {
        await UniTask.Delay(_directorUpdateTime);
        _capacitiesInvalidated = false;
        OnDirectorUpdate();
    }

    //Most basic logic for now
    private void ProcessOnePickup(InkPickupController pickup, bool limitResources)
    {
        if(pickup.RespawnThreshold >= _cachedInkCapacities[pickup.InkType])
            pickup.RequestEnable();
        else if(limitResources)
            pickup.gameObject.SetActive(false);
    }

    private void OnDirectorUpdate()
    {
        _globalPickpups.ForEach(pickup => ProcessOnePickup(pickup, false));
        _directedAreas.ForEach(directedArea => directedArea.ProcessChilds(ProcessOnePickup));
    }
}
