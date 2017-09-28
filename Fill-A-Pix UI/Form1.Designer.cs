namespace Fill_A_Pix_UI
{
    partial class FrmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            if( disposing && (components != null) )
            {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.imageMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.captureMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gameMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.resetMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stepBackMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stepMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.solveMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.tsbOpen = new System.Windows.Forms.ToolStripButton();
            this.tsbCapture = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbReset = new System.Windows.Forms.ToolStripButton();
            this.tsbStepBack = new System.Windows.Forms.ToolStripButton();
            this.tsbStep = new System.Windows.Forms.ToolStripButton();
            this.tsbSolve = new System.Windows.Forms.ToolStripButton();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.panel1 = new System.Windows.Forms.Panel();
            this.menuStrip1.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.imageMenu,
            this.gameMenu});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(898, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.Visible = false;
            // 
            // imageMenu
            // 
            this.imageMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.captureMenuItem,
            this.openMenuItem});
            this.imageMenu.Name = "imageMenu";
            this.imageMenu.Size = new System.Drawing.Size(52, 20);
            this.imageMenu.Text = "Image";
            // 
            // captureMenuItem
            // 
            this.captureMenuItem.Name = "captureMenuItem";
            this.captureMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.captureMenuItem.Size = new System.Drawing.Size(186, 22);
            this.captureMenuItem.Text = "New Capture";
            this.captureMenuItem.Click += new System.EventHandler(this.CaptureMenuItem_Click);
            // 
            // openMenuItem
            // 
            this.openMenuItem.Name = "openMenuItem";
            this.openMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openMenuItem.Size = new System.Drawing.Size(186, 22);
            this.openMenuItem.Text = "Open";
            this.openMenuItem.Click += new System.EventHandler(this.OpenMenuItem_Click);
            // 
            // gameMenu
            // 
            this.gameMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.resetMenuItem,
            this.stepBackMenuItem,
            this.stepMenuItem,
            this.solveMenuItem});
            this.gameMenu.Enabled = false;
            this.gameMenu.Name = "gameMenu";
            this.gameMenu.Size = new System.Drawing.Size(50, 20);
            this.gameMenu.Text = "Game";
            // 
            // resetMenuItem
            // 
            this.resetMenuItem.Enabled = false;
            this.resetMenuItem.Name = "resetMenuItem";
            this.resetMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.resetMenuItem.Size = new System.Drawing.Size(179, 22);
            this.resetMenuItem.Text = "Reset";
            this.resetMenuItem.Click += new System.EventHandler(this.ResetMenuItem_Click);
            // 
            // stepBackMenuItem
            // 
            this.stepBackMenuItem.Enabled = false;
            this.stepBackMenuItem.Name = "stepBackMenuItem";
            this.stepBackMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Left)));
            this.stepBackMenuItem.Size = new System.Drawing.Size(179, 22);
            this.stepBackMenuItem.Text = "Step Back";
            this.stepBackMenuItem.Click += new System.EventHandler(this.StepBackMenuItem_Click);
            // 
            // stepMenuItem
            // 
            this.stepMenuItem.Enabled = false;
            this.stepMenuItem.Name = "stepMenuItem";
            this.stepMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Right)));
            this.stepMenuItem.Size = new System.Drawing.Size(179, 22);
            this.stepMenuItem.Text = "Step";
            this.stepMenuItem.Click += new System.EventHandler(this.StepMenuItem_Click);
            // 
            // solveMenuItem
            // 
            this.solveMenuItem.Name = "solveMenuItem";
            this.solveMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.solveMenuItem.Size = new System.Drawing.Size(179, 22);
            this.solveMenuItem.Text = "Solve";
            this.solveMenuItem.Click += new System.EventHandler(this.SolveMenuItem_Click);
            // 
            // toolStrip
            // 
            this.toolStrip.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbOpen,
            this.tsbCapture,
            this.toolStripSeparator1,
            this.tsbReset,
            this.tsbStepBack,
            this.tsbStep,
            this.tsbSolve});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(898, 39);
            this.toolStrip.TabIndex = 2;
            this.toolStrip.Text = "toolStrip1";
            // 
            // tsbOpen
            // 
            this.tsbOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbOpen.Font = new System.Drawing.Font("Felix Titling", 12F);
            this.tsbOpen.Image = ((System.Drawing.Image)(resources.GetObject("tsbOpen.Image")));
            this.tsbOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbOpen.Name = "tsbOpen";
            this.tsbOpen.Size = new System.Drawing.Size(36, 36);
            this.tsbOpen.Text = "Open Image";
            this.tsbOpen.Click += new System.EventHandler(this.OpenMenuItem_Click);
            // 
            // tsbCapture
            // 
            this.tsbCapture.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbCapture.Image = ((System.Drawing.Image)(resources.GetObject("tsbCapture.Image")));
            this.tsbCapture.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbCapture.Name = "tsbCapture";
            this.tsbCapture.Size = new System.Drawing.Size(36, 36);
            this.tsbCapture.Text = "Capture Window";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 39);
            // 
            // tsbReset
            // 
            this.tsbReset.AccessibleRole = System.Windows.Forms.AccessibleRole.Animation;
            this.tsbReset.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbReset.Enabled = false;
            this.tsbReset.Image = ((System.Drawing.Image)(resources.GetObject("tsbReset.Image")));
            this.tsbReset.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbReset.Name = "tsbReset";
            this.tsbReset.Size = new System.Drawing.Size(36, 36);
            this.tsbReset.Text = "Reset State";
            this.tsbReset.Click += new System.EventHandler(this.ResetMenuItem_Click);
            // 
            // tsbStepBack
            // 
            this.tsbStepBack.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbStepBack.Enabled = false;
            this.tsbStepBack.Image = ((System.Drawing.Image)(resources.GetObject("tsbStepBack.Image")));
            this.tsbStepBack.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbStepBack.Name = "tsbStepBack";
            this.tsbStepBack.Size = new System.Drawing.Size(36, 36);
            this.tsbStepBack.Text = "Step Backwards";
            this.tsbStepBack.Click += new System.EventHandler(this.StepBackMenuItem_Click);
            // 
            // tsbStep
            // 
            this.tsbStep.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbStep.Enabled = false;
            this.tsbStep.Image = ((System.Drawing.Image)(resources.GetObject("tsbStep.Image")));
            this.tsbStep.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbStep.Name = "tsbStep";
            this.tsbStep.Size = new System.Drawing.Size(36, 36);
            this.tsbStep.Text = "Step Forwards";
            this.tsbStep.Click += new System.EventHandler(this.StepMenuItem_Click);
            // 
            // tsbSolve
            // 
            this.tsbSolve.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbSolve.Image = ((System.Drawing.Image)(resources.GetObject("tsbSolve.Image")));
            this.tsbSolve.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSolve.Name = "tsbSolve";
            this.tsbSolve.Size = new System.Drawing.Size(36, 36);
            this.tsbSolve.Text = "Solve";
            this.tsbSolve.Click += new System.EventHandler(this.SolveMenuItem_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "*.png";
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Filter = "PNG|*.png|All Files|*.*";
            this.openFileDialog1.SupportMultiDottedExtensions = true;
            this.openFileDialog1.Title = "Open Image File";
            this.openFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.OpenFileDialog1_FileOk);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 39);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(898, 633);
            this.panel1.TabIndex = 3;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.Panel1_Paint);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(898, 672);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FrmMain";
            this.Text = "Fill-A-Pix";
            this.Resize += new System.EventHandler(this.FrmMain_Resize);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem imageMenu;
        private System.Windows.Forms.ToolStripMenuItem openMenuItem;
        private System.Windows.Forms.ToolStripMenuItem captureMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ToolStripMenuItem gameMenu;
        private System.Windows.Forms.ToolStripMenuItem resetMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stepMenuItem;
        private System.Windows.Forms.ToolStripMenuItem solveMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton tsbOpen;
        private System.Windows.Forms.ToolStripButton tsbCapture;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton tsbReset;
        private System.Windows.Forms.ToolStripButton tsbStepBack;
        private System.Windows.Forms.ToolStripButton tsbStep;
        private System.Windows.Forms.ToolStripButton tsbSolve;
        private System.Windows.Forms.ToolStripMenuItem stepBackMenuItem;
        private System.Windows.Forms.Panel panel1;
    }
}

