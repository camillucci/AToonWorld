using System.Collections;
using System.Collections.Generic;
using Assets.AToonWorld.Scripts;
using UnityEngine;

[CreateAssetMenu(fileName = "DamageInkHandler", menuName = "Inkverse/Inks/Handlers/Damage Ink Handler", order = 3)]
public class DamageInkHandler : ScriptableExpendableInkHandler, IBulletInk
{
    private Vector2 _playerPosition;
    private BulletController _bulletController;
    [SerializeField] private float _distanceFromPlayer = 0.5f;
    private static BulletBehaviour _bulletBehaviour = new ParabolicBullet();

    public void BindBulletAndPosition(BulletController bulletController, Vector2 playerPosition)
    {
        _playerPosition = playerPosition;
        _bulletController = bulletController;
        _bulletController.BulletBehaviour = _bulletBehaviour;
    }
    
    public override bool OnDrawDown(Vector2 mouseWorldPosition)
    {
        if(Vector2.Distance(_playerPosition, mouseWorldPosition) > _distanceFromPlayer && _expendableResource.ConsumeOrFail(1))
            _bulletController.Shoot(_playerPosition, mouseWorldPosition);
        else
            _bulletController.gameObject.SetActive(false);
        return true;
    }

    public override bool OnDrawHeld(Vector2 mouseWorldPosition) => true;

    public override void OnDrawReleased(Vector2 mouseWorldPosition) {}
}
