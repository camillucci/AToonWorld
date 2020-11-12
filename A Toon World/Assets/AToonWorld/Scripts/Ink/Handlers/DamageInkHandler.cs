using System.Collections;
using System.Collections.Generic;
using Assets.AToonWorld.Scripts;
using UnityEngine;

public class DamageInkHandler : IInkHandler, IBulletInk
{
    private PlayerInkController _playerInkController;
    private Vector2 _playerPosition;
    private BulletController _bulletController;

    public DamageInkHandler(PlayerInkController playerInkController)
    {
        _playerInkController = playerInkController;
    }

    public void BindBulletAndPosition(BulletController bulletController, Vector2 playerPosition)
    {
        _playerPosition = playerPosition;
        _bulletController = bulletController;
    }
    
    public void OnDrawDown(Vector2 mouseWorldPosition)
    {
        _bulletController.Shoot(mouseWorldPosition, _playerPosition);
    }

    public void OnDrawHeld(Vector2 mouseWorldPosition) {}

    public void OnDrawReleased(Vector2 mouseWorldPosition) {}
}
