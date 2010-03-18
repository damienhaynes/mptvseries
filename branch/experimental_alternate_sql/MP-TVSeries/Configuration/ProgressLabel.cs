using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace WindowPlugins.GUITVSeries.Configuration
{
    public enum ProgressLabelStatus
    {
        Waiting,
        InProgress,
        Finished
    }
    public partial class ProgressLabel : UserControl
    {
        public Label Label { get { return this.label1; } }
        public Label Progress { get { return this.label3; } }
        ProgressLabelStatus status = ProgressLabelStatus.Waiting;
        public ProgressLabelStatus Status
        {
            get { return status; }
            set { status = value; statusChanged(); }
        }

        void statusChanged()
        {
            switch (Status)
            {
                case ProgressLabelStatus.Waiting:
                    this.pictureBox1.BackColor = Color.White;
                    break;
                case ProgressLabelStatus.InProgress:
                    this.pictureBox1.BackColor = Color.Yellow;
                    break;
                case ProgressLabelStatus.Finished:
                    this.pictureBox1.BackColor = Color.Green;
                    break;
                default:
                    break;
            }
        }

        public ProgressLabel()
        {
            InitializeComponent();
        }
    }
}
