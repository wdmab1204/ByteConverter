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

            for (int i = 0; i < 10 * 1024 * 1024; i++)
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

            var path = Path.Combine(workspace, "dummy.csv");
            using (var writer = new StreamWriter(path, false))
            {
                foreach(var item in items)
                {
                    writer.WriteLine(string.Join(",",
                        item.timeStamp, item.dbID, item.userDbId, item.templateId, item.amount));
                }
            }

            System.Diagnostics.Process.Start(workspace);
        }
    }
}
