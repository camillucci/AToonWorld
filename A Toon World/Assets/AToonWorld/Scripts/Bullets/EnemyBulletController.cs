using System;
using System.Collections;
using System.Collections.Generic;
using Assets.AToonWorld.Scripts;
using UnityEngine;

public class EnemyBulletController : BulletController
{
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(UnityTag.Player))
        {
            this.gameObject.SetActive(false);
            Events.PlayerEvents.Death.Invoke();
            return;
        }

        if (other.CompareTag(UnityTag.Ground))
            this.gameObject.SetActive(false);
    }
}