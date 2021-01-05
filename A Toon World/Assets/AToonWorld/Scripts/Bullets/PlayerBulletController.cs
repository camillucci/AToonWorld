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
            //TODO: Teoricamente l'explosion dovrebbe gestirsela il Killable
            Instantiate(_explosion, gameObject.transform.position, gameObject.transform.rotation);
            this.gameObject.SetActive(false);
            
            IKillable killable = other.GetComponent<IKillable>();
            if(killable != null)
                killable.Kill();
            else
            {
                Events.LevelEvents.EnemyKilled.Invoke(other.gameObject);
                other.gameObject.SetActive(false);
            }
            
            return;
        }

        if (other.CompareTag(UnityTag.Ground) || other.CompareTag(UnityTag.DarkLake))
        {
            Instantiate(_explosion, gameObject.transform.position, gameObject.transform.rotation);
            this.gameObject.SetActive(false);
        }
    }
}