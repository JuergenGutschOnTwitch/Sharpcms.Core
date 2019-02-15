// sharpcms is licensed under the open source license GPL - GNU General Public License.

using Sharpcms.Base.Library.Http;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Web;

namespace Sharpcms.Base.Library
{
    public class Cache
    {
        private readonly HttpApplicationState _applicationState;

        public Cache(HttpApplicationState applicationState)
        {
            _applicationState = applicationState;
        }

        private Hashtable CacheTable
        {
            get
            {
                if (_applicationState["cache"] == null)
                {
                    Clean();
                }

                return _applicationState["cache"] as Hashtable;
            }
        }

        public object this[string key, FileInfo fileDependency]
        {
            get
            {
                object value = this[key];
                if (value == null)
                {
                    return null;
                }

                string fileModified = fileDependency.LastWriteTime.ToString(CultureInfo.InvariantCulture);
                string cacheModifiedKey = FormatModifiedKey(key);
                if (this[cacheModifiedKey] == null || this[cacheModifiedKey].ToString() != fileModified)
                {
                    return null;
                }

                return value;
            }
            set
            {
                this[key] = value;

                string fileModified = fileDependency.LastWriteTime.ToString(CultureInfo.InvariantCulture);
                string cacheModifiedKey = FormatModifiedKey(key);
                this[cacheModifiedKey] = fileModified;
            }
        }

        public object this[string key]
        {
            get
            {
                return CacheTable[key];
            }
            set
            {
                CacheTable[key] = value;
            }
        }

        public void Clean()
        {
            Hashtable cache = new Hashtable();
            _applicationState["cache"] = cache;
        }

        private static string FormatModifiedKey(string key)
        {
            string modifiedKey = string.Format("{0}_filedependency", key);

            return modifiedKey;
        }
    }
}