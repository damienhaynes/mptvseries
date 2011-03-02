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
using MediaPortal.GUI.Video;

namespace Test_MP_TVSeries
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            MediaPortal.GUI.Video.TVSeriesPlugin plugin = new MediaPortal.GUI.Video.TVSeriesPlugin();
            plugin.ShowPlugin();
        }
    }
}