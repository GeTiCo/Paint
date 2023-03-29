using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Paint
{
    public partial class Form1 : Form
    {
        //--------------------------------------------------------------------------------------------------
        //Ключевые переменные
        //--------------------------------------------------------------------------------------------------
        Bitmap btm;
        Graphics graph;
        bool paint = false;
        Point px, py;
        Pen pen = new Pen(Color.Black);
        Pen clear = new Pen(Color.White);
        int active;
        int x, y, sx, sy, cx, cy;
        Color New_Color;
        ColorDialog cd = new ColorDialog();
        
        //--------------------------------------------------------------------------------------------------
        //Критические функции
        //--------------------------------------------------------------------------------------------------
        public Form1()
        {
            InitializeComponent();
            btm = new Bitmap(picture.Width, picture.Height);
            graph = Graphics.FromImage(btm);
            graph.Clear(Color.White);
            picture.Image = btm;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        //--------------------------------------------------------------------------------------------------
        //Основной функционал
        //--------------------------------------------------------------------------------------------------
        //Выбор цвета
        private void btnColors_Click(object sender, EventArgs e)
        {
            cd.ShowDialog();
            New_Color = cd.Color;
            picture.BackColor = cd.Color;
            pen.Color = cd.Color;
            
        }
        //Карандаш
        private void btnPensil_Click(object sender, EventArgs e)
        {
            active = 2;
        }
        //Заливка
        private void btnFill_Click(object sender, EventArgs e)
        {
            active = 3;
        }
        //Ластик
        private void btnClear_Click(object sender, EventArgs e)
        {
            active = 4;
        }
        //Элипс
        private void btnElipse_Click(object sender, EventArgs e)
        {
            active = 5;
        }
        //Прямоугольник
        private void btnRectangle_Click(object sender, EventArgs e)
        {
            active = 6;
        }
        //Линия
        private void btnLine_Click(object sender, EventArgs e)
        {
            active = 7;
        }
        //--------------------------------------------------------------------------------------------------
        //Глобальные кнопки
        //--------------------------------------------------------------------------------------------------
        //Сохранить
        private void btnSave_Click(object sender, EventArgs e)
        {
            var save = new SaveFileDialog();
            save.Filter = "Image(*.png)";
            if(save.ShowDialog() == DialogResult.OK)
            {
                Bitmap btm = bm.Clone(new Rectangle(0, 0, picture.Width, picture.Height), bm.PixelFormat);
                btm.Save(save.FileName, ImageFormat.Png);
            }
        }

        //Очистить
        private void fullClear_Click(object sender, EventArgs e)
        {
            graph.Clear(Color.White);
            picture.Image = btm;
            active = 0;
        }

        //--------------------------------------------------------------------------------------------------
        //Вторичные элементы
        //--------------------------------------------------------------------------------------------------
        public void trackBar1_Scroll(object sender, EventArgs e)
        {
            
        }

        //--------------------------------------------------------------------------------------------------
        //Вспомогательные элементы
        //--------------------------------------------------------------------------------------------------
        private void picture_MouseClick(object sender, MouseEventArgs e)
        {
            if(active == 3)
            {
                Point point = set_Point(picture, e.Location);
                Fill(btm, point.X, point.Y, New_Color);
            }
        }
        private void picture_Paint(object sender, PaintEventArgs e)
        {
            Graphics graph = e.Graphics;
            if (paint)
            {
                if (active == 5)
                {
                    graph.DrawEllipse(pen, cx, cy, sx, sy);
                }
                if (active == 6)
                {
                    graph.DrawRectangle(pen, cx, cy, sx, sy);
                }
                if (active == 7)
                {
                    graph.DrawLine(pen, cx, cy, x, y);
                }
            }
        }
        private void picture_MouseUp(object sender, MouseEventArgs e)
        {
            paint = false;
            sx = x - cx;
            sy = y - cy;
            if(active == 5)
            {
                graph.DrawEllipse(pen, cx, cy, sx, sy);
            }
            if(active == 6)
            {
                graph.DrawRectangle(pen, cx, cy, sx, sy);
            }
            if (active == 7)
            {
                graph.DrawLine(pen, cx, cy, x, y);
            }
        }
        private void picture_MouseDown(object sender, MouseEventArgs e)
        {
            paint = true;
            py = e.Location;

            cx = e.X;
            cy = e.Y;
        }
        private void picture_MouseMove(object sender, MouseEventArgs e)
        {
            widthPens();
            if (paint)
            {
                switch (active)
                {
                    case 2:
                        px = e.Location;
                        graph.DrawLine(pen, px, py);
                        py = px;
                        break;
                    case 4:
                        px = e.Location;
                        graph.DrawLine(clear, px, py);
                        py = px;
                        break;
                }
                picture.Refresh();

                x = e.X;
                y = e.Y;
                sx = e.X - cx;
                sy = e.Y - cy;
            }
        }
        //--------------------------------------------------------------------------------------------------
        //Ручные элементы
        //--------------------------------------------------------------------------------------------------
        public void widthPens()
        {
            pen.Width = trackBar1.Value;
            clear.Width = trackBar1.Value;
        }
        static Point set_Point(PictureBox PB, Point point)
        {
            float px = 1f * PB.Width / PB.Width;
            float py = 1f * PB.Height / PB.Height;
            return new Point((int)(point.X * px), (int)(point.Y * py));
        }
        private void Validate(Bitmap bm, Stack<Point> sp, int x, int y, Color Old_Color, Color New_Color)
        {
            Color cx = bm.GetPixel(x, y);
            if (cx == Old_Color)
            {
                sp.Push(new Point(x, y));
                bm.SetPixel(x, y, New_Color);
            }
        }
        public void Fill(Bitmap bm, int x, int y, Color New_Color)
        {
            Color Old_Color = bm.GetPixel(x, y);
            Stack<Point> pixel = new Stack<Point>();
            pixel.Push(new Point(x, y));
            bm.SetPixel(x, y, New_Color);
            if (Old_Color == New_Color)
            {
                return;
            }
            while (pixel.Count > 0)
            {
                Point pt = (Point)pixel.Pop();
                if (pt.X > 0 && pt.Y > 0 && pt.X < bm.Width - 1 && pt.Y < bm.Height - 1)
                {
                    Validate(bm, pixel, pt.X - 1, pt.Y, Old_Color, New_Color);
                    Validate(bm, pixel, pt.X, pt.Y - 1, Old_Color, New_Color);
                    Validate(bm, pixel, pt.X + 1, pt.Y, Old_Color, New_Color);
                    Validate(bm, pixel, pt.X, pt.Y + 1, Old_Color, New_Color);
                }
            }
        }
    }
}
