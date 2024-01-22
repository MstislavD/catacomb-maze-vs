using System;
using System.Windows.Forms;
using System.Drawing;

namespace MyFormControls
{
    public class ParameterPanel : FlowLayoutPanel
    {
        protected const int _padding = 0;
        const bool _border = true;
        protected const int _margin = 3;

        ToolTip _tooltip;

        public virtual event EventHandler ValueChanged = delegate { };

        public ParameterPanel()
        {
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            AutoSize = true;
            FlowDirection = FlowDirection.TopDown;
            BorderStyle = _border ? BorderStyle.FixedSingle : BorderStyle.None;
            Padding = new Padding(_padding);
            Margin = new Padding(_margin);

            _tooltip = new ToolTip();
        }

        public ParameterPanel(int clientWidth) : this()
        {
            MaximumSize = new System.Drawing.Size(clientWidth - _margin * 2, 0);
            MinimumSize = new System.Drawing.Size(clientWidth - _margin * 2, 0);
        }

        public CheckBox InitializeCheckBox(string text, bool check)
        {
            CheckBox checkBox = new CheckBox();
            checkBox.Text = text;
            checkBox.Checked = check;
            checkBox.Margin = new Padding(_margin);
            checkBox.CheckedChanged += (s, e) => ValueChanged(s, e);
            Controls.Add(checkBox);

            return checkBox;
        }

        public TextPanel InitializeTextPanel(string text)
        {
            TextPanel panel = new TextPanel(text);
            Controls.Add(panel);
            return panel;
        }

        public NUDPanel InitializeNUDPanel(string text, decimal value)
        {
            NUDPanel panel = MaximumSize.Width > 0 ? new NUDPanel(text, value, ClientSize.Width) : new NUDPanel(text, value);
            panel.ValueChanged += (s, e) => ValueChanged.Invoke(s, e);
            Controls.Add(panel);
            return panel;
        }

        public NUDPanel InitializeNUDPanel(string text, decimal value, int dec)
        {
            NUDPanel panel = MaximumSize.Width > 0 ? new NUDPanel(text, value, ClientSize.Width, dec) : new NUDPanel(text, value, dec);
            panel.Numeric.Increment = (decimal)(Math.Pow(10, -dec));
            panel.ValueChanged += (s, e) => ValueChanged.Invoke(s, e);
            Controls.Add(panel);
            return panel;
        }

        public NUDPanel InitializeNUDPanel(string text, decimal value, NUDPanel masterPanel)
        {
            NUDPanel panel = MaximumSize.Width > 0 ? new NUDPanel(text, value, ClientSize.Width) : new NUDPanel(text, value);
            panel.Numeric.Maximum = masterPanel.Numeric.Value;
            masterPanel.Numeric.ValueChanged += (s,e) => panel.Numeric.Maximum = masterPanel.Numeric.Value;
            panel.ValueChanged += (s, e) => ValueChanged.Invoke(s, e);
            Controls.Add(panel);
            return panel;
        }

        public NUDPanel InitializeNUDPanel(string text, decimal value, decimal min, decimal max)
        {
            NUDPanel panel = MaximumSize.Width > 0 ? new NUDPanel(text, value, ClientSize.Width) : new NUDPanel(text, value);
            panel.Numeric.Minimum = min;
            panel.Numeric.Maximum = max;
            panel.ValueChanged += (s, e) => ValueChanged.Invoke(s, e);
            Controls.Add(panel);
            return panel;
        }

        public ComboBox InitializeComboBox(object[] objects)
        {
            ComboBox combo = new ComboBox();
            combo.DropDownStyle = ComboBoxStyle.DropDownList;
            combo.Items.AddRange(objects);
            combo.SelectedIndex = 0;
            combo.Width = ClientSize.Width - _margin * 2;
            combo.SelectedValueChanged += (s, e) => ValueChanged(s, e);
            Controls.Add(combo);
            return combo;
        }

        public ComboBox InitializeComboBox(object[] objects, string tooltip)
        {
            ComboBox combo = InitializeComboBox(objects);
            _tooltip.SetToolTip(combo, tooltip);
            return combo;
        }

        public GComboBox<T> InitializeGComboBox<T>(T defaultMode)
        {
            GComboBox<T> combo = new GComboBox<T>(defaultMode);

            combo.DropDownStyle = ComboBoxStyle.DropDownList;
            combo.Width = ClientSize.Width - _margin * 2;
            combo.SelectedValueChanged += (s, e) => ValueChanged(s, e);
            Controls.Add(combo);

            return combo;
        }

