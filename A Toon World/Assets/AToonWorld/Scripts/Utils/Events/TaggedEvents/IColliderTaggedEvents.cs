using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.AToonWorld.Scripts.Utils.Events.TaggedEvents
{
    public interface IColliderTaggedEvents<T>
    {
        ITaggedEvent<string, T> Enter { get; }
        ITaggedEvent<string, T> Exit { get; }
        IReadOnlyCollection<T> GetInsideWithTag(string tag);
    }
}
