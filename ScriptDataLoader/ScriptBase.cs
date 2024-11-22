using ByteConverter;
using System.Collections.Generic;
using System.IO;

namespace ScriptDataLoader
{
    public abstract class ScriptBase<TScriptData, TData> 
        where TScriptData : ScriptDataBase<TData>, new()
        where TData : struct 
    {
        protected Dictionary<int, TScriptData> map = new Dictionary<int, TScriptData>();

        public TScriptData Get(int key)
        {
            return map[key];
        }

        public unsafe void LoadScript(string filePath)
        {
            map.Clear();

            using (var fileStream = File.OpenRead(filePath))
            using (var reader = new BinaryReader(fileStream))
            {
                //컬럼 개수
                int columnCount = reader.ReadInt32();
                Define.ColumnType[] columnTypes = new Define.ColumnType[columnCount];
                for (int i = 0; i < columnCount; i++)
                    //각 컬럼 타입들 
                    columnTypes[i] = (Define.ColumnType)reader.ReadByte();

                int row = 0;
                while (fileStream.Position < fileStream.Length)
                {
                    TData data = new TData();
                    TData* pData = &data;
                    byte* dataAddress = (byte*)pData;
                    for (int i = 0; i < columnCount; i++)
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
                    }
                    row++;

                    TScriptData scriptData = new TScriptData();
                    scriptData.Data = data;

                    map.Add(scriptData.GetKey(), scriptData);
                }
            }
        }
    }
}
