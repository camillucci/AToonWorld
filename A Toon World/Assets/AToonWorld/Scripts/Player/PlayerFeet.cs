using Assets.AToonWorld.Scripts.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.Player
{
    public class PlayerFeet : MonoBehaviour
    {
        private readonly TaggedEvent<string, Collider2D> _triggerEnter = new TaggedEvent<string, Collider2D>();
        private readonly TaggedEvent<string, Collider2D> _triggerExit = new TaggedEvent<string, Collider2D>();
        //private readonly TaggedEvent<string, Collider2D> _triggerStay = new TaggedEvent<string, Collider2D>();        


        public ITaggedEvent<string, Collider2D> TriggerEnter => _triggerEnter;
        public ITaggedEvent<string, Collider2D> TriggerExit => _triggerExit;
        //public ITaggedEvent<string, Collider2D> TriggerStay => _triggerStay;


        private void OnTriggerEnter2D(Collider2D collider)
            => _triggerEnter.InvokeWithTag(collider.gameObject.tag, collider);

        private void OnTriggerExit2D(Collider2D collider)
            => _triggerExit.InvokeWithTag(collider.gameObject.tag, collider);
    }
}
