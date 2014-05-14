using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace LyvInDisco
{
    public partial class Form1 : Form
    {
        private Disco disco;

        public Form1()
        {
            disco = new Disco();
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int timer, transition;
            if(textBoxTimer.Text!="")
            {
                if (textBoxTransition.Text!="")
                {
                    if (checkBox1.Checked||checkBox2.Checked||checkBox3.Checked)
                    {
                        try
                        {
                            timer = Convert.ToInt32(this.textBoxTimer.Text);
                            transition = Convert.ToInt32(this.textBoxTransition.Text);

                            if ((timer < 60001) && (timer > 0))
                            {
                                if (timer < transition)
                                {
                                    MessageBox.Show("Please use a transition value lower than the timer value!");
                                }
                                else
                                {
                                    transition = transition / 100;
                                    Dictionary<int, bool> lights = new Dictionary<int, bool>();
                                    lights.Add(1, checkBox1.Checked);
                                    lights.Add(2, checkBox2.Checked);
                                    lights.Add(3, checkBox3.Checked);
                                    disco.StartDisco(timer, transition, lights);
                                }
                            }
                            else
                            {
                                MessageBox.Show("Please use a timer value between 1 and 60000!");
                            }
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("Please only type numbers into the timer and transition fields!");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please select at least one of the lights.");
                    }
                }
                else
                {
                    MessageBox.Show("Please use a transition value between 0 and the timer value!");
                }
            }
            else
            {
                MessageBox.Show("Please enter a timer value!");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            disco.StopDisco();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int timer, transition;
            if (textBoxTimer.Text != "")
            {
                if (textBoxTransition.Text != "")
                {
                    if (checkBox1.Checked || checkBox2.Checked || checkBox3.Checked)
                    {
                        try
                        {
                            timer = Convert.ToInt32(this.textBoxTimer.Text);
                            transition = Convert.ToInt32(this.textBoxTransition.Text);

                            if ((timer < 60001) && (timer > 0))
                            {
                                if (timer < transition)
                                {
                                    MessageBox.Show("Please use a transition value lower than the timer value!");
                                }
                                else
                                {
                                    transition = transition / 100;
                                    Dictionary<int, bool> lights = new Dictionary<int, bool>();
                                    lights.Add(1, checkBox1.Checked);
                                    lights.Add(2, checkBox2.Checked);
                                    lights.Add(3, checkBox3.Checked);
                                    try
                                    {
                                        disco.StartTest(timer, transition, lights);
                                    }
                                    catch (Exception)
                                    {
                                        MessageBox.Show("woepsie");
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("Please use a timer value between 1 and 60000!");
                            }
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("Please only type numbers into the timer and transition fields!");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please select at least one of the lights.");
                    }
                }
                else
                {
                    MessageBox.Show("Please use a transition value between 0 and the timer value!");
                }
            }
            else
            {
                MessageBox.Show("Please enter a timer value!");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            disco.StopTest();
        }
    }
}
