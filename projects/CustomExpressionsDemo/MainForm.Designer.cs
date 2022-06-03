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
            this.lbExamples = new System.Windows.Forms.ListBox();
            this.tbAutoMarkers = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(682, 27);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 15);
            this.label3.TabIndex = 6;
            this.label3.Text = "Formula:";
            // 
            // tbFormula
            // 
            this.tbFormula.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbFormula.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.tbFormula.Location = new System.Drawing.Point(682, 27);
            this.tbFormula.Multiline = true;
            this.tbFormula.Name = "tbFormula";
            this.tbFormula.Size = new System.Drawing.Size(421, 166);
            this.tbFormula.TabIndex = 7;
            // 
            // tbResult
            // 
            this.tbResult.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbResult.Location = new System.Drawing.Point(682, 399);
            this.tbResult.Name = "tbResult";
            this.tbResult.ReadOnly = true;
            this.tbResult.Size = new System.Drawing.Size(421, 23);
            this.tbResult.TabIndex = 9;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(682, 381);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(42, 15);
            this.label4.TabIndex = 10;
            this.label4.Text = "Result:";
            // 
            // bCalcExpressionEvaluator
            // 
            this.bCalcExpressionEvaluator.Location = new System.Drawing.Point(682, 199);
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
            this.tbMarkers.Size = new System.Drawing.Size(309, 485);
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
            this.tbPairs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.tbPairs.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.tbPairs.Location = new System.Drawing.Point(367, 27);
            this.tbPairs.Multiline = true;
            this.tbPairs.Name = "tbPairs";
            this.tbPairs.Size = new System.Drawing.Size(309, 675);
            this.tbPairs.TabIndex = 16;
            this.tbPairs.Text = "[{\r\n    \"Name\": \"P1\",\r\n    \"MarkerNum1\": 1,\r\n    \"MarkerNum2\": 2\r\n}, {\r\n    \"Name" +
    "\":\"P2\",\r\n    \"MarkerNum1\": 2,\r\n    \"MarkerNum2\": 4\r\n}]";
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
            // lbExamples
            // 
            this.lbExamples.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbExamples.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lbExamples.FormattingEnabled = true;
            this.lbExamples.ItemHeight = 15;
            this.lbExamples.Items.AddRange(new object[] {
            "34 * Force * Data_1/(2 * Position * Sqr(Data_1)) where MarkerNum = 1",
            "34 * M_1.Force * M_1.Data_1/(2 * M_1.Position * Sqr(M_1.Data_1))",
            "456 * M_2.Force * M_PeakForce.Data_1/(2 * M_3.Position * Sqr(M_2.Data_1))",
            "456 * P1.End.Force * M_PeakForce.Data_1/(2 * M_3.Position * Sqr(P2.Start.Data_1))" +
                "",
            "ForceSum(P2) + 10.4",
            "2+2*2",
            "(2+2)*2"});
            this.lbExamples.Location = new System.Drawing.Point(682, 428);
            this.lbExamples.Name = "lbExamples";
            this.lbExamples.Size = new System.Drawing.Size(421, 274);
            this.lbExamples.TabIndex = 18;
            this.lbExamples.SelectedIndexChanged += new System.EventHandler(this.lbExamples_SelectedIndexChanged);
            // 
            // tbAutoMarkers
            // 
            this.tbAutoMarkers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.tbAutoMarkers.Location = new System.Drawing.Point(28, 518);
            this.tbAutoMarkers.Multiline = true;
            this.tbAutoMarkers.Name = "tbAutoMarkers";
            this.tbAutoMarkers.ReadOnly = true;
            this.tbAutoMarkers.Size = new System.Drawing.Size(309, 184);
            this.tbAutoMarkers.TabIndex = 19;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1115, 729);
            this.Controls.Add(this.tbAutoMarkers);
            this.Controls.Add(this.lbExamples);
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
            this.Load += new System.EventHandler(this.MainForm_Load);
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
        private System.Windows.Forms.ListBox lbExamples;
        private System.Windows.Forms.TextBox tbAutoMarkers;
    }
}
