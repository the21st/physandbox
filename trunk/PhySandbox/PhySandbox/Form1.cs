using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Physics;

namespace PhySandbox
{
    public partial class Form1 : Form
    {
        public World world;
        long frames;
        long start;
        Graphics graphics, buffer;
        Bitmap bufferBmp;
        bool mouseDown;
        Vector mouseStart, mouseEnd;
        Point lastLocation;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load( object sender, EventArgs e )
        {
            bufferBmp = new Bitmap( this.Width, this.Height );
            buffer = Graphics.FromImage( bufferBmp );
            graphics = panel1.CreateGraphics();

            buffer.SmoothingMode = SmoothingMode.HighQuality;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            //buffer.SmoothingMode = SmoothingMode.HighSpeed;
            //graphics.SmoothingMode = SmoothingMode.HighSpeed;

            world = new World( buffer, new Rectangle( 0, 0, panel1.Width, panel1.Height - 1 ) );
            world.Gravity.x = 0;
            world.Gravity.y = 1;
            world.AirFriction = 0.001f;

            world.Collisions = true;
            world.PrettySpheres = true;

            frames = 0;
            start = DateTime.Now.ToFileTime();

            mouseDown = false;
            mouseStart = new Vector();
            mouseEnd = new Vector();

            //buttonGlobal.Appearance = Appearance.Button;
            //buttonAddingBalls.Appearance = Appearance.Button;

            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            lastLocation = this.Location;

            Random r = new Random();
            //int x = 8;
            //Sphere[] ss = new Sphere[ x ];


            //for (int i = 0; i < x; ++i)
            //{
            //    ss[ i ] = new Sphere( world );
            //    ss[ i ].Location = new Vector( 200 + r.Next( 200 ), 200 + r.Next( 200 ) );
            //    ss[ i ].Mass = 1;
            //    ss[ i ].Radius = 5;
            //    ss[ i ].Elasticity = 0.5f;
            //    ss[ i ].Clr = Color.FromArgb( 60, 150, 160 );
            //    ss[ i ].GravityStrength = -10f;
            //    world.AddSphere( ss[ i ] );
            //}

            //int k = 1;
            //int length = 50;
            //world.AddSpring( ss[ 0 ], ss[ 1 ], k, length );
            //world.AddSpring( ss[ 0 ], ss[ 2 ], k, length );
            //world.AddSpring( ss[ 0 ], ss[ 3 ], k, length );
            //world.AddSpring( ss[ 1 ], ss[ 4 ], k, length );
            //world.AddSpring( ss[ 1 ], ss[ 5 ], k, length );
            //world.AddSpring( ss[ 2 ], ss[ 5 ], k, length );
            //world.AddSpring( ss[ 2 ], ss[ 7 ], k, length );
            //world.AddSpring( ss[ 3 ], ss[ 4 ], k, length );
            //world.AddSpring( ss[ 3 ], ss[ 7 ], k, length );
            //world.AddSpring( ss[ 4 ], ss[ 6 ], k, length );
            //world.AddSpring( ss[ 5 ], ss[ 6 ], k, length );
            //world.AddSpring( ss[ 6 ], ss[ 7 ], k, length );

            //for (int i = 0; i < x - 1; ++i)
            //{
            //    for (int j = i + 1; j < x; ++j)
            //    {
            //        world.AddSpring( ss[ i ], ss[ j ], 1, 200 );
            //    }
            //}

            //int x = 8;

            //Sphere[ , ] ss = new Sphere[ x, x ];
            //world.Collisions = false;

            //for (int i = 0; i < x; i++)
            //{
            //    for (int j = 0; j < x; ++j)
            //    {
            //        ss[ i, j ] = new Sphere( world );
            //        Sphere s = ss[ i, j ];
            //        s.Location = new Vector( 140 + r.Next( 210 ), 140 + r.Next( 210 ) );
            //        //s.Location = new Vector( 200 + i * 10, 200 + j * 10 );
            //        s.Mass = 1;
            //        s.Radius = 2;
            //        s.GravityStrength = -0.91f;
            //        world.AddSphere( s );
            //    }
            //}

            //for (int i = 0; i < x; i++)
            //{
            //    for (int j = 0; j < x; ++j)
            //    {
            //        for (int k = 0; k < x; k++)
            //        {
            //            for (int l = 0; l < x; ++l)
            //            {
            //                if (i != k || j != l)
            //                {
            //                    //if (r.Next( 70 ) == 0)
            //                    //    world.AddSpring( ss[ i, j ], ss[ k, l ], 1, 20 );
            //                    //if (Math.Abs( i - k ) == 1 && Math.Abs( j - l ) == 1)
            //                    //{
            //                    //    world.AddSpring( ss[ i, j ], ss[ k, l ], 1, 40 );
            //                    //}
            //                    if (Math.Abs( i - k ) == 1 && j == l)
            //                    {
            //                        if (r.Next(100) < 80)
            //                        world.AddSpring( ss[ i, j ], ss[ k, l ], 1, 20 );
            //                    }
            //                    if (Math.Abs( j - l ) == 1 && i == k)
            //                    {
            //                        if (r.Next( 100 ) < 80)
            //                        world.AddSpring( ss[ i, j ], ss[ k, l ], 1, 20 );
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}

