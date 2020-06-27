namespace Nonogram
{
    partial class MainWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.display = new System.Windows.Forms.PictureBox();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonAdd = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonOpen = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonSave = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonRemove = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonHint = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonFinish = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonStartOver = new System.Windows.Forms.ToolStripButton();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.toolStripButtonUndo = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonRedo = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.display)).BeginInit();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // display
            // 
            this.display.Dock = System.Windows.Forms.DockStyle.Fill;
            this.display.InitialImage = ((System.Drawing.Image)(resources.GetObject("display.InitialImage")));
            this.display.Location = new System.Drawing.Point(0, 25);
            this.display.Name = "display";
            this.display.Size = new System.Drawing.Size(426, 408);
            this.display.TabIndex = 0;
            this.display.TabStop = false;
            this.display.SizeChanged += new System.EventHandler(this.display_SizeChanged);
            this.display.MouseDown += new System.Windows.Forms.MouseEventHandler(this.display_MouseDown);
            this.display.MouseMove += new System.Windows.Forms.MouseEventHandler(this.display_MouseMove);
            this.display.MouseUp += new System.Windows.Forms.MouseEventHandler(this.display_MouseUp);
            // 
            // toolStrip
            // 
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonAdd,
            this.toolStripButtonOpen,
            this.toolStripButtonSave,
            this.toolStripButtonRemove,
            this.toolStripButtonHint,
            this.toolStripButtonFinish,
            this.toolStripButtonStartOver,
            this.toolStripButtonUndo,
            this.toolStripButtonRedo});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(426, 25);
            this.toolStrip.Stretch = true;
            this.toolStrip.TabIndex = 0;
            this.toolStrip.Text = "toolStrip";
            // 
            // toolStripButtonAdd
            // 
            this.toolStripButtonAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonAdd.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonAdd.Image")));
            this.toolStripButtonAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonAdd.Name = "toolStripButtonAdd";
            this.toolStripButtonAdd.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonAdd.Text = "New";
            this.toolStripButtonAdd.Click += new System.EventHandler(this.toolStripButtonNew_Click);
            // 
            // toolStripButtonOpen
            // 
            this.toolStripButtonOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonOpen.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonOpen.Image")));
            this.toolStripButtonOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonOpen.Name = "toolStripButtonOpen";
            this.toolStripButtonOpen.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonOpen.Text = "Open";
            this.toolStripButtonOpen.Click += new System.EventHandler(this.toolStripButtonOpen_Click);
            // 
            // toolStripButtonSave
            // 
            this.toolStripButtonSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonSave.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonSave.Image")));
            this.toolStripButtonSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSave.Name = "toolStripButtonSave";
            this.toolStripButtonSave.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonSave.Text = "Save";
            this.toolStripButtonSave.Visible = false;
            this.toolStripButtonSave.Click += new System.EventHandler(this.toolStripButtonSave_Click);
            // 
            // toolStripButtonRemove
            // 
            this.toolStripButtonRemove.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonRemove.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonRemove.Image")));
            this.toolStripButtonRemove.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRemove.Name = "toolStripButtonRemove";
            this.toolStripButtonRemove.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonRemove.Text = "Remove Puzzle";
            this.toolStripButtonRemove.Visible = false;
            this.toolStripButtonRemove.Click += new System.EventHandler(this.toolStripButtonDelete_Click);
            // 
            // toolStripButtonHint
            // 
            this.toolStripButtonHint.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButtonHint.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonHint.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonHint.Image")));
            this.toolStripButtonHint.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonHint.Name = "toolStripButtonHint";
            this.toolStripButtonHint.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonHint.Text = "Hint";
            this.toolStripButtonHint.Visible = false;
            this.toolStripButtonHint.Click += new System.EventHandler(this.toolStripButtonHint_Click);
            // 
            // toolStripButtonFinish
            // 
            this.toolStripButtonFinish.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButtonFinish.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonFinish.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.toolStripButtonFinish.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonFinish.Image")));
            this.toolStripButtonFinish.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonFinish.Name = "toolStripButtonFinish";
            this.toolStripButtonFinish.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.toolStripButtonFinish.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonFinish.Text = "Finish Puzzle";
            this.toolStripButtonFinish.Visible = false;
            this.toolStripButtonFinish.Click += new System.EventHandler(this.FinishButton_Click);
            // 
            // toolStripButtonStartOver
            // 
            this.toolStripButtonStartOver.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonStartOver.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonStartOver.Image")));
            this.toolStripButtonStartOver.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonStartOver.Name = "toolStripButtonStartOver";
            this.toolStripButtonStartOver.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonStartOver.Text = "Start over";
            this.toolStripButtonStartOver.Visible = false;
            this.toolStripButtonStartOver.Click += new System.EventHandler(this.toolStripButtonRedo_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog";
            this.openFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog_FileOk);
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Interval = 16;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.saveFileDialog_FileOk);
            // 
            // toolStripButtonUndo
            // 
            this.toolStripButtonUndo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonUndo.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonUndo.Image")));
            this.toolStripButtonUndo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonUndo.Name = "toolStripButtonUndo";
            this.toolStripButtonUndo.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonUndo.Text = "Undo";
            this.toolStripButtonUndo.ToolTipText = "toolStripButtonUndo";
            this.toolStripButtonUndo.Visible = false;
            this.toolStripButtonUndo.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripButtonRedo
            // 
            this.toolStripButtonRedo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonRedo.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonRedo.Image")));
            this.toolStripButtonRedo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRedo.Name = "toolStripButtonRedo";
            this.toolStripButtonRedo.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonRedo.Text = "Redo";
            this.toolStripButtonRedo.ToolTipText = "toolStripButtonRedo";
            this.toolStripButtonRedo.Visible = false;
            this.toolStripButtonRedo.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(426, 433);
            this.Controls.Add(this.display);
            this.Controls.Add(this.toolStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainWindow";
            this.Text = "Nonogram";
            ((System.ComponentModel.ISupportInitialize)(this.display)).EndInit();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox display;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton toolStripButtonOpen;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.ToolStripButton toolStripButtonAdd;
        private System.Windows.Forms.ToolStripButton toolStripButtonSave;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.ToolStripButton toolStripButtonRemove;
        private System.Windows.Forms.ToolStripButton toolStripButtonHint;
        private System.Windows.Forms.ToolStripButton toolStripButtonFinish;
        private System.Windows.Forms.ToolStripButton toolStripButtonStartOver;
        private System.Windows.Forms.ToolStripButton toolStripButtonUndo;
        private System.Windows.Forms.ToolStripButton toolStripButtonRedo;
    }
}

