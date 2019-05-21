// sharpcms is licensed under the open source license GPL - GNU General Public License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Sharpcms.Base.Library.Plugin
{
    /// <summary>
    /// Summary description for PluginServices.
    /// </summary>
    public class PluginServices : IPluginHost
    {
        private AvailablePlugins _colAvailablePlugins = new AvailablePlugins();

        public IPlugin this[String pluginNameOrPath]
        {
            get
            {
                AvailablePlugin availablePlugin = AvailablePlugins.Find(pluginNameOrPath);
                IPlugin plugin = availablePlugin == null
                    ? null
                    : availablePlugin.Instance;

                return plugin;
            }
        }

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
        /// Invokes the action on all plug-ins implementing the specified API.
        /// </summary>
        /// <param name="api">API</param>
        /// <param name="action">Action to invoke</param>
        /// <param name="args">Arguments, are passed on to the Plugins</param>
        /// <returns>An array of results from the Plugins (only non-null values are included)</returns>
        public Object[] InvokeAll(String api, String action, params Object[] args)
        {
            List<Object> results = new List<Object>();

            IEnumerable<AvailablePlugin> plugins = AvailablePlugins.FindImplementations(api);
            foreach (AvailablePlugin plugin in plugins)
            {
                IPlugin2 invokablePlugin = plugin.Instance as IPlugin2;
                if (invokablePlugin == null)
                {
                    continue;
                }

                Object result = invokablePlugin.Invoke(api, action, args);
                if (result != null)
                {
                    results.Add(result);
                }
            }

            return results.ToArray();
        }

        public static IEnumerable<Object> Flatten(IEnumerable<Object> results)
        {
            List<Object> flattened = new List<Object>();

            try
            {
                foreach (Object result in results)
                {
                    if (result == null)
                    {
                        continue;
                    }

                    Object[] partResults = result as Object[];
                    if (partResults != null)
                    {
                        flattened.AddRange(partResults);
                    }
                }
            }
            catch
            {
                flattened = new List<Object>();
            }

            return flattened.ToArray();
        }

        /// <summary>
        /// Searches the passed Path for Plugins
        /// </summary>
        /// <param name="process"> </param>
        /// <param name="path">Directory to search for Plugins in</param>
        public void FindPlugins(Process.Process process, String path)
        {
            //First empty the collection, we're reloading them all
            _colAvailablePlugins.Clear();

            //Go through all the files in the plugin directory
            foreach (String fileOn in Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories))
            {
                FileInfo file = new FileInfo(fileOn);

                // Preliminary check, must be .dll
                if (file.Extension.Equals(".dll"))
                {
                    AddPlugin(fileOn, process); //Add the 'plugin'
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

        private void AddPlugin(String fileName, Process.Process process)
        {
            //Create a new assembly from the plugin file we're adding..
            Assembly pluginAssembly = Assembly.LoadFrom(fileName);

            //Next we'll loop through all the Types found in the assembly
            foreach (Type pluginType in pluginAssembly.GetTypes())
            {
                if (pluginType.IsPublic) //Only look at public types
                {
                    if (!pluginType.IsAbstract) //Only look at non-abstract types
                    {
                        //Gets a type Object of the interface we need the plugins to match
                        Type typeInterface = pluginType.GetInterface("Sharpcms.Base.Library.Plugin.IPlugin", true);

                        //Make sure the interface we want to use actually exists
                        if (typeInterface != null)
                        {
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
                                Instance = (IPlugin)Activator.CreateInstance(pluginAssembly.GetType(pluginType.ToString()))
                            };

                            //Set the Plugin's host to this class which inherited IPluginHost
                            newPlugin.Instance.Host = this;

                            //Set the Plugin's process
                            newPlugin.Instance.Process = process;

                            //Call the initialization sub of the plugin
                            newPlugin.Instance.Initialize();

                            // Do not add the BasePlugin
                            if (!newPlugin.Instance.Name.StartsWith("BasePlugin"))
                            {
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