            world.AddBoard( 200, 300, 400, 300 );
            //world.AddBoard( 100, 100, panel1.Width / 2, panel1.Height / 2 );
            //world.AddBoard( panel1.Width / 2, panel1.Height / 2, panel1.Width - 100, 100 );

            labelBalls.Text = world.SpheresCount().ToString();
        }

        private void panel1_MouseUp( object sender, MouseEventArgs e )
        {
            if (e.Button == MouseButtons.Left)
                world.AddSphere( e.X, e.Y, 0, 0, 20, 100, 0.9f, 0, Color.Blue, false );

            if (e.Button == MouseButtons.Middle)
                world.AddSphere( e.X, e.Y, 0, 0, 30, 70, 0.5f, 1f, Color.Brown, true );

            if (e.Button == MouseButtons.Right)
                world.AddSphere( e.X, e.Y, 0, 0, 20, 50, 0.5f, -10f, Color.Green, false );

            labelBalls.Text = world.SpheresCount().ToString();
        }

        private void timer1_Tick( object sender, EventArgs e )
        {
            float speed = 0.3f;
            world.Tick( speed );

            world.Render(); // naplnanie bufferu
            //this.Render();

            graphics.DrawImage( bufferBmp, 0, 0 ); // vykreslenie obsahu bufferu
            buffer.Clear( Color.White ); // vyprazdnenie bufferu


            // Frames Per Second
            frames++;
            if (frames == 30)
            {
                frames = 0;
                start = DateTime.Now.ToFileTime();
            }


            ////---sranda---

            //int dx = lastLocation.X - Location.X;
            //int dy = lastLocation.Y - Location.Y;
            //if (dx != 0 || dy != 0)
            //{
            //    world.ChangeLocationOfSpheres( dx, dy );
            //    lastLocation = this.Location;
            //}

            ////---koniec srandy---
        }

        private void exitToolStripMenuItem_Click( object sender, EventArgs e )
        {
            this.Close();
        }

        private void buttonClearAll_Click( object sender, EventArgs e )
        {
            world.ClearAll();
            labelBalls.Text = world.SpheresCount().ToString();
        }

        private void panel1_MouseMove( object sender, MouseEventArgs e )
        {
            labelLocation.Text = "(" + e.X + ", " + e.Y + ")";
        }

        private void panel1_MouseLeave( object sender, EventArgs e )
        {
            labelLocation.Text = "";
        }

        private void buttonAddMrte_Click( object sender, EventArgs e )
        {
            Random r = new Random();
            //Color c = Color.FromArgb( r.Next( 256 ), r.Next( 256 ), r.Next( 256 ) );
            for (int i = 0; i < 30; ++i)
            {
                int x = r.Next( panel1.Width );
                int y = r.Next( panel1.Height );
                Color c = Color.FromArgb( r.Next( 256 ), r.Next( 256 ), r.Next( 256 ) );

                //%%% aby to generovalo navolene
                world.AddSphere( x, y, 0, 0, 3, 1, 0.9f, 0, c, false );
            }

            labelBalls.Text = world.SpheresCount().ToString();
        }

        private void timer2_Tick( object sender, EventArgs e )
        {
            try
            {
                long now = DateTime.Now.ToFileTime();
                labelFps.Text = (frames * 10000000 / (now - start)).ToString();
                //label5.Text = frames.ToString() + " / " + (now - start).ToString(); // kontrola
            }
            catch (DivideByZeroException ex)
            {
                //label4.Text = ex.Message;
            }
        }
    }
}
