using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.AToonWorld.Scripts.Utils
{
    public class CancellationTokenLite
    {
        public bool IsCancelled { get; private set; } = false;
        public void Cancel() => IsCancelled = true;
    }
}
