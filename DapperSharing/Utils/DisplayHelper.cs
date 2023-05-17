using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DapperSharing.Utils
{
    public static class DisplayHelper
    {
        public static void PrintJson (object value)
        {
            Console.WriteLine(JsonSerializer.Serialize(value, new JsonSerializerOptions
            {
                WriteIndented = true
            }));
        }
    }
}
