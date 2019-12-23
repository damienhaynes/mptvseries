using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;

namespace WindowPlugins.GUITVSeries.Configuration
{
    static class UIExtensions
    {
        public static void DoubleBuffered( this DataGridView dgv, bool setting )
        {
            if ( LicenseManager.UsageMode == LicenseUsageMode.Designtime )
                return;

            Type dgvType = dgv.GetType();
            PropertyInfo pi = dgvType.GetProperty( "DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic );
            pi.SetValue( dgv, setting, null );
        }
    }
}
