using System;
using System.Linq;
using System.Windows.Forms;

namespace Nonogram
{
    public class StateSpaceSearchLine
    {
        private int[] _offsets;
        private int _thisNumber;
        private Func<int, Cell> _getter;
        private Checksum _checksum;
        private int _size;
        public StateSpaceSearchLine(Puzzle puzzle, int index, Orientation orientation)
        {
            if (orientation == Orientation.Horizontal)
            {
                _getter = (i) => { return puzzle[i, _thisNumber]; };
                _size = puzzle.SizeX;
                _checksum = puzzle.Horizontal[index];
            }
            if (orientation == Orientation.Vertical)
            {
                _getter = (i) => { return puzzle[_thisNumber, i]; };
                _size = puzzle.SizeY;
                _checksum = puzzle.Vertical[index];
            }
            _thisNumber = index;
            _offsets = new int[_checksum.Count];
            // Set puzzle for the zero offset
            TryPut();

        }
        /// <summary>
        /// Cell in this line on
        /// </summary>
        /// <param name="i">Index of the cell</param>
        /// <returns></returns>
        private Cell this[int i]
        {
            get
            {
                return _getter(i);
            }
        }
        /// <summary>
        /// Sets all offsets back to 0
        /// </summary>
        public void ResetOffsets()
        {
            for (int i = 0; i < _offsets.Length; i++)
            {
                _offsets[i] = 0;
            }
            TryPut();
        }
        /// <summary>
        /// Increases next offset
        /// </summary>
        /// <returns>False if none of the offsets can be increased anymore</returns>
        public bool IncreaseOffsets()
        {
            if (_offsets.Length == 0)
            {
                return false;
            }
            do
            {
                _offsets[0]++;
                int lastIncreased = 0;
                while (true)
                {
                    // A complete area a line will occupy is sum of checksum values, offsets and count of checksums to acommodate for spaces
                    int area = _checksum.Sum + _offsets.Sum() + _checksum.Count - 1;
                    if (area > _size)
                    {
                        if (lastIncreased == _offsets.Length - 1)
                        {
                            return false;
                        }
                        _offsets[lastIncreased] = 0;
                        _offsets[++lastIncreased]++;
                    }
                    else
                    {
                        break;
                    }
                } 
            } while (!TryPut());
            return true;
        }
        
        /// <summary>
        /// Try to set test values of the line based on the offsets
        /// </summary>
        /// <returns>True if test values don't conflict with the current state of line</returns>
        private bool TryPut()
        {
            int position = 0;
            for (int i = 0; i < _checksum.Count; i++)
            {
                // in adittion to offsets skip one space on all except first checksum value
                for (int j = 0; j < _offsets[i] + (i > 0 ? 1 : 0); j++)
                {
                    if (this[position].State == CellState.filled)
                    {
                        return false;
                    }
                    this[position].Test = false;
                    position++;
                }
                for (int j = 0; j < _checksum[i]; j++)
                {
                    if (this[position].State == CellState.empty)
                    {
                        return false;
                    }
                    this[position].Test = true;
                    position++;
                }
            }
            // Set the rest of line to empty
            for (; position < _size; position++)
            {
                this[position].Test = false;
            }
            return true;
        }
    }
}