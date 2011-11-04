namespace SleepyTunes
{
    partial class Gui
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Gui));
            this.comboAction = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtRunCmd = new System.Windows.Forms.TextBox();
            this.chkExit = new System.Windows.Forms.CheckBox();
            this.txtRestoreVol = new System.Windows.Forms.TextBox();
            this.chkRestoreVol = new System.Windows.Forms.CheckBox();
            this.chkMuteWhenDone = new System.Windows.Forms.CheckBox();
            this.zg1 = new ZedGraph.ZedGraphControl();
            this.btnGo = new System.Windows.Forms.Button();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboAction
            // 
            this.comboAction.Items.AddRange(new object[] {
            "Do nothing",
            "Standby",
            "Hibernate",
            "Shut Down",
            "Reboot",
            "Lock Workstation (Show Welcome Screen)",
            "Log Off",
            "Execute Command..."});
            this.comboAction.Location = new System.Drawing.Point(6, 19);
            this.comboAction.Name = "comboAction";
            this.comboAction.Size = new System.Drawing.Size(428, 21);
            this.comboAction.TabIndex = 1;
            this.comboAction.Text = "Do nothing";
            this.comboAction.SelectedIndexChanged += new System.EventHandler(this.ComboActionSelectedIndexChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.AutoSize = true;
            this.groupBox2.Controls.Add(this.txtRunCmd);
            this.groupBox2.Controls.Add(this.chkExit);
            this.groupBox2.Controls.Add(this.txtRestoreVol);
            this.groupBox2.Controls.Add(this.chkRestoreVol);
            this.groupBox2.Controls.Add(this.chkMuteWhenDone);
            this.groupBox2.Controls.Add(this.comboAction);
            this.groupBox2.Location = new System.Drawing.Point(192, 393);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(440, 131);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "When time is up...";
            // 
            // txtRunCmd
            // 
            this.txtRunCmd.Location = new System.Drawing.Point(6, 46);
            this.txtRunCmd.Name = "txtRunCmd";
            this.txtRunCmd.Size = new System.Drawing.Size(428, 20);
            this.txtRunCmd.TabIndex = 6;
            this.txtRunCmd.Visible = false;
            // 
            // chkExit
            // 
            this.chkExit.AutoSize = true;
            this.chkExit.Location = new System.Drawing.Point(6, 72);
            this.chkExit.Name = "chkExit";
            this.chkExit.Size = new System.Drawing.Size(108, 17);
            this.chkExit.TabIndex = 5;
            this.chkExit.Text = "Exit SleepyTunes";
            this.chkExit.UseVisualStyleBackColor = true;
            // 
            // txtRestoreVol
            // 
            this.txtRestoreVol.Enabled = false;
            this.txtRestoreVol.Location = new System.Drawing.Point(133, 92);
            this.txtRestoreVol.Name = "txtRestoreVol";
            this.txtRestoreVol.Size = new System.Drawing.Size(36, 20);
            this.txtRestoreVol.TabIndex = 4;
            this.txtRestoreVol.Text = "50";
            this.txtRestoreVol.Leave += new System.EventHandler(this.TxtRestoreVolLeave);
            // 
            // chkRestoreVol
            // 
            this.chkRestoreVol.AutoSize = true;
            this.chkRestoreVol.Location = new System.Drawing.Point(6, 95);
            this.chkRestoreVol.Name = "chkRestoreVol";
            this.chkRestoreVol.Size = new System.Drawing.Size(121, 17);
            this.chkRestoreVol.TabIndex = 3;
            this.chkRestoreVol.Text = "Reset Volume Level";
            this.chkRestoreVol.UseVisualStyleBackColor = true;
            this.chkRestoreVol.CheckStateChanged += new System.EventHandler(this.ChkRestoreVolCheckedStateChanged);
            // 
            // chkMuteWhenDone
            // 
            this.chkMuteWhenDone.AutoSize = true;
            this.chkMuteWhenDone.Location = new System.Drawing.Point(120, 72);
            this.chkMuteWhenDone.Name = "chkMuteWhenDone";
            this.chkMuteWhenDone.Size = new System.Drawing.Size(98, 17);
            this.chkMuteWhenDone.TabIndex = 2;
            this.chkMuteWhenDone.Text = "Mute the Audio";
            this.chkMuteWhenDone.UseVisualStyleBackColor = true;
            // 
            // zg1
            // 
            this.zg1.EditButtons = System.Windows.Forms.MouseButtons.Left;
            this.zg1.EditModifierKeys = System.Windows.Forms.Keys.None;
            this.zg1.IsAntiAlias = true;
            this.zg1.IsEnableHEdit = true;
            this.zg1.IsEnableVEdit = true;
            this.zg1.IsEnableVPan = false;
            this.zg1.IsEnableVZoom = false;
            this.zg1.IsShowPointValues = true;
            this.zg1.IsSynchronizeXAxes = true;
            this.zg1.IsSynchronizeYAxes = true;
            this.zg1.Location = new System.Drawing.Point(12, 12);
            this.zg1.Name = "zg1";
            this.zg1.PanModifierKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.None)));
            this.zg1.ScrollGrace = 0D;
            this.zg1.ScrollMaxX = 0D;
            this.zg1.ScrollMaxY = 100D;
            this.zg1.ScrollMaxY2 = 0D;
            this.zg1.ScrollMinX = 0D;
            this.zg1.ScrollMinY = 0D;
            this.zg1.ScrollMinY2 = 0D;
            this.zg1.Size = new System.Drawing.Size(614, 375);
            this.zg1.TabIndex = 6;
            this.zg1.ZoomModifierKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.None)));
            this.zg1.ZoomEvent += new ZedGraph.ZedGraphControl.ZoomEventHandler(this.Zg1ZoomEvent);
            this.zg1.PointEditEvent += new ZedGraph.ZedGraphControl.PointEditHandler(this.Zg1PointEditEvent);
            this.zg1.PointValueEvent += new ZedGraph.ZedGraphControl.PointValueHandler(this.Zg1PointValueEvent);
            this.zg1.DoubleClickEvent += new ZedGraph.ZedGraphControl.ZedMouseEventHandler(this.Zg1DoubleClick);
            // 
            // btnGo
            // 
            this.btnGo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnGo.Image = global::SleepyTunes.Properties.Resources.startButton;
            this.btnGo.Location = new System.Drawing.Point(12, 393);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(174, 131);
            this.btnGo.TabIndex = 2;
            this.btnGo.UseVisualStyleBackColor = true;
            this.btnGo.Click += new System.EventHandler(this.BtnGoClick);
            // 
            // gui
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(638, 536);
            this.Controls.Add(this.zg1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnGo);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Gui";
            this.Text = "SleepyTunes";
            this.Resize += new System.EventHandler(this.GuiResize);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboAction;
        private System.Windows.Forms.Button btnGo;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox chkMuteWhenDone;
        private System.Windows.Forms.TextBox txtRestoreVol;
        private System.Windows.Forms.CheckBox chkRestoreVol;
        private System.Windows.Forms.CheckBox chkExit;
        private System.Windows.Forms.TextBox txtRunCmd;
        private ZedGraph.ZedGraphControl zg1;
    }
}

