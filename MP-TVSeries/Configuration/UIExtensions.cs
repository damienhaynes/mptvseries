using System;
using System.Reflection;
using System.Windows.Forms;

namespace WindowPlugins.GUITVSeries.Configuration
{
  public static class UIExtensions
  {
    public static void DoubleBuffered( this DataGridView dgv, bool setting )
    {
      Type dgvType = dgv.GetType();
      PropertyInfo pi = dgvType.GetProperty( "DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic );
      pi.SetValue( dgv, setting, null );
    }
  }
}
