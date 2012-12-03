using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace T8SuitePro
{
    public enum FormGrowMode : int
    {
        LeftTopToRightBottom,
        LeftBottomToRightTop,
        RightTopToLeftBottom,
        RightBottomToLeftTop
    }

    public partial class frmMapHelper : DevExpress.XtraEditors.XtraForm
    {
        private double m_maxopacity = 0.85;
        private Size m_size = new Size(300, 300);
        private FormGrowMode m_growmode = FormGrowMode.LeftTopToRightBottom;

        public FormGrowMode Growmode
        {
            get { return m_growmode; }
            set { m_growmode = value; }
        }

        public frmMapHelper()
        {
            InitializeComponent();
            SetMaxOpacity(75);
            SetSize(new Size(400, 500));

        }

        public void SetMaxOpacity(int percentage)
        {
            m_maxopacity = (double)percentage / 100;
        }

        public void SetSize(Size s)
        {
            m_size = s;
        }

        public void Reset()
        {
            try
            {
                this.Opacity = 0;
                //this.Size = new Size(0, 0);
                timer1.Stop();
                this.mapViewer1.ClearGrid();
                this.Hide();
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (this.Opacity < m_maxopacity)
                {
                    this.Opacity += m_maxopacity / 20;
                    // de kaart moet ook groeien (slide-in) in 30 passen
                    if (this.Size.Width < m_size.Width)
                    {
                        this.Size = new Size(this.Size.Width + ((int)((double)m_size.Width / 15)), this.Size.Height + ((int)((double)m_size.Height / 15)));
                        if ((this.Location.Y + this.Size.Height) > Screen.GetWorkingArea(this).Height)
                        {
                            this.Location = new Point(this.Location.X, Screen.GetWorkingArea(this).Height - this.Size.Height);
                        }
                        if ((this.Location.X + this.Size.Width) > Screen.GetWorkingArea(this).Width)
                        {
                            this.Location = new Point(Screen.GetWorkingArea(this).Width - this.Size.Width, this.Location.Y);
                        }
                    }
                    Application.DoEvents();
                    //this.Size.Width += (int) ((double)m_size.Width / 30);
                    //this.Size.Height += (int) ((double)m_size.Height / 30);
                }
                else
                {
                    timer1.Stop();
                }
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
        }

        public void ShowPosition()
        {
            try
            {
                //timer1.Start();
                //this.Opacity = 0;
                //this.Size = new Size(0, 0);
                this.Size = m_size;
                if ((this.Location.Y + this.Size.Height) > Screen.GetWorkingArea(this).Height)
                {
                    this.Location = new Point(this.Location.X , Screen.GetWorkingArea(this).Height - this.Size.Height);
                }
                if ((this.Location.X + this.Size.Width) > Screen.GetWorkingArea(this).Width)
                {
                    this.Location = new Point(Screen.GetWorkingArea(this).Width - this.Size.Width, this.Location.Y);
                }
                this.Opacity = 100;
                this.Show();
                Application.DoEvents();
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
        }

    }
}