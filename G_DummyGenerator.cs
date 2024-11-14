using System;
using System.Collections.Generic;
using System.IO;

namespace ByteConverter
{
    internal static class DummyGenerator
    {
        struct ItemData
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

            for (int i = 0; i < 1024 * 1024; i++)
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
            string workspace = Directory.GetCurrentDirectory();
            var path = Path.Combine(workspace, "dummy.csv");
            using (var writer = new StreamWriter(path, false))
            {
                foreach (var item in items)
                {
                    writer.WriteLine(string.Join(",",
                        item.timeStamp, item.dbID, item.userDbId, item.templateId, item.amount));
                }
            }

            System.Diagnostics.Process.Start(workspace);
        }
    }
}
