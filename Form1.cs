using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ByteConverter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void OnClickSerializeButton(object sender, EventArgs e)
        {
            SerializeFiles(filePathText.Text);
        }

        private void SerializeFiles(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath))
                return;

           string[] filePaths = Directory.GetFiles(folderPath);

            foreach(string filePath in filePaths)
            {
                string extension = System.IO.Path.GetExtension(filePath);
                if (string.Compare(extension, ".csv") != 0)
                    continue;

                Console.WriteLine(filePath);
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] values = line.Split(',');
                        foreach (string value in values)
                        {
                            Console.Write($"{value}\t");
                        }
                        Console.WriteLine();
                    }
                }
                
            }
        }

        private void DeserializeFiles(string folderPath)
        {
            
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
