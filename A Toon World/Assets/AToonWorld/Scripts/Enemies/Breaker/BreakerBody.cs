using Assets.AToonWorld.Scripts.Utils;
using Assets.AToonWorld.Scripts.Utils.Events.TaggedEvents;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.Enemies.Breaker
{
    public class BreakerBody : MonoBehaviour, IKillable
    {
        // Private Fields
        private readonly ColliderTaggedEvents<Collider2D> _colliderTrigger = new ColliderTaggedEvents<Collider2D>();

        // Initialization
        private void Awake()
        {
            ColliderTrigger.Enter.SubscribeWithTag(UnityTag.Drawing, OnDrawingEnter);
            ColliderTrigger.Exit.SubscribeWithTag(UnityTag.Drawing, OnDrawingExit);
        }


        // Public Properties
        public IColliderTaggedEvents<Collider2D> ColliderTrigger => _colliderTrigger;


        // Unity Events
        private void OnTriggerEnter2D(Collider2D collision)
        {
            _colliderTrigger.NotifyEnter(collision.gameObject.tag, collision);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            _colliderTrigger.NotifyExit(collision.gameObject.tag, collision);
        }



        // Breaker Events
        private void OnDrawingEnter(Collider2D collision)
        {
            collision.gameObject.SetActive(false);
            
            /*
             * // Workaround. OnTriggerExit can be called before onTriggerEnter if entering and exiting happens in the same instant
            DeleteLineAfter(100, collision.gameObject).Forget();
            */
        }

        private void OnDrawingExit(Collider2D collision)
        {
            
        }

        private async UniTaskVoid DeleteLineAfter(int ms, GameObject line)
        {
            await UniTask.Delay(ms);
            line.SetActive(false);
        }

        public void Kill()
        {
            this.transform.parent.GetComponent<IKillable>()?.Kill();
        }
    }
}
