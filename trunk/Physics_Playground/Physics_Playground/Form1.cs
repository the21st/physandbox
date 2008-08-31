using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Physics;

namespace Physics_Playground
{
    public partial class Form1 : Form
    {
        private const float minBoardLength = 5;

        private World world;

        Graphics graph, buffer;
        Bitmap bufferBmp;

        private bool running, mouseDown;
        Vector mouseStart, mouseEnd, moveLast;
        float speed;
        long frames, start;
        string sceneFile;

        // options
        private bool antiAliasing, hqSpheres, hqSprings;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load( object sender, EventArgs e )
        {
            // APP START
            bufferBmp = new Bitmap( panel1.Width, panel1.Height );
            buffer = Graphics.FromImage( bufferBmp );
            graph = panel1.CreateGraphics();

            loadConfig();
            if (antiAliasing)
                buffer.SmoothingMode = SmoothingMode.HighQuality;
            else
                buffer.SmoothingMode = SmoothingMode.HighSpeed;

            world = new World( buffer, new Rectangle( 0, 0, panel1.Width, panel1.Height - 1 ) );

            world.hqSpheres = hqSpheres;
            world.hqSprings = hqSprings;

            buttonColorSphere.BackColor = Color.FromArgb( 51, 153, 255 );
            buttonColorSpring.BackColor = Color.FromArgb( 0, 0, 0 );
            buttonColorBoard.BackColor = Color.FromArgb( 0, 0, 0 );
            colorDialogSphere.Color = buttonColorSphere.BackColor;
            colorDialogSpring.Color = buttonColorSpring.BackColor;
            colorDialogBoard.Color = buttonColorBoard.BackColor;

            tabControl1.SelectedIndex = 3;
            tabControl2.SelectedIndex = -1;
            tabControl1.SelectedIndex = -1;
            textBoxElasticity.Text = (trackBar2.Value / 100.0).ToString();
            toolStripStatusLabelInfo.Text = "";
            toolStripStatusLabelFPS.Text = "";
            toolStripStatusLabelMouseLocation.Text = "";
            toolStripStatusLabelObjectCount.Text = "0 spheres";

            mouseDown = false;
            mouseStart = new Vector();
            mouseEnd = new Vector();
            moveLast = new Vector();
            running = true;
            speed = 1;
            frames = 0;
            start = DateTime.Now.ToFileTime();
            sceneFile = "";

            generate();
        }

        private void generate()
        {
            //Random r = new Random();

            //int x = 8;

            //Sphere[ , ] ss = new Sphere[ x, x ];
            //checkBoxCollisions.Checked = false;
            //trackBar12.Value = 0;
            //running = false;

            //for (int i = 0; i < x; i++)
            //{
            //    for (int j = 0; j < x; ++j)
            //    {
            //        ss[ i, j ] = new Sphere( world );
            //        Sphere s = ss[ i, j ];
            //        s.Location = new Vector( 140 + r.Next( 210 ), 140 + r.Next( 210 ) );
            //        //s.Location = new Vector( 200 + i * 10, 200 + j * 10 );
            //        s.Mass = 1;
            //        s.Radius = 5;
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
            //                    //{
            //                    //    Spring spr = new Spring( world );
            //                    //    spr.Sphere1 = ss[ i, j ];
            //                    //    spr.Sphere2 = ss[ k, l ];
            //                    //    spr.k = 1;
            //                    //    spr.Length = 20;
            //                    //    world.AddSpring( spr );
            //                    //}
            //                    if (Math.Abs( i - k ) == 1 && j == l)
            //                    {
            //                        if (r.Next( 100 ) < 100)
            //                        {
            //                            Spring spr = new Spring( world );
            //                            spr.Sphere1 = ss[ i, j ];
            //                            spr.Sphere2 = ss[ k, l ];
            //                            spr.k = 1;
            //                            spr.Length = 20;
            //                            world.AddSpring( spr );
            //                        }
            //                    }
            //                    if (Math.Abs( j - l ) == 1 && i == k)
            //                    {
            //                        if (r.Next( 100 ) < 100)
            //                        {
            //                            Spring spr = new Spring( world );
            //                            spr.Sphere1 = ss[ i, j ];
            //                            spr.Sphere2 = ss[ k, l ];
            //                            spr.k = 1;
            //                            spr.Length = 20;
            //                            world.AddSpring( spr );
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}
        }

