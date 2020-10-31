using System.Drawing;

namespace ArduinoVolumeControl
{
    partial class ArduinoVolumeControl
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
            this.AurdioSessionVolumeLabel = new System.Windows.Forms.Label();
            this.AudioSessionVolumeSlider1 = new System.Windows.Forms.TrackBar();
            this.AudioSessionDropDown1 = new System.Windows.Forms.ComboBox();
            this.AudioSessionLabel = new System.Windows.Forms.Label();
            this.AudioSessionVolumeSlider2 = new System.Windows.Forms.TrackBar();
            this.AudioSessionDropDown2 = new System.Windows.Forms.ComboBox();
            this.AudioSessionVolumeSlider3 = new System.Windows.Forms.TrackBar();
            this.AudioSessionDropDown3 = new System.Windows.Forms.ComboBox();
            this.SwitchOutputVolumeLabel = new System.Windows.Forms.Label();
            this.SwitchOutputLabel = new System.Windows.Forms.Label();
            this.SwitchOutputVolumeSlider = new System.Windows.Forms.TrackBar();
            this.SwitchOutputDropDown = new System.Windows.Forms.ComboBox();
            this.TestLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize) (this.AudioSessionVolumeSlider1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize) (this.AudioSessionVolumeSlider2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize) (this.AudioSessionVolumeSlider3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize) (this.SwitchOutputVolumeSlider)).BeginInit();
            this.SuspendLayout();
            // 
            // AurdioSessionVolumeLabel
            // 
            this.AurdioSessionVolumeLabel.Location = new System.Drawing.Point(339, 22);
            this.AurdioSessionVolumeLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.AurdioSessionVolumeLabel.Name = "AurdioSessionVolumeLabel";
            this.AurdioSessionVolumeLabel.Size = new System.Drawing.Size(381, 30);
            this.AurdioSessionVolumeLabel.TabIndex = 2;
            this.AurdioSessionVolumeLabel.Text = "Volume";
            this.AurdioSessionVolumeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // AudioSessionVolumeSlider1
            // 
            this.AudioSessionVolumeSlider1.Cursor = System.Windows.Forms.Cursors.SizeWE;
            this.AudioSessionVolumeSlider1.LargeChange = 20;
            this.AudioSessionVolumeSlider1.Location = new System.Drawing.Point(339, 55);
            this.AudioSessionVolumeSlider1.Maximum = 100;
            this.AudioSessionVolumeSlider1.Name = "AudioSessionVolumeSlider1";
            this.AudioSessionVolumeSlider1.Size = new System.Drawing.Size(381, 45);
            this.AudioSessionVolumeSlider1.SmallChange = 5;
            this.AudioSessionVolumeSlider1.TabIndex = 4;
            this.AudioSessionVolumeSlider1.TickFrequency = 5;
            this.AudioSessionVolumeSlider1.TickStyle = System.Windows.Forms.TickStyle.Both;
            // 
            // AudioSessionDropDown1
            // 
            this.AudioSessionDropDown1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.AudioSessionDropDown1.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.AudioSessionDropDown1.FormattingEnabled = true;
            this.AudioSessionDropDown1.Location = new System.Drawing.Point(66, 63);
            this.AudioSessionDropDown1.Name = "AudioSessionDropDown1";
            this.AudioSessionDropDown1.Size = new System.Drawing.Size(267, 28);
            this.AudioSessionDropDown1.TabIndex = 3;
            // 
            // AudioSessionLabel
            // 
            this.AudioSessionLabel.Location = new System.Drawing.Point(66, 22);
            this.AudioSessionLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.AudioSessionLabel.Name = "AudioSessionLabel";
            this.AudioSessionLabel.Size = new System.Drawing.Size(267, 30);
            this.AudioSessionLabel.TabIndex = 1;
            this.AudioSessionLabel.Text = "Audio Session";
            this.AudioSessionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // AudioSessionVolumeSlider2
            // 
            this.AudioSessionVolumeSlider2.Cursor = System.Windows.Forms.Cursors.SizeWE;
            this.AudioSessionVolumeSlider2.LargeChange = 20;
            this.AudioSessionVolumeSlider2.Location = new System.Drawing.Point(339, 106);
            this.AudioSessionVolumeSlider2.Maximum = 100;
            this.AudioSessionVolumeSlider2.Name = "AudioSessionVolumeSlider2";
            this.AudioSessionVolumeSlider2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.AudioSessionVolumeSlider2.Size = new System.Drawing.Size(381, 45);
            this.AudioSessionVolumeSlider2.SmallChange = 5;
            this.AudioSessionVolumeSlider2.TabIndex = 6;
            this.AudioSessionVolumeSlider2.TickFrequency = 5;
            this.AudioSessionVolumeSlider2.TickStyle = System.Windows.Forms.TickStyle.Both;
            // 
            // AudioSessionDropDown2
            // 
            this.AudioSessionDropDown2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.AudioSessionDropDown2.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.AudioSessionDropDown2.FormattingEnabled = true;
            this.AudioSessionDropDown2.Location = new System.Drawing.Point(66, 114);
            this.AudioSessionDropDown2.Name = "AudioSessionDropDown2";
            this.AudioSessionDropDown2.Size = new System.Drawing.Size(267, 28);
            this.AudioSessionDropDown2.TabIndex = 5;
            // 
            // AudioSessionVolumeSlider3
            // 
            this.AudioSessionVolumeSlider3.Cursor = System.Windows.Forms.Cursors.SizeWE;
            this.AudioSessionVolumeSlider3.LargeChange = 20;
            this.AudioSessionVolumeSlider3.Location = new System.Drawing.Point(339, 157);
            this.AudioSessionVolumeSlider3.Maximum = 100;
            this.AudioSessionVolumeSlider3.Name = "AudioSessionVolumeSlider3";
            this.AudioSessionVolumeSlider3.Size = new System.Drawing.Size(381, 45);
            this.AudioSessionVolumeSlider3.SmallChange = 5;
            this.AudioSessionVolumeSlider3.TabIndex = 8;
            this.AudioSessionVolumeSlider3.TickFrequency = 5;
            this.AudioSessionVolumeSlider3.TickStyle = System.Windows.Forms.TickStyle.Both;
            // 
            // AudioSessionDropDown3
            // 
            this.AudioSessionDropDown3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.AudioSessionDropDown3.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.AudioSessionDropDown3.FormattingEnabled = true;
            this.AudioSessionDropDown3.Location = new System.Drawing.Point(66, 165);
            this.AudioSessionDropDown3.Name = "AudioSessionDropDown3";
            this.AudioSessionDropDown3.Size = new System.Drawing.Size(267, 28);
            this.AudioSessionDropDown3.TabIndex = 7;
            // 
            // SwitchOutputVolumeLabel
            // 
            this.SwitchOutputVolumeLabel.Location = new System.Drawing.Point(339, 253);
            this.SwitchOutputVolumeLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.SwitchOutputVolumeLabel.Name = "SwitchOutputVolumeLabel";
            this.SwitchOutputVolumeLabel.Size = new System.Drawing.Size(381, 30);
            this.SwitchOutputVolumeLabel.TabIndex = 10;
            this.SwitchOutputVolumeLabel.Text = "Volume";
            this.SwitchOutputVolumeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // SwitchOutputLabel
            // 
            this.SwitchOutputLabel.Location = new System.Drawing.Point(66, 253);
            this.SwitchOutputLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.SwitchOutputLabel.Name = "SwitchOutputLabel";
            this.SwitchOutputLabel.Size = new System.Drawing.Size(267, 30);
            this.SwitchOutputLabel.TabIndex = 9;
            this.SwitchOutputLabel.Text = "Switch Output";
            this.SwitchOutputLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // SwitchOutputVolumeSlider
            // 
            this.SwitchOutputVolumeSlider.Cursor = System.Windows.Forms.Cursors.SizeWE;
            this.SwitchOutputVolumeSlider.LargeChange = 20;
            this.SwitchOutputVolumeSlider.Location = new System.Drawing.Point(339, 286);
            this.SwitchOutputVolumeSlider.Maximum = 100;
            this.SwitchOutputVolumeSlider.Name = "SwitchOutputVolumeSlider";
            this.SwitchOutputVolumeSlider.Size = new System.Drawing.Size(381, 45);
            this.SwitchOutputVolumeSlider.SmallChange = 5;
            this.SwitchOutputVolumeSlider.TabIndex = 12;
            this.SwitchOutputVolumeSlider.TickFrequency = 5;
            this.SwitchOutputVolumeSlider.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.SwitchOutputVolumeSlider.Scroll += new System.EventHandler(this.SwitchOutputVolumeSlider_Scroll);
            // 
            // SwitchOutputDropDown
            // 
            this.SwitchOutputDropDown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SwitchOutputDropDown.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.SwitchOutputDropDown.FormattingEnabled = true;
            this.SwitchOutputDropDown.Location = new System.Drawing.Point(66, 294);
            this.SwitchOutputDropDown.Name = "SwitchOutputDropDown";
            this.SwitchOutputDropDown.Size = new System.Drawing.Size(267, 28);
            this.SwitchOutputDropDown.TabIndex = 11;
            this.SwitchOutputDropDown.SelectedIndexChanged += new System.EventHandler(this.SwitchOutputDropDown_SelectedIndexChanged);
            // 
            // TestLabel
            // 
            this.TestLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.TestLabel.Location = new System.Drawing.Point(155, 364);
            this.TestLabel.Name = "TestLabel";
            this.TestLabel.Size = new System.Drawing.Size(100, 23);
            this.TestLabel.TabIndex = 13;
            this.TestLabel.Text = "Test Label";
            // 
            // ArduinoVolumeControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 30F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(818, 420);
            this.Controls.Add(this.TestLabel);
            this.Controls.Add(this.AudioSessionDropDown3);
            this.Controls.Add(this.AudioSessionDropDown2);
            this.Controls.Add(this.AudioSessionVolumeSlider3);
            this.Controls.Add(this.SwitchOutputDropDown);
            this.Controls.Add(this.AudioSessionDropDown1);
            this.Controls.Add(this.SwitchOutputVolumeSlider);
            this.Controls.Add(this.AudioSessionVolumeSlider2);
            this.Controls.Add(this.SwitchOutputLabel);
            this.Controls.Add(this.AudioSessionVolumeSlider1);
            this.Controls.Add(this.SwitchOutputVolumeLabel);
            this.Controls.Add(this.AudioSessionLabel);
            this.Controls.Add(this.AurdioSessionVolumeLabel);
            this.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.Name = "ArduinoVolumeControl";
            this.Text = "Arduino Volume Control";
            ((System.ComponentModel.ISupportInitialize) (this.AudioSessionVolumeSlider1)).EndInit();
            ((System.ComponentModel.ISupportInitialize) (this.AudioSessionVolumeSlider2)).EndInit();
            ((System.ComponentModel.ISupportInitialize) (this.AudioSessionVolumeSlider3)).EndInit();
            ((System.ComponentModel.ISupportInitialize) (this.SwitchOutputVolumeSlider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label TestLabel;

        private System.Windows.Forms.ComboBox AudioSessionDropDown1;
        private System.Windows.Forms.ComboBox AudioSessionDropDown2;
        private System.Windows.Forms.ComboBox AudioSessionDropDown3;
        private System.Windows.Forms.TrackBar AudioSessionVolumeSlider1;
        private System.Windows.Forms.TrackBar AudioSessionVolumeSlider2;
        private System.Windows.Forms.TrackBar AudioSessionVolumeSlider3;

        private System.Windows.Forms.ComboBox SwitchOutputDropDown;
        private System.Windows.Forms.Label SwitchOutputLabel;
        private System.Windows.Forms.Label SwitchOutputVolumeLabel;
        private System.Windows.Forms.TrackBar SwitchOutputVolumeSlider;

        private System.Windows.Forms.Label AudioSessionLabel;
        private System.Windows.Forms.Label AurdioSessionVolumeLabel;

        #endregion
    }
}