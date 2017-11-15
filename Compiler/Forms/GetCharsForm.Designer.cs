namespace Compiler.Forms
{
    partial class GetCharsForm
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
            this.IntroStatementLabel = new System.Windows.Forms.Label();
            this.LineCharacterLabel = new System.Windows.Forms.Label();
            this.ResetButton = new System.Windows.Forms.Button();
            this.NextCharButton = new System.Windows.Forms.Button();
            this.StepBackButton = new System.Windows.Forms.Button();
            this.DoneButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // IntroStatementLabel
            // 
            this.IntroStatementLabel.AutoSize = true;
            this.IntroStatementLabel.Location = new System.Drawing.Point(28, 36);
            this.IntroStatementLabel.Name = "IntroStatementLabel";
            this.IntroStatementLabel.Size = new System.Drawing.Size(198, 13);
            this.IntroStatementLabel.TabIndex = 0;
            this.IntroStatementLabel.Text = "The next character of 02_HelloWorld is :";
            // 
            // LineCharacterLabel
            // 
            this.LineCharacterLabel.AutoSize = true;
            this.LineCharacterLabel.Location = new System.Drawing.Point(232, 36);
            this.LineCharacterLabel.Name = "LineCharacterLabel";
            this.LineCharacterLabel.Size = new System.Drawing.Size(54, 13);
            this.LineCharacterLabel.TabIndex = 1;
            this.LineCharacterLabel.Text = "Line 1 - M";
            // 
            // ResetButton
            // 
            this.ResetButton.Location = new System.Drawing.Point(31, 74);
            this.ResetButton.Name = "ResetButton";
            this.ResetButton.Size = new System.Drawing.Size(75, 23);
            this.ResetButton.TabIndex = 2;
            this.ResetButton.Text = "Reset";
            this.ResetButton.UseVisualStyleBackColor = true;
            this.ResetButton.Click += new System.EventHandler(this.ResetButton_Click);
            // 
            // NextCharButton
            // 
            this.NextCharButton.Location = new System.Drawing.Point(112, 74);
            this.NextCharButton.Name = "NextCharButton";
            this.NextCharButton.Size = new System.Drawing.Size(75, 23);
            this.NextCharButton.TabIndex = 3;
            this.NextCharButton.Text = "Get Next";
            this.NextCharButton.UseVisualStyleBackColor = true;
            this.NextCharButton.Click += new System.EventHandler(this.NextCharButton_Click);
            // 
            // StepBackButton
            // 
            this.StepBackButton.Location = new System.Drawing.Point(193, 74);
            this.StepBackButton.Name = "StepBackButton";
            this.StepBackButton.Size = new System.Drawing.Size(75, 23);
            this.StepBackButton.TabIndex = 4;
            this.StepBackButton.Text = "Step Back";
            this.StepBackButton.UseVisualStyleBackColor = true;
            this.StepBackButton.Click += new System.EventHandler(this.StepBackButton_Click);
            // 
            // DoneButton
            // 
            this.DoneButton.Location = new System.Drawing.Point(274, 74);
            this.DoneButton.Name = "DoneButton";
            this.DoneButton.Size = new System.Drawing.Size(75, 23);
            this.DoneButton.TabIndex = 5;
            this.DoneButton.Text = "Done";
            this.DoneButton.UseVisualStyleBackColor = true;
            this.DoneButton.Click += new System.EventHandler(this.DoneButton_Click);
            // 
            // GetCharsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(405, 149);
            this.Controls.Add(this.DoneButton);
            this.Controls.Add(this.StepBackButton);
            this.Controls.Add(this.NextCharButton);
            this.Controls.Add(this.ResetButton);
            this.Controls.Add(this.LineCharacterLabel);
            this.Controls.Add(this.IntroStatementLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GetCharsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "GetCharsForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label IntroStatementLabel;
        private System.Windows.Forms.Label LineCharacterLabel;
        private System.Windows.Forms.Button ResetButton;
        private System.Windows.Forms.Button NextCharButton;
        private System.Windows.Forms.Button StepBackButton;
        private System.Windows.Forms.Button DoneButton;
    }
}