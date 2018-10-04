using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.IO;

namespace LessonSchelduer2
{
    public partial class Form1 : Form
    {
        List<Lesson> list = new List<Lesson>();
        List<string> listDays = new List<string>() {"Понедельник", "Вторник", "Среда", "Четверг", "Пятница", "Суббота", "Воскресенье" };
        List<string> checkedDays = new List<string>();

        public Form1()
        {
            InitializeComponent();

            dataGridView1.ColumnCount = 7;
            dataGridView1.Columns[0].Name = "Время начала";
            dataGridView1.Columns[1].Name = "Время конца";
            dataGridView1.Columns[2].Name = "Аудитория";
            dataGridView1.Columns[3].Name = "Название предмета";
            dataGridView1.Columns[4].Name = "Преподаватель";
            dataGridView1.Columns[5].Name = "Группа";
            dataGridView1.Columns[6].Name = "День недели";
            dataGridView1.Columns[0].Width = 60;
            dataGridView1.Columns[1].Width = 60;
            dataGridView1.Columns[3].Width = 130;
            dataGridView1.Columns[4].Width = 130;
            dataGridView1.Columns[5].Width = 130;
            dataGridView1.Columns[6].Width = 130;

            saveFileDialog1.Filter = "Text files(*.txt)|*.txt|All files(*.*)|*.*";
            openFileDialog1.Filter = "Text files(*.txt)|*.txt|All files(*.*)|*.*";

            for(int i = 0; i < listDays.Count; i++)
            {
                checkedListBox1.Items.Add(listDays[i]);
            }

            for(int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                checkedListBox1.SetItemChecked(i, true);
            }

            checkedDays.Clear();
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                if (checkedListBox1.GetItemChecked(i))
                {
                    checkedDays.Add(listDays[i].ToString());
                }
            }
            Search();

            trackBar1.Value = 23;
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            int x = dataGridView1.CurrentCellAddress.X;
            int y = dataGridView1.CurrentCellAddress.Y;

            switch (x)
            {
                case 0:
                case 1:
                    {
                        try
                        {
                            string value = dataGridView1.Rows[y].Cells[x].Value.ToString();
                            string dt = Convert.ToDateTime(value).ToShortTimeString();

                            if (dt == "0:00")
                            {
                                throw new FormatException();
                            }
                            dataGridView1.Rows[y].Cells[x].Value = dt;
                        }
                        catch (FormatException)
                        {
                            MessageBox.Show("Введите время в формате чч:мм");
                            dataGridView1.Rows[y].Cells[x].Value = "";
                        }
                        catch (NullReferenceException)
                        {

                        }

                    }
                    break;

                case 2:
                case 3:
                case 4:
                case 5:
                    {
                        try
                        {
                            string str = dataGridView1.Rows[y].Cells[x].Value.ToString();
                            if (str.Length > 20)
                            {
                                MessageBox.Show("Введите значение меньшее 20 символов");
                                dataGridView1.Rows[y].Cells[x].Value = "";
                            }
                        }
                        catch (NullReferenceException)
                        {

                        }
                    }
                    break;
               case 6:
                    {
                        if (!listDays.Contains(dataGridView1.Rows[y].Cells[x].Value.ToString()))
                        {
                            MessageBox.Show("Введите корректное значение");
                            return;
                        }


                    }
                    break;
            }

        }

