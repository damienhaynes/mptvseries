using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using MediaPortal.GUI.Library;

namespace PluginConfigLoader {
    static class Program {

        private static string pluginFile = string.Empty;

        enum MsgBoxIcon {
            INFO,
            WARNING,
            CRITICAL
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] arguments) {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // Exit if we could not process the command line arguments
            if (!ProcessArguments(arguments)) {
                return;
            }

            // Load Plugin Configuration
            LoadPluginConfiguration();

        }

        private static bool ProcessArguments(string[] arguments) {            
            // Check if there are any arguments
            if (arguments == null || arguments.Length == 0) {
                string message = "No Arguments were specified on the command line.\n\n";
                message += "Usage: PluginConfigLoader /plugin=<filename>";
                ShowMessage(message, MsgBoxIcon.INFO);
                return false;
            }

            // App only supports one argument
            string trimmedArgument = arguments[0].ToLower();

            // Valid argument starts with /plugin
            if (!trimmedArgument.StartsWith("/plugin")) {
                string message = "Invalid argument specified on the command line.\n\n";
                message += "Usage: PluginConfigLoader /plugin=<filename>";
                ShowMessage(message, MsgBoxIcon.INFO);
                return false;
            }
            
            // Get Filename of plugin
            string[] subArguments = trimmedArgument.Split('=');

            if (subArguments.Length < 2) {
                string message = "No filename specified in command line argument.\n\n";
                message += "Usage: PluginConfigLoader /plugin=<filename>";
                ShowMessage(message, MsgBoxIcon.INFO);
                return false;
            }

            pluginFile = subArguments[1];
            return true;
        }

        public static void LoadPluginConfiguration() {
            if (!File.Exists(pluginFile)) {
                string message = string.Format("Plugin not found: {0}", pluginFile);
                ShowMessage(message, MsgBoxIcon.INFO);
                return;
            }

            // Flag if plugin has a configuration form
            bool hasConfigForm = false;

            try {
                // Get Plugin Assembly and check if its valid
                Assembly pluginAssembly = Assembly.LoadFrom(pluginFile);

                if (pluginAssembly == null) {
                    string message = string.Format("Invalid Plugin: {0}", pluginFile);
                    ShowMessage(message, MsgBoxIcon.INFO);
                }

                Type[] exportedTypes = pluginAssembly.GetExportedTypes();

                foreach (Type type in exportedTypes) {
                    if (type.IsAbstract) {
                        continue;
                    }

                    if (type.GetInterface("MediaPortal.GUI.Library.ISetupForm") != null) {
                        try {                           
                            // Create instance of the current type                           
                            object pluginObject = Activator.CreateInstance(type);
                            ISetupForm pluginForm = pluginObject as ISetupForm;                            

                            // If the plugin has a Configuration form, show it
                            if (pluginForm != null) {
                                if (pluginForm.HasSetup()) {
                                    hasConfigForm = true;
                                    pluginForm.ShowPlugin();
                                    return;
                                }                                   
                            }
                        }
                        catch (Exception setupFormException) {
                            string message = string.Format("Exception in plugin SetupForm loading: \n\n{0} ", setupFormException.Message);
                            ShowMessage(message, MsgBoxIcon.CRITICAL);
                        }
                    }
                }
                
            }
            catch (Exception unknownException) {
                string message = string.Format("Exception in plugin loading: \n\n{0}", unknownException.Message);
                ShowMessage(message, MsgBoxIcon.CRITICAL);
            }

            if (!hasConfigForm) {
                string message = "Plugin does not have a configuration form to show";
                ShowMessage(message, MsgBoxIcon.INFO);
            }

        }

        private static void ShowMessage(string message, MsgBoxIcon icon) {
            MessageBoxIcon MessageBoxIcon = MessageBoxIcon.Information;
            if (icon == MsgBoxIcon.CRITICAL) MessageBoxIcon = MessageBoxIcon.Error;
            MessageBox.Show(message, "Plugin Config Loader", MessageBoxButtons.OK, MessageBoxIcon);
            return;
        }

    }
}
