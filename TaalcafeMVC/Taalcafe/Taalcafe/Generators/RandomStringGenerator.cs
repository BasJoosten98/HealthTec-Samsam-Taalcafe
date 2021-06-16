using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taalcafe.Generators
{
    public static class RandomStringGenerator
    {
        private static readonly Random random = new Random();
        private static readonly int maxAllowedChars = 20;
        private static readonly char[] allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();

        public static string CreateString(int amountOfChars)
        {
            string holder = "";
            for (int i = 0; i < amountOfChars; i++)
            {
                holder += allowedChars[random.Next(0, allowedChars.Length)];
            }
            return holder;
            //return "1234";
        }

        public static string CreateString()
        {
            int randomAmount = random.Next(1, maxAllowedChars + 1);
            return CreateString(randomAmount);
        }


    }
}
