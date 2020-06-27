using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Nonogram
{
    /// <summary>
    /// Static class containing an user interface for drawing and interaction with a puzzle
    /// </summary>
    public static class UI
    {
        private static Puzzle _puzzle;
        private static int _cellSize;
        private static int _gridOffsetX;
        private static int _gridOffsetY;
        private static int _displaySizeX;
        private static int _displaySizeY;
        private static Graphics _graphics;

        /// <summary>
        /// Current active puzzle
        /// </summary>
        public static Puzzle Puzzle { private get => _puzzle; set => _puzzle = value; }
        /// <summary>
        /// Horizontal size of canvas in pixels
        /// </summary>
        public static int DisplaySizeX { private get => _displaySizeX; set => _displaySizeX = value; }
        /// <summary>
        /// Vertical size of canvas in pixels
        /// </summary>
        public static int DisplaySizeY { private get => _displaySizeY; set => _displaySizeY = value; }
        /// <summary>
        /// The canvas
        /// </summary>
        public static Graphics Graphics { private get => _graphics; set => _graphics = value; }

        #region Drawing
        /// <summary>
        /// Completely redraw a puzzle. If puzzle doesn't exist, draws clear screen
        /// </summary>
        public static void Redraw()
        {
            Graphics.Clear(SystemColors.Control);
            if (Puzzle != null)
            {
                _cellSize = GetCellSize();

                // center the puzzle
                int centerOffsetX = GetCenterOffset(DisplaySizeX, Puzzle.SizeX, Puzzle.Horizontal.MaxCount);
                int centerOffsetY = GetCenterOffset(DisplaySizeY, Puzzle.SizeY, Puzzle.Vertical.MaxCount);


                _gridOffsetX = centerOffsetX + Puzzle.Horizontal.MaxCount * _cellSize;
                _gridOffsetY = centerOffsetY + Puzzle.Vertical.MaxCount * _cellSize;

                DrawGrid();
                DrawChecksum();
                DrawState();
            }
        }

        /// <summary>
        /// Draws grid based on values of local static variables
        /// </summary>
        public static void DrawGrid()
        {
            Pen drawPen;
            int startY = _gridOffsetY;
            int endY = _gridOffsetY + Puzzle.SizeY * _cellSize;
            for (int i = 0; i <= Puzzle.SizeX; i++)
            {
                drawPen = chooseGridPen(i, DisplaySizeX);
                int x = _gridOffsetX + i * _cellSize;
                Graphics.DrawLine(drawPen, x, startY, x, endY);
            }
            int startX = _gridOffsetX;
            int endX = _gridOffsetX + Puzzle.SizeX * _cellSize;
            for (int i = 0; i <= Puzzle.SizeY; i++)
            {
                drawPen = chooseGridPen(i, DisplaySizeY);
                int y = _gridOffsetY + i * _cellSize;
                Graphics.DrawLine(drawPen, startX, y, endX, y);
            }
        }

        /// <summary>
        /// Draws checksum based on local static variables
        /// </summary>
        public static void DrawChecksum()
        {
            DrawOneDirectionChecksum(Puzzle.Horizontal, Orientation.Horizontal);
            DrawOneDirectionChecksum(Puzzle.Vertical, Orientation.Vertical);
        }
        private static void DrawOneDirectionChecksum(Puzzle.ChecksumCollection toDraw, Orientation orientation)
        {
            Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;
            Font font1 = new Font("Yu Gothic UI", _cellSize / 2 > 0 ? _cellSize / 2 : 1, FontStyle.Regular, GraphicsUnit.Point);
            Func<int, int, Rectangle> rectangleFactory;

            if (orientation == Orientation.Horizontal)
            {
                rectangleFactory = (i, j) => { return new Rectangle(_gridOffsetX - (j + 1) * _cellSize, _gridOffsetY + i * _cellSize, _cellSize, _cellSize); };
            }
            else // if(orientation == Orientation.Vertical)
            {
                rectangleFactory = (i, j) => { return new Rectangle(_gridOffsetX + i * _cellSize, _gridOffsetY - (j + 1) * _cellSize, _cellSize, _cellSize); };
            }
            for (int i = 0; i < toDraw.Count; i++)
            {
                for (int j = 0; j < toDraw[i].Count; j++)
                {
                    Rectangle rect = rectangleFactory(i, j);
                    Graphics.DrawString(toDraw[i][toDraw[i].Count - 1 - j].ToString(), font1, Brushes.Black, rect, stringFormat);

                }
            }
        }

        /// <summary>
        /// Draws state of the puzzle in local static variable
        /// </summary>
        public static void DrawState()
        {
            for (int i = 0; i < Puzzle.SizeX; i++)
            {
                for (int j = 0; j < Puzzle.SizeY; j++)
                {
                    Point toDraw = new Point(_gridOffsetX + i * _cellSize, _gridOffsetY + j * _cellSize);
                    if (Puzzle[i, j].State == CellState.filled)
                    {
                        drawFilled(Brushes.Black, toDraw.X, toDraw.Y);
                    }
                    else if(Puzzle[i,j].State == CellState.empty)
                    {
                        drawEmpty(toDraw.X, toDraw.Y);
                    }
                }
            }
        }
        /// <summary>
        /// Draw a single filled square
        /// </summary>
        /// <param name="color">Color of the square being drawn</param>
        /// <param name="posX">Horisontal position of the square on the grid</param>
        /// <param name="posY">Vertical position of the square on the grid</param>
        private static void drawFilled(Brush color, int posX, int posY)
        {
            Graphics.FillRectangle(color, posX + 2, posY + 2, _cellSize - 3, _cellSize - 3);
        }

        /// <summary>
        /// Draw a single empty square
        /// </summary>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        private static void drawEmpty(int posX, int posY)
        {
            // A dot in a center
            Graphics.FillEllipse(Brushes.Black, posX + 4 * _cellSize / 10, posY + 4 * _cellSize / 10, _cellSize / 5, _cellSize / 5);
        }


        #endregion
        // Interaction

        /// <summary>
        /// Transforms position given in pixels to a position on a grid
        /// </summary>
        /// <param name="pointClicked">Position clicked in pixels</param>
        /// <returns>Position of cell clicked</returns>
        public static Point ClickedCell(Point pointClicked)
        {
                int clickedX = (pointClicked.X - _gridOffsetX) / _cellSize;
                int clickedY = (pointClicked.Y - _gridOffsetY) / _cellSize;
                DrawBoundaries(ref clickedX, ref clickedY);
                return new Point(clickedX, clickedY);
        }

        /// <summary>
        /// Transforms position given in pixels to a position on a checksum
        /// </summary>
        /// <param name="pointClicked">Position clicked in pixels</param>
        /// <returns>Tuple containing orientation and position of checksum clicked</returns>
        public static Tuple<Point, Orientation> ClickedChecksum(Point pointClicked)
        {
            if (!ChecksumBoundaries(pointClicked))
            {
                return null;
            }
            var orientation = pointClicked.X < _gridOffsetX ? Orientation.Horizontal : Orientation.Vertical;
            int checksumClicked, valueClicked;
            if (orientation == Orientation.Horizontal)
            {
                checksumClicked = (pointClicked.Y - _gridOffsetY) / _cellSize;
                valueClicked = _puzzle.Horizontal[checksumClicked].Count - (_gridOffsetX - pointClicked.X) / _cellSize - 1;
            }
            else
            {
                checksumClicked = (pointClicked.X - _gridOffsetX) / _cellSize;
                valueClicked = _puzzle.Vertical[checksumClicked].Count - (_gridOffsetY - pointClicked.Y) / _cellSize - 1;
            }
            if (valueClicked < 0)
            {
                valueClicked = 0;
            }
            return new Tuple<Point, Orientation>(new Point(checksumClicked,valueClicked), orientation);
        }

        /// <summary>
        /// Checks if checksum was clicked
        /// </summary>
        /// <param name="pointClicked">Position of clicked point</param>
        /// <returns>True if checksum was clicked</returns>
        private static bool ChecksumBoundaries(Point pointClicked)
        {
            if (pointClicked.X > _gridOffsetX && pointClicked.Y > _gridOffsetY)
            {
                // Clicked on grid
                return false;
            }
            if (pointClicked.X < _gridOffsetX && pointClicked.Y < _gridOffsetY)
            {
                // Clicked between checksums
                return false;
            }
            if (pointClicked.X > _gridOffsetX + _cellSize * _puzzle.SizeX ||
                pointClicked.Y > _gridOffsetY + _cellSize * _puzzle.SizeY)
            {
                // Clicked outside of checksums
                return false;
            }
            return true;
        }

        /// <summary>
        /// Make sure user doesn't click outside the boundaries
        /// </summary>
        /// <param name="clickedX">Horizontal clicked cell position</param>
        /// <param name="clickedY">Vertical clicked cell position</param>
        private static void DrawBoundaries(ref int clickedX, ref int clickedY)
        {
            // To the left of the puzzle
            if (clickedX < 0)
            {
                clickedX = 0;
            }
            // To the right of the puzzle
            if (clickedX >= Puzzle.SizeX)
            {
                clickedX = Puzzle.SizeX - 1;
            }
            // Above the puzzle
            if (clickedY < 0)
            {
                clickedY = 0;
            }
            // Below the puzzle
            if (clickedY >= Puzzle.SizeY)
            {
                clickedY = Puzzle.SizeY - 1;
            }
        }

        // Private functions
        /// <summary>
        /// Chooses the thickness of a grid line based on its position in the grid
        /// </summary>
        /// <param name="lineNumber">Number of a line</param>
        /// <param name="size">Size of the grid</param>
        /// <returns></returns>
        private static Pen chooseGridPen(int lineNumber, int size)
        {
            // edge line is 3 pixels thick
            if (lineNumber == 0 || lineNumber == size)
            {
                return new Pen(Color.Black, 3);
            }
            // every fifth line is 2 thick 
            if (lineNumber % 5 == 0)
            {
                return new Pen(Color.Black, 2);
            }
            // rest of the lines are regular black pen
            return Pens.Black;
        }

        /// <summary>
        /// Generates a size of an offset needed to center the puzzle in pixels
        /// </summary>
        /// <param name="displaySize">Size of a canvas</param>
        /// <param name="puzzleSize">Number of cells of puzzle in a given direction</param>
        /// <param name="maxCount">Highest number of checksum values for a single line in a given orientation</param>
        /// <returns>Size of an ofset needed to center the puzzle in pixels</returns>
        private static int GetCenterOffset(int displaySize, int puzzleSize, int maxCount)
        {
            return (displaySize - (puzzleSize + maxCount) * _cellSize) / 2;
        }

        /// <summary>
        /// Generates the size of a cell based on a sie of window and size of a puzzle
        /// </summary>
        /// <returns>Size of a cell in pixels</returns>
        private static int GetCellSize()
        {
            // calculate cellsize, making maximal possible use of a given display
            int cellSize1 = DisplaySizeX / (Puzzle.SizeX + Puzzle.Horizontal.MaxCount);
            int cellSize2 = DisplaySizeY / (Puzzle.SizeY + Puzzle.Vertical.MaxCount);
            return Math.Min(cellSize1, cellSize2);
        }
    }
}
