using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;

namespace FindWay
{
    public class Map : Control
    {
        public Map() 
        {
            RectangleSize = 10;
            CreateMap(Size.Width, Size.Height);
            RowCount = Size.Width;
            ColCount = Size.Height;
            _rectangleColor.Add(Color.White);
            _rectangleColor.Add(Color.Brown);
            _rectangleColor.Add(Color.Blue);
            _rectangleColor.Add(Color.Black);
            _rectangleColor.Add(Color.Magenta);
            _rectangleColor.Add(Color.DarkMagenta);
        }

        #region Переменные

        private int[,] _data;
        private int _rectangleSize;
        private bool _flagReSize = true;
        private Point _clickPosition;
        private List<Color> _rectangleColor = new List<Color>();
        private int _choice = 0;
        private Point _startPosition = new Point(0,0);
        private Point _endPosition = new Point(0,0);

        #endregion

        public int RectangleSize
        {
            get => _rectangleSize;
            set 
            {
                if (value < 5)
                    value = 5;
                if (value > 100)
                    value = 100;
                if (value != _rectangleSize){
                    _rectangleSize = value;
                    Invalidate();
                }
            }
        }

        #region Работа с массивом

        public virtual int this[int Row, int Col]
        {
            get => _data[Row, Col];
            set => _data[Row, Col] = value;
        }
        public virtual int RowCount
        {
            get => _data.GetLength(0);
            set {
                CreateMap(value / RectangleSize, ColCount); 
            }
        }
        public virtual int ColCount
        {
            get => _data.GetLength(1);
            set{
                CreateMap(RowCount, value / RectangleSize);
            }
        }

        public virtual void CreateMap(int NewRow, int NewCol)
        {
            if (NewRow < 1)
                NewRow = 1;
            if (NewCol < 1)
                NewCol = 1;
            if (_data == null)
                _data = new int[NewRow, NewCol];
            if (NewRow != _data.GetLength(0) || NewCol != _data.GetLength(1)){
                int[,] tmp = _data;
                _data = new int[NewRow, NewCol];
                for (int row = 0; (row < tmp.GetLength(0)) && (row < _data.GetLength(0)); row++)
                    for (int col = 0; (col < tmp.GetLength(1)) && (col < _data.GetLength(1)); col++)
                        _data[row, col] = tmp[row, col];
            }
        }

        #endregion

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) {
                if (e.X < RowCount * RectangleSize - 1 && e.X > 0 && e.Y > _rectangleSize && e.Y < ColCount * RectangleSize - 1) {
                    if (_data[e.X / RectangleSize, e.Y / RectangleSize] != _choice) {
                        _data[e.X / RectangleSize, e.Y / RectangleSize] = _choice;
                        
                        if (_startPosition.X == 0 && _startPosition.Y == 0 && _choice == 4) {
                            _startPosition = new Point(e.X / RectangleSize, e.Y / RectangleSize);
                        } else {
                            if (_choice == 4 && _startPosition.X != 0 && _startPosition.Y != 0){
                                _data[_startPosition.X, _startPosition.Y] = 0;
                                Invalidate(new Rectangle(_startPosition.X * RectangleSize, _startPosition.Y * RectangleSize, RectangleSize, RectangleSize));
                                _startPosition = new Point(0, 0);
                                return;
                            }
                        }
                        _clickPosition = new Point(e.X / RectangleSize, e.Y / RectangleSize);
                        
                        Invalidate(new Rectangle(e.X / RectangleSize * RectangleSize, e.Y / RectangleSize * RectangleSize, RectangleSize, RectangleSize));
                    }
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics Canvas = e.Graphics;
            Canvas.FillRectangle(new SolidBrush(_rectangleColor[_choice]), _rectangleSize * (RowCount - 1), 0, RectangleSize, RectangleSize);
            if (_flagReSize) {
                for (int x = 0; x < _data.GetLength(0); x++)
                    for (int y = 1; y < _data.GetLength(1); y++){
                        Canvas.FillRectangle(new SolidBrush(_rectangleColor[_data[x,y]]), x * RectangleSize + 1, y * RectangleSize + 1, RectangleSize - 1, RectangleSize - 1);
                        Canvas.DrawRectangle(new Pen(Color.Black), x * RectangleSize, y * RectangleSize, RectangleSize, RectangleSize);
                    }
                _flagReSize = false;
            }else{
                Canvas.DrawRectangle(new Pen(Color.Black), _clickPosition.X * RectangleSize, _clickPosition.Y * RectangleSize, RectangleSize, RectangleSize);
                Canvas.FillRectangle(new SolidBrush(_rectangleColor[_data[_clickPosition.X, _clickPosition.Y]]), _clickPosition.X * RectangleSize + 1, _clickPosition.Y * RectangleSize + 1, RectangleSize - 1, RectangleSize - 1);
            }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            OnMouseClick(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            _choice = (e.Delta > 20) ? (_choice + 1) % _rectangleColor.Count:(_choice-1 >=0)? _choice = (_choice - 1) % _rectangleColor.Count : _choice = _rectangleColor.Count - 1;
            Invalidate(new Rectangle(_rectangleSize * (RowCount - 1), 0, RectangleSize, RectangleSize));
        }

        protected override void OnResize(EventArgs e)
        {
            if (Math.Abs(Size.Width - RowCount) > RectangleSize || Math.Abs(Size.Height - ColCount) > RectangleSize)
            {
                RowCount = Size.Width;
                ColCount = Size.Height;
                _flagReSize = true;
                Invalidate();
            }
        }
    }
}