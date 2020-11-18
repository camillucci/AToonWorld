using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.AToonWorld.Scripts.PathFinding.Discrete
{
    public class PathStepsContainer
    {
        private readonly Dictionary<INode, HashSet<INode>> _stepsForNode = new Dictionary<INode, HashSet<INode>>();



        // Public Methods
        public void Add(INode from, INode to)
        {
            AddUnidirectionalStep(from, to);
            AddUnidirectionalStep(to, from);
        }

        public bool Contains(INode from, INode to)
        {
            return ContainsUnidirectionaStep(from, to) || ContainsUnidirectionaStep(to, from);
        }

        public void CLear() => _stepsForNode.Clear();



        // Private Methods
        private void AddUnidirectionalStep(INode from, INode to)
        {
            if (_stepsForNode.TryGetValue(from, out var set))
                set.Add(from);
            else
                _stepsForNode.Add(from, new HashSet<INode> { to });
        }
        private bool ContainsUnidirectionaStep(INode from, INode to)
        {
            if (_stepsForNode.TryGetValue(from, out var set))
                return set.Contains(to);
            return false;
        }
    }
}