        private void saveConfig()
        {
            System.IO.StreamWriter streamWriter = new System.IO.StreamWriter( "config.cfg" );

            streamWriter.WriteLine( antiAliasing.ToString() );
            streamWriter.WriteLine( hqSpheres.ToString() );
            streamWriter.WriteLine( hqSprings.ToString() );

            streamWriter.Close();
        }

        private void loadConfig()
        {
            System.IO.StreamReader streamReader = new System.IO.StreamReader( "config.cfg" );

            antiAliasing = bool.Parse( streamReader.ReadLine() );
            hqSpheres = bool.Parse( streamReader.ReadLine() );
            hqSprings = bool.Parse( streamReader.ReadLine() );

            streamReader.Close();
        }

        private void updateInterface()
        {
            toolStripStatusLabelObjectCount.Text = world.SpheresCount().ToString() + " spheres";
        }

        private void render()
        {
            switch (tabControl1.SelectedIndex)
            {
            case 0:
                if (mouseDown)
                {
                    Pen p = new Pen( colorDialogSphere.Color, 2 );
                    int radius = int.Parse( textBoxRadius.Text );

                    buffer.DrawEllipse( p, mouseStart.x - radius, mouseStart.y - radius, 2 * radius, 2 * radius );

                    p.Width = 8;
                    p.EndCap = LineCap.ArrowAnchor;
                    p.StartCap = LineCap.Round;

                    float max = panel1.Width;
                    float ratio = (mouseEnd - mouseStart).Abs() / max;
                    ratio = ratio > 1 ? 1 : ratio;

                    p.Color = Color.FromArgb( (int)(ratio * 255), (int)(255 * (1 - ratio)), 0 );
                    buffer.DrawLine( p, mouseStart.x, mouseStart.y, mouseEnd.x, mouseEnd.y );

                    p.Dispose();
                }
                break;
            case 1:
                break;
            case 2:
                if (mouseDown)
                    buffer.DrawLine( new Pen( colorDialogBoard.Color, 2 ), mouseStart.x, mouseStart.y, mouseEnd.x, mouseEnd.y );
                break;
            case 3:
                break;
            case 4:
                break;
            case 5:
                break;
            }
        }

        private void buttonGo_Click( object sender, EventArgs e )
        {
            if (running)
            {
                buttonGo.Text = "GO";
                running = false;
            }
            else
            {
                buttonGo.Text = "STOP";
                running = true;
            }
        }

        private void trackBar1_ValueChanged( object sender, EventArgs e )
        {
            int radius = trackBar1.Value;
            textBoxRadius.Text = radius.ToString();
            textBoxMass.Text = (Math.Round( (Math.PI * radius * radius / 125.663706143592), 3 )).ToString();
        }

        private void trackBar2_ValueChanged( object sender, EventArgs e )
        {
            textBoxElasticity.Text = (trackBar2.Value / 100.0).ToString();
        }

        private void checkBox1_CheckedChanged( object sender, EventArgs e )
        {
            if (checkBoxGravity.Checked)
            {
                trackBar4.Enabled = true;
                textBoxGravity.Enabled = true;
            }
            else
            {
                trackBar4.Enabled = false;
                textBoxGravity.Enabled = false;
            }
        }

        private void buttonColorSphere_Click( object sender, EventArgs e )
        {
            colorDialogSphere.Color = buttonColorSphere.BackColor;
            if (colorDialogSphere.ShowDialog() == DialogResult.OK)
            {
                buttonColorSphere.BackColor = colorDialogSphere.Color;
            }
        }

        private void timer_Tick( object sender, EventArgs e )
        {
            if (running)
            {
                world.Tick( 0.25f * speed );
            }

            world.Render(); // naplnanie bufferu
            this.render();

            graph.DrawImage( bufferBmp, 0, 0 ); // vykreslenie obsahu bufferu
            buffer.Clear( Color.White ); // vyprazdnenie bufferu

            // Frames per second
            frames++;
            if (frames == 60)
            {
                frames = 0;
                start = DateTime.Now.ToFileTime();
            }
        }

