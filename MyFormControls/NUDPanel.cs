using System;
using System.Windows.Forms;

namespace MyFormControls
{
    public class NUDPanel : FlowLayoutPanel
    {
        const decimal _maxValueMultiplier = 10.5m;
        const int _margin = 3;
        const int _minMaximumValue = 10;

        public NumericUpDown Numeric;

        public decimal Value => Numeric.Value;

        public event EventHandler ValueChanged = delegate { };

        public static implicit operator int(NUDPanel panel) => (int)panel.Value;

        public NUDPanel(String text, decimal value)
        {
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            AutoSize = true;
            Margin = new Padding(_margin);
            FlowDirection = FlowDirection.LeftToRight;
            WrapContents = false;

            Numeric = new NumericUpDownAdvanced();
            Numeric.AutoSize = true;
            Numeric.Margin = new Padding(0);
            Numeric.Maximum = Math.Max(value * _maxValueMultiplier, _minMaximumValue);
            Numeric.Value = value;
            Numeric.ValueChanged += (s, e) => ValueChanged.Invoke(s, e);
            Controls.Add(Numeric);

            Label label = new Label();
            label.Text = text;
            label.Margin = new Padding(0);
            label.AutoSize = true;
            Controls.Add(label);

            label.Margin = new Padding(0, (Numeric.Height - label.Height) / 2, 0, 0);
        }

        public NUDPanel(String text, decimal value, decimal dec)
        {
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            AutoSize = true;
            Margin = new Padding(_margin);
            FlowDirection = FlowDirection.LeftToRight;
            WrapContents = false;

            Numeric = new NumericUpDownAdvanced();
            Numeric.AutoSize = true;
            Numeric.Margin = new Padding(0);
            Numeric.Maximum = Math.Max(value * _maxValueMultiplier, _minMaximumValue);
            Numeric.Value = value;
            Numeric.DecimalPlaces = (int)dec;
            Numeric.ValueChanged += (s, e) => ValueChanged.Invoke(s, e);
            Controls.Add(Numeric);

            Label label = new Label();
            label.Text = text;
            label.Margin = new Padding(0);
            label.AutoSize = true;
            Controls.Add(label);

            label.Margin = new Padding(0, (Numeric.Height - label.Height) / 2, 0, 0);
        }

        public NUDPanel(String text, decimal value, int clientWidth) : this(text, value)
        {
            MaximumSize = new System.Drawing.Size(clientWidth - _margin * 2, 0);
            MinimumSize = new System.Drawing.Size(clientWidth - _margin * 2, 0);
        }

        public NUDPanel(String text, decimal value, int clientWidth, decimal dec) : this(text, value, dec)
        {
            MaximumSize = new System.Drawing.Size(clientWidth - _margin * 2, 0);
            MinimumSize = new System.Drawing.Size(clientWidth - _margin * 2, 0);
        }


    }

    
}
