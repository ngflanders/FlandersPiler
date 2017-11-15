namespace Compiler.Forms
{
    partial class EnvironmentForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.MASMDirTextBox = new System.Windows.Forms.TextBox();
            this.MASMDirSelector = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.SourceDirTextBox = new System.Windows.Forms.TextBox();
            this.SourceDirSelector = new System.Windows.Forms.Button();
            this.CloseEnvironmentFormButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(53, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "MASM Dir";
            // 
            // MASMDirTextBox
            // 
            this.MASMDirTextBox.Enabled = false;
            this.MASMDirTextBox.Location = new System.Drawing.Point(114, 42);
            this.MASMDirTextBox.Name = "MASMDirTextBox";
            this.MASMDirTextBox.Size = new System.Drawing.Size(301, 20);
            this.MASMDirTextBox.TabIndex = 1;
            // 
            // MASMDirSelector
            // 
            this.MASMDirSelector.Location = new System.Drawing.Point(421, 40);
            this.MASMDirSelector.Name = "MASMDirSelector";
            this.MASMDirSelector.Size = new System.Drawing.Size(75, 23);
            this.MASMDirSelector.TabIndex = 2;
            this.MASMDirSelector.Text = "Browse";
            this.MASMDirSelector.UseVisualStyleBackColor = true;
            this.MASMDirSelector.Click += new System.EventHandler(this.MASMDirSelector_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(51, 79);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Source Dir";
            // 
            // SourceDirTextBox
            // 
            this.SourceDirTextBox.Enabled = false;
            this.SourceDirTextBox.Location = new System.Drawing.Point(114, 76);
            this.SourceDirTextBox.Name = "SourceDirTextBox";
            this.SourceDirTextBox.Size = new System.Drawing.Size(301, 20);
            this.SourceDirTextBox.TabIndex = 4;
            // 
            // SourceDirSelector
            // 
            this.SourceDirSelector.Location = new System.Drawing.Point(421, 74);
            this.SourceDirSelector.Name = "SourceDirSelector";
            this.SourceDirSelector.Size = new System.Drawing.Size(75, 23);
            this.SourceDirSelector.TabIndex = 5;
            this.SourceDirSelector.Text = "Browse";
            this.SourceDirSelector.UseVisualStyleBackColor = true;
            this.SourceDirSelector.Click += new System.EventHandler(this.SourceDirSelector_Click);
            // 
            // CloseEnvironmentFormButton
            // 
            this.CloseEnvironmentFormButton.Location = new System.Drawing.Point(421, 106);
            this.CloseEnvironmentFormButton.Name = "CloseEnvironmentFormButton";
            this.CloseEnvironmentFormButton.Size = new System.Drawing.Size(75, 23);
            this.CloseEnvironmentFormButton.TabIndex = 6;
            this.CloseEnvironmentFormButton.Text = "OK";
            this.CloseEnvironmentFormButton.UseVisualStyleBackColor = true;
            this.CloseEnvironmentFormButton.Click += new System.EventHandler(this.CloseEnvironmentFormButton_Click);
            // 
            // EnvironmentForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(576, 141);
            this.Controls.Add(this.CloseEnvironmentFormButton);
            this.Controls.Add(this.SourceDirSelector);
            this.Controls.Add(this.SourceDirTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.MASMDirSelector);
            this.Controls.Add(this.MASMDirTextBox);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EnvironmentForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Environment";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox MASMDirTextBox;
        private System.Windows.Forms.Button MASMDirSelector;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox SourceDirTextBox;
        private System.Windows.Forms.Button SourceDirSelector;
        private System.Windows.Forms.Button CloseEnvironmentFormButton;
    }
}