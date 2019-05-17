// sharpcms is licensed under the open source license GPL - GNU General Public License.

using System;

namespace Sharpcms.Data.SiteTree
{
    public class PagePath
    {
        private readonly String _name;
        private readonly String _path;

        public PagePath(String path)
        {
            int lastPathPosition = path.LastIndexOf("/", System.StringComparison.Ordinal);
            if (lastPathPosition > 0)
            {
                _path = path.Substring(0, lastPathPosition);
                _name = path.Substring(lastPathPosition + 1);
            }
            else
            {
                _path = string.Empty;
                _name = path;
            }
        }

        public String Path
        {
            get
            {
                return _path;
            }
        }

        public String Name
        {
            get
            {
                return _name;
            }
        }
    }
}