        private void buttonAdd_Click( object sender, EventArgs e )
        {
            Random r = new Random();
            int count = int.Parse( textBoxHowMany.Text );
            Color c = colorDialogSphere.Color;
            Sphere s;

            for (int i = 0; i < count; ++i)
            {
                s = new Sphere( world );

                s.Location.x = r.Next( panel1.Width );
                s.Location.y = r.Next( panel1.Height );
                s.Radius = int.Parse( textBoxRadius.Text );
                s.Mass = float.Parse( textBoxMass.Text );
                s.Elasticity = float.Parse( textBoxElasticity.Text );

                if (checkBoxGravity.Checked)
                    s.GravityStrength = float.Parse( textBoxGravity.Text );
                else
                    s.GravityStrength = 0;

                if (checkBoxStationary.Checked)
                    s.Stationary = true;
                else
                    s.Stationary = false;

                if (checkBoxRandom.Checked)
                    c = Color.FromArgb( r.Next( 256 ), r.Next( 256 ), r.Next( 256 ) );

                s.Clr = c;

                world.AddSphere( s );
            }
        }

        private void trackBar3_ValueChanged( object sender, EventArgs e )
        {
            textBoxK.Text = (trackBar3.Value / 10f).ToString();
        }

        private void panel1_MouseDown( object sender, MouseEventArgs e )
        {
            mouseDown = true;
            mouseStart.x = mouseEnd.x = e.X;
            mouseStart.y = mouseEnd.y = e.Y;
            moveLast = new Vector( mouseStart );

            switch (tabControl1.SelectedIndex)
            {
            case 1:
                if (e.Button == MouseButtons.Left)
                {
                    if (world.Selected != null && world.Selected is Sphere)
                    {
                        Sphere s1 = world.Selected as Sphere;
                        if (world.SelectSphere( e.X, e.Y ) != null && (world.Selected as Sphere) != s1)
                        {
                            Spring spring = new Spring( world );
                            spring.Sphere1 = s1;
                            spring.Sphere2 = world.Selected as Sphere;
                            spring.k = float.Parse( textBoxK.Text );
                            spring.Clr = colorDialogSpring.Color;

                            if (checkBoxLength.Checked)
                                spring.Length = (spring.Sphere1.Location - spring.Sphere2.Location).Abs();
                            else
                                spring.Length = int.Parse( textBoxLength.Text );

                            world.AddSpring( spring );
                            world.Deselect();
                        }
                    }
                    else
                        world.SelectSphere( e.X, e.Y );
                }

                if (e.Button == MouseButtons.Right)
                {
                    world.Deselect();
                }
                break;
            case 3:
                if (e.Button == MouseButtons.Left)
                {
                    if (world.SelectObject( e.X, e.Y ) != null)
                    {
                        buttonDelete.Enabled = true;

                        if (world.Selected is Sphere)
                        {
                            tabControl2.SelectedIndex = 0;

                            Sphere sph = world.Selected as Sphere;
                            trackBar9.Value = Convert.ToInt32( sph.Radius );
                            textBoxMassEdit.Text = sph.Mass.ToString();
                            trackBar8.Value = Convert.ToInt32( sph.Elasticity * 100 );

                            if (sph.GravityStrength == 0)
                            {
                                trackBar7.Value = 0;
                                checkBoxGravityEdit.Checked = false;
                            }
                            else
                            {
                                trackBar7.Value = Convert.ToInt32( sph.GravityStrength * 10 );
                                checkBoxGravityEdit.Checked = true;
                            }

                            if (sph.Stationary)
                                checkBoxStationaryEdit.Checked = true;
                            else
                                checkBoxStationaryEdit.Checked = false;

                            buttonColorSphereEdit.BackColor = sph.Clr;
                        }

                        if (world.Selected is Spring)
                        {
                            tabControl2.SelectedIndex = 1;

                            Spring spr = world.Selected as Spring;
                            trackBar11.Value = Convert.ToInt32( spr.k * 10 );
                            trackBar10.Value = Convert.ToInt32( spr.Length );
                            buttonColorSpringEdit.BackColor = spr.Clr;
                        }

                        if (world.Selected is Board)
                        {
                            tabControl2.SelectedIndex = 2;

                            Board b = world.Selected as Board;
                            buttonColorBoardEdit.BackColor = b.Clr;
                        }
                    }
                    else
                    {
                        buttonDelete.Enabled = false;
                        tabControl2.SelectedIndex = -1;
                    }
                }
                break;
            }
        }

