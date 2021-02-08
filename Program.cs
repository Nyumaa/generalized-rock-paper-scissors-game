using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Globalization;
using System.Text;

namespace _3
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length >= 3 && args.Length % 2 == 1 && checkDuplicate(args))
            {
                byte[] key = GeneratorKeys();
                int idComputerMove = GetMoveComputer(0, args.Length - 1, key);
                int idPlayerMove;
                string computerMove = $"Computer move: {args[idComputerMove]}";
                System.Console.WriteLine("HMAC: " + HMACHASH(computerMove, HashEncodeToString(key)));
                while (true)
                {
                    System.Console.WriteLine("Available moves: \n");

                    for (var i = 0; i < args.Length; i++)
                    {
                        System.Console.WriteLine($"{i + 1} - " + args[i] + "\n");
                    }
                    System.Console.WriteLine("0 - EXIT");
                    System.Console.Write("\nEnter your move: ");
                    string select = Console.ReadLine();

                    if (int.TryParse(select, out idPlayerMove))
                    {
                        if (idPlayerMove > 0 && idPlayerMove <= args.Length)
                        {
                            System.Console.WriteLine($"\nYour move: {args[idPlayerMove - 1]}");
                            System.Console.WriteLine("\n" + computerMove);
                            CheckWinner(idComputerMove, idPlayerMove - 1, args);
                            System.Console.WriteLine("\nHMAC key: " + HashEncodeToString(key));
                            break;
                        }
                        else if (idPlayerMove == 0)
                        {
                            break;
                        }
                    }
                }

            }
            else
            {
                System.Console.WriteLine("Неверная передача параметров, пример: \"Камень Ножницы Бумага\" , \"Камень Ножницы Бумага Ящерица Спок!\"");
            }
        }

        static string HMACHASH(string str, string key)
        {
            byte[] bkey = Encoding.Default.GetBytes(key);
            using (var hmac = new HMACSHA256(bkey))
            {
                byte[] bstr = Encoding.Default.GetBytes(str);
                var bhash = hmac.ComputeHash(bstr);
                return BitConverter.ToString(bhash).Replace("-", string.Empty).ToLower();
            }
        }

        private static bool checkDuplicate(string[] data)
        {
            string[] array = new string[data.Length];

            data.CopyTo(array, 0);
            Array.Sort(array);

            for (int i = 1; i < array.Length; i++)
            {
                if (array[i - 1].Equals(array[i]))
                {
                    return false;
                }
            }
            return true;
        }

        private static int GetMoveComputer(int min, int max, byte[] bytes)
        {
            UInt32 scale = BitConverter.ToUInt32(bytes, 0);
            return (int)Math.Round(min + (max - min) * (scale / (uint.MaxValue + 1.0)), 0, MidpointRounding.AwayFromZero);
        }

        private static void CheckWinner(int computer, int player, string[] array)
        {
            string result = "";
            int range = array.Length / 2;
            int indexArray = computer;
            List<int> losers = new List<int>();
            if (array[computer] == array[player])
            {
                result = "\nDraw!";
            }
            else
            {
                for (var i = range; i != 0; i--)
                {
                    indexArray--;
                    if (indexArray < 0)
                    {
                        indexArray = array.Length - 1;
                    }
                    losers.Add(indexArray);
                }
                result = losers.Contains(player) ? "\nYou lose!" : "\nYou win!";
            }
            System.Console.WriteLine(result);
        }

        private static byte[] GeneratorKeys()
        {
            using (var generator = RandomNumberGenerator.Create())
            {
                var salt = new byte[16];
                generator.GetBytes(salt);
                return salt;
            }
        }


        private static string HashEncodeToString(byte[] hash)
        {
            return BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();
        }

    }
}

