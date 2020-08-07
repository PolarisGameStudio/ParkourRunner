using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Object = System.Object;

namespace Utilities
{
    public static class SaveSystem
    {
        public static void BinarySave<T>(string fileName, Object value)
        {
            BinaryFormatter bf = new BinaryFormatter();
            string path = Path.Combine(UnityEngine.Application.persistentDataPath, fileName);
            // Debug.Log($"Saving {typeof(T).Name} to {path}");
            FileStream file = File.Create(path);

            bf.Serialize(file, (T) value);
            file.Close();
        }

        public static T BinaryLoad<T>(string fileName)
        {
            BinaryFormatter bf = new BinaryFormatter();
            string path = Path.Combine(UnityEngine.Application.persistentDataPath, fileName);
            FileStream file = File.Open(path, FileMode.Open);

            var data = bf.Deserialize(file);
            file.Close();

            return (T) data;
        }


        public static bool Exists(string fileName) {
            var path = Path.Combine(UnityEngine.Application.persistentDataPath, fileName);
            return File.Exists(path);
        }
    }
}
