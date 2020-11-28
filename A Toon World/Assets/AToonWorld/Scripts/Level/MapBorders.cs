using Assets.AToonWorld.Scripts.Utils;
using Assets.AToonWorld.Scripts.Utils.Events.TaggedEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.Level
{
    public class MapBorders : MonoBehaviour
    {
        // Private Fields
        private readonly ColliderTaggedEvents<Collider2D> _colliderTrigger = new ColliderTaggedEvents<Collider2D>();


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
    }
}
