using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.AToonWorld.Scripts.PathFinding
{
    public static class GridFactory
    {
        public static IGrid GetNewGrid(int width, int height) => new PathFindingGrid(width, height);
    }
}
