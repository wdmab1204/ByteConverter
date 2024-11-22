using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace ByteConverter
{
    public struct ItemData
    {
        public int timeStamp;
        public int templateId;
        public long dbID;
        public long userDbId;
        public int amount;
    }

    //데이터 10*1024*1024일 때
    //csv : 약 332mb
    //bin : 약 286mb
    //약 60mb 감소
    internal static class DummyGenerator
    {
        static string workspace => Directory.GetCurrentDirectory();

        public static void Create()
        {
            var items = new List<ItemData>();
            var random = new Random();

            for (int i = 0; i < 20; i++)
            {
                items.Add(new ItemData
                {
                    timeStamp = i + 1,
                    dbID = random.Next(10000, 30000),
                    userDbId = random.Next(),
                    templateId = random.Next(1000, 4000),
                    amount = random.Next(1, 50)
                });
            }

            items.Sort((left, right) => left.timeStamp - right.timeStamp);

            byte[] columnTypes = new byte[5]
            {
                (byte)Define.ColumnType.Int32,
                (byte)Define.ColumnType.Int32,
                (byte)Define.ColumnType.Int64,
                (byte)Define.ColumnType.Int64,
                (byte)Define.ColumnType.Int32,
            };

            var path = Path.Combine(workspace, "dummy.csv");
            using (var writer = new StreamWriter(path, false))
            {
                writer.Write($"{columnTypes.Length},");
                writer.WriteLine(string.Join(",", columnTypes));
                foreach(var item in items)
                {
                    writer.WriteLine(string.Join(",",
                        item.timeStamp, item.dbID, item.userDbId, item.templateId, item.amount));
                }
            }

            //GenerateBin(columnTypes, items);
            //PrintBinFile<ItemData>();
            System.Diagnostics.Process.Start(workspace);
        }
    }
}
