namespace OCCPort.Tester
{
    partial class Form1
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
			this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
			this.frontToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.topToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// timer1
			// 
			this.timer1.Enabled = true;
			this.timer1.Interval = 22;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// toolStrip1
			// 
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripDropDownButton1});
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(907, 25);
			this.toolStrip1.TabIndex = 0;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// toolStripButton1
			// 
			this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
			this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton1.Name = "toolStripButton1";
			this.toolStripButton1.Size = new System.Drawing.Size(42, 22);
			this.toolStripButton1.Text = "rotate";
			this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
			// 
			// toolStripDropDownButton1
			// 
			this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.frontToolStripMenuItem,
            this.topToolStripMenuItem});
			this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
			this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
			this.toolStripDropDownButton1.Size = new System.Drawing.Size(59, 22);
			this.toolStripDropDownButton1.Text = "camera";
			// 
			// frontToolStripMenuItem
			// 
			this.frontToolStripMenuItem.Name = "frontToolStripMenuItem";
			this.frontToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.frontToolStripMenuItem.Text = "front";
			this.frontToolStripMenuItem.Click += new System.EventHandler(this.frontToolStripMenuItem_Click);
			// 
			// topToolStripMenuItem
			// 
			this.topToolStripMenuItem.Name = "topToolStripMenuItem";
			this.topToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.topToolStripMenuItem.Text = "top";
			this.topToolStripMenuItem.Click += new System.EventHandler(this.topToolStripMenuItem_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(907, 512);
			this.Controls.Add(this.toolStrip1);
			this.Name = "Form1";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "OCCPort testing";
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
		private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
		private System.Windows.Forms.ToolStripMenuItem frontToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem topToolStripMenuItem;
	}
}

