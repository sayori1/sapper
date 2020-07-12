using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;

namespace WindowsFormsApplication3
{
    public enum type { bomb, num, empty, flag};
    public enum state { menu, game, losed }
    
    public partial class Form1 : Form
    {
        public class dot
        {
            public bool flag = false;
            public bool hidden = true;
            public type tp;
        }
        public class bomb : dot
        {
            public bomb()
            {
                tp = type.bomb;
            }
        }
        public class num : dot
        {
            public int val;
            public num(int value)
            {
                tp = type.num;
                val = value;
            }
        }
        public class empty : dot
        {
            public empty()
            {
                tp = type.empty;
            }
        }
        System.Timers.Timer timer = new System.Timers.Timer(100);
        int ww = 300;
        int hw = 300;
        public int wc = 20;
        int hc = 20;
        int w, h;
        public dot[,] dots;
        int fullness = 20; //velocity of how field filled by bombs
        public state form = state.menu;
        public Form1()
        {
            InitializeComponent();
        }
        private void start_the_game()
        {
            Int32.TryParse(textBox1.Text, out wc); hc = wc;
            w = ww / wc;
            h = hw / hc;
            dots = new dot[wc, hc];
            Random rand = new Random(69);
            for (int x = 0; x < wc; x++)
            {
                for (int y = 0; y < hc; y++)
                {
                    int n = rand.Next(0, 100);
                    if (n < fullness)
                    {
                        dots[x, y] = new bomb();
                    }
                    else
                    {
                        dots[x, y] = new empty();
                    }
                }
            }
            for (int x = 0; x < wc; x++)
            {
                for (int y = 0; y < hc; y++)
                {
                    if (dots[x, y].tp != type.bomb)
                    {
                        int count = 0;
                        for (int x1 = -1; x1 <= 1; x1++)
                        {
                            for (int y1 = -1; y1 <= 1; y1++)
                            {
                                if (x + x1 < 0 || y + y1 < 0 || y + y1 >= hc || x + x1 >= wc)
                                {
                                    continue;
                                }
                                if (dots[x + x1, y + y1].tp == type.bomb)
                                {
                                    count++;
                                }
                            }
                        }
                        if (count == 0) dots[x, y] = new empty();
                        else
                        {
                            dots[x, y] = new num(count);
                        }
                    }
                }
            }
            this.Controls.Clear();
            form = state.game;
            Invalidate();
        }
        private void update(object sender, PaintEventArgs e)
        {
            Graphics gp = e.Graphics;
            if (form == state.game)
            {
                make_lattice(0, 0, ref gp);
                draw_dots(ref gp);
            }
        }
        private void make_lattice(int x, int y, ref Graphics gp)
        {
            Pen pen = new Pen(Color.Black, 1.0f);
            for (int x1 = 0; x1 < wc; x1++)
            {
                gp.DrawLine(pen, new Point(x1 * w, 0), new Point(x1* w, hw));
                for (int y1 = 0; y1 < hc; y1++)
                {
                    gp.DrawLine(pen, new Point(0, y1 * h), new Point(ww, y1 * h));
                }
            }
        }
        private void draw_dots(ref Graphics gp){
            for (int x1 = 0; x1 < wc; x1++)
            {
                for (int y1 = 0; y1 < hc; y1++)
                {
                    if (dots[x1, y1].flag)
                    {
                        gp.DrawRectangle(new Pen(Color.Red), x1 * w, y1 * h, w, h);
                    }
                    else if (dots[x1, y1].tp == type.bomb && dots[x1,y1].hidden == false)
                    {
                        gp.DrawRectangle(new Pen(Color.Gray), x1 * w, y1 * h, w, h);
                    }
                    else if (dots[x1, y1].tp == type.num && dots[x1, y1].hidden == false)
                    {
                        gp.DrawString(((dots[x1, y1] as num).val).ToString(), new Font("Impact", 10.0f), Brushes.Black, new Point(x1 * w, y1 * h));
                    }
                    else if (dots[x1, y1].tp == type.empty)
                    {

                    }
                }
            }
        }
        private void recursive_open(int x, int y)
        {
            for (int x1 = -1; x1 <= 1; x1++)
            {
                for (int y1 = -1; y1 <= 1; y1++)
                {
                    if (x + x1 < 0 || y + y1 < 0 || y + y1 >= hc || x + x1 >= wc) continue;
                    if (dots[x + x1, y + y1].hidden)
                    {
                        dots[x + x1, y + y1].hidden = false;
                        if (dots[x + x1, y + y1].tp == type.empty) recursive_open(x + x1, y + y1);
                    }
                }
            }
        }
        private void Failed()
        {
            form = state.losed;
            Label label = new Label();
            label.Text = "Game Over!!!";
            label.Location = new Point(10, 10);
            this.Controls.Add(label);
        }
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            int x = e.Location.X / w;
            int y = e.Location.Y / h;
            if (x < 0 || y < 0 || x >= wc || y >= hc)
                return;
            dots[x, y].hidden = false;
            if (e.Button == MouseButtons.Right)
                {
                    (dots[x, y]).flag = !(dots[x,y].flag);
                }
            if (dots[x, y].tp == type.bomb && e.Button == MouseButtons.Left)
            {
                Failed();
            }
            if (dots[x, y].tp == type.empty) recursive_open(x, y);
            Invalidate();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            start_the_game();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
