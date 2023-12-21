using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Луг
{
    class Луг
    {
        public List<Существо> сущности = new List<Существо> ();

        public void ДобавитьСущность(Существо сущность)
        {
            сущности.Add(сущность);
        }

        public Цветок НайтиБлижайшийЦветок(Существо опылитель)
        {
            float best_r = float.MaxValue;
            Цветок nearest_flower = null;
            foreach(var сущность in сущности)
            {
                if (опылитель == сущность) continue;
                if (сущность is not Цветок) continue;

                //float dx = (сущность.X - опылитель.X);
                //float dy = (сущность.Y - опылитель.Y);
                //float r = (float)Math.Sqrt(dx * dx + dy * dy);
                float r = опылитель.Distance(сущность);
                if (r < best_r)
                {
                    best_r = r;
                    nearest_flower = (Цветок)сущность;
                }
            }
            return nearest_flower;
        }
    }
    
    internal abstract class Существо
    {
        public Color цвет_фона = Color.LightGreen;
        public float dt = 10;
        
        public float X { get; set; }
        public float Y { get; set; }
        public float Vx { get; set; } = 1;
        public float Vy { get; set; } = 1;
        public float  Size { get; set; }
               
        static protected Graphics g; // ?? тоже static?
        static protected Form1 refForm;
        static protected Луг луг;

        public void SetMeadowRef(Луг _ref)
        {
            луг = _ref;
        }

        public void SetFormRef(Form1 _refForm) // ?? required? init?
        {
            refForm = _refForm;
            //g = refForm.panel1.CreateGraphics();
            g = refForm.CreateGraphics();
        }

        public float Distance(Существо цель)
        {
            float dx = цель.X - X;
            float dy = цель.Y - Y;
            return (float)Math.Sqrt(dx*dx + dy*dy);
        }

        public abstract void Show();
        public abstract void Disappear();
        public abstract void Tick();
    }

    class Grass : Существо
    {
        public override void Show()
        {
            g.DrawLine(Pens.Green, X, Y, X, Y - 10);
        }

        public override void Disappear()
        {
            Pen pen = new Pen(цвет_фона);
            g.DrawLine(pen, X, Y, X, Y - 10);
        }

        public override void Tick()
        {            
        }
    }

    class Ball : Существо
    {
        public override void Show()
        {           
            g.DrawEllipse(Pens.DarkBlue, X, Y, Size, Size);                           
        }

        public override void Disappear()
        {
            //Pen pen = new Pen(цвет_фона);
            g.FillEllipse(Brushes.LightGreen, X-1, Y-1, Size+2, Size+2);            
        }

        public override void Tick()
        {            
            Disappear();
            X += Vx*dt;
            Y += Vy*dt;
            if ((X > refForm.Size.Width) || (X < 0)) Vx *= (-1);
            if ((Y > refForm.Size.Height) || (Y < 0)) Vy *= (-1);
            Show(); //g.Flush();            
        }
    }

    class Bee : Существо
    {
        //const int УДАЛЕНИЕ    = 18489;
        //const int ПРИБЛИЖЕНИЕ = 18608;
        //const int КРУЖЕНИЕ    = 10111;
        float V = 1;

        enum Режимы { УДАЛЕНИЕ, ПРИБЛИЖЕНИЕ, КРУЖЕНИЕ, ЗАВИСАНИЕ};

        Режимы режим = Режимы.ЗАВИСАНИЕ;

        public override void Disappear()
        {
            g.FillRectangle(Brushes.LightGreen, X - Size, Y - Size, Size*2, Size*2);
        }
        public override void Tick()
        {
            Disappear();

            // 1. Найти ближайший цветок 
            Цветок ближний = луг.НайтиБлижайшийЦветок(this);

            // 2. Найти расстояние до него
            float d = Distance(ближний);

            // 3а) Если это расстояние <  90 пкс, то вкл. режим "УДАЛЕНИЕ"
            if (d < 90) режим = Режимы.УДАЛЕНИЕ;
            // 3б) Если это расстояние > 110 пкс, то вкл. режим "ПРИБЛИЖЕНИЕ"
            else if (d > 110) режим = Режимы.ПРИБЛИЖЕНИЕ;
            // 3в) Иначе вкл. режим "КРУЖЕНИЕ"
            else режим = Режимы.КРУЖЕНИЕ;

            switch(режим)
            {
                case Режимы.УДАЛЕНИЕ: Улетай(ближний); break;
                case Режимы.ПРИБЛИЖЕНИЕ: Прилетай(ближний); break;
                case Режимы.КРУЖЕНИЕ: Кружись(ближний); break;
            }    


            /*X += Vx * dt;
            Y += Vy * dt;
            if ((X > refForm.Size.Width) || (X < 0)) Vx *= (-1);
            if ((Y > refForm.Size.Height) || (Y < 0)) Vy *= (-1);*/
            Show();
        }

        void Кружись(Цветок цветок)
        {
            double dy = цветок.Y - Y;
            double dx = цветок.X - X;
            double alpha = Math.Atan2(dy, dx) + Math.PI/2.0;
            float lx = (float)(V * dt * Math.Cos(alpha));
            float ly = (float)(V * dt * Math.Sin(alpha));
            X -= lx;
            Y -= ly;
        }

        void Улетай(Цветок цветок)
        {
            double dy = цветок.Y - Y;
            double dx = цветок.X - X;
            double alpha = Math.Atan2(dy, dx);
            float lx = (float)(V * dt * Math.Cos(alpha));
            float ly = (float)(V * dt * Math.Sin(alpha));
            X -= lx;
            Y -= ly;
        }

        void Прилетай(Цветок цветок)
        {
            double dy = цветок.Y - Y;
            double dx = цветок.X - X;
            double alpha = Math.Atan2(dy, dx);
            float lx = (float)(V * dt * Math.Cos(alpha));
            float ly = (float)(V * dt * Math.Sin(alpha));
            X += lx;
            Y += ly;
            //if ((X > refForm.Size.Width) || (X < 0)) Vx *= (-1);
            //if ((Y > refForm.Size.Height) || (Y < 0)) Vy *= (-1);
        }

        public override void Show()
        {
            #region 
            //тело
            g.FillEllipse(Brushes.Yellow, X - Size / 2, Y - Size / 2, Size, Size);
            g.FillEllipse(Brushes.Black, X - Size / 2, Y - Size / 13, Size, Size / 5);
            g.FillEllipse(Brushes.Black, X - Size / 2.35f, Y - Size / 12 - Size / 4, Size - Size / 15 * 2, Size / 5);
            g.FillEllipse(Brushes.Black, X - Size / 2.35f, Y - Size / 12 + Size / 4, Size - Size / 15 * 2, Size / 5);

            //жалко
            PointF point1 = new PointF(X - Size / 15, Y + Size / 2);
            PointF point2 = new PointF(X + Size / 15, Y + Size / 2);
            PointF point3 = new PointF(X, Y + Size * 2 / 3);
            PointF[] points = { point1, point2, point3 };
            g.FillPolygon(Brushes.Black, points);

            //крылья
            g.FillEllipse(Brushes.Aqua, X + Size / 2, Y, Size / 2.4f, Size / 8);
            g.FillEllipse(Brushes.Aqua, X - Size / 1.1f, Y, Size / 2.4f, Size / 8);

            //глазки
            g.FillEllipse(Brushes.White, X + Size / 10 - Size / 30, Y - Size / 3, Size / 15, Size / 15);
            g.FillEllipse(Brushes.White, X - Size / 10 - Size / 30, Y - Size / 3, Size / 15, Size / 15);
            g.FillEllipse(Brushes.Black, X + Size / 10 - Size / 60, Y - Size / 3 + Size / 60, Size / 30, Size / 30);
            g.FillEllipse(Brushes.Black, X - Size / 10 - Size / 60, Y - Size / 3 + Size / 60, Size / 30, Size / 30);

            //улыбка :)
            Pen pen_mouth = new Pen(Color.Red, Size / 30);
            PointF mpoint1 = new PointF(X - Size / 5, Y - Size / 5);
            PointF mpoint2 = new PointF(X, Y);
            PointF mpoint3 = new PointF(X, Y);
            PointF mpoint4 = new PointF(X + Size / 5, Y - Size / 5);
            g.DrawBezier(pen_mouth, mpoint1, mpoint2, mpoint3, mpoint4);
            #endregion
        }
    }
    class Цветок : Существо
    {
        int size = 20;
        public override void Show()
        {            
            g.DrawLine(Pens.Red, X, Y, X, Convert.ToInt32(Math.Ceiling(Y - 1.5 * Size)));
            g.DrawLine(Pens.Purple, X, Y, X - 1 * Size, Y - 1 * Size);
            g.DrawLine(Pens.Yellow, X, Y, Convert.ToInt32(Math.Ceiling(X - 1.5 * Size)), Y);
            g.DrawLine(Pens.Green, X, Y, X + 1 * Size, Y - 1 * Size);
            g.DrawLine(Pens.DarkBlue, X, Y, X, Convert.ToInt32(Math.Ceiling(Y + 1.5 * Size)));
            g.DrawLine(Pens.Blue, X, Y, X + 1 * Size, Y + 1 * Size);
            g.DrawLine(Pens.Purple, X, Y, Convert.ToInt32(Math.Ceiling(X + 1.5 * Size)), Y);
            g.DrawLine(Pens.Pink, X, Y, X - 1 * Size, Y + 1 * Size);
            g.FillEllipse(Brushes.Orange, X - Size, Y - Size, 2 * Size, 2 * Size);
            g.DrawEllipse(Pens.Black, X - Size, Y - Size, 2 * Size, 2 * Size);
        }

        public override void Disappear()
        {
            g.FillEllipse(Brushes.LightGreen, X - Size*2, Y - Size*2, 4 * Size, 4 * Size);
            //g.DrawEllipse(Pens.Green, X - Size, Y - Size, 2 * Size, 2 * Size);           
        }

        public override void Tick()
        {
            /*Disappear();
            X += Vx * dt;
            Y += Vy * dt;
            if ((X > refForm.Size.Width) || (X < 0)) Vx *= (-1);
            if ((Y > refForm.Size.Height) || (Y < 0)) Vy *= (-1);*/
            Show();

            // !! TODO: добавить вращение
        }
    }

}