        private void panel1_MouseUp( object sender, MouseEventArgs e )
        {
            mouseDown = false;

            switch (tabControl1.SelectedIndex)
            {
            case 0:
                Sphere sphere = new Sphere( world );

                sphere.Location = new Vector( mouseStart );
                sphere.Velocity = mouseEnd - mouseStart;
                sphere.Radius = int.Parse( textBoxRadius.Text );
                sphere.Mass = float.Parse( textBoxMass.Text );
                sphere.Elasticity = float.Parse( textBoxElasticity.Text );

                if (checkBoxGravity.Checked)
                    sphere.GravityStrength = float.Parse( textBoxGravity.Text );
                else
                    sphere.GravityStrength = 0;

                if (checkBoxStationary.Checked)
                    sphere.Stationary = true;
                else
                    sphere.Stationary = false;

                sphere.Clr = colorDialogSphere.Color;

                world.AddSphere( sphere );
                break;
            case 1:
                //if (e.Button == MouseButtons.Left)
                //{
                //    if (world.Selected != null && world.Selected is Sphere)
                //    {
                //        Sphere s1 = world.Selected as Sphere;
                //        if (world.SelectSphere( e.X, e.Y ) != null && (world.Selected as Sphere) != s1)
                //        {
                //            Spring spring = new Spring( world );
                //            spring.Sphere1 = s1;
                //            spring.Sphere2 = world.Selected as Sphere;
                //            spring.k = float.Parse( textBoxK.Text );
                //            spring.Clr = colorDialogSpring.Color;

                //            if (checkBoxLength.Checked)
                //                spring.Length = (spring.Sphere1.Location - spring.Sphere2.Location).Abs();
                //            else
                //                spring.Length = int.Parse( textBoxLength.Text );

                //            world.AddSpring( spring );
                //            world.Deselect();
                //        }
                //    }
                //    else
                //        world.SelectSphere( e.X, e.Y );
                //}

                //if (e.Button == MouseButtons.Right)
                //{
                //    world.Deselect();
                //}
                break;
            case 2:
                Board board = new Board( world );
                board.line.Start = new Vector( mouseStart );
                board.line.End = new Vector( mouseEnd );
                board.Clr = colorDialogBoard.Color;

                world.AddBoard( board );
                break;
            case 3:
                //if (world.SelectObject( e.X, e.Y ) != null)
                //{
                //    if (world.Selected is Sphere)
                //    {
                //        tabControl2.SelectedIndex = 0;

                //        Sphere sph = world.Selected as Sphere;
                //        trackBar9.Value = Convert.ToInt32( sph.Radius );
                //        textBoxMassEdit.Text = sph.Mass.ToString();
                //        trackBar8.Value = Convert.ToInt32( sph.Elasticity * 100 );

                //        if (sph.GravityStrength == 0)
                //        {
                //            trackBar7.Value = 0;
                //            checkBoxGravityEdit.Checked = false;
                //        }
                //        else
                //        {
                //            trackBar7.Value = Convert.ToInt32( sph.GravityStrength * 10 );
                //            checkBoxGravityEdit.Checked = true;
                //        }

                //        if (sph.Stationary)
                //            checkBoxStationaryEdit.Checked = true;
                //        else
                //            checkBoxStationaryEdit.Checked = false;

                //        buttonColorSphereEdit.BackColor = sph.Clr;
                //    }

                //    if (world.Selected is Spring)
                //    {
                //        tabControl2.SelectedIndex = 1;

                //        Spring spr = world.Selected as Spring;
                //        trackBar11.Value = Convert.ToInt32( spr.k * 10 );
                //        trackBar10.Value = Convert.ToInt32( spr.Length );
                //        buttonColorSpringEdit.BackColor = spr.Clr;
                //    }

                //    if (world.Selected is Board)
                //    {
                //        tabControl2.SelectedIndex = 2;

                //        Board b = world.Selected as Board;
                //        buttonColorBoardEdit.BackColor = b.Clr;
                //    }
                //}
                //else
                //{
                //    tabControl2.SelectedIndex = -1;
                //}
                break;
            case 4:
                break;
            case 5:
                break;
            }
        }

