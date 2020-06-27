using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Nonogram
{
    interface IPuzzle
    {
        Puzzle.ChecksumCollection Horizontal { get; }
        Puzzle.ChecksumCollection Vertical { get; }
        Cell this[int x, int y] { get; }
        int SizeX { get; }
        int SizeY { get; }
        void ChecksumFromDrawn();
        void Save(string path, Action<Puzzle, TextWriter> parser, bool saveState);
        void ClearChecksum();
        void ClearGrid();
        bool IsSolved();
        Checksum GetChecksum(Orientation orientation, int index);
        bool Solve(SolvingMode mode);
        void Hint();


    }
}
