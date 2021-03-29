using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace CustomExpressionsDemo
{
    public partial class MainForm : Form
    {
        // Result4 = 3*Force*Data1/(2*Dimen_1*Dimen_2*Dimen_2) where MarkerNum = 2
        // 3 * D1 * D3/(2 * D2 * sqr(D3)) where MarkerNum = 2

        private readonly IList<Marker> markers = new List<Marker>
        {
            new Marker(1),
            new Marker(2),
            new Marker(3),
        };

        public MainForm()
        {
            InitializeComponent();

            cbMarker.SelectedIndex = 0;
        }

        private void bSet_Click(object sender, EventArgs e)
        {
            Marker marker = null;

            switch (cbMarker.Text)
            {
                case "Marker 1":
                    marker = markers[0];
                    break;
                case "Marker 2":
                    marker = markers[1];
                    break;
                case "Marker 3":
                    marker = markers[2];
                    break;
            }
            marker.D1 = double.Parse(tbD1.Text);
            marker.D2 = double.Parse(tbD2.Text);
            marker.D3 = double.Parse(tbD3.Text);
        }

        private void btnCalculateWithFSharp_Click(object sender, EventArgs e)
        {
            var d1 = double.Parse(tbD1.Text);
            var d2 = double.Parse(tbD2.Text);
            var d3 = double.Parse(tbD3.Text);

            var sbOut = new StringBuilder();
            var sbErr = new StringBuilder();
            using var inStream = new StringReader("");
            using var outStream = new StringWriter(sbOut);
            using var errStream = new StringWriter(sbErr);
            try
            {
                var fsiConfig = FSharp.Compiler.Interactive.Shell.FsiEvaluationSession.GetDefaultConfiguration();
                string[] argv = new string[] { "--noninteractive", "--nologo" };
                using var fsiSession = FSharp.Compiler.Interactive.Shell.FsiEvaluationSession.Create(fsiConfig, argv, inStream, outStream, errStream,
                    collectible: null,
                    legacyReferenceResolver: null);
                fsiSession.EvalInteraction($"let D1 = {d1}", new Microsoft.FSharp.Core.FSharpOption<CancellationToken>(CancellationToken.None));
                fsiSession.EvalInteraction($"let D2 = {d2}", new Microsoft.FSharp.Core.FSharpOption<CancellationToken>(CancellationToken.None));
                fsiSession.EvalInteraction($"let D3 = {d3}", new Microsoft.FSharp.Core.FSharpOption<CancellationToken>(CancellationToken.None));

                var result = fsiSession.EvalExpression(tbFormula.Text);
                var value = double.Parse(result.Value.ReflectionValue.ToString());
                tbResult.Text = value.ToString();
            }
            catch (FSharp.Compiler.Interactive.Shell.FsiCompilationException)
            {
                MessageBox.Show(sbErr.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void bCalcExpressionEvaluator_Click(object sender, EventArgs e)
        {
            try
            {
                var evaluator = new CodingSeb.ExpressionEvaluator.ExpressionEvaluator();
                evaluator.PreEvaluateFunction += Evaluator_PreEvaluateFunction;
                var formula = tbFormula.Text;

                // WHERE processing.
                var indexWhere = formula.IndexOf("WHERE", StringComparison.OrdinalIgnoreCase);
                if (indexWhere > -1)
                {
                    var wherePart = formula[(indexWhere + 5)..^0].Trim();
                    var markerNum = int.Parse(wherePart.Split('=')[1]);
                    evaluator.Context = markers[markerNum - 1];
                    formula = formula[0..(indexWhere - 1)];
                }
                else
                {
                    evaluator.Context = markers[0];
                }

                tbResult.Text = evaluator.Evaluate(formula).ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Evaluator_PreEvaluateFunction(object sender, CodingSeb.ExpressionEvaluator.FunctionPreEvaluationEventArg e)
        {
            if (e.Name.Equals("sqr") && e.Args.Count == 1)
            {
                e.Value = Math.Pow(double.Parse(e.EvaluateArg(0).ToString()), 2);
            }
        }
    }
}
