using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.Extensions
{
    public static class BoundsExtensions
    {
        public static Bounds Get2DBounds(this Bounds aBounds)
        {
            var ext = aBounds.extents;
            ext.z = float.PositiveInfinity;
            aBounds.extents = ext;
            return aBounds;
        }
    }
}
