﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Simple.IO
{
    public class FileLocator : List<string>
    {
        public static string ByPattern(string pattern)
        {
            var dir = Path.GetDirectoryName(pattern);
            var file = Path.GetFileName(pattern);
            if (string.IsNullOrEmpty(dir)) dir = ".";
            var firstFile = Directory.GetFiles(dir, file).FirstOrDefault();
            if (firstFile == null)
                throw new FileNotFoundException("Not found: {0}".AsFormatFor(pattern), pattern);
            else
                return firstFile;
        }

        public IEnumerable<string> ExistsWhere(string file)
        {
            string temp;
            foreach (var path in this)
                if (File.Exists(temp = Path.Combine(path, file)))
                    yield return temp;
        }

        public string Find(string file, bool shouldThrow)
        {
            string path = ExistsWhere(file).FirstOrDefault();
            if (path == null && shouldThrow)
                throw new FileNotFoundException(string.Format("Search locations: {0}", string.Join(", ", this.ToArray())));

            return path;
        }

        public void AddDefaults(Assembly asm)
        {
            this.Add(".");
            this.Add(Path.GetDirectoryName(asm.Location));
            this.Add(Path.GetDirectoryName(Uri.UnescapeDataString(new Uri(asm.CodeBase).AbsolutePath)));
        }

        public void AddPath(params string[] paths)
        {
            if (paths == null || paths.Length == 0) return;
            
            string path = string.Empty;
            foreach (var item in paths)
            {
                path = Path.Combine(path, item);
            }

            Add(path);
        }

        public string Find(string file)
        {
            return Find(file, true);
        }
    }
}
