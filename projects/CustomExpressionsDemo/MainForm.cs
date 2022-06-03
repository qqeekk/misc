using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace CustomExpressionsDemo
{
    /// <summary>
    /// Main form.
    /// </summary>
    public partial class MainForm : Form
    {
        // Original: Result4 = 3*Force*Data1/(2*Dimen_1*Dimen_2*Dimen_2) where MarkerNum = 2

        private const string MarkerNumKey = "MarkerNum";
        private const string PairNumKey = "PairNum";
        private const string ForceKey = "Force";

        private IList<Dictionary<string, object>> data = new List<Dictionary<string, object>>();
        private IList<Pair> pairs = new List<Pair>();

        private Dictionary<string, Var> variables = new Dictionary<string, Var>();

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

            variables.Clear();

            foreach (var m in data)
            {
                DefineVar(m["Name"]?.ToString(), VarType.Marker, m);
            }

            foreach (var p in pairs)
            {
                if (p.Start == null)
                {
                    p.Start = data
                        .FirstOrDefault(d => d.ContainsKey(MarkerNumKey) && d[MarkerNumKey].ToString() == p.MarkerNum1.ToString());
                }
                if (p.End == null)
                {
                    p.End = data
                        .FirstOrDefault(d => d.ContainsKey(MarkerNumKey) && d[MarkerNumKey].ToString() == p.MarkerNum2.ToString());
                }

                DefineVar(p.Name.ToString(), VarType.Pair, p);
            }

            var peakForceMarker = data
                .Where(d => d.ContainsKey("Force"))
                .OrderByDescending(d => double.Parse(d["Force"].ToString()))
                .FirstOrDefault();

            if (peakForceMarker != null)
            {
                DefineVar("M_PeakForce", VarType.Marker, peakForceMarker);

                tbAutoMarkers.Text = "M_PeakForce:\n" + JsonConvert.SerializeObject(peakForceMarker);
            }
        }

        private void DefineVar(string varName, VarType varType, object data)
        {
            if (string.IsNullOrEmpty(varName))
            {
                throw new Exception("Name not defined.");
            }

            varName = varName?.ToUpperInvariant();

            if (variables.ContainsKey(varName))
            {
                throw new Exception("Duplicated var.");
            }

            variables[varName] = new Var { Data = data, VarType = varType };
        }

        private void bCalcExpressionEvaluator_Click(object sender, EventArgs e)
        {
            try
            {
                SetData();
                var evaluator = new CodingSeb.ExpressionEvaluator.ExpressionEvaluator();
                evaluator.OptionStringEvaluationActive = false;
                evaluator.OptionCharEvaluationActive = false;
                evaluator.OptionEvaluateFunctionActive = false;
                evaluator.OptionVariableAssignationActive = false;
                evaluator.OptionScriptEvaluateFunctionActive = false;
                evaluator.OptionInlineNamespacesEvaluationActive = false;
                evaluator.Namespaces.Clear();
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

                tbResult.Text = evaluator.Evaluate(formula)?.ToString();
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
                var name = e.Name.ToUpperInvariant();

                if (variables.ContainsKey(name))
                {
                    e.Value = variables[name].Data;
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

        private void lbExamples_SelectedIndexChanged(object sender, EventArgs e)
        {
            tbFormula.Text = lbExamples.SelectedItem.ToString();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            lbExamples.SelectedIndex = 0;
        }
    }

    internal enum VarType
    {
        Undefined,
        Marker,
        Pair
    };

    internal class Var
    {
        public VarType VarType { get; set; }

        public object Data { get; set; }
    }
}
