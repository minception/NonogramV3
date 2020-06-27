using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nonogram
{
    /// <summary>
    /// A single change made by a user
    /// </summary>
    struct Step
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="position">Position on a grid where the step is being made</param>
        /// <param name="previouState">State of the cell before the step is made</param>
        /// <param name="newState">State of the cell after the step is made</param>
        public Step(Point position, CellState previouState, CellState newState)
        {
            Position = position;
            FromState = previouState;
            ToState = newState;
        }
        /// <summary>
        /// Position of the change
        /// </summary>
        public Point Position { get; private set; }
        /// <summary>
        /// The state before the change
        /// </summary>
        public CellState FromState { get; private set; }
        /// <summary>
        /// The state after the change
        /// </summary>
        public CellState ToState { get; private set; }
    }
}
