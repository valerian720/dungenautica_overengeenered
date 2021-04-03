using System.Collections.Generic;
using Godot;

namespace SibGameJam2021.Core.Utility
{
    internal static class PrefabHelper
    {
        public static Dictionary<string, PackedScene> LoadPrefabsDictionary(string path)
        {
            var dict = new Dictionary<string, PackedScene>();
            var dir = new Directory();

            if (dir.Open(path) == Error.Ok)
            {
                dir.ListDirBegin();
                var filename = dir.GetNext();

                while (!string.IsNullOrEmpty(filename))
                {
                    if (dir.CurrentIsDir())
                    {
                        filename = dir.GetNext();
                        continue;
                    }

                    var scene = GD.Load<PackedScene>($"{path}/{filename}");
                    dict.Add(System.IO.Path.GetFileNameWithoutExtension(filename), scene);

                    filename = dir.GetNext();
                }
            }

            return dict;
        }

        public static List<PackedScene> LoadPrefabsList(string path)
        {
            var list = new List<PackedScene>();
            var dir = new Directory();

            if (dir.Open(path) == Error.Ok)
            {
                dir.ListDirBegin();
                var filename = dir.GetNext();

                while (!string.IsNullOrEmpty(filename))
                {
                    if (dir.CurrentIsDir())
                    {
                        filename = dir.GetNext();
                        continue;
                    }

                    var scene = GD.Load<PackedScene>($"{path}/{filename}");
                    list.Add(scene);

                    filename = dir.GetNext();
                }
            }

            return list;
        }
    }
}