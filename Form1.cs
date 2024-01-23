using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Rendering;
using CatacombHex;
using MyFormControls;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace CatacombHexForm
{
    public partial class Form1 : Form
    {
        int _margin = 5;
        int _panelWidth = 75;
        bool _showTooltip = false;
        bool _showMessages = true;

        PictureBox _picture;
        GComboBox<GameObject> _cboxObjects;
        Button _btnMode;
        Button _btnSave, _btnLoad;
        Label _status;

        ToolTip _toolTip;

        HexCell _currentCell;
        HexEdge _currentEdge;
        Level _level;

        LevelMode _mode;

        float _mouseX, _mouseY;

        RendererData<HexCell, HexEdge, HexVertex> _renderer;
        Bitmap _image;

        public Form1()
        {
            InitializeComponent();

            WindowState = FormWindowState.Maximized;
            Visible = true;

            Text = "Hexit v0.03a (03.03.2020a)";

            string info = "Добавлены ограничения видимости в игровом режиме (туман войны).";
            info += "\nПри запуске уровня, видимость ограничена радиусом в полторы клетки.";
            info += "\nНа карту можно добавлять сферы (Light Orbs), увеличивающие радиус видимости.";
            info += "\nКнопка \"Generate\" генерирует новый уровень и переводит в режим игры. Увидеть сгенерированный уровень можно, вернувшись в режим редактирования.";
            info += "\nСлучайные уровни содержат 100 клеток (со связностью по ребрам равной единице), вход, выход, ключ и десять сфер.";
            info += "\nИнформационное окно в режиме игры показывает число собранных сфер.";
            info += "\nКнопка \"Clear\" очищает уровень.";
            info += "\nФлаг \"Messages\" включает/отключает окна с сообщениями.";

            FlowLayoutPanel panel = new FlowLayoutPanel()
            {
                FlowDirection = FlowDirection.TopDown,
                Width = _panelWidth,
                Padding = new Padding(0),
                Location = new Point(_margin, _margin),
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                AutoSize = true
            };
            Controls.Add(panel);

            Button btnReset = new Button() { Margin = new Padding(0), Text = "Clear", Width = _panelWidth };
            btnReset.Click += _resetLevel;
            panel.Controls.Add(btnReset);

            Button Run = new Button() { Margin = new Padding(0), Text = "Generate", Width = _panelWidth };
            Run.Click += _generate;
            panel.Controls.Add(Run);

            Button Info = new Button() { Margin = new Padding(0), Text = "Info", Width = _panelWidth };
            Info.Click += (s, e) => MessageBox.Show(info, Text);
            panel.Controls.Add(Info);

            _btnMode = new Button() { Margin = new Padding(0), Text = "Play!", Width = _panelWidth, Enabled = false };
            _btnMode.Click += _changeMode;
            panel.Controls.Add(_btnMode);

            _btnSave = new Button() { Margin = new Padding(0), Text = "Save", Width = _panelWidth, Enabled = true };
            _btnSave.Click += _saveLevel;
            panel.Controls.Add(_btnSave);

            _btnLoad = new Button() { Margin = new Padding(0), Text = "Load", Width = _panelWidth, Enabled = true };
            _btnLoad.Click += _loadLevel;
            panel.Controls.Add(_btnLoad);

            Button Exit = new Button() { Margin = new Padding(0), Text = "Exit", Width = _panelWidth };
            Exit.Click += (s, e) => Application.Exit();
            panel.Controls.Add(Exit);

            Label lblObjects = new Label { Text = "Objects", TextAlign = ContentAlignment.MiddleCenter, Width = _panelWidth };
            panel.Controls.Add(lblObjects);

            _cboxObjects = new GComboBox<GameObject>(GameObject.Empty);
            _cboxObjects.Width = _panelWidth;
            _cboxObjects.Margin = new Padding(0);
            _cboxObjects.DropDownStyle = ComboBoxStyle.DropDownList;
            _cboxObjects.Enabled = false;
            _cboxObjects.SelectedIndexChanged += (s, e) => _level.SetGameObject(_cboxObjects.SelectedItem);
            panel.Controls.Add(_cboxObjects);
            
            CheckBox chbMessages = new CheckBox();
            chbMessages.Text = "Messages";
            chbMessages.Checked = true;
            chbMessages.CheckedChanged += (s, e) => _showMessages = chbMessages.Checked;
            chbMessages.Width = panel.Width - panel.Margin.Left - panel.Margin.Right;
            chbMessages.Margin = new Padding(0);
            panel.Controls.Add(chbMessages);

            _picture = new PictureBox()
            {
                Size = new Size(ClientSize.Width - _margin * 3 - panel.Width, ClientSize.Height - _margin * 2),
                Location = new Point(panel.Right + _margin, _margin)
            };
            Controls.Add(_picture);

            _status = new Label() { Location = _picture.Location, BackColor = Color.White, Visible = false, AutoSize = true };
            _status.Top += _margin;
            _status.Left += _margin;
            Controls.Add(_status);
            _status.BringToFront();
            
            _toolTip = new ToolTip();

            _picture.MouseMove += _processMouseMove;
            _picture.MouseClick += _processClick;
                      
            if (_showMessages)
            {
                MessageBox.Show(info, Text);
            }

            _level = new Level();
            _level.MessageSent += _processMessage;
            _level.LevelFinished += () => _changeMode(null, null);

            _setEditMode();

            _generateImage();
        }

        void _resetLevel(object sender, EventArgs e)
        {
            _level.Reset();
            if (_mode == LevelMode.Play)
            {
                _setEditMode();
            }
            _generateImage();
        }

        private void _processClick(object sender, MouseEventArgs e)
        {       
            if (_image != null)
            {
                float x = (float)e.Location.X / _image.Width;
                float y = (float)e.Location.Y / _image.Height;

                if (x < 1 && y < 1 && x > 0 && y > 0)
                {
                    _currentCell = _level.Grid.GetHex(x, y);
                    _currentEdge = _level.Grid.GetEdge(x, y);

                    _level.ProcessClick(_currentCell, _currentEdge);

                    if(_mode == LevelMode.Edit)
                    {
                        _btnMode.Enabled = _level.IsPlayable();
                    }

                    _generateImage();
                    _status.Text = _getStatus();
                }
            }
        }

        private void _generateImage()
        {
            HexGrid _grid = _level.Grid;

            Dictionary<HexCell, Color> _colorByCell = _grid.Cells.ToDictionary(c => c, c => c.Passable ? Color.Gray : Color.Tan);

            List<EdgeSet<HexEdge>> eSets = new List<EdgeSet<HexEdge>>();
            if(_mode == LevelMode.Edit)
            {
                EdgeSet<HexEdge> regionBorders = new EdgeSet<HexEdge>(_grid.Edges, Color.Black, EdgeThickness.THIN);
                eSets.Add(regionBorders);
            }

            Color wallColor = Rendering.Extensions.Lerp(Color.Black, Color.DarkRed, 0.3);
            EdgeSet<HexEdge> walls = new EdgeSet<HexEdge>(_grid.Edges.Where(e => e.Walled), wallColor, EdgeThickness.THICK);
            eSets.Add(walls);

            List<SymbolSet<HexVertex>> sSets = new List<SymbolSet<HexVertex>>();
            SymbolSet<HexVertex> key = new SymbolSet<HexVertex>(_grid.Cells.Where(c => c.HasGameObject(GameObject.Key)).Select(c => c.Center), Symbol.Key);
            sSets.Add(key);
            SymbolSet<HexVertex> entrance = new SymbolSet<HexVertex>(_grid.Cells.Where(c => c.HasGameObject(GameObject.Entrance)).Select(c => c.Center), Symbol.Hex, Color.Black);
            sSets.Add(entrance);
            SymbolSet<HexVertex> exit = new SymbolSet<HexVertex>(_grid.Cells.Where(c => c.HasGameObject(GameObject.Exit)).Select(c => c.Center), Symbol.Hex, Color.Green);
            sSets.Add(exit);
            SymbolSet<HexVertex> orbs = new SymbolSet<HexVertex>(_grid.Cells.Where(c => c.HasGameObject(GameObject.LightOrb)).Select(c => c.Center), Symbol.Circle, Color.White, 0.3f);
            orbs.SecondaryColor = Color.Yellow;
            sSets.Add(orbs);

            if (_mode == LevelMode.Play)
            {
                SymbolSet<HexVertex> character = new SymbolSet<HexVertex>(_grid.Cells.Where(c => _level.Character == c).Select(c => c.Center), Symbol.Character, Color.Red);
                sSets.Add(character);
            }

            HexGridAdapter gr = new HexGridAdapter(_grid);
                        
            if (_mode == LevelMode.Edit)
            {
                _renderer = GridRenderer<HexCell, HexEdge, HexVertex>.GetRenderData(gr, _picture.Width, _picture.Height, _colorByCell, eSets, sSets, 0, false);
            }
            else
            {
                float visInc = (float)Level.VisibilityIncrement;
                _renderer = GridRenderer<HexCell, HexEdge, HexVertex>.GetRenderData(gr, _picture.Width, _picture.Height, _colorByCell, eSets, sSets, _level.Character, visInc * (1 + _level.Orbs));
            }
            

            _image = _renderer.Image;
            _picture.Image = _image;
        }

        void _saveLevel(object sender, EventArgs e)
        {
            IFormatter formatter = new BinaryFormatter();

            using (FileStream s = File.Create("serialized.bin"))
            {
                formatter.Serialize(s, _level);
            }
        }

        void _loadLevel(object sender, EventArgs e)
        {
            IFormatter formatter = new BinaryFormatter();

            using (FileStream s = File.OpenRead("serialized.bin"))
            {
                _level = (Level)formatter.Deserialize(s);
                _level.MessageSent += _processMessage;
                _level.LevelFinished += () => _changeMode(null, null);
                _btnMode.Enabled = _level.IsPlayable();
                _generateImage();
            }
        }

        void _modifyImage(HexCell cell)
        {
            HexGrid _grid = _level.Grid;

            List<EdgeSet<HexEdge>> eSets = new List<EdgeSet<HexEdge>>();
            EdgeSet<HexEdge> regionBorders = new EdgeSet<HexEdge>(cell.GetEdges(), Color.Black, EdgeThickness.THIN);
            eSets.Add(regionBorders);

            EdgeSet<HexEdge> walls = new EdgeSet<HexEdge>(cell.GetEdges().Where(e => e.Walled), Color.Black, EdgeThickness.THICK);
            eSets.Add(walls);

            Color color = cell.Passable ? Color.Gray : Color.Tan;

            GridRenderer<HexCell, HexEdge, HexVertex>.ModifyImage(_renderer, cell, color, eSets);
        }

        private void _processMouseMove(object sender, MouseEventArgs e)
        {
            float x = e.Location.X;
            float y = e.Location.Y;

            if (_showTooltip && _image != null && (x != _mouseX || y != _mouseY))
            {
                _mouseX = x;
                _mouseY = y;

                Point location = e.Location;
                location.X += _picture.Location.X;
                location.Y += _picture.Location.Y;

                float fx = x / _image.Width;
                float fy = y / _image.Height;

                if (fx < 1 && fy < 1 && fx > 0.001f && fy > 0.001f)
                {
                    _currentCell = _level.Grid.GetHex(fx, fy);
                    _currentEdge = _level.Grid.GetEdge(fx, fy);

                    if (_currentCell != null)
                    {
                        if (_currentEdge != null)
                        {
                            _toolTip.Show($"x: {_currentCell.X}, y: {_currentCell.Y}, edge: {_currentCell.GetDirection(_currentEdge)}", this, location);
                        }
                        else
                        {
                            _toolTip.Show($"x: {_currentCell.X}, y: {_currentCell.Y}, edge: -", this, location);
                        }

                    }
                    else
                    {
                        _toolTip.Show($"x: , y: , edge: ", this, location);
                    }
                }
                else
                {
                    _toolTip.Hide(this);
                }
            }
        }

        private void _generate(object sender, EventArgs e)
        {
            _level.Generate();
            _setPlayMode();
        }

        void _changeMode(object s, EventArgs e)
        {
            if (_mode == LevelMode.Edit)
            {
                _setPlayMode();
            }
            else
            {
                _setEditMode();
            }
        }

        void _setEditMode()
        {
            _mode = LevelMode.Edit;
            _level.SetMode(LevelMode.Edit);
            _btnMode.Text = "Play";
            _btnMode.Enabled = _level.IsPlayable();
            _cboxObjects.Enabled = true;
            _btnSave.Enabled = true;
            _btnLoad.Enabled = true;
            _generateImage();
            _status.Visible = false;
        }

        void _setPlayMode()
        {
            _mode = LevelMode.Play;
            _level.SetMode(LevelMode.Play);
            _btnMode.Text = "Edit";
            _btnMode.Enabled = true;
            _btnSave.Enabled = false;
            _btnLoad.Enabled = false;
            _cboxObjects.Enabled = false;
            _generateImage();
            _status.Visible = true;
            _status.Text = _getStatus();
        }

        string _getStatus()
        {
            string status = $"Moves: {_level.Moves}";
            status += "\nItems: " + (_level.HasKey ? "Key" : "");
            status += _level.Orbs > 0 && _level.HasKey ? "," : "";
            status += _level.Orbs > 0 ? $" Orbs ({_level.Orbs})" : "";
            return status;
        }

        void _processMessage(string message)
        {
            _generateImage();
            if (_showMessages)
            {
                MessageBox.Show(message);
            }
        }
    }
}
