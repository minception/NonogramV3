using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nonogram
{
    public class Cell:ICell
    {
        private object stateLock = new object();
        private CellState _state;
        public Cell()
        {
            State = CellState.unknown;
        }
        /// <summary>
        /// Copy constructor
        /// </summary>
        public Cell(Cell otherCell)
        {
            State = otherCell.State;
        }
        /// <summary>
        /// Thread safe property of a cell
        /// </summary>
        public CellState State
        {
            get
            {
                CellState value;
                lock (stateLock)
                {
                    value = _state;
                }
                return value;
            }
            set
            {
                lock (stateLock)
                {
                    _state = value;
                }
            }
        }
        /// <summary>
        /// Test value meant for state space search
        /// </summary>
        public bool Test { get; set; }
    }
    public enum CellState { empty, filled, unknown }
}