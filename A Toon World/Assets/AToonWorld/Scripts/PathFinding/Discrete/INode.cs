using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.AToonWorld.Scripts.PathFinding
{
    public interface INode
    {
        int X { get; }
        int Y { get; }
        bool Walkable { get; set; }
        void Deconstruct(out int x, out int y);
    }
}
