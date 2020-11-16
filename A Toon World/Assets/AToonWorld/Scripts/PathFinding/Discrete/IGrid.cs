using Assets.AToonWorld.Scripts.PathFinding.Discrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.AToonWorld.Scripts.PathFinding
{
    public interface IGrid : IEnumerable<INode>
    {
        int Width { get; }
        int Height { get; }        
        INode BottomLeft { get; }
        INode BottomRight { get; }
        INode TopLeft { get; }
        INode TopRight { get; }
        INode this[int row, int column] { get; }
        INode this[(int row, int column) coordinates] { get; }
        IEnumerable<INode> GetNeighbours(INode node);
        IEnumerable<INode> FindMinimumPath(INode start, INode destination);
        IEnumerable<INode> FindMinimumPath(INode start, INode destination, PathStepsContainer forbiddenSteps);
        bool TryGetValue(int x, int y, out INode node);
        int Distance(INode start, INode destination);        
    }
}
