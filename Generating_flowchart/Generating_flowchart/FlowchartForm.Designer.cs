namespace Generating_flowchart
{
    partial class FlowchartForm1
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
            this.SuspendLayout();
            // 
            // FlowchartForm1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Name = "FlowchartForm1";
            this.Text = "FlowchartForm";
            this.Load += new System.EventHandler(this.FlowchartForm_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.FlowchartForm1_Paint);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.FlowchartForm1_MouseClick);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FlowchartForm1_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FlowchartForm1_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.FlowchartForm1_MouseUp);
            this.ResumeLayout(false);

        }

        #endregion
    }
}