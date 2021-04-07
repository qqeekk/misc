using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CustomExpressionsDemo
{
    /// <summary>
    /// Main form.
    /// </summary>
    public partial class MainForm : Form
    {
        // Original: Result4 = 3*Force*Data1/(2*Dimen_1*Dimen_2*Dimen_2) where MarkerNum = 2

        /*
         * Demo formulas:
         *
         * - 34 * Force * Data_1/(2 * Position * Sqr(Data_1)) where MarkerNum = 1
         * - 34 * M_1.Force * M_1.Data_1/(2 * M_1.Position * Sqr(M_1.Data_1))
         * - 456 * M_2.Force * M_PeakForce.Data_1/(2 * M_3.Position * Sqr(M_2.Data_1))
         * - 456 * P_1.End.Force * M_PeakForce.Data_1/(2 * M_3.Position * Sqr(P_2.Start.Data_1))
         * - ForceSum(P_2) + 10.4
         *
         */

        private const string MarkerNumKey = "MarkerNum";
        private const string PairNumKey = "PairNum";
        private const string ForceKey = "Force";

        private IList<Dictionary<string, object>> data = new List<Dictionary<string, object>>();
        private IList<Pair> pairs = new List<Pair>();

        public MainForm()
        {
            InitializeComponent();

            if (!DesignMode)
            {
                tbResult.Focus();
            }
        }

        private void SetData()
        {
            data = Newtonsoft.Json.JsonConvert.DeserializeObject<IList<Dictionary<string, object>>>(tbMarkers.Text);
            pairs = Newtonsoft.Json.JsonConvert.DeserializeObject<IList<Pair>>(tbPairs.Text);
        }

        private void bCalcExpressionEvaluator_Click(object sender, EventArgs e)
        {
            try
            {
                SetData();
                var evaluator = new CodingSeb.ExpressionEvaluator.ExpressionEvaluator();
                evaluator.PreEvaluateFunction += Evaluator_PreEvaluateFunction;
                evaluator.PreEvaluateVariable += Evaluator_PreEvaluateVariable;
                var formula = tbFormula.Text;

                // WHERE processing.
                var indexWhere = formula.IndexOf("WHERE", StringComparison.OrdinalIgnoreCase);
                if (indexWhere > -1)
                {
                    var wherePart = formula[(indexWhere + 5)..^0].Trim();
                    var markerNum = int.Parse(wherePart.Split('=')[1]);
                    var dictItem = data.Where(d => d.TryGetValue(MarkerNumKey, out object marker) &&
                        int.Parse(marker.ToString()) == markerNum).FirstOrDefault();
                    if (dictItem == null)
                    {
                        throw new InvalidOperationException($"Cannot get marker num {markerNum}.");
                    }
                    evaluator.Variables = dictItem;
                    formula = formula[0..(indexWhere - 1)];
                }

                tbResult.Text = evaluator.Evaluate(formula).ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Evaluator_PreEvaluateVariable(object sender, CodingSeb.ExpressionEvaluator.VariablePreEvaluationEventArg e)
        {
            if (e.This == null)
            {
                var splitString = e.Name.Split('_', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if (splitString.Length != 2)
                {
                    return;
                }

                // Markers processing.
                if (splitString[0] == "M")
                {
                    var index = splitString[1];
                    var field = MarkerNumKey;
                    if (index == "PeakForce")
                    {
                        e.Value = data
                            .Where(d => d.ContainsKey(field) && d.ContainsKey("Force"))
                            .OrderByDescending(d => double.Parse(d["Force"].ToString()))
                            .FirstOrDefault();
                    }
                    else
                    {
                        e.Value = data
                            .FirstOrDefault(d => d.ContainsKey(field) && d[field].ToString() == index);
                    }
                }
                // Pairs processing.
                else if (splitString[0] == "P")
                {
                    var index = splitString[1];
                    var pair = pairs.FirstOrDefault(p => p.PairNum.ToString() == index);
                    if (pair == null)
                    {
                        throw new InvalidOperationException("Cannot find the pair.");
                    }
                    if (pair.Start == null)
                    {
                        pair.Start = data
                            .FirstOrDefault(d => d.ContainsKey(MarkerNumKey) && d[MarkerNumKey].ToString() == pair.MarkerNum1.ToString());
                    }
                    if (pair.End == null)
                    {
                        pair.End = data
                            .FirstOrDefault(d => d.ContainsKey(MarkerNumKey) && d[MarkerNumKey].ToString() == pair.MarkerNum2.ToString());
                    }
                    e.Value = pair;
                }
            }
            else if (e.This is IDictionary<string, object> dict)
            {
                e.Value = dict[e.Name];
            }
        }

        private static IDictionary<string, object> GroupByIdentifier(IList<Dictionary<string, object>> data, string id)
        {
            var dict = new Dictionary<string, object>();
            foreach (var itemDict in data)
            {
                if (itemDict.TryGetValue(id, out object idValue))
                {
                    dict[idValue.ToString()] = itemDict;
                }
            }
            return dict;
        }

        private void Evaluator_PreEvaluateFunction(object sender, CodingSeb.ExpressionEvaluator.FunctionPreEvaluationEventArg e)
        {
            if (e.Name.Equals("Sqr") && e.Args.Count == 1)
            {
                var arg = e.Args[0];
                e.Value = e.Evaluator.Evaluate($"({arg} * {arg})");
            }
            else if (e.Name.Equals("ForceSum") && e.Args.Count == 1)
            {
                var arg = e.Args[0];
                e.Value = e.Evaluator.Evaluate($"({arg}.Start.Force + {arg}.End.Force)");
            }
        }
    }
}
