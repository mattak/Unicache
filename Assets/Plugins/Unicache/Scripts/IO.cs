using System.IO;

namespace Unicache
{
    public static class IO
    {
        public static void MakeParentDirectory(string path)
        {
            MakeDirectory(Path.GetDirectoryName(path));
        }

        public static void MakeDirectory(string path)
        {
            var components = path.Split(Path.DirectorySeparatorChar);
            if (components.Length < 1)
            {
                return;
            }

            var currentPath = "";

            for (int i = 0; i < components.Length; i++)
            {
                currentPath = Path.Combine(currentPath, components[i]);

                if (!string.IsNullOrEmpty(currentPath) && !Directory.Exists(currentPath))
                {
                    Directory.CreateDirectory(currentPath);
                }
            }
        }

        public static void CleanDirectory(string path)
        {
            var directory = new DirectoryInfo(path);

            foreach (var file in directory.GetFiles())
            {
                file.Delete();
            }

            foreach (var dir in directory.GetDirectories())
            {
                dir.Delete(true);
            }
        }

        public static void Write(string fullpath, byte[] data)
        {
            using (var writer = new FileStream(fullpath, FileMode.Create))
            {
                writer.Write(data, 0, data.Length);
            }
        }

        public static byte[] Read(string fullpath)
        {
            using (var reader = new FileStream(fullpath, FileMode.Open))
            {
                byte[] data = new byte[reader.Length];
                reader.Read(data, 0, data.Length);
                return data;
            }
        }
    }
}