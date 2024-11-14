using System;
using System.Collections.Generic;
using System.IO;

namespace ByteConverter
{
    internal static class DummyGenerator
    {
        static string workspace => Directory.GetCurrentDirectory();

        public struct ItemData
        {
            public int timeStamp;
            public long dbID;
            public long userDbId;
            public int templateId;
            public int amount;
        }

        public static void Create()
        {
            var items = new List<ItemData>();
            var random = new Random();

            for (int i = 0; i < 100; i++)
            {
                items.Add(new ItemData
                {
                    timeStamp = random.Next(1404715000, 1404716000),
                    dbID = random.Next(10000, 30000),
                    userDbId = random.Next(),
                    templateId = random.Next(1000, 4000),
                    amount = random.Next(1, 50)
                });
            }

            items.Sort((left, right) => left.timeStamp - right.timeStamp);
            var path = Path.Combine(workspace, "dummy.csv");
            using (var writer = new StreamWriter(path, false))
            {
                foreach(var item in items)
                {
                    writer.WriteLine(string.Join(",",
                        item.timeStamp, item.dbID, item.userDbId, item.templateId, item.amount));
                }
            }

            GenerateBin(items);
            System.Diagnostics.Process.Start(workspace);
        }

        public static void GenerateBin(List<ItemData> items)
        {
            var binPath = Path.Combine(workspace, "dummy.bin");
            using (var fileStream = new FileStream(binPath, FileMode.Create, FileAccess.Write, FileShare.None))
            using (var writer = new BinaryWriter(fileStream))
            {
                foreach (var item in items)
                {
                    writer.Write(item.timeStamp);
                    writer.Write(item.dbID);
                    writer.Write(item.userDbId);
                    writer.Write(item.templateId);
                    writer.Write(item.amount);
                }
            }

            PrintBinFile();
        }

        public static void PrintBinFile()
        {
            Console.WriteLine("TimeStamp\tDbId\tUserDbId\tTemplateId\tAmount");

            var binPath = Path.Combine(workspace, "dummy.bin");
            using (var fileStream = File.OpenRead(binPath))
            using (var reader = new BinaryReader(fileStream))
            {
                while (fileStream.Position < fileStream.Length)
                {
                    Console.Write       ($"{reader.ReadInt32()}\t");
                    Console.Write       ($"{reader.ReadInt64()}\t");
                    Console.Write       ($"{reader.ReadInt64()}\t");
                    Console.Write       ($"{reader.ReadInt32()}\t");
                    Console.WriteLine   ($"{reader.ReadInt32()}\t");
                }
            }
        }
    }
}
