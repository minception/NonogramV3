using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nonogram
{
    /// <summary>
    /// A single row or column of a puzzle represented as a line for the purpose of finding a solution
    /// </summary>
    public class DirectSolveLine
    {
        private int _thisNumber;
        private Func<int, Cell> _getter;
        private Checksum _checksum;
        private int _firstUnfinished;
        private int _lastUnfinished;
        private int _unfinishedRangeBeginning;
        private int _unfinishedRangeEnd;
        private int _size;

        /// <summary>
        /// DirectSolveLine Constructor
        /// </summary>
        /// <param name="puzzle">Puzzle this line is located in</param>
        /// <param name="orientation">Orientation of this line</param>
        /// <param name="index">Index of this line in the puzzle</param>
        public DirectSolveLine(Puzzle puzzle, Orientation orientation, int index)
        {
            if (orientation == Orientation.Horizontal)
            {
                _getter = (i) => { return puzzle[i, _thisNumber]; };
                _checksum = puzzle.Horizontal[index];
                _size = puzzle.SizeX;
            }
            else
            {
                _getter = (i) => { return puzzle[_thisNumber, i]; };
                _checksum = puzzle.Vertical[index];
                _size = puzzle.SizeY;
            }
            _thisNumber = index;
            _firstUnfinished = 0;
            _lastUnfinished = _checksum.Count - 1;
            _unfinishedRangeBeginning = 0;
            _unfinishedRangeEnd = _size - 1;
        }

        /// <summary>
        /// Solves a single line (row or column) of a puzzle using only one iteration and several methods of solving puzzle
        /// </summary>
        /// <param name="mode">Solving mode fills all of them, hint mode only fills one</param>
        /// <returns>True if a change occured, null if puzzle doesn't have a solution</returns>
        public bool? Solve(SolvingMode mode = SolvingMode.solve)
        {
            if (Check())
            {
                return FinishLine(mode);
            }
            bool? change = false;
            if (_firstUnfinished > _lastUnfinished)
            {
                return FillWithEmpty(mode);
            }
            var newChange = SolveRowIntersect(mode);
            if (mode == SolvingMode.hint && newChange == true)
            {
                return true;
            }
            if (newChange == null)
            {
                return null;
            }
            change |= newChange;
            newChange = SolveFirst(mode);
            if (mode == SolvingMode.hint && newChange == true)
            {
                return true;
            }
            if (newChange == null)
            {
                return null;
            }
            change |= newChange;
            if (_firstUnfinished >= _checksum.Count)
            {
                return change;
            }
            newChange = SolveLast(mode);
            if (mode == SolvingMode.hint && newChange == true)
            {
                return true;
            }
            if (newChange == null)
            {
                return null;
            }
            change |= newChange;
            return change;
        }

        /// <summary>
        /// This function is called after the line is properly colored and only fills unknown spaces with empty values
        /// </summary>
        /// <param name="mode">Solving mode fills all of them, hint mode only fills one</param>
        /// <returns>True if change occured</returns>
        private bool FinishLine(SolvingMode mode)
        {
            bool change = false;
            for (int i = 0; i < _size; i++)
            {
                if (this[i].State == CellState.unknown)
                {
                    this[i].State = CellState.empty;
                    change = true;
                    if (mode == SolvingMode.hint)
                    {
                        return change;
                    }
                }
            }
            return change;
        }

        private Cell this[int i]
        {
            get
            {
                return _getter(i);
            }
        }
        /// <summary>
        /// Sets the state of every cell in line to empty
        /// </summary>
        /// <returns></returns>
        private bool? FillWithEmpty(SolvingMode mode = SolvingMode.solve)
        {
            bool change = false;
            for (int i = _unfinishedRangeBeginning; i <= _unfinishedRangeEnd; i++)
            {
                if (this[i].State == CellState.unknown)
                {
                    this[i].State = CellState.empty;
                    change = true;
                    if (mode == SolvingMode.hint)
                    {
                        return change;
                    }
                }
                // preasummably empty line shouldn't contain filled cell
                if (this[i].State == CellState.filled)
                {
                    return null;
                }
            }
            return change;
        }

        
        /// <summary>
        /// Solves the line using intersection method
        /// </summary>
        /// <param name="mode">Can be either solveall which solves the whole puzzle, or hint, which only finds a single tile</param>
        /// <returns></returns>
        private bool? SolveRowIntersect(SolvingMode mode)
        {
            bool change = false;
            var putBeg = PutFromBeginning();
            if (putBeg == null)
            {
                // this means puzzle doesn't have a solution
                return null;
            }
            var putEnd = PutFromEnd();
            if (putEnd == null)
            {
                return null;
            }
            // coloring
            for (int i = _firstUnfinished; i <= _lastUnfinished; i++)
            {
                if (putBeg[i - _firstUnfinished] >= putEnd[i - _firstUnfinished])
                {
                    for (int j = putEnd[i - _firstUnfinished]; j <= putBeg[i - _firstUnfinished]; j++)
                    {
                        if (this[j].State != CellState.filled)
                        {
                            this[j].State = CellState.filled;
                            if (mode == SolvingMode.hint)
                            {
                                return true;
                            }
                            change = true;
                        }
                    }
                }
            }

            return change;
        }

        /// <summary>
        /// Try putting all values from the beginning
        /// </summary>
        /// <returns>List of end positions of values being put</returns>
        private int[] PutFromBeginning()
        {
            var position = _unfinishedRangeBeginning;
            var putResult = new int[_lastUnfinished - _firstUnfinished + 1];
            for (int i = _firstUnfinished; i <= _lastUnfinished; i++)
            {
                var newPosition = PutOneFromBeginning(i, position);
                if (newPosition == null)
                {
                    return null;
                }
                position = (int)newPosition;
                putResult[i - _firstUnfinished] = position;
                // add a space between values
                position += 2;
            }
            return putResult;
        }

        /// <summary>
        /// Try putting all values from the end
        /// </summary>
        /// <returns>List of end positions of values being put</returns>
        private int[] PutFromEnd()
        {
            var position = _unfinishedRangeEnd;
            var putResult = new int[_lastUnfinished - _firstUnfinished + 1];
            for (int i = _lastUnfinished; i >= _firstUnfinished; i--)
            {
                var newPosition = PutOneFromEnd(i, position);
                if (newPosition == null)
                {
                    return null;
                }
                position = (int)newPosition;
                putResult[i - _firstUnfinished] = position;
                // space between values
                position -= 2;
            }
            return putResult;

        }
        /// <summary>
        /// Try putting one value from the beginning
        /// </summary>
        /// <param name="index"> The index of value being put from the beginning</param>
        /// <param name="position">The current position of the beginning of the line</param>
        /// <returns>An end index of value being put</returns>
        private int? PutOneFromBeginning(int index, int position)
        {
            var isPut = false;
            while (!isPut)
            {
                isPut = true;
                for (int j = 0; j < _checksum[index]; j++)
                {
                    if (position > _unfinishedRangeEnd)
                    {
                        // when this occurs it means the puzzle doesn't have a solution
                        return null;
                    }
                    if (this[position].State == CellState.unknown)
                    {
                        position++;
                    }
                    else if (this[position].State == CellState.filled)
                    {
                        position++;
                    }
                    else if (this[position].State == CellState.empty)
                    {
                        // this means the value doesn't fit in this location, and will be put further
                        isPut = false;
                        position++;
                        break;
                    }
                }
            }

            while (position < _unfinishedRangeEnd && this[position].State == CellState.filled)
            {
                position++;
            }
            // position was once increased after going through whole value
            return --position;
        }

        /// <summary>
        /// Try putting one value from the end
        /// </summary>
        /// <param name="index"> The index of value being put from the end</param>
        /// <param name="position"> The current position of the end of the line</param>
        /// <returns></returns>
        private int? PutOneFromEnd(int index, int position)
        {
            var isPut = false;
            while (!isPut)
            {
                isPut = true;
                for (int j = 0; j < _checksum[index]; j++)
                {
                    if (position < _unfinishedRangeBeginning)
                    {
                        // when this occurs it means the puzzle doesn't have a solution
                        return null;
                    }
                    if (this[position].State == CellState.unknown)
                    {
                        position--;
                    }
                    else if (this[position].State == CellState.filled)
                    {
                        position--;
                    }
                    else if (this[position].State == CellState.empty)
                    {
                        // this means the value doesn't fit in this location, and will be put further
                        isPut = false;
                        position--;
                        break;
                    }
                }
            }
            while (position > _unfinishedRangeBeginning && this[position].State == CellState.filled)
            {
                position--;
            }
            // position was once decreased after going through whole value
            return ++position;
        }

        /// <summary>
        /// Solve for the first unfinished value in checksum
        /// </summary>
        /// <param name="mode">Solve mode colors the whole puzzle, while hint mode colors only one cell</param>
        /// <returns></returns>
        private bool? SolveFirst(SolvingMode mode)
        {
            bool change = false;
            int position = (int)PutOneFromBeginning(_firstUnfinished, _unfinishedRangeBeginning);
            var firstFilled = FirstFilled();


            // position now marks where the put end is

            // we can now possibly shrink unfinished range
            change |= Color(_unfinishedRangeBeginning, position - _checksum[_firstUnfinished], CellState.empty, mode);
            if (change && mode == SolvingMode.hint)
            {
                return true;
            }
            // unfinished range begins after last colored cell
            _unfinishedRangeBeginning = position - _checksum[_firstUnfinished] + 1;

            //coloring
            if (position - firstFilled >= _checksum[_firstUnfinished])
            {
                // when this happens, error occured (something is wrong with the puzzle)
                return null;
            }
            else if (position - firstFilled >= 0)
            {
                change |= Color(firstFilled, position, CellState.filled, mode);
                if (change && mode == SolvingMode.hint)
                {
                    return true;
                }

                // if the whole value is finished, put an empty cell after it, increase beginning of an unfinished range and change first unfinished value to the next one
                if (position - firstFilled + 1 == _checksum[_firstUnfinished])
                {
                    if (position + 1 < _size && this[position + 1].State != CellState.empty)
                    {
                        this[position + 1].State = CellState.empty;
                        if (mode == SolvingMode.hint)
                        {
                            return true;
                        }
                        change = true;
                    }
                    // position marks the end of first value, position + 1 is an empty space and position + 2 is a new beginning of an unfinished range
                    _unfinishedRangeBeginning = position + 2;
                    _firstUnfinished++;
                }

            }
            return change;
        }


        private bool Check()
        {
            return _checksum.Equals(GetChecksum());
        }

        /// <summary>
        /// Generates checksum of a line determined by its current state
        /// </summary>
        /// <returns></returns>
        private Checksum GetChecksum()
        {
            var checksum = new Checksum();
            int temp = 0;
            for (int i = 0; i < _size; i++)
            {
                if (this[i].State == CellState.filled)
                {
                    temp++;
                }
                else if (temp > 0)
                {
                    checksum.Add(temp);
                    temp = 0;
                }
            }
            if (temp > 0)
            {
                checksum.Add(temp);
            }
            return checksum;
        }

        /// <summary>
        /// Set state of cells in a given range to a given state
        /// </summary>
        /// <param name="start">Beginning of a range to color</param>
        /// <param name="finish">End of a range to color</param>
        /// <param name="state">State to which cells will be set</param>
        /// <param name="mode">Solve mode colors the whole range, while hint mode only colors the first cell</param>
        /// <returns>True if something changed</returns>
        private bool Color(int start, int finish, CellState state, SolvingMode mode)
        {
            bool change = false;
            for (int i = start; i <= finish; i++)
            {
                if (this[i].State != state)
                {
                    this[i].State = state;
                    if (mode == SolvingMode.hint)
                    {
                        return true;
                    }
                    change = true;
                }
            }
            return change;
        }

        /// <summary>
        /// Get the position of the first filled cell
        /// </summary>
        private int FirstFilled()
        {
            int position = _unfinishedRangeBeginning;
            while (position <= _unfinishedRangeEnd)
            {
                if (this[position].State == CellState.filled)
                {
                    return position;
                }
                position++;
            }
            return position;
        }

        /// <summary>
        /// Get the position of the last filled cell
        /// </summary>
        private int LastFilled()
        {
            int position = _unfinishedRangeEnd;
            while (position >= _unfinishedRangeBeginning)
            {
                if (this[position].State == CellState.filled)
                {
                    return position;
                }
                position--;
            }
            return position;
        }

        /// <summary>
        /// Solve for the last unfinished value in checksum
        /// </summary>
        /// <param name="mode">Solve mode colors the whole puzzle, while hint mode colors only one cell</param>
        /// <returns>True if change occured, null if error occured</returns>
        private bool? SolveLast(SolvingMode mode)
        {
            bool change = false;
            int position = (int)PutOneFromEnd(_lastUnfinished, _unfinishedRangeEnd);
            var lastFilled = LastFilled();

            // position now marks where the put end is

            // we can now possibly shrink unfinished range
            change |= Color(position + _checksum[_lastUnfinished], _unfinishedRangeEnd, CellState.empty, mode);
            if (change && mode == SolvingMode.hint)
            {
                return true;
            }
            // unfinished range begins after last colored cell
            _unfinishedRangeEnd = position + _checksum[_lastUnfinished] - 1;


            //coloring
            if (lastFilled - position >= _checksum[_lastUnfinished])
            {
                // when this happens, error occured (something is wrong with the puzzle)
                return null;
            }

            if (lastFilled - position >= 0)
            {
                change |= Color(position, lastFilled, CellState.filled, mode);
                if (mode == SolvingMode.hint && change)
                {
                    return true;
                }
                // if the whole value is finished, put an empty cell before it, decrease end of an unfinished range and change last unfinished value to the next one
                if (lastFilled - position + 1 == _checksum[_lastUnfinished])
                {
                    if (position - 1 > 0 && this[position - 1].State != CellState.empty)
                    {
                        this[position - 1].State = CellState.empty;
                        if (mode == SolvingMode.hint)
                        {
                            return true;
                        }

                    }
                    // position marks the end of first value, position - 1 is an empty space and position - 2 is a new beginning of an unfinished range
                    _unfinishedRangeEnd = position - 2;
                    _lastUnfinished--;
                }

            }

            return change;
        }
        
    }
    public enum SolvingMode { hint, solve }
}
