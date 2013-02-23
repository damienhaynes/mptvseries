using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace WindowPlugins.GUITVSeries.Configuration
{
    public enum UserFinishedRequestedAction
    {
        Cancel,
        Next,
        Prev,
        ShowMe
    }
    public delegate void UserFinishedEditingDelegate(UserInputResults UserInputResult, UserFinishedRequestedAction RequestedAction);    
    
    public partial class ImportWizard : UserControl
    {
        public enum WizardButton
        {
            Cancel,
            Next,
            Prev,
            Finish
        }

        public delegate void WizardNavigateDelegate(UserFinishedRequestedAction RequestedAction);

        public static event WizardNavigateDelegate OnWizardNavigate;
        public event EventHandler ImportFinished;
        ConfigurationForm owner = null;

        internal ImportWizard(ConfigurationForm owner)
        {
            InitializeComponent();

            this.buttonFinish.Visible = false;
            this.buttonNext.Visible = true;

            this.owner = owner;                       
        }

        internal void Init()
        {
            //OnlineParsing.OnlineParsingCompleted += new OnlineParsing.OnlineParsingCompletedHandler(b =>
            //{
            //    this.buttonFinish.Visible = true;
            //    this.buttonCancel.Visible = false;
            //    this.buttonNext.Enabled = false;
            //    this.buttonPrev.Enabled = false;
            //});            
        }

        #region Wizard Navigation
        private void buttonCancel_Click(object sender, EventArgs e)
        {            
            DialogResult dialogResult = MessageBox.Show("Are you sure you wish to Cancel the Import?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                if (OnWizardNavigate != null)
                    OnWizardNavigate(UserFinishedRequestedAction.Cancel);

                if (this.owner != null)
                    owner.AbortImport();

                if (this.ImportFinished != null)
                    ImportFinished(this, new EventArgs());

                owner.EnableImportButtonState(true);
            }
        }

        private void buttonFinish_Click(object sender, EventArgs e)
        {
            if (ImportFinished != null)
                ImportFinished(this, new EventArgs());
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            // show next step
            OnWizardNavigate(UserFinishedRequestedAction.Next);
        }

        private void buttonPrev_Click(object sender, EventArgs e)
        {
            // show previous step
            OnWizardNavigate(UserFinishedRequestedAction.Prev);
        }
        #endregion

        #region Add/Remove Panels
        public void ShowDetailsPanel(Control c)
        {
            if (this.panelWizardSteps.Controls.Contains(c))
            {
                c.BringToFront();
                c.Visible = true;
                return;
            }

            this.panelWizardSteps.Controls.Add(c);
            c.Dock = DockStyle.Fill;            
            c.BringToFront();
            c.Visible = true;
        }

        public void RemoveDetailsPanel(Control c)
        {
            this.panelWizardSteps.Controls.Remove(c);            
        }

        public void AddSleepingDetailsPanel(Control c)
        {
            this.panelWizardSteps.Controls.Add(c);
            c.Dock = DockStyle.Fill;
            c.BringToFront();
            c.Visible = false;
        }
        #endregion

        #region Public functions
        public void SetButtonLabel(WizardButton button, string label)
        {
            switch (button)
            {
                case WizardButton.Cancel:
                    this.buttonCancel.Text = label;
                    break;

                case WizardButton.Next:
                    this.buttonNext.Text = label;
                    break;

                case WizardButton.Prev:
                    this.buttonCancel.Text = label;
                    break;
            }
        }

        public void SetButtonState(WizardButton button, bool state)
        {
            switch (button)
            {
                case WizardButton.Cancel:
                    this.buttonCancel.Enabled = state;
                    break;

                case WizardButton.Next:
                    this.buttonNext.Enabled = state;
                    break;

                case WizardButton.Prev:
                    this.buttonPrev.Enabled = state;
                    break;
            }
        }

        public void SetButtonVisibility(WizardButton button, bool state)
        {
            switch (button)
            {
                case WizardButton.Finish:
                    this.buttonFinish.Visible = state;
                    break;

                case WizardButton.Cancel:
                    this.buttonCancel.Visible = state;
                    break;

                case WizardButton.Next:
                    this.buttonNext.Visible = state;
                    break;

                case WizardButton.Prev:
                    this.buttonPrev.Visible = state;
                    break;
            }
        }
        #endregion
    }
}
