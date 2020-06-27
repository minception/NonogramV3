using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nonogram
{
    public partial class MainWindow : Form
    {
        private Puzzle _puzzle;
        private bool _checking = false;
        private bool _dragging = false;
        private Point _dragPos;
        private CellState _dragState;
        private InputStyle _inputStyle;

        /// <summary>
        /// An instance of puzzle curently displayed
        /// </summary>
        public Puzzle ActivePuzzle
        {
            get => _puzzle;
            set
            {
                if (value == null)
                {
                    toolStripButtonSave.Visible = false;
                    toolStripButtonRemove.Visible = false;
                    toolStripButtonRedo.Visible = false;
                    toolStripButtonUndo.Visible = false;
                }
                else
                {
                    toolStripButtonSave.Visible = true;
                    toolStripButtonRemove.Visible = true;
                    if (_inputStyle != InputStyle.Checksum)
                    {
                        toolStripButtonRedo.Visible = true;
                        toolStripButtonUndo.Visible = true;
                    }
                }
                _puzzle = value;
                UI.Puzzle = value;
            }
        }

        /// <summary>
        /// True if the puzzle is in checking mode (checks if puzzle is solved after every move)
        /// </summary>
        public bool Checking
        {
            get => _checking;
            set
            {
                if (value)
                {
                    toolStripButtonHint.Visible = true;
                    toolStripButtonFinish.Visible = true;
                    toolStripButtonStartOver.Visible = true;
                    _inputStyle = InputStyle.State;
                }
                else
                {
                    toolStripButtonHint.Visible = false;
                    toolStripButtonFinish.Visible = false;
                    toolStripButtonStartOver.Visible = false;
                }
                _checking = value;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            display.Image = new Bitmap(display.Width, display.Height);
            UI.Graphics = Graphics.FromImage(display.Image);
            UI.DisplaySizeX = display.Width;
            UI.DisplaySizeY = display.Height;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
        }

        private void toolStripButtonOpen_Click(object sender, EventArgs e)
        {
            openFileDialog.Filter = "Saved puzzles|*.json;*.xml";
            openFileDialog.FilterIndex = 2;
            openFileDialog.ShowDialog();
        }

        private void openFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            var extension = Path.GetExtension(openFileDialog.FileName);
            extension = extension.ToUpper();
            switch (extension)
            {
                case ".XML":
                    {
                        ActivePuzzle = Puzzle.LoadFromFile(openFileDialog.FileName, Parser.XmlToPuzzle);
                        break;
                    }
                case ".JSON":
                    {
                        ActivePuzzle = Puzzle.LoadFromFile(openFileDialog.FileName, Parser.JsonToPuzzle);
                        break;
                    }
                default:
                    {
                        throw new FileLoadException();
                    }
            }
            UI.Redraw();
            // change to solving mode
            Checking = true;
        }

        private void toolStripButtonNew_Click(object sender, EventArgs e)
        {
            var newForm = new NewPuzzleForm();
            newForm.ShowDialog();
            if(newForm.DialogResult == DialogResult.OK)
            {
                _inputStyle = newForm.InputStyle;
                ActivePuzzle = new Puzzle(newForm.PuzzleWidth, newForm.PuzzleHeight, _inputStyle);
                UI.Redraw();
            }
            Checking = false;

        }

        private void timer_Tick(object sender, EventArgs e)
        {
            display.Refresh();
        }

        private void display_SizeChanged(object sender, EventArgs e)
        {
            if(ActivePuzzle != null)
            {
                display.Image = new Bitmap(display.Width > 0 ? display.Width : 1, display.Height > 0 ? display.Height : 1);
                UI.Graphics = Graphics.FromImage(display.Image);
                UI.DisplaySizeX = display.Width;
                UI.DisplaySizeY = display.Height;
                UI.Redraw();
            }
        }

        private void display_MouseDown(object sender, MouseEventArgs e)
        {
            if (ActivePuzzle != null)
            {
                Point point = display.PointToClient(Cursor.Position);
                if (_inputStyle == InputStyle.State)
                {
                    ClickedState(point, e.Button);
                }
                else
                {
                    ClickedChecksum(point, e.Button);
                }
                UI.Redraw();
            }
        }

        private void ClickedChecksum(Point point, MouseButtons button)
        {
            bool insert = false;
            var clickedChecksum = UI.ClickedChecksum(point);
            if (clickedChecksum == null)
            {
                return;
            }
            Puzzle.ChecksumCollection checksumCollection;
            if (clickedChecksum.Item2 == Orientation.Horizontal)
            {
                checksumCollection = _puzzle.Horizontal;
            }
            else
            {
                checksumCollection = _puzzle.Vertical;
            }
            var position = clickedChecksum.Item1;
            if (button == MouseButtons.Left)
            {
                checksumCollection[position.X][position.Y]++;
                if (checksumCollection[position.X][position.Y] == 1)
                {
                    checksumCollection[position.X].Insert(0,0);
                }
            }
            else if(checksumCollection[position.X][position.Y] != 0)
            {
                checksumCollection[position.X][position.Y]--;
            }
        }

        private void ClickedState(Point point, MouseButtons button)
        {
            _dragPos = UI.ClickedCell(point);
            switch (button)
            {
                // left mouse press: if filled redraw, if empty fill
                case MouseButtons.Left:
                {
                    LeftClick(point);
                    break;
                }
                case MouseButtons.Right:
                {
                    RightClick(point);
                    break;
                }
            }
            ActivePuzzle.Draw(_dragPos, _dragState);
            UI.Redraw();
            if (button != MouseButtons.None && Checking)
            {
                if (ActivePuzzle.IsSolved())
                {
                    Win();
                    _dragging = false;
                    return;
                }
            }

            _dragging = true;

        }

        private void RightClick(Point point)
        {
            if (ActivePuzzle[_dragPos].State == CellState.empty)
            {
                _dragState = CellState.unknown;
            }
            else
            {
                _dragState = CellState.empty;
            }
        }

        private void LeftClick(Point point)
        {
            if (ActivePuzzle[_dragPos].State == CellState.filled)
            {
                _dragState = CellState.unknown;
            }
            else
            {
                _dragState = CellState.filled;
            }
        }

        private void toolStripButtonSave_Click(object sender, EventArgs e)
        {
            // No need to generate checksum when saving progress or inputing by checksum
            if (!Checking && _inputStyle == InputStyle.State)
            {
                ActivePuzzle.ChecksumFromDrawn();
            }
            if (_inputStyle == InputStyle.Checksum)
            {
                _puzzle.CleanChecsums();
                if (!_puzzle.Solve())
                {
                    MessageBox.Show("Only solvable puzzles allowed");
                    _puzzle.ClearGrid();
                    _puzzle.InsertZeros();
                    return;
                }
                _puzzle.ClearGrid();
            }
            saveFileDialog.Filter = "json files (*.json)|*.json|xml files (*.xml)|*.xml";
            saveFileDialog.FilterIndex = 2;
            saveFileDialog.ShowDialog();
        }

        private void saveFileDialog_FileOk(object sender, CancelEventArgs e)
        {

            var extension = Path.GetExtension(saveFileDialog.FileName);
            extension = extension.ToUpper();
            switch (extension)
            {
                case ".XML":
                    {
                        ActivePuzzle.Save(saveFileDialog.FileName, Parser.PuzzleToXml, Checking);
                        break;
                    }
                case ".JSON":
                    {
                        ActivePuzzle.Save(saveFileDialog.FileName, Parser.PuzzleToJson, Checking);
                        break;
                    }
                default:
                    {
                        throw new FileLoadException();
                    }
            }
            // Remove puzzle after saving
            _puzzle = null;
            UI.Redraw();
        }

        private void toolStripButtonDelete_Click(object sender, EventArgs e)
        {
            ActivePuzzle = null;
            UI.Redraw();
            Checking = false;
        }

        private void toolStripButtonHint_Click(object sender, EventArgs e)
        {
            var solution = new Thread(ActivePuzzle.Hint);
            solution.Start();

        }

        private void Win()
        {
            var winForm = new WinningFormcs();
            winForm.ShowDialog();
        }
        private void display_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragging)
            {
                Point point = display.PointToClient(Cursor.Position);
                Point coordinates = UI.ClickedCell(point);
                if (_dragPos != coordinates)
                {
                    ActivePuzzle.Draw(coordinates, _dragState);
                    UI.Redraw();
                    if (e.Button != MouseButtons.None && Checking)
                    {
                        if (ActivePuzzle.IsSolved())
                        {
                            Win();
                            _dragging = false;
                            return;
                        }
                    }
                    _dragPos = coordinates;
                }

            }
        }

        private void display_MouseUp(object sender, MouseEventArgs e)
        {
            _dragging = false;
        }

        private void FinishButton_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("Are you sure you want to autosolve the puzzle ??",
                                     "Confirm autosolve!!",
                                     MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                _puzzle.Solve();
                UI.Redraw();
            }
        }

        private void toolStripButtonRedo_Click(object sender, EventArgs e)
        {
            ActivePuzzle.ClearGrid();
            UI.Redraw();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            ActivePuzzle.Undo();
            UI.Redraw();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            ActivePuzzle.Redo();
            UI.Redraw();
        }
        

        public enum InputStyle
        {
            State,Checksum
        }
    }
}
