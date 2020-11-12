using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBulletInk
{
    void BindBulletAndPosition(BulletController bulletController, Vector2 playerPosition);
}
