using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.AToonWorld.Scripts.Utils
{
    public interface ITaggedEvent<TTag, VArgs>
    {
        void Subscribe(Action<VArgs> handler);
        void SubscribeWithTag(TTag tag, Action<VArgs> handler);
        void SubscribeWithTag(params (TTag tag, Action<VArgs> handler)[] handlers);
        void UnSubscribeWithTag(TTag tag, Action<VArgs> handler);
        void UnSubscribeWithTag(params (TTag tag, Action<VArgs> handler)[] handlers);
    }
}
