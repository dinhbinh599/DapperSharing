﻿using DapperSharing.Examples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        public static void PrintListOfMethods(Type classType)
        {
            Console.WriteLine("List of methods:");
            var countMethod = 1;
            foreach (var method in classType.GetMethods(BindingFlags.Static | BindingFlags.NonPublic))
            {
                Console.WriteLine($"{countMethod}. {method.Name}");
                countMethod++;
            }
        }

    }
}
