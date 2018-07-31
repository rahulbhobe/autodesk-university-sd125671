using System.Collections.Generic;
using System.IO;

namespace LinksDemo
{
    public class FileUtils
    {
        public static bool FileBelongsToFolder(string folder, string file)
        {
            string fullFolder = Path.GetFullPath(folder);
            string fullFile   = Path.GetFullPath("/" + file);
            return fullFile.StartsWith(fullFolder);
        }

        public static string[] SplitRelativePath(string folder, string file)
        {
            string fullFolder = Path.GetFullPath(folder);
            string fullFile = Path.GetFullPath("/" + file);
            string relPath = fullFile.Replace(fullFolder, "").TrimStart('\\');
            return relPath.Split(Path.DirectorySeparatorChar);
        }
    }
}
