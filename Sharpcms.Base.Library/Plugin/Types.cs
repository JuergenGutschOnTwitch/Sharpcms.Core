// sharpcms is licensed under the open source license GPL - GNU General Public License.

using System;
using System.Collections;
using System.Collections.Generic;

namespace Sharpcms.Base.Library.Plugin
{
    /// <summary>
    /// Collection for AvailablePlugin Type
    /// </summary>
    public class AvailablePlugins : CollectionBase
    {
        //A Simple Home-brew class to hold some info about our Available Plugins

        /// <summary>
        /// Add a Plugin to the collection of Available plugins
        /// </summary>
        /// <param name="pluginToAdd">The Plugin to Add</param>
        public void Add(AvailablePlugin pluginToAdd)
        {
            List.Add(pluginToAdd);
        }

        /// <summary>
        /// Remove a Plugin to the collection of Available plugins
        /// </summary>
        /// <param name="pluginToRemove">The Plugin to Remove</param>
        public void Remove(AvailablePlugin pluginToRemove)
        {
            List.Remove(pluginToRemove);
        }

        /// <summary>
        /// Finds a plugin in the available Plugins
        /// </summary>
        /// <param name="pluginNameOrPath">The name or File path of the plugin to find</param>
        /// <returns>Available Plugin, or null if the plugin is not found</returns>
        public AvailablePlugin Find(String pluginNameOrPath)
        {
            AvailablePlugin availablePlugin = null;

            //Loop through all the plugins
            foreach (AvailablePlugin pluginOn in List)
            {
                //Find the one with the matching name or filename
                if ((pluginOn.Instance.Name.Equals(pluginNameOrPath)) || pluginOn.AssemblyPath.Equals(pluginNameOrPath))
                {
                    availablePlugin = pluginOn;
                    break;
                }
            }

            return availablePlugin;
        }

        public IEnumerable<AvailablePlugin> FindImplementations(String api)
        {
            List<AvailablePlugin> availablePlugins = new List<AvailablePlugin>();

            foreach (AvailablePlugin availablePlugin in List)
            {
                IPlugin2 instance = availablePlugin.Instance as IPlugin2;
                if (instance != null && instance.Implements != null)
                {
                    List<String> implements = new List<String>(instance.Implements);
                    if (implements.Contains(api))
                    {
                        availablePlugins.Add(availablePlugin);
                    }
                }
            }

            return availablePlugins;
        }
    }
}