using System;
using System.Collections;
using System.Collections.Generic;
using Assets.AToonWorld.Scripts;
using UnityEngine;

public class PlayerBulletController : BulletController
{
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(UnityTag.Enemy))
        {
            this.gameObject.SetActive(false);
            other.gameObject.SetActive(false);
            Events.LevelEvents.EnemyKilled.Invoke(other.gameObject);
            return;
        }

        if (other.CompareTag(UnityTag.Ground))
            this.gameObject.SetActive(false);
    }
}