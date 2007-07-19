using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WindowPlugins.GUITVSeries
{
    public partial class ExpressionBuilder : Form
    {
        #region   Members 

            private string m_expressionString;

        #endregion

        #region  Enumerators 
        
        enum ExpressionTokens 
	    {
    	    Series = 1,
            Season,
            Episode,
            Episode2,
            Title,
            Extension,
            Numbers,
            WordChars,
            Optional            
	    };

        #endregion

        #region Constructors

        public ExpressionBuilder()
        {
            InitializeComponent();

            ddTokens.DataSource = System.Enum.GetValues(typeof(ExpressionTokens));
        }

        #endregion

        #region Properties 

        public string Expression 
        {
            get
            {
                return m_expressionString;
            }
        }

        #endregion

        #region Methods 

        private string GetToken(ExpressionTokens  eToken, String strConstraint  ) 
        {
            try
            {
                switch (eToken)
                {
                    case ExpressionTokens.Series:
                    case ExpressionTokens.Title:
                        if (strConstraint.Length > 0)
                        {
                            strConstraint = " {" + strConstraint + "}";
                        }
                        break;

                    case ExpressionTokens.Season:
                    case ExpressionTokens.Episode:
                    case ExpressionTokens.Episode2:
                        double retNum;
                        if (strConstraint.Length > 0 && double.TryParse(strConstraint, System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum))
                        {
                            strConstraint = " {" + strConstraint + "}";
                        }
                        break;
                }

                return "<" + eToken.ToString() + strConstraint + ">";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return null;
            }
        }

        #endregion

        #region Events

        private void ddTokens_SelectedIndexChanged(object sender, EventArgs e)
        {
            panConstraint.Visible = true;

            switch ((ExpressionTokens)ddTokens.SelectedValue)
            {
                case ExpressionTokens.Series:
                case ExpressionTokens.Title:
                    lblDescription.Text = "Stop characters";
                    break;

                case ExpressionTokens.Season:
                case ExpressionTokens.Episode:
                case ExpressionTokens.Episode2:
                    lblDescription.Text = "Number of digits";
                    break;

                case ExpressionTokens.WordChars:
                case ExpressionTokens.Numbers:
                case ExpressionTokens.Optional:
                case ExpressionTokens.Extension:
                    panConstraint.Visible = false;
                    break;
            }
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            try
            {
                if ((ExpressionTokens)ddTokens.SelectedValue != ExpressionTokens.Optional)
                {
                    string tempString = txtExpression.Text;
                    int caretPosition = txtExpression.SelectionStart;
                    txtExpression.Text = tempString.Substring(0, caretPosition) +
                        GetToken((ExpressionTokens)ddTokens.SelectedValue, txtConstraint.Text) +
                        tempString.Substring(caretPosition, tempString.Length - caretPosition);
                    txtConstraint.Text = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnReplace_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtExpression.SelectedText.Length > 0)
                {
                    if ((ExpressionTokens)ddTokens.SelectedValue == ExpressionTokens.Optional)
                    {
                        txtExpression.SelectedText = "<" + ddTokens.SelectedValue.ToString() + "{" + txtExpression.SelectedText + "}>";
                    }
                    else
                    {
                        txtExpression.SelectedText = GetToken((ExpressionTokens)ddTokens.SelectedValue, txtConstraint.Text);
                    }
                    txtConstraint.Text = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog objFileDialog = new OpenFileDialog();
                objFileDialog.CheckFileExists = true;
                objFileDialog.CheckPathExists = true;
                objFileDialog.Filter = "All Files|*.*";
                objFileDialog.Multiselect = false;

                if (objFileDialog.ShowDialog() == DialogResult.OK)
                {
                    txtExpression.Text = objFileDialog.FileName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                this.DialogResult = DialogResult.OK;
                m_expressionString = txtExpression.Text;
                this.Close();
            }
            catch (Exception ex)
            {
                this.DialogResult = DialogResult.Abort;
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Abort;
            this.Close();
        }

#endregion

    }
}