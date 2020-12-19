using System;
using System.Collections;
using System.Collections.Generic;
using Assets.AToonWorld.Scripts;
using Assets.AToonWorld.Scripts.UnityAnimations;
using UnityEngine;

public class PlayerBulletController : BulletController
{
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(UnityTag.Enemy))
        {
            Instantiate(_explosion, gameObject.transform.position, gameObject.transform.rotation);
            this.gameObject.SetActive(false);
            other.gameObject.SetActive(false);
            Events.LevelEvents.EnemyKilled.Invoke(other.gameObject);
            GenericAnimations.InkCloud(other.transform.position).PlayAndForget();

            #if AnaliticsEnabled
                Events.AnaliticsEvents.EnemyKilled.Invoke(new Analitic(other.gameObject.name, other.gameObject.GetInstanceID(), other.gameObject.transform.position.x, other.gameObject.transform.position.y));
            #endif
            
            return;
        }

        if (other.CompareTag(UnityTag.Ground))
        {
            Instantiate(_explosion, gameObject.transform.position, gameObject.transform.rotation);
            this.gameObject.SetActive(false);
        }
    }
}