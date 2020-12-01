using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.Utils.Events.TaggedEvents
{
    public class ColliderTaggedEvents<T> : IColliderTaggedEvents<T>
    {
        // Private Fields
        private readonly Dictionary<string, HashSet<T>> _objectsInsideByTag = new Dictionary<string, HashSet<T>>();
        private readonly TaggedEvent<string, T> _enter = new TaggedEvent<string, T>();
        private readonly TaggedEvent<string, T> _exit = new TaggedEvent<string, T>();



        // Public Properties
        public ITaggedEvent<string, T> Enter => _enter;
        public ITaggedEvent<string, T> Exit => _exit;



        // Public Methods
        public void NotifyEnter(string tag, T t)
        {
            ChangeStateAndNotify(tag, t);
        }

        public void NotifyExit(string tag, T t)
        {
            ChangeStateAndNotify(tag, t);
        }

        public IReadOnlyCollection<T> GetInsideWithTag(string tag)
        {
            if (_objectsInsideByTag.TryGetValue(tag, out var ret))
                return ret;
            return Array.Empty<T>();
        }


        // Private Methods
        private void ChangeStateAndNotify(string tag, T t)
        {
            if (_objectsInsideByTag.TryGetValue(tag, out var ts))
                if (ts.Remove(t))
                    _exit?.InvokeWithTag(tag, t);
                else
                {
                    ts.Add(t);
                    _enter.InvokeWithTag(tag, t);
                }
            else
            {
                _objectsInsideByTag.Add(tag, new HashSet<T> { t });
                _enter.InvokeWithTag(tag, t);
            }
        }
    }
}
