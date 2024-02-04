using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Code.Utility
{
    public static class RNG
    {
        private static readonly Random random = new();

        public static bool Chance(double chance) => random.NextDouble() <= chance;


        //Extensions
        public static T GetRandomElement<T>(List<T> objects) => objects[random.Next(0, objects.Count)];
        public static T GetRandomElement<T>(T[] objects) => objects[random.Next(0, objects.Length)];
        public static T GetRandomElement<T>(IEnumerable<T> objects) => objects.ElementAtOrDefault(random.Next(0, objects.Count()));
    }
}
