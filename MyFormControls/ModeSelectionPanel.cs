using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyFormControls
{
    public class ModeSelectionPanel<TMode> : FlowLayoutPanel
    {
        const bool _border = true;
        const int _padding = 0;
        const int _controlsMargin = 3;

        ComboBox _modeSelection;
        Dictionary<TMode, ParameterPanel> _panelByMode;

        public virtual event EventHandler ModeChanged = delegate { };

        public ModeSelectionPanel()
        {
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            AutoSize = true;
            FlowDirection = FlowDirection.TopDown;
            BorderStyle = _border ? BorderStyle.FixedSingle : BorderStyle.None;
            Padding = new Padding(_padding);

            _modeSelection = new ComboBox();
            _modeSelection.DropDownStyle = ComboBoxStyle.DropDownList;
            _modeSelection.SelectedIndexChanged += _modeChanged;
            _modeSelection.Margin = new Padding(_controlsMargin);
            Controls.Add(_modeSelection);

            _panelByMode = new Dictionary<TMode, ParameterPanel>();
        }

        public ModeSelectionPanel(int width) : this ()
        {
            MaximumSize = new System.Drawing.Size(width, 0);
            MinimumSize = new System.Drawing.Size(width, 0);

            _modeSelection.Width = ClientSize.Width - _controlsMargin * 2;
        }
        
        public void AddMode(TMode mode, ParameterPanel panel)
        {
            _modeSelection.Items.Add(mode);
            _panelByMode[mode] = panel;
            panel.Visible = false;

            if(_modeSelection.Items.Count == 1)
            {
                _modeSelection.SelectedItem = mode;
                panel.Visible = true;
            }

            Controls.Add(panel);            
        }

        public TMode ModeSelected => (TMode)_modeSelection.SelectedItem;

        public void SetMode(TMode mode) => _modeSelection.SelectedItem = mode;

        public ParameterPanel GetPanel(TMode mode) => _panelByMode[mode];

        void _modeChanged(object sender, EventArgs e)
        {
            foreach(TMode mode in _modeSelection.Items)
            {
                _panelByMode[mode].Visible = false;
            }

            _panelByMode[ModeSelected].Visible = true;

            ModeChanged.Invoke(sender, e);
        }

    }
}
