using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using WindowPlugins.GUITVSeries.DataClass;

namespace WindowPlugins.GUITVSeries.Configuration
{
    /// <summary>
    /// A Checkbox derived control that can automatically load/save it's value to the MP-TVSeries DB
    /// </summary>
    internal class DBOptionCheckBox : CheckBox
    {
        #region Vars
        string _option = string.Empty;
    	readonly ToolTip _toolTip = new ToolTip();
        #endregion

        public DBOptionCheckBox() : base() { }

        #region Properties
        /// <summary>
        /// Gets or Sets the Control's Option which will automatically be saved upon
        /// CheckStateChanged and loaded during the Control's initialization. 
        /// Can only be Set once or will throw an InvalidOperationException
        /// </summary>
        [Browsable(true)]
        [Category("_MP-TVSeries"),
        Description("The Control's Option which will automatically be saved upon CheckStateChanged and loaded during the Control's initialization.")]
        public string Option
        {
            get { return _option; }
            set { InitializeOption(value); }
        }

        /// <summary>
        /// Gets or Sets the Tooltip associated with this Control
        /// </summary>
        [Browsable(true)]
        public string ToolTip
        {
            get { return _toolTip == null ? string.Empty : _toolTip.GetToolTip(this); }
            set { if(value != string.Empty) _toolTip.SetToolTip(this, value); }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Saves the Current Value to the DB with the currently Set Option Name
        /// </summary>
        public void SaveOptionToDB()
        {
            if (Option != string.Empty)
            {
                DBOption.SetOptions(Option, this.Checked);
            }
        }

        /// <summary>
        /// Loads the Value from the DB with the currently Set Option Name
        /// </summary>
        /// <returns>Value from DB</returns>
        public bool LoadOptionFromDB()
        {
            if (_option == string.Empty)
            {
                DBOption.GetOptions(_option);
            }
            return true;
        }
        #endregion

        #region Internals
        void InitializeOption(string newOption)
        {
            if (newOption != string.Empty && newOption != _option) // else no need to do anything
            {
                if (_option == string.Empty) // empty, this is the first Set
                {
                    _option = newOption;
                    // Load the Current Value from the DB (before Eventhandler is added)
                    this.Checked = DBOption.GetOptions(newOption);
                    // Register the Eventhandler for the CheckStateChanged Event
                    this.CheckStateChanged += new EventHandler(OptionCheckBox_CheckStateChanged);
                }
                else // has been Set before and wants to change -> illegal
                {
                    throw new InvalidOperationException(string.Format("The Option Property of the OptionCheckBox instance \"{0}\" was attempted to be changed to \"{1}\", but the Property is already set to \"{2}\".",
                        this.Name, newOption, Option));
                }
            }
        }

        void OptionCheckBox_CheckStateChanged(object sender, EventArgs e)
        {
            SaveOptionToDB();
        }
        #endregion

    }

}
