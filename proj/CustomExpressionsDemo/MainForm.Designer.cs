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
            this.tbPairs = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(694, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 15);
            this.label3.TabIndex = 6;
            this.label3.Text = "Formula:";
            // 
            // tbFormula
            // 
            this.tbFormula.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.tbFormula.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.tbFormula.Location = new System.Drawing.Point(694, 27);
            this.tbFormula.Multiline = true;
            this.tbFormula.Name = "tbFormula";
            this.tbFormula.Size = new System.Drawing.Size(309, 155);
            this.tbFormula.TabIndex = 7;
            this.tbFormula.Text = "349 * Force * Data_1/(2 * Position * sqr(Data_1)) where MarkerNum = 1";
            // 
            // tbResult
            // 
            this.tbResult.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.tbResult.Location = new System.Drawing.Point(694, 344);
            this.tbResult.Name = "tbResult";
            this.tbResult.ReadOnly = true;
            this.tbResult.Size = new System.Drawing.Size(309, 23);
            this.tbResult.TabIndex = 9;
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(694, 326);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(42, 15);
            this.label4.TabIndex = 10;
            this.label4.Text = "Result:";
            // 
            // bCalcExpressionEvaluator
            // 
            this.bCalcExpressionEvaluator.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.bCalcExpressionEvaluator.Location = new System.Drawing.Point(694, 202);
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
            this.tbMarkers.Size = new System.Drawing.Size(309, 631);
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
            // tbPairs
            // 
            this.tbPairs.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.tbPairs.Location = new System.Drawing.Point(367, 27);
            this.tbPairs.Multiline = true;
            this.tbPairs.Name = "tbPairs";
            this.tbPairs.Size = new System.Drawing.Size(309, 631);
            this.tbPairs.TabIndex = 16;
            this.tbPairs.Text = "[{\r\n    \"PairNum\": 1,\r\n    \"MarkerNum1\": 1,\r\n    \"MarkerNum2\": 2\r\n}, {\r\n    \"Pair" +
    "Num\": 2,\r\n    \"MarkerNum1\": 2,\r\n    \"MarkerNum2\": 4\r\n}]";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(367, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 15);
            this.label2.TabIndex = 17;
            this.label2.Text = "Pairs:";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1037, 670);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbPairs);
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
        private System.Windows.Forms.TextBox tbPairs;
        private System.Windows.Forms.Label label2;
    }
}
