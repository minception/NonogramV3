using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nonogram
{
    interface ICell
    {
        CellState State { get; set; }
        bool Test { get; set; }
    }
}
