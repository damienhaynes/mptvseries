using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Setup
{
    public class Web
    {
        public void LaunchBrowser(string url)
        {
            System.Diagnostics.Process.Start(url);
        }
    }
}
