using WindowPlugins.GUITVSeries;
using System;

namespace ConfigLauncher
{
    public class MPTVSeriesLauncher : PluginConfigLauncher
    {
        public override string FriendlyPluginName
        {
            get { return "MP-TVSeries"; }
        }

        public override void Launch()
        {
            ConfigConnector plugin = new ConfigConnector();
            plugin.ShowPlugin();
        }
    }
}


