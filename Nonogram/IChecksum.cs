using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Nonogram
{
    public interface IChecksum:IEquatable<IChecksum>, IEnumerable<int>
    {
        int this[int x] { get; set; }

        void Add(int value);
        int Count { get; }
        void Clear();
    }
}
