using Assets.AToonWorld.Scripts.Utils;
using Assets.AToonWorld.Scripts.Utils.Events.TaggedEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.Enemies.Seeker
{
    public class SeekerBody : MonoBehaviour
    {
        // Private Fields
        private readonly ColliderTaggedEvents<Collider2D> _colliderTrigger = new ColliderTaggedEvents<Collider2D>();

        // Initialization
        private void Awake()
        {
            _colliderTrigger.Enter.SubscribeWithTag(UnityTag.Drawing, OnDrawingEnter);
        }



        // Public Properties
        public IColliderTaggedEvents<Collider2D> ColliderTrigger => _colliderTrigger;



        // UNity Events
        private void OnTriggerEnter2D(Collider2D collision)
        {
            _colliderTrigger.NotifyEnter(collision.gameObject.tag, collision);            
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            _colliderTrigger.NotifyExit(collision.gameObject.tag, collision);
        }


        // Seeker Events

        private void OnDrawingEnter(Collider2D collision)
        {
            collision.gameObject.SetActive(false);
        }

    }
}
