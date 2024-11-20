using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

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

            for (int i = 0; i < 10; i++)
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

            byte[] columnTypes =
            {
                (byte)Define.ColumnType.Int32,
                (byte)Define.ColumnType.Int64,
                (byte)Define.ColumnType.Int64,
                (byte)Define.ColumnType.Int32,
                (byte)Define.ColumnType.Int32,
            };

            var path = Path.Combine(workspace, "dummy.csv");
            using (var writer = new StreamWriter(path, false))
            {
                writer.WriteLine(string.Join(",", columnTypes));
                foreach(var item in items)
                {
                    writer.WriteLine(string.Join(",",
                        item.timeStamp, item.dbID, item.userDbId, item.templateId, item.amount));
                }
            }

            GenerateBin(columnTypes, items);
            PrintBinFile<ItemData>();
            System.Diagnostics.Process.Start(workspace);
        }

        public static unsafe void GenerateBin<T>(byte[] columnTypes, List<T> items) where T : struct
        {
            var binPath = Path.Combine(workspace, "dummy.bin");
            using (var fileStream = new FileStream(binPath, FileMode.Create, FileAccess.Write, FileShare.None))
            using (var writer = new BinaryWriter(fileStream))
            {
                //컬럼 개수 저장(int)
                writer.Write(columnTypes.Length);

                //컬럼 별 자료형 저장
                for (int i = 0; i < columnTypes.Length; i++)
                    writer.Write(columnTypes[i]);

                foreach (var item in items)
                {
                    T* dataAddress = &item;
                    int offset = 0;
                    for (int i = 0; i < columnTypes.Length; i++)
                    {
                        if (columnTypes[i] == (byte)Define.ColumnType.Int32)
                        {
                            writer.Write(*(int*)(dataAddress + offset));
                            offset += 4;
                        }
                        else if (columnTypes[i] == (byte)Define.ColumnType.Int64)
                        {
                            writer.Write(*(long*)(dataAddress + offset));
                            offset += 8;
                        }
                    }
                }
            }
        }

        public static unsafe void PrintBinFile<T>() where T : struct
        {
            Console.WriteLine("TimeStamp\tDbId\tUserDbId\tTemplateId\tAmount");

            List<T> list = new List<T>();
            var binPath = Path.Combine(workspace, "dummy.bin");
            

            using (var fileStream = File.OpenRead(binPath))
            using (var reader = new BinaryReader(fileStream))
            {
                //컬럼 개수
                int columnCount = reader.ReadInt32();
                Define.ColumnType[] columnTypes = new Define.ColumnType[columnCount];
                for (int i = 0; i < columnCount; i++)
                    //각 컬럼 타입들 
                    columnTypes[i] = (Define.ColumnType)reader.ReadByte();

                int row = 0;
                FieldInfo[] fields = typeof(T).GetFields();
                while (fileStream.Position < fileStream.Length)
                {
                    T data = new T();
                    T* pData = &data;
                    byte* dataAddress = (byte*)pData;
                    for (int i=0; i< columnCount; i++)
                    {
                        if (columnTypes[i] == Define.ColumnType.Int32)
                        {
                            *(int*)(dataAddress) = reader.ReadInt32();
                            dataAddress += 4;
                        } 
                        else if (columnTypes[i] == Define.ColumnType.Int64)
                        {
                            *(long*)(dataAddress) = reader.ReadInt64();
                            dataAddress += 8;
                        }
                        row++;
                    }
                    list.Add(data);
                }
            }
        }
    }
}
