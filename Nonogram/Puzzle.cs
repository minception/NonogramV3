using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Nonogram
{


    /// <summary>
    /// Class representing a single nonogram puzzle
    /// </summary>
    public class Puzzle:IPuzzle
    {
        /// <summary>
        /// Loads a puzzle from file using provided parser
        /// </summary>
        /// <param name="path">A file location</param>
        /// <param name="parser">A parser used to parse the file</param>
        /// <returns></returns>
        public static Puzzle LoadFromFile(string path, Func<TextReader, Puzzle> parser)
        {
            StreamReader reader;
            try
            {
                reader = new StreamReader(path);
            }
            catch (Exception e)
            {
                MessageBox.Show("Error loading file");
                return null;
            }
            var puzzle = parser(reader);
            reader.Close();
            return puzzle;
        }

        private readonly Cell[,] _grid;
        private List<Step> _steps = new List<Step>();
        private int _currentStep = -1;

        /// <summary>
        /// Collection of checksums
        /// </summary>
        public class ChecksumCollection:IEnumerable<Checksum>
        {
            private Checksum[] checksums_;

            public ChecksumCollection(int size)
            {
                checksums_ = new Checksum[size];
                // populate array
                for (int i = 0; i < size; i++)
                {
                    checksums_[i] = new Checksum();
                }
            }
            public Checksum this[int x]
            {
                get { return checksums_[x]; }
            }

            public int Count
            {
                get { return checksums_.Length; }
            }
            /// <summary>
            /// Maximum amount of values between all checksums
            /// </summary>
            public int MaxCount
            {
                get { return checksums_.Select((x) => { return x.Count; }).Max(); }
            }

            public void Clear()
            {
                foreach (var checksum in checksums_)
                {
                    checksum.Clear();
                }
            }

            public IEnumerator<Checksum> GetEnumerator()
            {
                return ((IEnumerable<Checksum>)checksums_).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        /// <summary>
        /// Horizontal checksums
        /// </summary>
        public ChecksumCollection Horizontal { get; }
        /// <summary>
        /// vertical checksums
        /// </summary>
        public ChecksumCollection Vertical { get; }

        /// <summary>
        /// Width of the puzzle
        /// </summary>
        public int SizeX { get; }

        /// <summary>
        /// Height of the puzzle
        /// </summary>
        public int SizeY { get; }

        /// <summary>
        /// Create empty puzzle with a defined height and width
        /// </summary>
        /// <param name="sizeX">Puzzle width</param>
        /// <param name="sizeY">Puzzle height</param>
        public Puzzle(int sizeX, int sizeY)
        {
            SizeX = sizeX;
            SizeY = sizeY;
            _grid = new Cell[sizeX, sizeY];
            // populate grid
            for (int i = 0; i < sizeX; i++)
            {
                for (int j = 0; j < sizeY; j++)
                {
                    _grid[i, j] = new Cell();
                }
            }
            Horizontal = new ChecksumCollection(sizeY);
            Vertical = new ChecksumCollection(sizeX);
        }

        /// <summary>
        /// Create an empty puzzle with a preset checksum
        /// </summary>
        /// <param name="horizontal">Horizontal checksum</param>
        /// <param name="vertical">Vertical Checksum</param>
        public Puzzle(ChecksumCollection horizontal, ChecksumCollection vertical)
        {
            Horizontal = horizontal;
            Vertical = vertical;
            SizeX = Vertical.Count;
            SizeY = Horizontal.Count;
            _grid = new Cell[SizeX, SizeY];
            // populate grid
            for (int i = 0; i < SizeX; i++)
            {
                for (int j = 0; j < SizeY; j++)
                {
                    _grid[i, j] = new Cell();
                }
            }
        }
        /// <summary>
        /// Create an empty puzzle of defined height and width and if it will be inputed by checksums, add 0 to every one
        /// </summary>
        /// <param name="sizeX">Puzzle width</param>
        /// <param name="sizeY">Puzzle height</param>
        /// <param name="inputStyle">Input style</param>
        public Puzzle(int sizeX, int sizeY, MainWindow.InputStyle inputStyle) : this(sizeX, sizeY)
        {
            if (inputStyle == MainWindow.InputStyle.Checksum)
            {
                InsertZeros();
            }
        }
        
        /// <summary>
        /// Access a cell in the grid of the puzzle
        /// </summary>
        /// <param name="x">Horizontal cell position</param>
        /// <param name="y">Vertical cell position</param>
        /// <returns>Cell at the given position</returns>
        public Cell this[int x, int y] => _grid[x, y];

        /// <summary>
        /// Access a cell in the grid of the puzzle
        /// </summary>
        /// <param name="point">Cell position</param>
        /// <returns>Cell at the given position</returns>
        public Cell this[Point point] => this[point.X, point.Y];
        
        /// <summary>
        /// Sets state of a cell at a given position
        /// </summary>
        /// <param name="coordinates">Position of a cell, state of which is being set</param>
        /// <param name="cellstate">State of cell that will be set</param>
        public void Draw(Point coordinates, CellState cellstate)
        {
            if (_currentStep >= 0)
            {
                _steps.RemoveRange(_currentStep + 1, _steps.Count - _currentStep - 1);

            }
            _steps.Add(new Step(coordinates, this[coordinates].State, cellstate));
            _currentStep++;
            this[coordinates].State = cellstate;
        }

        /// <summary>
        /// Undoes the last step
        /// </summary>
        /// <returns>True if undo was successful</returns>
        public bool Undo()
        {
            if (_currentStep < 0)
            {
                return false;
            }
            var lastStep = _steps[_currentStep];
            this[lastStep.Position].State = lastStep.FromState;
            _currentStep--;
            return true;
        }
        /// <summary>
        /// Redoes the last step that was undone
        /// </summary>
        /// <returns>True if redo was successful</returns>
        public bool Redo()
        {
            if (_currentStep == _steps.Count - 1)
            {
                return false;
            }
            var nextStep = _steps[_currentStep + 1];
            this[nextStep.Position].State = nextStep.ToState;
            _currentStep++;
            return true;
        }

        /// <summary>
        /// Creates checksums from the state of a grid
        /// </summary>
        public void ChecksumFromDrawn()
        {
            Vertical.Clear();
            Horizontal.Clear();
            // create vertical checksum
            for (int i = 0; i < SizeX; i++)
            {
                int temp = 0;
                for (int j = 0; j < SizeY; j++)
                {
                    if (this[i, j].State == CellState.filled)
                    {
                        temp++;
                    }
                    else if (temp > 0)
                    {
                        Vertical[i].Add(temp);
                        temp = 0;
                    }

                }
                if (temp > 0)
                {
                    Vertical[i].Add(temp);
                }

            }
            // create horizontal checksum
            for (int i = 0; i < SizeY; i++)
            {
                int temp = 0;
                for (int j = 0; j < SizeX; j++)
                {
                    if (this[j, i].State == CellState.filled)
                    {
                        temp++;
                    }
                    else if (temp > 0)
                    {
                        Horizontal[i].Add(temp);
                        temp = 0;
                    }
                }
                if (temp > 0)
                {
                    Horizontal[i].Add(temp);
                }
            }
        }

        /// <summary>
        /// Save the puzzle into file
        /// </summary>
        /// <param name="path">File location</param>
        /// <param name="parser">Puzzle parser</param>
        public void Save(string path, Action<Puzzle, TextWriter> parser, bool saveState)
        {
            if (!saveState)
            {
                for (int i = 0; i < SizeX; i++)
                {
                    for (int j = 0; j < SizeY; j++)
                    {
                        this[i, j].State = CellState.unknown;
                    }
                }
            }
            var writer = new StreamWriter(path);
            parser(this, writer);
            writer.Close();
        }

        /// <summary>
        /// Erases the puzzle checksum
        /// </summary>
        public void ClearChecksum()
        {
            Vertical.Clear();
            Horizontal.Clear();
        }

        /// <summary>
        /// Erases the puzzle grid
        /// </summary>
        public void ClearGrid()
        {
            for (int i = 0; i < SizeX; i++)
            {
                for (int j = 0; j < SizeY; j++)
                {
                    this[i, j].State = CellState.unknown;
                }
            }
        }

        /// <summary>
        /// Returns true if the grid state fits the checksum
        /// </summary>
        /// <returns>True if solved</returns>
        public bool IsSolved()
        {
            return CheckHorizontal() && CheckVertical();
        }

        /// <summary>
        /// Check vertical checksums
        /// </summary>
        /// <returns></returns>
        private bool CheckVertical()
        {
            for (int i = 0; i < SizeX; i++)
            {
                if (!CheckVertical(i))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Check a single vertical checksum
        /// </summary>
        /// <param name="index">Index of a line to check</param>
        /// <returns>True if a state of the line coresponds with the checksum</returns>
        private bool CheckVertical(int index)
        {
            return Vertical[index].Equals(GetChecksum(Orientation.Vertical, index));
        }

        /// <summary>
        /// Check horizontal checksums
        /// </summary>
        /// <returns>True if a state of the puzzle coresponds with the checksum</returns>
        private bool CheckHorizontal()
        {
            for (int i = 0; i < SizeY; i++)
            {
                if (!CheckHorizontal(i))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Check a single horizontal checksum
        /// </summary>
        /// <param name="index">Index of a line to check</param>
        /// <returns>True if a state of the line coresponds with the checksum</returns>
        private bool CheckHorizontal(int index)
        {
            return Horizontal[index].Equals(GetChecksum(Orientation.Horizontal, index));
        }



        /// <summary>
        /// Get a specific checksum from the puzzle grid state
        /// </summary>
        /// <param name="orientation">checksum orientation</param>
        /// <param name="index">checksum index</param>
        /// <returns></returns>
        public Checksum GetChecksum(Orientation orientation, int index)
        {
            return GetChecksum(orientation, index, cell => cell.State == CellState.filled);
        }

        /// <summary>
        /// Get a specific checksum from the puzzle grid state using your own way to determine filled cell
        /// </summary>
        /// <param name="orientation">checksum orientation</param>
        /// <param name="index">checksum index</param>
        /// <param name="filledTest">delegate that returns true if cell is filled</param>
        /// <returns></returns>
        public Checksum GetChecksum(Orientation orientation, int index, Func<Cell, bool> filledTest)
        {
            Func<int, Cell> getCell;
            Func<int> getSize;
            if (orientation == Orientation.Horizontal)
            {
                getCell = i => { return this[i, index]; };
                getSize = () => { return SizeX; };
            }
            else
            {
                getCell = i => { return this[index, i]; };
                getSize = () => { return SizeY; };
            }
            var checksum = new Checksum();
            int temp = 0;
            for (int i = 0; i < getSize(); i++)
            {
                if (filledTest(getCell(i)))
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
        /// Solve the puzzle
        /// </summary>
        /// <param name="mode">Solve mode colors the whole puzzle, while hint mode colors only one cell</param>
        /// <returns>Returns false if the puzzle in its current state doesn't have a solution</returns>
        public bool Solve(SolvingMode mode = SolvingMode.solve)
        {
            bool? change = false;
            var lines = new DirectSolveLine[SizeX + SizeY];
            for (int i = 0; i < SizeY; i++)
            {
                lines[i] = new DirectSolveLine(this, Orientation.Horizontal, i);
            }
            for (int i = SizeY; i < SizeX + SizeY; i++)
            {
                lines[i] = new DirectSolveLine(this, Orientation.Vertical, i - SizeY);
            }
            // Iterate through all rows and collumns and solve them as long as change occurs
            do
            {
                change = false;
                for (int i = 0; i < SizeX + SizeY; i++)
                {
                    var newChange = lines[i].Solve(mode);
                    if (mode == SolvingMode.hint && newChange == true)
                    {
                        return true;
                    }
                    if (newChange == null)
                    {
                        return false;
                    }
                    change |= newChange;
                }
            } while ((bool) change);

            // After the direct solving method fails, solve with state space search
            if (!IsSolved())
            {
                return SolveStateSpace(mode);
            }
            return true;
        }

        /// <summary>
        /// Remove all zero values from all checksums
        /// </summary>
        public void CleanChecsums()
        {
            foreach (var checksum in Horizontal)
            {
                checksum.Clean();
            }
            foreach (var checksum in Vertical)
            {
                checksum.Clean();
            }
        }
        /// <summary>
        /// Inserts zeros at the start of every checksum
        /// </summary>
        public void InsertZeros()
        {
            foreach (var checksum in Horizontal)
            {
                checksum.Insert(0, 0);
            }
            foreach (var checksum in Vertical)
            {
                checksum.Insert(0, 0);
            }
        }

        /// <summary>
        /// Solve the puzzle using state space search
        /// </summary>
        /// <param name="mode">Solve mode colors the whole puzzle, while hint mode colors only one cell</param>
        private bool SolveStateSpace(SolvingMode mode)
        {
            StateSpaceSearchLine[] lines;
            Func<bool> check;
            Orientation offsetsOrientation;
            // choose to solve in an orientation that has less lines
            if (SizeX > SizeY)
            {
                lines = new StateSpaceSearchLine[SizeY];
                offsetsOrientation = Orientation.Horizontal;
                check = CheckVerticalTest;
            }
            else
            {
                lines = new StateSpaceSearchLine[SizeX];
                offsetsOrientation = Orientation.Vertical;
                check = CheckHorizontalTest;
            }
            for (int i = 0; i < SizeY; i++)
            {
                lines[i] = new StateSpaceSearchLine(this, i, offsetsOrientation);
            }
            while (true)
            {
                if (check())
                {
                    break;
                }
                int lastIncreased = 0;
                while (!lines[lastIncreased].IncreaseOffsets())
                {
                    lines[lastIncreased].ResetOffsets();
                    lastIncreased++;
                    if (lastIncreased >= SizeX)
                    {
                        return false;
                    }
                }
            }
            ColorTest(mode);
            return true;
        }

        /// <summary>
        /// Color the puzzle according to the "Test" value of the cell
        /// </summary>
        /// <param name="mode">Solve mode colors the whole puzzle, while hint mode colors only one cell</param>
        private void ColorTest(SolvingMode mode)
        {
            for (int i = 0; i < SizeX; i++)
            {
                for (int j = 0; j < SizeY; j++)
                {
                    if (this[i, j].Test && this[i,j].State != CellState.filled)
                    {
                        this[i, j].State = CellState.filled;
                        if (mode == SolvingMode.hint)
                        {
                            return;
                        }
                    }
                    if (!this[i, j].Test && this[i, j].State != CellState.empty)
                    {
                        this[i, j].State = CellState.empty;
                        if (mode == SolvingMode.hint)
                        {
                            return;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Check if test values of horizontal cells corespond with the horizontal checksum
        /// </summary>
        private bool CheckHorizontalTest()
        {

            for (int i = 0; i < SizeY; i++)
            {
                if (!Horizontal[i].Equals(GetChecksum(Orientation.Horizontal, i, cell => cell.Test)))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Check if test values of horizontal cells corespond with the horizontal checksum
        /// </summary>
        private bool CheckVerticalTest()
        {

            for (int i = 0; i < SizeX; i++)
            {
                if (!Vertical[i].Equals(GetChecksum(Orientation.Vertical, i, cell => cell.Test)))
                {
                    return false;
                }
            }
            return true;
        }


        /// <summary>
        /// Solve for one tile only
        /// </summary>
        public void Hint()
        {
            Solve(SolvingMode.hint);
            UI.Redraw();
        }

    }

    
}