        private void panel1_MouseMove( object sender, MouseEventArgs e )
        {
            if (mouseDown)
            {
                mouseEnd.x = e.X;
                mouseEnd.y = e.Y;

                if (tabControl1.SelectedIndex == 3)
                {
                    if (running && world.Selected != null && world.Selected is Sphere)
                    {
                        buttonGo.Text = "GO";
                        running = false;
                    }

                    world.MoveSelected( mouseEnd - moveLast );
                    moveLast = new Vector( mouseEnd );
                }

                // EXPERIMENTAL
                if (tabControl1.SelectedIndex == 2 && checkBoxCurve.Checked)
                {
                    if ((mouseEnd - mouseStart).Abs() >= minBoardLength)
                    {
                        Board board = new Board( world );
                        board.line.Start = new Vector( mouseStart );
                        board.line.End = new Vector( mouseEnd );
                        board.Clr = colorDialogBoard.Color;

                        world.AddBoard( board );

                        mouseStart = new Vector( mouseEnd );
                    }
                }
                // END
            }

            toolStripStatusLabelMouseLocation.Text = e.X.ToString() + ", " + e.Y.ToString();
        }

        private void panel1_MouseLeave( object sender, EventArgs e )
        {
            toolStripStatusLabelMouseLocation.Text = "";
        }

        private void tabControl1_SelectedIndexChanged( object sender, EventArgs e )
        {
            world.Deselect();
            buttonDelete.Enabled = false;
            tabControl2.SelectedIndex = -1;

            colorDialogSphere.Color = buttonColorSphere.BackColor;
            colorDialogSpring.Color = buttonColorSpring.BackColor;
            colorDialogBoard.Color = buttonColorBoard.BackColor;

            switch (tabControl1.SelectedIndex)
            {
            case 0:
                toolStripStatusLabelInfo.Text = "To add a single sphere, click and drag the cursor to set the sphere's location and velocity.";
                break;
            case 1:
                toolStripStatusLabelInfo.Text = "To add a string, select two spheres to be linked by clicking on them.";
                break;
            case 2:
                toolStripStatusLabelInfo.Text = "To add a board, click and drag the cursor.";
                break;
            case 3:
                toolStripStatusLabelInfo.Text = "Select an object by clicking on it. You can move it by clicking and dragging the cursor.";
                break;
            case 4:
                toolStripStatusLabelInfo.Text = "";
                break;
            case 5:
                toolStripStatusLabelInfo.Text = "";
                break;
            }
        }

        private void timerFPS_Tick( object sMOender, EventArgs e )
        {
            updateInterface();
            try
            {
                long now = DateTime.Now.ToFileTime();
                toolStripStatusLabelFPS.Text = "FPS: " + (frames * 10000000 / (now - start)).ToString();
            }
            catch (DivideByZeroException ex)
            {
            }
        }

        private void checkBoxLength_CheckedChanged( object sender, EventArgs e )
        {
            if (checkBoxLength.Checked)
            {
                trackBar5.Enabled = false;
                textBoxLength.Enabled = false;
            }
            else
            {
                trackBar5.Enabled = true;
                textBoxLength.Enabled = true;
            }
        }

        private void trackBar4_ValueChanged( object sender, EventArgs e )
        {
            textBoxGravity.Text = (trackBar4.Value / 10f).ToString();
        }

        private void trackBar5_ValueChanged( object sender, EventArgs e )
        {
            textBoxLength.Text = trackBar5.Value.ToString();
        }

        private void buttonColorSpring_Click( object sender, EventArgs e )
        {
            colorDialogSpring.Color = buttonColorSpring.BackColor;
            if (colorDialogSpring.ShowDialog() == DialogResult.OK)
            {
                buttonColorSpring.BackColor = colorDialogSpring.Color;
            }
        }

        private void buttonColorBoard_Click( object sender, EventArgs e )
        {
            colorDialogBoard.Color = buttonColorBoard.BackColor;
            if (colorDialogBoard.ShowDialog() == DialogResult.OK)
            {
                buttonColorBoard.BackColor = colorDialogBoard.Color;
            }
        }

        private void trackBar6_ValueChanged( object sender, EventArgs e )
        {
            speed = (trackBar6.Value / 10f);
            textBoxSpeed.Text = speed.ToString();
        }

        private void exitToolStripMenuItem_Click( object sender, EventArgs e )
        {
            Close();
        }

        private void tabControl2_Click( object sender, EventArgs e )
        {
            tabControl2.SelectedIndex = -1;
        }

        private void tabControl2_DoubleClick( object sender, EventArgs e )
        {
            tabControl2.SelectedIndex = -1;
        }

        private void tabControl2_MouseClick( object sender, MouseEventArgs e )
        {
            tabControl2.SelectedIndex = -1;
        }

