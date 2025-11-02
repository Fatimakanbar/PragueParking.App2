using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PragueParking.Data
{
    public static class FileHandler
    {
        public static T LoadJson<T>(string path)
        {
            if (!File.Exists(path)) return default!;
            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<T>(json)!;
        }

        public static void SaveJson<T>(string path, T data)
        {
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, json);
        }
    }
}
