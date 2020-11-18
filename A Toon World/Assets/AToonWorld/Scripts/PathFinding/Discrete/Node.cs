using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.AToonWorld.Scripts.PathFinding
{
    public class Node : INode
    {
        public Node(int x, int y)
        {
            (X, Y) = (x, y);
        }

        // Public Properties
        public int X { get; }
        public int Y { get; }
        public int HCost { get; set; }
        public int GCost { get; set; }
        public int FCost => HCost + GCost;
        public bool Walkable { get; set; } = true;
        public Node Parent { get; set; }



        // Public Methods
        public void ResetCostsAndParent()
        {
            HCost = 0;
            GCost = 0;
            Parent = null;
        }
        public void Deconstruct(out int x, out int y)
        {
            (x, y) = (X, Y);
        }
    }
}
