using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;

namespace BlazorChess.Extensions
{
    public static class ObjectExtensions
    {
        public static void JsonPrint(this object obj)
        {
            Console.WriteLine(JsonSerializer.Serialize(obj));
        }
    }
}
