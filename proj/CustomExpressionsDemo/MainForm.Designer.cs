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
            this.lD1 = new System.Windows.Forms.Label();
            this.tbD1 = new System.Windows.Forms.TextBox();
            this.tbD2 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbD3 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tbFormula = new System.Windows.Forms.TextBox();
            this.btnCalculateWithFSharp = new System.Windows.Forms.Button();
            this.tbResult = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cbMarker = new System.Windows.Forms.ComboBox();
            this.bSet = new System.Windows.Forms.Button();
            this.bCalcExpressionEvaluator = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lD1
            // 
            this.lD1.AutoSize = true;
            this.lD1.Location = new System.Drawing.Point(28, 54);
            this.lD1.Name = "lD1";
            this.lD1.Size = new System.Drawing.Size(21, 15);
            this.lD1.TabIndex = 0;
            this.lD1.Text = "D1";
            // 
            // tbD1
            // 
            this.tbD1.Location = new System.Drawing.Point(55, 51);
            this.tbD1.Name = "tbD1";
            this.tbD1.Size = new System.Drawing.Size(282, 23);
            this.tbD1.TabIndex = 1;
            this.tbD1.Text = "2.5";
            // 
            // tbD2
            // 
            this.tbD2.Location = new System.Drawing.Point(55, 80);
            this.tbD2.Name = "tbD2";
            this.tbD2.Size = new System.Drawing.Size(282, 23);
            this.tbD2.TabIndex = 3;
            this.tbD2.Text = "6";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 83);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(21, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "D2";
            // 
            // tbD3
            // 
            this.tbD3.Location = new System.Drawing.Point(55, 109);
            this.tbD3.Name = "tbD3";
            this.tbD3.Size = new System.Drawing.Size(282, 23);
            this.tbD3.TabIndex = 5;
            this.tbD3.Text = "10";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(28, 112);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(21, 15);
            this.label2.TabIndex = 4;
            this.label2.Text = "D3";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(28, 185);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 15);
            this.label3.TabIndex = 6;
            this.label3.Text = "Formula:";
            // 
            // tbFormula
            // 
            this.tbFormula.Location = new System.Drawing.Point(28, 203);
            this.tbFormula.Multiline = true;
            this.tbFormula.Name = "tbFormula";
            this.tbFormula.Size = new System.Drawing.Size(309, 139);
            this.tbFormula.TabIndex = 7;
            this.tbFormula.Text = "3 * D1 * D3/(2 * D2 * sqr(D3)) where MarkerNum = 1";
            // 
            // btnCalculateWithFSharp
            // 
            this.btnCalculateWithFSharp.Location = new System.Drawing.Point(28, 348);
            this.btnCalculateWithFSharp.Name = "btnCalculateWithFSharp";
            this.btnCalculateWithFSharp.Size = new System.Drawing.Size(97, 23);
            this.btnCalculateWithFSharp.TabIndex = 8;
            this.btnCalculateWithFSharp.Text = "Calc with F#";
            this.btnCalculateWithFSharp.UseVisualStyleBackColor = true;
            this.btnCalculateWithFSharp.Click += new System.EventHandler(this.btnCalculateWithFSharp_Click);
            // 
            // tbResult
            // 
            this.tbResult.Location = new System.Drawing.Point(28, 423);
            this.tbResult.Name = "tbResult";
            this.tbResult.ReadOnly = true;
            this.tbResult.Size = new System.Drawing.Size(309, 23);
            this.tbResult.TabIndex = 9;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(28, 405);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(42, 15);
            this.label4.TabIndex = 10;
            this.label4.Text = "Result:";
            // 
            // cbMarker
            // 
            this.cbMarker.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMarker.FormattingEnabled = true;
            this.cbMarker.Items.AddRange(new object[] {
            "Marker 1",
            "Marker 2",
            "Marker 3"});
            this.cbMarker.Location = new System.Drawing.Point(28, 22);
            this.cbMarker.Name = "cbMarker";
            this.cbMarker.Size = new System.Drawing.Size(309, 23);
            this.cbMarker.TabIndex = 11;
            // 
            // bSet
            // 
            this.bSet.Location = new System.Drawing.Point(28, 138);
            this.bSet.Name = "bSet";
            this.bSet.Size = new System.Drawing.Size(75, 23);
            this.bSet.TabIndex = 12;
            this.bSet.Text = "Set";
            this.bSet.UseVisualStyleBackColor = true;
            this.bSet.Click += new System.EventHandler(this.bSet_Click);
            // 
            // bCalcExpressionEvaluator
            // 
            this.bCalcExpressionEvaluator.Location = new System.Drawing.Point(131, 348);
            this.bCalcExpressionEvaluator.Name = "bCalcExpressionEvaluator";
            this.bCalcExpressionEvaluator.Size = new System.Drawing.Size(206, 23);
            this.bCalcExpressionEvaluator.TabIndex = 13;
            this.bCalcExpressionEvaluator.Text = "Calc with ExpressionEvaluator";
            this.bCalcExpressionEvaluator.UseVisualStyleBackColor = true;
            this.bCalcExpressionEvaluator.Click += new System.EventHandler(this.bCalcExpressionEvaluator_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(349, 497);
            this.Controls.Add(this.bCalcExpressionEvaluator);
            this.Controls.Add(this.bSet);
            this.Controls.Add(this.cbMarker);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tbResult);
            this.Controls.Add(this.btnCalculateWithFSharp);
            this.Controls.Add(this.tbFormula);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbD3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbD2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbD1);
            this.Controls.Add(this.lD1);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Custom Expressions Demo";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lD1;
        private System.Windows.Forms.TextBox tbD1;
        private System.Windows.Forms.TextBox tbD2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbD3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbFormula;
        private System.Windows.Forms.Button btnCalculateWithFSharp;
        private System.Windows.Forms.TextBox tbResult;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cbMarker;
        private System.Windows.Forms.Button bSet;
        private System.Windows.Forms.Button bCalcExpressionEvaluator;
    }
}
