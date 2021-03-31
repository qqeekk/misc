using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CustomExpressionsDemo
{
    public partial class MainForm : Form
    {
        // Result4 = 3*Force*Data1/(2*Dimen_1*Dimen_2*Dimen_2) where MarkerNum = 2

        /*
         * Demo formulas:
         * 
         * - 34 * Force * Data_1/(2 * Position * sqr(Data_1)) where MarkerNum = 1
         * - 34 * M_1.Force * M_1.Data_1/(2 * M_1.Position * sqr(M_1.Data_1))
         * - 456 * M_2.Force * M_PeakForce.Data_1/(2 * P_6.Position * sqr(M_2.Data_1))
         * 
         */

        private const string MarkerNumKey = "MarkerNum";
        private const string PairNumKey = "PairNum";

        private IList<Dictionary<string, object>> data = new List<Dictionary<string, object>>();

        public MainForm()
        {
            InitializeComponent();
        }

        private void SetData()
        {
            data = Newtonsoft.Json.JsonConvert.DeserializeObject<IList<Dictionary<string, object>>>(tbMarkers.Text);
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

                var field = splitString[0] switch
                {
                    "M" => MarkerNumKey,
                    "P" => PairNumKey,
                    _ => string.Empty
                };
                if (string.IsNullOrEmpty(field))
                {
                    return;
                }

                var index = splitString[1];
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
            if (e.Name.Equals("sqr") && e.Args.Count == 1)
            {
                e.Value = Math.Pow(double.Parse(e.EvaluateArg(0).ToString()), 2);
            }
        }
    }
}
