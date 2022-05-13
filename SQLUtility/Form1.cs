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

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private System.Windows.Forms.OpenFileDialog openFileDialog1;

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void lblChoseTheFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();

            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                string folderName = folderBrowserDialog1.SelectedPath;
                var fileNameList = System.IO.Directory.GetFiles(folderName, "*.sql");
                var outputFile = $"{folderName}\\output.sql";

                //FileStream fs = File.Create($"{folderName}\\output.sql");
                //fs.Close();
                File.Delete(outputFile);

                if (fileNameList != null && fileNameList.Length > 0)
                {
                    for (int i = 0; i < fileNameList.Length; i++)
                    {

                        string readText = File.ReadAllText((fileNameList[i]));

                        //if (i == 0)
                        //    File.WriteAllText(($"{folderName}\\output.sql", $"{readText} {Environment.NewLine}");
                        //else
                            File.AppendAllText(outputFile, $"{readText} {Environment.NewLine}");


                    }

                    DialogResult msgResult = MessageBox.Show($"Result: {outputFile} file created successfully...", "Result", MessageBoxButtons.OK);

                    if (msgResult == DialogResult.OK)
                        Application.Exit();
                }
                else
                {
                    DialogResult msgResult = MessageBox.Show($"Result: Empty {outputFile} file created...", "Result", MessageBoxButtons.OK);

                    if (msgResult == DialogResult.OK)
                        Application.Exit();

                }
            }
        }

    }
}
