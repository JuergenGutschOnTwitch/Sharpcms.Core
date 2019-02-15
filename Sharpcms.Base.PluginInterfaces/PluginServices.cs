// sharpcms is licensed under the open source license GPL - GNU General Public License.

using Sharpcms.Base.Library.Process;
using System;
using System.IO;
using System.Reflection;

namespace Sharpcms.Base.PluginInterface
{
    /// <summary>
    /// Summary description for PluginServices.
    /// </summary>
    public class PluginServices : IPluginHost
    {
        private AvailablePlugins _colAvailablePlugins = new AvailablePlugins();

        /// <summary>
        /// A Collection of all Plugins Found and Loaded by the FindPlugins() Method
        /// </summary>
        public AvailablePlugins AvailablePlugins
        {
            get
            {
                return _colAvailablePlugins;
            }
            set
            {
                _colAvailablePlugins = value;
            }
        }

        /// <summary>
        /// Searches the Application's Startup Directory for Plugins
        /// </summary>
        public void FindPlugins(Process process)
        {
            FindPlugins(AppDomain.CurrentDomain.BaseDirectory);
        }

        /// <summary>
        /// Searches the passed Path for Plugins
        /// </summary>
        /// <param name="path">Directory to search for Plugins in</param>
        public void FindPlugins(String path)
        {
            //First empty the collection, we're reloading them all
            _colAvailablePlugins.Clear();

            //Go through all the files in the plugin directory
            foreach (String fileOn in Directory.GetFiles(path))
            {
                FileInfo file = new FileInfo(fileOn);

                //Preliminary check, must be .dll
                if (file.Extension.Equals(".dll"))
                {
                    //Add the 'plugin'
                    //AddPlugin(fileOn, process);
                }
            }
        }

        /// <summary>
        /// Unloads and Closes all AvailablePlugins
        /// </summary>
        public void ClosePlugins()
        {
            foreach (AvailablePlugin pluginOn in _colAvailablePlugins)
            {
                //Close all plugin instances
                //We call the plugins Dispose sub first incase it has to do 
                //Its own cleanup stuff
                if (pluginOn.Instance != null)
                {
                    pluginOn.Instance.Dispose();
                }

                //After we give the plugin a chance to tidy up, get rid of it
                pluginOn.Instance = null;
            }

            //Finally, clear our collection of available plugins
            _colAvailablePlugins.Clear();
        }

        private void AddPlugin(String fileName, Process process)
        {
            //Create a new assembly from the plugin file we're adding..
            Assembly pluginAssembly = Assembly.LoadFrom(fileName);

            //Next we'll loop through all the Types found in the assembly
            foreach (Type pluginType in pluginAssembly.GetTypes())
            {
                if (pluginType.IsPublic) //Only look at public types
                {
                    if (!pluginType.IsAbstract)  //Only look at non-abstract types
                    {
                        //Gets a type object of the interface we need the plugins to match
                        Type typeInterface = pluginType.GetInterface("Sharpcms.PluginInterface.IPlugin", true);

                        //Make sure the interface we want to use actually exists
                        if (typeInterface != null)
                        {
                            String pluginTypeName = pluginType.ToString();
                            IPlugin instance = Activator.CreateInstance(pluginAssembly.GetType(pluginTypeName)) as IPlugin;

                            if (instance != null)
                            {
                                instance.Host = this;

                                //Set the Plugin's process
                                instance.Process = process;

                                //Call the initialization sub of the plugin
                                instance.Initialize();

                                //Create a new available plugin since the type implements the IPlugin interface
                                AvailablePlugin newPlugin = new AvailablePlugin
                                {
                                    //Set the filename where we found it
                                    AssemblyPath = fileName,

                                    //Create a new instance and store the instance in the collection for later use
                                    //We could change this later on to not load an instance.. we have 2 options
                                    //1- Make one instance, and use it whenever we need it.. it's always there
                                    //2- Don't make an instance, and instead make an instance whenever we use it, then close it
                                    //For now we'll just make an instance of all the plugins

                                    //Set the Plugin's host to this class which inherited IPluginHost
                                    Instance = instance
                                };

                                //Add the new plugin to our collection here
                                _colAvailablePlugins.Add(newPlugin);
                            }
                        }
                    }
                }
            }
        }
    }
}