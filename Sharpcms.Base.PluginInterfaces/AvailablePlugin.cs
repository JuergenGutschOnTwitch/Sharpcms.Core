using System;

namespace Sharpcms.Base.PluginInterface
{
    /// <summary>
    /// Data Class for Available Plugin. Holds an instance of the loaded Plugin, as well as the Plugin's Assembly Path
    /// </summary>
    public class AvailablePlugin
    {
        //This is the actual AvailablePlugin object.. 
        //Holds an instance of the plugin to access
        //ALso holds assembly path... not really necessary
        private String _assemblyPath = String.Empty;

        public String AssemblyPath
        {
            get
            {
                return _assemblyPath;
            }
            set
            {
                _assemblyPath = value;
            }
        }

        public IPlugin Instance { get; set; }

        public AvailablePlugin()
        {
            Instance = null;
        }
    }
}