using Sharpcms.Base.Library.Process;

namespace Sharpcms.Base.Library.Plugin
{
    public class BasePlugin2 : BasePlugin, IPlugin2
    {
        public string[] Implements
        {
            get { return null; }
        }

        public object Invoke(string api, string action, params object[] args)
        {
            return null;
        }

        public void Load(ControlList control, string action, string value, string pathTrail)
        {
            Load(control, action, pathTrail);
        }
    }
}