        public GComboBox<T> InitializeGComboBox<T>(T defaultMode, string tooltip)
        {
            GComboBox<T> combo = new GComboBox<T>(defaultMode);

            combo.DropDownStyle = ComboBoxStyle.DropDownList;
            combo.Width = ClientSize.Width - _margin * 2;
            combo.SelectedValueChanged += (s, e) => ValueChanged(s, e);
            _tooltip.SetToolTip(combo, tooltip);
            Controls.Add(combo);

            return combo;
        }

        public GComboBox<T> InitializeGComboBox<T>(T[] modes,  T defaultMode, string tooltip)
        {
            GComboBox<T> combo = new GComboBox<T>(modes, defaultMode);

            combo.DropDownStyle = ComboBoxStyle.DropDownList;
            combo.Width = ClientSize.Width - _margin * 2;
            combo.SelectedValueChanged += (s, e) => ValueChanged(s, e);
            _tooltip.SetToolTip(combo, tooltip);
            Controls.Add(combo);

            return combo;
        }

        public TextBox InitializeTextBox(string text)
        {
            TextBox tbox = new TextBox();
            tbox.Text = text;
            tbox.Width = ClientSize.Width - _margin * 2;
            tbox.TextChanged += (s, e) => ValueChanged(s, e);
            Controls.Add(tbox);
            return tbox;
        }

        protected ModeSelectionPanel<TMode> InitializeModeSelectionPanel<TMode>(TMode[] modes, TMode startingMode)
        {
            ModeSelectionPanel<TMode> modePanel = new ModeSelectionPanel<TMode>(ClientSize.Width - _margin * 2);
            foreach(TMode mode in modes)
            {
                ParameterPanel parPanel = new ParameterPanel(modePanel.ClientSize.Width);
                parPanel.ValueChanged += (s, e) => ValueChanged(s, e);
                modePanel.AddMode(mode, parPanel);
            }
            modePanel.SetMode(startingMode);
            modePanel.ModeChanged += (s, e) => ValueChanged(s, e);
            Controls.Add(modePanel);
            
            return modePanel;
        }

        protected ModeSelectionPanel<TMode> InitializeModeSelectionPanel<TMode>(TMode startingMode)
        {
            if (typeof(TMode).BaseType != typeof(Enum))
            {
                throw new InvalidCastException();
            }

            TMode[] modes = (TMode[])Enum.GetValues(typeof(TMode));

            ModeSelectionPanel<TMode> modePanel = InitializeModeSelectionPanel(modes, startingMode);
            return modePanel;
        }

        public Button InitializeButton(string text, EventHandler click)
        {
            Button button = new Button();
            button.Width = ClientSize.Width - _margin * 2;
            button.Text = text;
            button.Click += click;
            Controls.Add(button);

            return button;
        }

        public Button InitializeExitButton()
        {
            Button button = new Button();
            button.Width = ClientSize.Width - _margin * 2;
            button.Text = "Exit";
            button.Click += (s, e) => Application.Exit();
            Controls.Add(button);

            return button;
        }

        public Button InitializeColorDialogButton(Color color, string labelText)
        {
            FlowLayoutPanel panel = new FlowLayoutPanel()
            {
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                AutoSize = true,
                Margin = new Padding(_margin),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false
            };

            Button colorButton = new Button() { Width = 20, Height = 20, BackColor = color, FlatStyle = FlatStyle.Flat };
            ColorDialog colorDialog = new ColorDialog() { Color = color };
            colorButton.Click += (s, e) => _colorChange(colorButton, colorDialog);
            panel.Controls.Add(colorButton);

            Label label = new Label() { Text = labelText, AutoSize = true }; 
            panel.Controls.Add(label);

            label.Margin = new Padding(0, (colorButton.Height - label.Height), 0, 0);

            Controls.Add(panel);

            colorButton.BackColorChanged += (s, e) => ValueChanged(s, e);

            return colorButton;
        }

        void _colorChange(Button button, ColorDialog dialog)
        {
            DialogResult result = dialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                button.BackColor = dialog.Color;
            }
        }

        protected void ParameterChanged(ref int par, TextBox text)
        {
            try
            {
                int numValue = Int32.Parse(text.Text);
                par = numValue;
            }
            catch
            {
                text.Text = par.ToString();
            }

        }
    }
}
