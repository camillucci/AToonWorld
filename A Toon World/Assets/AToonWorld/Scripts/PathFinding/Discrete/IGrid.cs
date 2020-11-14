using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.AToonWorld.Scripts.PathFinding
{
    public interface IGrid
    {
        int Width { get; }
        int Height { get; }        
        INode this[int row, int column] { get; }
        IEnumerable<INode> GetNeighbours(INode node);
        IEnumerable<INode> FindMinimumPath(INode start, INode destination);
        int Distance(INode start, INode destination);
    }
}
