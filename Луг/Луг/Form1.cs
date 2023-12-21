namespace Луг
{
    public partial class Form1 : Form
    {
        Луг мой_луг;
        public Form1()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //MessageBox.Show(мой_луг.сущности.Count.ToString());
            if (мой_луг == null) return;

            foreach (Существо сущность in мой_луг.сущности)
            {
                сущность.Tick();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
                     
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            Graphics g = CreateGraphics();
            switch (e.KeyCode)
            {
                case Keys.Q:
                    {
                        мой_луг = new Луг();
                        Bee bee = new Bee();
                        //Ball ball = new Ball();
                        bee.SetFormRef(this);
                        bee.SetMeadowRef(мой_луг);
                        //ball.SetFormRef(this);
                        Random rnd = new Random();
                        for (int i = 0; i < 5; i++)
                        {
                            bee = new Bee();
                            bee.X = rnd.Next(Width);
                            bee.Y = rnd.Next(Height);
                            bee.Size = 20;
                            мой_луг.ДобавитьСущность(bee);
                            Цветок flower = new Цветок();
                            flower.X = rnd.Next(Width);
                            flower.Y = rnd.Next(Height);
                            flower.Size = 20;
                            мой_луг.ДобавитьСущность(flower);
                        }

                        
                        g.Clear(Color.LightGreen);
                    }
                    break;
                                
                case Keys.A: 
                    foreach(var сущ in мой_луг.сущности)
                    {
                        if (сущ is Цветок)
                        {
                            сущ.X -= 3;
                        }    
                    }
                    

                break;
            }

            
            
            
            
            g.Clear(Color.LightGreen);
        }
    }
}
