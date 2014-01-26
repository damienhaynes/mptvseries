using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace WindowPlugins.GUITVSeries.Configuration {
    public partial class ProgressDialog : Form {
        public enum Status { RUNNING, CANCELED, DONE }
        public delegate bool ProgressDelegate(Status status, int value);

        bool canceled = false;

        public ProgressDialog() {
            InitializeComponent();
        }

        public bool SetProgress(Status status, int position) {
            if (InvokeRequired) return (bool)Invoke(new ProgressDelegate(SetProgress), new object[] { status, position });

            // update progress bar
            progressBar.Value = position;

            // if we are done, schedule the dialog to be closed           
            if (status != Status.RUNNING) {
                Thread closeAction = new Thread(new ThreadStart(delegate() {
                    Thread.Sleep(600);
                    Close();
                }));
                closeAction.IsBackground = true;
                closeAction.Start();
            }

            return canceled;
        }

        private void Center() {
            if (Owner == null)
                return;

            Point center = new Point();
            center.X = Owner.Location.X + (Owner.Width / 2);
            center.Y = Owner.Location.Y + (Owner.Height / 2);

            Point newLocation = new Point();
            newLocation.X = center.X - (Width / 2);
            newLocation.Y = center.Y - (Height / 2);

            Location = newLocation;
        }

        private void cancelButton_Click(object sender, EventArgs e) {
            cancelButton.Text = "Canceling...";
            cancelButton.Enabled = false;
            canceled = true;
        }

        private void ProgressDialog_Load(object sender, EventArgs e) {
            Center();
        }
    }
}
