using BinDataLoader;
using System;
using System.IO;
using System.Windows.Forms;
using static ByteConverter.Define;

namespace ByteConverter
{
    public partial class Form1 : Form
    {
        static string workspace => Directory.GetCurrentDirectory();

        public Form1()
        {
            InitializeComponent();
            Console.WriteLine(Environment.Is64BitOperatingSystem);
        }

        private void OnClickSerializeButton(object sender, EventArgs e)
        {
            SerializeFiles(filePathText.Text);
            //ItemScript itemScript = new ItemScript();
            CSVDataLoader.ItemScript itemScript = new CSVDataLoader.ItemScript();
            itemScript.LoadScript(Path.Combine(workspace, "dummy.csv"));

            for(int i = 0; i< 20; i++)
            {
                var data = itemScript.Get(i + 1);
                Console.WriteLine($"{data.timeStamp}\t{data.dbID}\t{data.userDbId}\t{data.templateId}\t{data.amount}");
            }
        }

        private void SerializeFiles(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath))
                return;

           string[] filePaths = Directory.GetFiles(folderPath);

            foreach(string csvFilePath in filePaths)
            {
                string extension = System.IO.Path.GetExtension(csvFilePath);
                if (string.Compare(extension, ".csv") != 0)
                    continue;

                var binPath = Path.ChangeExtension(csvFilePath, ".bin");
                using (var reader = new StreamReader(csvFilePath))
                using (var fileStream = new FileStream(binPath, FileMode.Create, FileAccess.Write, FileShare.None))
                using (var writer = new BinaryWriter(fileStream))
                {
                    string line;
                    string[] columnInfo = reader.ReadLine().Split(',');

                    int columnCount = int.Parse(columnInfo[0]);
                    Define.ColumnType[] columnTypes = new Define.ColumnType[columnCount];
                    for (int i = 0; i < columnCount; i++)
                        columnTypes[i] = (Define.ColumnType)byte.Parse(columnInfo[i + 1]);

                    //컬럼 개수 저장(int)
                    writer.Write(columnTypes.Length);

                    //컬럼 별 자료형 저장
                    for (int i = 0; i < columnTypes.Length; i++)
                        writer.Write((byte)columnTypes[i]);

                    while((line = reader.ReadLine()) != null)
                    {
                        string[] values = line.Split(',');
                        int columnIndex = 0;
                        foreach (string value in values)
                        {
                            if (columnTypes[columnIndex] == ColumnType.Int32)
                                writer.Write(int.Parse(value));
                            else if (columnTypes[columnIndex] == ColumnType.Int64)
                                writer.Write(long.Parse(value));

                            columnIndex++;
                        }
                    }
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void OnClickCreateDummy(object sender, EventArgs e)
        {
            DummyGenerator.Create();
        }
    }
}