        private void tabControl2_MouseDoubleClick( object sender, MouseEventArgs e )
        {
            tabControl2.SelectedIndex = -1;
        }

        private void trackBar9_ValueChanged( object sender, EventArgs e )
        {
            int radius = trackBar9.Value;
            textBoxRadiusEdit.Text = radius.ToString();
            (world.Selected as Sphere).Radius = radius;
        }

        private void textBoxMassEdit_TextChanged( object sender, EventArgs e )
        {
            (world.Selected as Sphere).Mass = float.Parse( textBoxMassEdit.Text );
        }

        private void trackBar8_ValueChanged( object sender, EventArgs e )
        {
            float elasticity = trackBar8.Value / 100f;
            textBoxElasticityEdit.Text = elasticity.ToString();
            (world.Selected as Sphere).Elasticity = elasticity;
        }

        private void checkBoxGravityEdit_CheckedChanged( object sender, EventArgs e )
        {
            if (checkBoxGravityEdit.Checked)
            {
                trackBar7.Enabled = true;
                textBoxGravityEdit.Enabled = true;
                (world.Selected as Sphere).GravityStrength = float.Parse( textBoxGravityEdit.Text );
            }
            else
            {
                trackBar7.Enabled = false;
                textBoxGravityEdit.Enabled = false;
                (world.Selected as Sphere).GravityStrength = 0;
            }
        }

        private void trackBar7_ValueChanged( object sender, EventArgs e )
        {
            float gravity = trackBar7.Value / 10f;
            textBoxGravityEdit.Text = gravity.ToString();
            (world.Selected as Sphere).GravityStrength = gravity;
        }

        private void checkBoxStationaryEdit_CheckedChanged( object sender, EventArgs e )
        {
            (world.Selected as Sphere).Stationary = checkBoxStationaryEdit.Checked;
        }

        private void buttonColorSphereEdit_Click( object sender, EventArgs e )
        {
            colorDialogSphere.Color = buttonColorSphereEdit.BackColor;
            if (colorDialogSphere.ShowDialog() == DialogResult.OK)
            {
                buttonColorSphereEdit.BackColor = colorDialogSphere.Color;
                Sphere sph = (world.Selected as Sphere);
                sph.Deselect();
                sph.Clr = colorDialogSphere.Color;
                sph.Select();
            }
        }

        private void trackBar11_ValueChanged( object sender, EventArgs e )
        {
            float k = trackBar11.Value / 10f;
            textBoxKEdit.Text = k.ToString();
            (world.Selected as Spring).k = k;
        }

        private void trackBar10_ValueChanged( object sender, EventArgs e )
        {
            float length = trackBar10.Value;
            textBoxLengthEdit.Text = length.ToString();
            (world.Selected as Spring).Length = length;
        }

        private void buttonColorSpringEdit_Click( object sender, EventArgs e )
        {
            colorDialogSpring.Color = buttonColorSpringEdit.BackColor;
            if (colorDialogSpring.ShowDialog() == DialogResult.OK)
            {
                buttonColorSpringEdit.BackColor = colorDialogSpring.Color;
                Spring spr = (world.Selected as Spring);
                spr.Deselect();
                spr.Clr = colorDialogSpring.Color;
                spr.Select();
            }
        }

        private void buttonColorBoardEdit_Click( object sender, EventArgs e )
        {
            colorDialogBoard.Color = buttonColorBoardEdit.BackColor;
            if (colorDialogBoard.ShowDialog() == DialogResult.OK)
            {
                buttonColorBoardEdit.BackColor = colorDialogBoard.Color;
                Board b = (world.Selected as Board);
                b.Deselect();
                b.Clr = colorDialogBoard.Color;
                b.Select();
            }
        }

        private void buttonDelete_Click( object sender, EventArgs e )
        {
            world.DeleteSelected();
        }

        private void trackBar12_ValueChanged( object sender, EventArgs e )
        {
            float gravity = trackBar12.Value / 10f;
            textBoxEnvGravity.Text = gravity.ToString();
            world.Gravity.y = gravity;
        }

        private void checkBoxFriction_CheckedChanged( object sender, EventArgs e )
        {
            if (checkBoxFriction.Checked)
            {
                world.AirFriction = 0.0005f;
            }
            else
            {
                world.AirFriction = 0;
            }
        }

        private void checkBoxWalls_CheckedChanged( object sender, EventArgs e )
        {
            if (checkBoxWalls.Checked)
            {
                world.Walls = true;
            }
            else
            {
                world.Walls = false;
            }
        }

