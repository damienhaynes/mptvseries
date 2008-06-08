using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using WindowPlugins.GUITVSeries;

namespace Test_MP_TVSeries
{
    public partial class Form1 : Form
    {
        public Form1()
        {
          WindowPlugins.GUITVSeries.TVSeriesPlugin plugin = new WindowPlugins.GUITVSeries.TVSeriesPlugin();
            
            plugin.ShowPlugin();
        }
    }
}