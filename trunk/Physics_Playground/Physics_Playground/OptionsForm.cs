using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Physics_Playground
{
    public partial class OptionsForm : Form
    {
        public bool AntiAliasing, HqSpheres, HqSprings;

        public OptionsForm(bool hqSpheres, bool hqSprings, bool antiAliasing )
        {
            InitializeComponent();

            HqSpheres = hqSpheres;
            HqSprings = hqSprings;
            AntiAliasing = antiAliasing;
        }

        private void button1_Click( object sender, EventArgs e )
        {
            if (comboBox1.SelectedIndex == 0)
                HqSpheres = false;
            else
                HqSpheres = true;

            if (comboBox2.SelectedIndex == 0)
                HqSprings = false;
            else
                HqSprings = true;

            if (comboBox3.SelectedIndex == 0)
                AntiAliasing = false;
            else
                AntiAliasing = true;

            

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button2_Click( object sender, EventArgs e )
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void OptionsForm_Load( object sender, EventArgs e )
        {
            if (HqSpheres)
                comboBox1.Text = "High";
            else
                comboBox1.Text = "Low";

            if (HqSprings)
                comboBox2.Text = "High";
            else
                comboBox2.Text = "Low";

            if (AntiAliasing)
                comboBox3.Text = "On";
            else
                comboBox3.Text = "Off";
        }
    }
}
