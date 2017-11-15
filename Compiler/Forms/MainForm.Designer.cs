namespace Compiler.Forms
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.mainMenuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.EnvironmentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.programToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testErrorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SourceFileTextBox = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.GetChars = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.MASMDirLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.SourceDirLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.listTokensButton = new System.Windows.Forms.Button();
            this.testSymbolsButton = new System.Windows.Forms.Button();
            this.parseAssembeButton = new System.Windows.Forms.Button();
            this.runButton = new System.Windows.Forms.Button();
            this.mainMenuStrip.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenuStrip
            // 
            this.mainMenuStrip.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.programToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.mainMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.mainMenuStrip.Name = "mainMenuStrip";
            this.mainMenuStrip.Size = new System.Drawing.Size(684, 24);
            this.mainMenuStrip.TabIndex = 0;
            this.mainMenuStrip.Text = "Main Menu Strip";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.EnvironmentToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(42, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // EnvironmentToolStripMenuItem
            // 
            this.EnvironmentToolStripMenuItem.Name = "EnvironmentToolStripMenuItem";
            this.EnvironmentToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.EnvironmentToolStripMenuItem.Text = "Environment";
            this.EnvironmentToolStripMenuItem.Click += new System.EventHandler(this.EnvironmentToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Q)));
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.Exit);
            // 
            // programToolStripMenuItem
            // 
            this.programToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.testErrorToolStripMenuItem});
            this.programToolStripMenuItem.Name = "programToolStripMenuItem";
            this.programToolStripMenuItem.Size = new System.Drawing.Size(72, 20);
            this.programToolStripMenuItem.Text = "Program";
            // 
            // testErrorToolStripMenuItem
            // 
            this.testErrorToolStripMenuItem.Name = "testErrorToolStripMenuItem";
            this.testErrorToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.testErrorToolStripMenuItem.Text = "Test Error";
            this.testErrorToolStripMenuItem.Click += new System.EventHandler(this.TestError);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(49, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("aboutToolStripMenuItem.Image")));
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(111, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.About);
            // 
            // SourceFileTextBox
            // 
            this.SourceFileTextBox.Enabled = false;
            this.SourceFileTextBox.Location = new System.Drawing.Point(104, 60);
            this.SourceFileTextBox.Name = "SourceFileTextBox";
            this.SourceFileTextBox.Size = new System.Drawing.Size(313, 22);
            this.SourceFileTextBox.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(423, 59);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Browse";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.SourceFileSelector);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 62);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 16);
            this.label1.TabIndex = 3;
            this.label1.Text = "Source File";
            // 
            // GetChars
            // 
            this.GetChars.Location = new System.Drawing.Point(25, 112);
            this.GetChars.Name = "GetChars";
            this.GetChars.Size = new System.Drawing.Size(120, 25);
            this.GetChars.TabIndex = 4;
            this.GetChars.Text = "Get Chars";
            this.GetChars.UseVisualStyleBackColor = true;
            this.GetChars.Click += new System.EventHandler(this.GetChars_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MASMDirLabel,
            this.SourceDirLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 239);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(684, 22);
            this.statusStrip1.TabIndex = 5;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // MASMDirLabel
            // 
            this.MASMDirLabel.Name = "MASMDirLabel";
            this.MASMDirLabel.Size = new System.Drawing.Size(67, 17);
            this.MASMDirLabel.Text = "MASM Dir: ";
            // 
            // SourceDirLabel
            // 
            this.SourceDirLabel.Name = "SourceDirLabel";
            this.SourceDirLabel.Size = new System.Drawing.Size(64, 17);
            this.SourceDirLabel.Text = "Source Dir:";
            // 
            // listTokensButton
            // 
            this.listTokensButton.Location = new System.Drawing.Point(151, 112);
            this.listTokensButton.Name = "listTokensButton";
            this.listTokensButton.Size = new System.Drawing.Size(120, 25);
            this.listTokensButton.TabIndex = 6;
            this.listTokensButton.Text = "List Tokens";
            this.listTokensButton.UseVisualStyleBackColor = true;
            this.listTokensButton.Click += new System.EventHandler(this.listTokensButton_Click);
            // 
            // testSymbolsButton
            // 
            this.testSymbolsButton.Location = new System.Drawing.Point(277, 112);
            this.testSymbolsButton.Name = "testSymbolsButton";
            this.testSymbolsButton.Size = new System.Drawing.Size(120, 25);
            this.testSymbolsButton.TabIndex = 7;
            this.testSymbolsButton.Text = "Test Symbols";
            this.testSymbolsButton.UseVisualStyleBackColor = true;
            this.testSymbolsButton.Click += new System.EventHandler(this.testSymbolsButton_Click);
            // 
            // parseAssembeButton
            // 
            this.parseAssembeButton.Location = new System.Drawing.Point(403, 112);
            this.parseAssembeButton.Name = "parseAssembeButton";
            this.parseAssembeButton.Size = new System.Drawing.Size(120, 25);
            this.parseAssembeButton.TabIndex = 1;
            this.parseAssembeButton.Text = "Parse/Assemble";
            this.parseAssembeButton.UseVisualStyleBackColor = true;
            this.parseAssembeButton.Click += new System.EventHandler(this.parseAssembeButton_Click);
            // 
            // runButton
            // 
            this.runButton.Location = new System.Drawing.Point(530, 112);
            this.runButton.Name = "runButton";
            this.runButton.Size = new System.Drawing.Size(100, 25);
            this.runButton.TabIndex = 9;
            this.runButton.Text = "Run";
            this.runButton.UseVisualStyleBackColor = true;
            this.runButton.Click += new System.EventHandler(this.runButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 261);
            this.Controls.Add(this.runButton);
            this.Controls.Add(this.parseAssembeButton);
            this.Controls.Add(this.testSymbolsButton);
            this.Controls.Add(this.listTokensButton);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.GetChars);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.SourceFileTextBox);
            this.Controls.Add(this.mainMenuStrip);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.mainMenuStrip;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximumSize = new System.Drawing.Size(700, 300);
            this.MinimumSize = new System.Drawing.Size(700, 300);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CompilerName";
            this.mainMenuStrip.ResumeLayout(false);
            this.mainMenuStrip.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip mainMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem programToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testErrorToolStripMenuItem;
        private System.Windows.Forms.TextBox SourceFileTextBox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripMenuItem EnvironmentToolStripMenuItem;
        private System.Windows.Forms.Button GetChars;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel MASMDirLabel;
        private System.Windows.Forms.ToolStripStatusLabel SourceDirLabel;
        private System.Windows.Forms.Button listTokensButton;
        private System.Windows.Forms.Button testSymbolsButton;
        private System.Windows.Forms.Button parseAssembeButton;
        private System.Windows.Forms.Button runButton;
    }
}