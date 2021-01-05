using System.Collections;
using System.Collections.Generic;
using Assets.AToonWorld.Scripts;
using Assets.AToonWorld.Scripts.Audio;
using Assets.AToonWorld.Scripts.Enemies;
using Assets.AToonWorld.Scripts.Extensions;
using Assets.AToonWorld.Scripts.UnityAnimations;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class EnemyController : MonoBehaviour, IKillable 
{
    public virtual void Kill()
    {
        GameObject toKill = this.gameObject;
        Events.LevelEvents.EnemyKilled.Invoke(toKill);
        #if AnaliticsEnabled
            Events.AnaliticsEvents.EnemyKilled.Invoke(new Analitic(toKill.name, toKill.GetInstanceID(), toKill.transform.position.x, toKill.transform.position.y));
        #endif

        toKill.SetActive(false);
    }
}