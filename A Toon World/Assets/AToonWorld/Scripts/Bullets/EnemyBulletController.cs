using System;
using System.Collections;
using System.Collections.Generic;
using Assets.AToonWorld.Scripts;
using UnityEngine;

public class EnemyBulletController : BulletController
{
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.gameObject.tag);
        switch(other.gameObject.tag)
        {
            case UnityTag.Player:
                this.gameObject.SetActive(false);
                Events.PlayerEvents.Death.Invoke();
                return;

            case UnityTag.Ground:
                this.gameObject.SetActive(false);
                return;

            case UnityTag.Drawing:
                other.gameObject.SetActive(false);
                this.gameObject.SetActive(false);
                return;
        }
    }
}