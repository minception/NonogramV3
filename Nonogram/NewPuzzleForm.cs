using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nonogram
{
    public partial class NewPuzzleForm : Form
    {
        public NewPuzzleForm()
        {
            InitializeComponent();
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            PuzzleWidth = (int)numericUpDownWidth.Value;
            PuzzleHeight = (int)numericUpDownHeight.Value;
        }
        public int PuzzleWidth { get; set; }
        public int PuzzleHeight { get; set; }
        public MainWindow.InputStyle InputStyle { get; set; } =  MainWindow.InputStyle.Checksum;

        private void radioButtonByChecksum_CheckedChanged(object sender, EventArgs e)
        {
            InputStyle = MainWindow.InputStyle.Checksum;
        }

        private void radioButtonByState_CheckedChanged(object sender, EventArgs e)
        {
            InputStyle = MainWindow.InputStyle.State;
        }
    }

}
