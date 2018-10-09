namespace Game
{
    partial class FrmColorBall
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.g游戏ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TmiStart = new System.Windows.Forms.ToolStripMenuItem();
            this.TmiQuit = new System.Windows.Forms.ToolStripMenuItem();
            this.h帮助ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TmiAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.PlDesk = new System.Windows.Forms.Panel();
            this.PBDesk = new System.Windows.Forms.PictureBox();
            this.menuStrip1.SuspendLayout();
            this.PlDesk.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PBDesk)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.g游戏ToolStripMenuItem,
            this.h帮助ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(497, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // g游戏ToolStripMenuItem
            // 
            this.g游戏ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TmiStart,
            this.TmiQuit});
            this.g游戏ToolStripMenuItem.Name = "g游戏ToolStripMenuItem";
            this.g游戏ToolStripMenuItem.Size = new System.Drawing.Size(53, 20);
            this.g游戏ToolStripMenuItem.Text = "&G 游戏";
            // 
            // TmiStart
            // 
            this.TmiStart.Name = "TmiStart";
            this.TmiStart.Size = new System.Drawing.Size(152, 22);
            this.TmiStart.Text = "&S 开局";
            this.TmiStart.Click += new System.EventHandler(this.TmiStart_Click);
            // 
            // TmiQuit
            // 
            this.TmiQuit.Name = "TmiQuit";
            this.TmiQuit.Size = new System.Drawing.Size(152, 22);
            this.TmiQuit.Text = "&E 退出";
            this.TmiQuit.Click += new System.EventHandler(this.TmiQuit_Click);
            // 
            // h帮助ToolStripMenuItem
            // 
            this.h帮助ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TmiAbout});
            this.h帮助ToolStripMenuItem.Name = "h帮助ToolStripMenuItem";
            this.h帮助ToolStripMenuItem.Size = new System.Drawing.Size(53, 20);
            this.h帮助ToolStripMenuItem.Text = "&H 帮助";
            // 
            // TmiAbout
            // 
            this.TmiAbout.Name = "TmiAbout";
            this.TmiAbout.Size = new System.Drawing.Size(152, 22);
            this.TmiAbout.Text = "&A 关于";
            this.TmiAbout.Click += new System.EventHandler(this.TmiAbout_Click);
            // 
            // PlDesk
            // 
            this.PlDesk.Controls.Add(this.PBDesk);
            this.PlDesk.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PlDesk.Location = new System.Drawing.Point(0, 24);
            this.PlDesk.Name = "PlDesk";
            this.PlDesk.Size = new System.Drawing.Size(497, 466);
            this.PlDesk.TabIndex = 1;
            // 
            // PBDesk
            // 
            this.PBDesk.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PBDesk.Location = new System.Drawing.Point(0, 0);
            this.PBDesk.Name = "PBDesk";
            this.PBDesk.Size = new System.Drawing.Size(497, 466);
            this.PBDesk.TabIndex = 0;
            this.PBDesk.TabStop = false;
            // 
            // FrmColorBall
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(497, 490);
            this.Controls.Add(this.PlDesk);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmColorBall";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "变色球";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.PlDesk.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.PBDesk)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem g游戏ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem TmiStart;
        private System.Windows.Forms.ToolStripMenuItem TmiQuit;
        private System.Windows.Forms.ToolStripMenuItem h帮助ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem TmiAbout;
        private System.Windows.Forms.Panel PlDesk;
        private System.Windows.Forms.PictureBox PBDesk;

    }
}

