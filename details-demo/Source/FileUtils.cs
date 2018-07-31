using System.Collections.Generic;
using System.IO;

namespace DetailsDemo
{
    public class FileUtils
    {
        public static bool FileBelongsToFolder(string folder, string file)
        {
            string fullFolder = Path.GetFullPath(folder);
            string fullFile   = Path.GetFullPath("/" + file);
            return fullFile.StartsWith(fullFolder);
        }

        public static string GetRelativePath(string folder, string file)
        {
            string fullFolder = Path.GetFullPath(folder);
            string fullFile = Path.GetFullPath("/" + file);
            return fullFile.Replace(fullFolder, "").TrimStart('\\').Replace('\\', '/');
        }
    }
}
