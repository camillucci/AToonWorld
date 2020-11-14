using System.Collections;
using System.Collections.Generic;
using Assets.AToonWorld.Scripts;
using UnityEngine;

public class DamageInkHandler : ExpendableResource, IInkHandler, IBulletInk
{
    private PlayerInkController _playerInkController;
    private Vector2 _playerPosition;
    private BulletController _bulletController;
    [SerializeField] private float _distanceFromPlayer = 0.5f;

    public DamageInkHandler(PlayerInkController playerInkController)
    {
        _playerInkController = playerInkController;
    }

    public override float MaxCapacity => 25;

    public void BindBulletAndPosition(BulletController bulletController, Vector2 playerPosition)
    {
        _playerPosition = playerPosition;
        _bulletController = bulletController;
    }
    
    public void OnDrawDown(Vector2 mouseWorldPosition)
    {
        if(Vector2.Distance(_playerPosition, mouseWorldPosition) > _distanceFromPlayer && this.ConsumeOrFail(1))
            _bulletController.Shoot(mouseWorldPosition, _playerPosition);
        else
            _bulletController.gameObject.SetActive(false);
    }

    public bool OnDrawHeld(Vector2 mouseWorldPosition) => true;

    public void OnDrawReleased(Vector2 mouseWorldPosition) {}

    public override void SetCapacity(float newCapacity)
    {
        base.SetCapacity(newCapacity);
        Events.InterfaceEvents.InkCapacityChanged.Invoke((PlayerInkController.InkType.Damage, newCapacity/MaxCapacity));
    }
}
