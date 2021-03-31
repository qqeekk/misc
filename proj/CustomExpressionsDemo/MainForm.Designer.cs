namespace CustomExpressionsDemo
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.label3 = new System.Windows.Forms.Label();
            this.tbFormula = new System.Windows.Forms.TextBox();
            this.tbResult = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.bCalcExpressionEvaluator = new System.Windows.Forms.Button();
            this.tbMarkers = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(371, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 15);
            this.label3.TabIndex = 6;
            this.label3.Text = "Formula:";
            // 
            // tbFormula
            // 
            this.tbFormula.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.tbFormula.Location = new System.Drawing.Point(371, 27);
            this.tbFormula.Multiline = true;
            this.tbFormula.Name = "tbFormula";
            this.tbFormula.Size = new System.Drawing.Size(309, 155);
            this.tbFormula.TabIndex = 7;
            this.tbFormula.Text = "349 * Force * Data_1/(2 * Position * sqr(Data_1)) where MarkerNum = 1";
            // 
            // tbResult
            // 
            this.tbResult.Location = new System.Drawing.Point(371, 344);
            this.tbResult.Name = "tbResult";
            this.tbResult.ReadOnly = true;
            this.tbResult.Size = new System.Drawing.Size(309, 23);
            this.tbResult.TabIndex = 9;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(371, 326);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(42, 15);
            this.label4.TabIndex = 10;
            this.label4.Text = "Result:";
            // 
            // bCalcExpressionEvaluator
            // 
            this.bCalcExpressionEvaluator.Location = new System.Drawing.Point(371, 202);
            this.bCalcExpressionEvaluator.Name = "bCalcExpressionEvaluator";
            this.bCalcExpressionEvaluator.Size = new System.Drawing.Size(206, 23);
            this.bCalcExpressionEvaluator.TabIndex = 13;
            this.bCalcExpressionEvaluator.Text = "Calc with ExpressionEvaluator";
            this.bCalcExpressionEvaluator.UseVisualStyleBackColor = true;
            this.bCalcExpressionEvaluator.Click += new System.EventHandler(this.bCalcExpressionEvaluator_Click);
            // 
            // tbMarkers
            // 
            this.tbMarkers.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.tbMarkers.Location = new System.Drawing.Point(28, 27);
            this.tbMarkers.Multiline = true;
            this.tbMarkers.Name = "tbMarkers";
            this.tbMarkers.Size = new System.Drawing.Size(309, 340);
            this.tbMarkers.TabIndex = 14;
            this.tbMarkers.Text = resources.GetString("tbMarkers.Text");
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 15);
            this.label1.TabIndex = 15;
            this.label1.Text = "Markerks:";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(714, 388);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbMarkers);
            this.Controls.Add(this.bCalcExpressionEvaluator);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tbResult);
            this.Controls.Add(this.tbFormula);
            this.Controls.Add(this.label3);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Custom Expressions Demo";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbFormula;
        private System.Windows.Forms.TextBox tbResult;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button bCalcExpressionEvaluator;
        private System.Windows.Forms.TextBox tbMarkers;
        private System.Windows.Forms.Label label1;
    }
}