        private void GetInfoFromFile()
        {
            try
            {
                if (MessageBox.Show("Текущее расписание будет утеряно. Продолжить?", "Продолжить?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    goto label1;
                }
                else
                {
                    return;
                }

                label1:
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string fileName = openFileDialog1.FileName;
                    string fileText = File.ReadAllText(fileName);

                    list.Clear();
                    list = JsonConvert.DeserializeObject<List<Lesson>>(fileText);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Данный формат не поддерживается");
                return;
            }

            dataGridView1.Rows.Clear();

            foreach(var i in list)
            {
                dataGridView1.Rows.Add(i.DTStart.ToShortTimeString(), i.DTFinish.ToShortTimeString(), i.Place, i.LessonName, i.Teacher, i.Group, i.Day);
            }
        }

        private void WriteToFile()
        {
            string str;

            for (int i = 0; i < dataGridView1.RowCount - 1; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    try
                    {
                        dataGridView1.Rows[i].Cells[j].Value.ToString();

                    }
                    catch (NullReferenceException)
                    {
                        MessageBox.Show("Есть пустые ячейки!");
                        return;
                    }
                }
            }

            list.Clear();
            for (int i = 0; i < dataGridView1.RowCount - 1; i++)
            {
                DateTime dtStart = Convert.ToDateTime(dataGridView1.Rows[i].Cells[0].Value);
                DateTime dtFinish = Convert.ToDateTime(dataGridView1.Rows[i].Cells[1].Value);
                string palce = Convert.ToString(dataGridView1.Rows[i].Cells[2].Value);
                string lesson = Convert.ToString(dataGridView1.Rows[i].Cells[3].Value);
                string teacher = Convert.ToString(dataGridView1.Rows[i].Cells[4].Value);
                string group = Convert.ToString(dataGridView1.Rows[i].Cells[5].Value);
                string day = Convert.ToString(dataGridView1.Rows[i].Cells[6].Value);

                list.Add(new Lesson(dtStart, dtFinish, palce, lesson, teacher, group, day));
            }

            str = JsonConvert.SerializeObject(list);

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string fileName = saveFileDialog1.FileName;
               
                File.WriteAllText(saveFileDialog1.FileName, string.Empty);
                File.WriteAllText(saveFileDialog1.FileName, str);
                MessageBox.Show("Файл сохранен");

            }

        }

        private void загрузитьФайлToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WriteToFile();
        }

        private void загрузитьФайлToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            GetInfoFromFile();
        }

        private void Search()
        {
            dataGridView1.Rows.Clear();
            string str = textBoxSerach.Text.ToLower();

            foreach(var i in list)
            {
                if (str == "" || i.Place.ToLower().Contains(str) || i.LessonName.ToLower().Contains(str)
                    || i.Teacher.ToLower().Contains(str) || i.Group.ToLower().Contains(str))
                    if (Convert.ToInt32(i.DTStart.Hour) <= Convert.ToInt32(trackBar1.Value))
                        if(checkedDays.Contains(i.Day))
                {
                    dataGridView1.Rows.Add(i.DTStart.ToShortTimeString(), i.DTFinish.ToShortTimeString(), i.Place, i.LessonName, i.Teacher, i.Group, i.Day);
                }
            
            }

        }

        private void textBoxSerach_TextChanged(object sender, EventArgs e)
        {           
            Search();

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            Search();
            label1.Text = "> " + trackBar1.Value.ToString() + ":00";
         }      

        private string CreateHTMLTable(DataGridView dgv)
        {
            string htmlCode = "";

            htmlCode += "<meta charset= \"utf-8\"> ";
            htmlCode += @"<head></head>";
            htmlCode += "\n <table> \n";

            htmlCode += "<tr>";
            for (int i = 0; i < dataGridView1.ColumnCount; i++)
            {
                htmlCode += "<th>" + dataGridView1.Columns[i].Name + "</th>";
            }
            htmlCode += "</tr>";

            for (int j = 0; j < dataGridView1.RowCount - 1; j++)
            {
                htmlCode += "<tr>";
                for (int i = 0; i < dataGridView1.ColumnCount; i++)
                {
                    htmlCode += "<th>" + dataGridView1.Rows[j].Cells[i].Value.ToString() + "</th> ";
                }
                htmlCode += "</tr> \n";
            }
            htmlCode += "</table> \n </body> \n </html>";
            return htmlCode;

        }

        private void создатьТаблицуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Text files(*.html)|*.html|All files(*.*)|*.*";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string fileName = saveFileDialog1.FileName;

                File.WriteAllText(saveFileDialog1.FileName, string.Empty);
                File.WriteAllText(saveFileDialog1.FileName, CreateHTMLTable(dataGridView1));
                MessageBox.Show("Файл сохранен");

            }

            saveFileDialog1.Filter = "Text files(*.txt)|*.txt|All files(*.*)|*.*";
        }
       
        private void button1_Click(object sender, EventArgs e)
        {
            checkedDays.Clear();
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                if (checkedListBox1.GetItemChecked(i))
                {
                    checkedDays.Add(listDays[i].ToString());
                }
            }
            Search();
        }

        private void buttonShowAll_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();

            trackBar1.Value = 23;
            textBoxSerach.Text = "";
            

            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                checkedListBox1.SetItemChecked(i, true);
            }

            foreach (var i in list)
            {              
                dataGridView1.Rows.Add(i.DTStart.ToShortTimeString(), i.DTFinish.ToShortTimeString(), i.Place, i.LessonName, i.Teacher, i.Group, i.Day);                     
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
