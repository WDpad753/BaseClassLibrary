using BaseClass.Model;
using BaseLogger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClass.Helper
{
    public class PathCombine
    {
        private LogWriter _logger;

        public PathCombine(LogWriter Logger)
        {
            _logger = Logger;
        }

        public string? CombinePath(CombinationType type, params string[] paths)
        {
            string? fullPath = null;

            if (paths.Length <= 0)
            {
                return null;
            }

            if(type == CombinationType.Folder)
            {
                fullPath = CombineFolderPaths(paths);
            }
            else if (type == CombinationType.URL)
            {
                fullPath = CombineURLPaths(paths);
            }

            // Combine the two paths
            return fullPath;
        }

        private string? CombineFolderPaths(string[] paths)
        {
            // Combine folder paths using Path.Combine
            return System.IO.Path.Combine(paths);
        }

        private string? CombineURLPaths(string[] paths)
        {
            // Combine URL paths using string.Join
            // Ensure that each path starts with a '/' and ends with a '/'
            return string.Join("/", paths.Select(p => p.Trim('/')));
        }
    }
}