        private void checkBoxCollisions_CheckedChanged( object sender, EventArgs e )
        {
            if (checkBoxCollisions.Checked)
            {
                world.Collisions = true;
            }
            else
            {
                world.Collisions = false;
            }
        }

        private void buttonClearAll_Click( object sender, EventArgs e )
        {
            world.ClearScene();
        }

        private void buttonDeleteSpheres_Click( object sender, EventArgs e )
        {
            world.ClearSpheres();
        }

        private void buttonDeleteSprings_Click( object sender, EventArgs e )
        {
            world.ClearSprings();
        }

        private void buttonDeleteBoards_Click( object sender, EventArgs e )
        {
            world.ClearBoards();
        }

        private void newToolStripMenuItem_Click( object sender, EventArgs e )
        {
            world.ClearScene();
            sceneFile = "";

            trackBar12.Value = 10;
            checkBoxFriction.Checked = true;
            checkBoxWalls.Checked = true;
            checkBoxCollisions.Checked = true;
        }

        private void saveToolStripMenuItem_Click( object sender, EventArgs e )
        {
            if (sceneFile == "")
            {
                bool wasRunning = running;
                running = false;

                saveFileDialog1.InitialDirectory = Application.StartupPath + @"\Scenes";
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    sceneFile = saveFileDialog1.FileName;
                    world.SaveScene( sceneFile );
                }

                running = wasRunning;
            }
            else
            {
                world.SaveScene( sceneFile );
            }
        }

        private void saveAsToolStripMenuItem_Click( object sender, EventArgs e )
        {
            bool wasRunning = running;
            running = false;

            if (sceneFile != "")
                saveFileDialog1.FileName = sceneFile.Substring( sceneFile.LastIndexOf( '\\' ) + 1 );

            saveFileDialog1.InitialDirectory = Application.StartupPath + @"\Scenes";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                sceneFile = saveFileDialog1.FileName;
                world.SaveScene( sceneFile );
            }

            running = wasRunning;
        }

        private void loadToolStripMenuItem_Click( object sender, EventArgs e )
        {
            bool wasRunning = running;
            running = false;

            openFileDialog1.InitialDirectory = Application.StartupPath + @"\Scenes";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                sceneFile = openFileDialog1.FileName;
                world.LoadScene( sceneFile );

                trackBar12.Value = Convert.ToInt32( world.Gravity.y * 12 );
                
                if (world.AirFriction != 0)
                    checkBoxFriction.Checked = true;
                else
                    checkBoxFriction.Checked = false;

                if (world.Walls)
                    checkBoxWalls.Checked = true;
                else
                    checkBoxWalls.Checked = false;

                if (world.Collisions)
                    checkBoxCollisions.Checked = true;
                else
                    checkBoxCollisions.Checked = false;
            }

            running = wasRunning;
        }

        private void optionsToolStripMenuItem1_Click( object sender, EventArgs e )
        {
            bool wasRunning = running;
            running = false;
            this.Enabled = false;

            OptionsForm optionsForm = new OptionsForm(hqSpheres, hqSprings, antiAliasing);
            optionsForm.FormBorderStyle = FormBorderStyle.FixedSingle;

            if (optionsForm.ShowDialog() == DialogResult.OK)
            {
                antiAliasing = optionsForm.AntiAliasing;
                if (antiAliasing)
                    buffer.SmoothingMode = SmoothingMode.HighQuality;
                else
                    buffer.SmoothingMode = SmoothingMode.HighSpeed;

                hqSpheres = optionsForm.HqSpheres;
                world.hqSpheres = hqSpheres;

                hqSprings = optionsForm.HqSprings;
                world.hqSprings = hqSprings;

                saveConfig();
            }

            this.Enabled = true;
            running = wasRunning;
        }

        private void aboutToolStripMenuItem_Click( object sender, EventArgs e )
        {
            MessageBox.Show( "         Physics Playground\n\n                     by Simon Sotak at MFF UK\n\n\n\nPlease contact me at the21st@gmail.com\n    if you find any bugs and/or glitches", "About" );
        }

        private void checkBoxCurve_CheckedChanged( object sender, EventArgs e )
        {

            if (checkBoxCurve.Checked)
            {
                label16.Visible = true;
            }
            else
            {
                label16.Visible = false;
            }
        }
    }
}
