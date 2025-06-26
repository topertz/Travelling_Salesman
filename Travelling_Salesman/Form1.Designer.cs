namespace Travelling_Salesman
{
    partial class Form1
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            buttonLoad = new Button();
            buttonCalculate = new Button();
            lblShortestDistance = new Label();
            pbCanvas = new PictureBox();
            lblStatus = new Label();
            lblRoute = new Label();
            lblDistances = new Label();
            ((System.ComponentModel.ISupportInitialize)pbCanvas).BeginInit();
            SuspendLayout();
            // 
            // buttonLoad
            // 
            buttonLoad.Location = new Point(91, 167);
            buttonLoad.Name = "buttonLoad";
            buttonLoad.Size = new Size(156, 33);
            buttonLoad.TabIndex = 0;
            buttonLoad.Text = "Load File";
            buttonLoad.UseVisualStyleBackColor = true;
            // 
            // buttonCalculate
            // 
            buttonCalculate.Location = new Point(91, 222);
            buttonCalculate.Name = "buttonCalculate";
            buttonCalculate.Size = new Size(156, 39);
            buttonCalculate.TabIndex = 1;
            buttonCalculate.Text = "Calculate Distance";
            buttonCalculate.UseVisualStyleBackColor = true;
            // 
            // lblShortestDistance
            // 
            lblShortestDistance.AutoSize = true;
            lblShortestDistance.Location = new Point(317, 66);
            lblShortestDistance.Name = "lblShortestDistance";
            lblShortestDistance.Size = new Size(139, 20);
            lblShortestDistance.TabIndex = 2;
            lblShortestDistance.Text = "Shortest Distance: 0";
            // 
            // pbCanvas
            // 
            pbCanvas.Location = new Point(317, 116);
            pbCanvas.Name = "pbCanvas";
            pbCanvas.Size = new Size(340, 268);
            pbCanvas.TabIndex = 3;
            pbCanvas.TabStop = false;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(89, 112);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(40, 20);
            lblStatus.TabIndex = 4;
            lblStatus.Text = "Stop";
            // 
            // lblRoute
            // 
            lblRoute.AutoSize = true;
            lblRoute.Location = new Point(565, 66);
            lblRoute.Name = "lblRoute";
            lblRoute.Size = new Size(103, 20);
            lblRoute.TabIndex = 5;
            lblRoute.Text = "Best Solution: ";
            // 
            // lblDistances
            // 
            lblDistances.AutoSize = true;
            lblDistances.Location = new Point(784, 112);
            lblDistances.Name = "lblDistances";
            lblDistances.Size = new Size(79, 20);
            lblDistances.TabIndex = 6;
            lblDistances.Text = "Distances: ";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1118, 510);
            Controls.Add(lblDistances);
            Controls.Add(lblRoute);
            Controls.Add(lblStatus);
            Controls.Add(pbCanvas);
            Controls.Add(lblShortestDistance);
            Controls.Add(buttonCalculate);
            Controls.Add(buttonLoad);
            Name = "Form1";
            Text = "Travelling Salesman";
            ((System.ComponentModel.ISupportInitialize)pbCanvas).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button buttonLoad;
        private Button buttonCalculate;
        private Label lblShortestDistance;
        private PictureBox pbCanvas;
        private Label lblStatus;
        private Label lblRoute;
        private Label lblDistances;
    }
}