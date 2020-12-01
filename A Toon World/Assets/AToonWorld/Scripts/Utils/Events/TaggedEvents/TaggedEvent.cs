using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.AToonWorld.Scripts.Utils
{
    public class TaggedEvent<TTag, VArgs> : ITaggedEvent<TTag, VArgs>
    {
        private readonly Dictionary<TTag, Action<VArgs>> _handlersDictionary = new Dictionary<TTag, Action<VArgs>>();
        private Action<VArgs> _notTaggedHandlers;

        public void Subscribe(Action<VArgs> handler)
        {
            _notTaggedHandlers += handler;
        }

        public void SubscribeWithTag(TTag tag, Action<VArgs> handler)
        {
            if (_handlersDictionary.TryGetValue(tag, out var savedHandler))
                _handlersDictionary[tag] = savedHandler + handler;
            else
                _handlersDictionary.Add(tag, handler);
        }

        public void SubscribeWithTag(params (TTag tag, Action<VArgs> handler)[] handlers)
        {
            foreach (var (tag, handler) in handlers)
                SubscribeWithTag(tag, handler);
        }

        public void InvokeWithTag(TTag tag, VArgs args)
        {
            if (_handlersDictionary.TryGetValue(tag, out var handler))
                handler?.Invoke(args);
            Invoke(args);
        }


        public void Invoke(VArgs args)
        {
            _notTaggedHandlers?.Invoke(args);
        }

        public void UnSubscribeWithTag(TTag tag, Action<VArgs> handler) => _handlersDictionary[tag] -= handler;

        public void UnSubscribeWithTag(params (TTag tag, Action<VArgs> handler)[] handlers)
        {
            foreach (var (tag, handler) in handlers)
                UnSubscribeWithTag(tag, handler);
        }
    }
}
