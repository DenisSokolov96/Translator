using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Translator
{
    public partial class Form1 : Form
    {
        /*---------------------------------------------------*/
        public string Str_Write = "";
        public static int Count_str_Read = 0;
        public static string Str_Write_Programm = "";       
        //список в момент выполнения
        public static List<string[]> listRunTime = new List<string[]>();
        //список для хранения ввода
        public static List<string> listRead = new List<string>();
        /*---------------------------------------------------*/

        public Form1()
        {
            InitializeComponent();
            /*******для теста***/
            richTextBox1.Text = System.IO.File.ReadAllText("D:\\Универ\\Компиляторы\\задача_Флавия.txt");
            richTextBox4.AppendText("Файл  <" + "D:\\Универ\\Компиляторы\\задача_Флавия.txt" + ">  загружен.\n");
            /*********///10 3 данные
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void выполнитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Start_Info();            
        }

        private void richTextBox1_KeyDown(object sender, KeyEventArgs e)
        {

            switch (e.KeyCode)
            {
                case Keys.F5: Start_Info(); break;
                case Keys.O: if (e.KeyCode == Keys.O && e.Control) OpenFile(); break;
                case Keys.S: if (e.KeyCode == Keys.S && e.Control) SaveFile(); break;
            }

        }
        
        private void Start_Info()
        {
            Clear();

            richTextBox4.AppendText("Программа запущена на выполнение...\n");            


            int I = richTextBox1.Lines.Length;
            string[] Text = new string[I];

            for (int i = 0; i < richTextBox1.Lines.Length; i++)               
                Text[i] = richTextBox1.Lines[i];


            //считывание ввода
            for (int i = 0; i < richTextBox2.Lines.Length; i++)
                listRead.Add( richTextBox2.Lines[i] );            
                

            //вызов класса для получения токенов (лексический анализ)
            Lexical_Analysis Lexical_Class = new Lexical_Analysis();            

            richTextBox4.AppendText(Lexical_Class.Start_Analysis(Text));
            richTextBox3.AppendText(Str_Write_Programm);
            richTextBox4.AppendText("Программа выполнилась...\n");
        }

        private void сохранитьФайлToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFile();
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        private void OpenFile()
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            string filename = openFileDialog1.FileName;
            richTextBox1.Text = System.IO.File.ReadAllText(filename);
            richTextBox4.AppendText("Файл  <" + filename + ">  загружен.\n");
        }

        private void SaveFile()
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            string filename = saveFileDialog1.FileName;
            System.IO.File.WriteAllText(filename, richTextBox1.Text);
            MessageBox.Show("Файл сохранен");
            richTextBox4.AppendText("Файл сохранен.\n");            
        }

        private void очиститьИнформациюToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox4.Text = "";
        }

        private void Clear()
        {
           Str_Write = "";
           Count_str_Read = 0;
           Str_Write_Programm = "";
           listRunTime.Clear();
           listRead.Clear();
           richTextBox3.Clear();
            //richTextBox2.Clear();
        }

    }
}
