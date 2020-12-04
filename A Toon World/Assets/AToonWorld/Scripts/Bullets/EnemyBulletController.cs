using System;
using System.Collections;
using System.Collections.Generic;
using Assets.AToonWorld.Scripts;
using UnityEngine;

public class EnemyBulletController : BulletController
{
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        switch(other.gameObject.tag)
        {
            case UnityTag.Player:
                Instantiate(_explosion, gameObject.transform.position, gameObject.transform.rotation);
                this.gameObject.SetActive(false);
                Events.PlayerEvents.Death.Invoke();
                return;

            case UnityTag.Ground:
                Instantiate(_explosion, gameObject.transform.position, gameObject.transform.rotation);
                this.gameObject.SetActive(false);
                return;

            case UnityTag.Drawing:
                Instantiate(_explosion, gameObject.transform.position, gameObject.transform.rotation);
                other.gameObject.SetActive(false);
                this.gameObject.SetActive(false);
                return;
        }
    }
}