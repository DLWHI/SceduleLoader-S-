using SceduleLoader.Core;
using SceduleLoader.SceduleMephi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace SceduleLoader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Worker<List<Lesson>> parser;
        //dictionary to get last part of url and connect it to full one
        readonly Dictionary<string, string> typeConv = new Dictionary<string, string>
        {
            ["Завтра"] = GetTomorrow(DateTime.Now),
            ["Сегодня"] = "day",
            ["Общее"] = "schedule",
            ["Чётная неделя"] = "schedule?period=2",
            ["Нечётная неделя"] = "schedule?period=1",
            ["Сессия"] = "exams",
        };
        //last func is the answer why i have list of russian days of week
        readonly List<string> RussianWeek = new List<string>() { "Понедельник", "Вторник", "Среда", "Четверг", "Пятница", "Суббота", "Воскресенье", };

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            parser = new Worker<List<Lesson>>(new MephiParser(), new MephiSets());
            DayWeek.SelectedItem = tmr;
        }//on window loading
        private void DayWeek_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            (parser.Sets as MephiSets).Sced = typeConv[((sender as ComboBox).SelectedItem as ComboBoxItem).Content.ToString()];
            ScedOut();
        }//after we changed type of sced we need
        private void GroupField_TextChanged(object sender, TextChangedEventArgs e)
        {
            (parser.Sets as MephiSets).GroupId = (sender as TextBox).Text.Trim();
            ScedOut();
        }//after we change group field
        private void sced_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            e.Cancel = true;
        }//to pervent datagrid  edit permission exception 

        private static string GetTomorrow(DateTime today)
        {
            string res = "day?date=";
            today = today.AddDays(1);
            res += today.Year.ToString() + "-" + today.Month.ToString() + "-" + today.Day.ToString();
            return res;
        }//we are gettin special url for tommorow date to save codelines
        private async void ScedOut()
        {
            List<Lesson> list = await parser.Work(new MephiLoader(parser.Sets as MephiSets));
            sced.Items.Clear();
            sced.AutoGenerateColumns = false;
            if (parser.ErrorMes != "Ошибок нет збс")//error treating
                sced.Items.Add(new Lesson(parser.ErrorMes));
            else//and output of our day in another func
                DayOut(list);
        }//main func of scedule output

        private void DayOut(List<Lesson> list)
        {
            if (DayWeek.SelectedItem == tmr || DayWeek.SelectedItem == tdy)//if user wants today's/tommorow's schedule we just give him day week name and output sced
            {
                if (DayWeek.SelectedItem == tmr)//if its tommorow's scedule we get name of tommorow's week day
                    sced.Items.Add(new Lesson(GetRussianWeek(DateTime.Now.AddDays(1).DayOfWeek)));
                else
                    sced.Items.Add(new Lesson(GetRussianWeek(DateTime.Now.DayOfWeek)));
                if (list.Count == 0)//if user does not have any lessons we inform him bout dat 
                    sced.Items.Add(new Lesson("Нет занятий"));
                foreach (Lesson lesson in list)//or(and) we just output it
                    sced.Items.Add(lesson);
            }
            else if (DayWeek.SelectedItem == fuck)//if user wants to see his exams
            {
                foreach (Lesson lesson in list)//just output them 'cause they listed by dates not by days of week
                    sced.Items.Add(lesson);
            }
            else//if its other left schedule
            {
                int c = 0;//list index counter
                foreach (string day in RussianWeek)//aaand for each day of week we output its schedule
                {
                    sced.Items.Add(new Lesson(day));
                    if (list.Count == 0) 
                    {
                        sced.Items.Add(new Lesson("Нет занятий"));
                        break;
                    }//if user does not have any lessons we inform him bout dat
                    //We have splitted our lesson by days of week by using num so now we just have to check if num becomes less than it was
                    do
                    {
                        sced.Items.Add(list[c]);
                        c++;
                    } while (c < list.Count && list[c].Num > list[c - 1].Num);
                    if (c >= list.Count)//if we got no lessons left but days are still going on we have to abort
                        break;
                }
            }
        }//outputing scheduly by its type

        private void ChangeCol(string type)
        {
            Brush col = Brushes.White;
            if (type == "Лек")
                col = Brushes.Green;
            else if (type == "Пр")
                col = Brushes.Purple;
            else if (type == "Лаб")
                col = Brushes.Blue;
            else if (type == "Зач")
                col = Brushes.DarkBlue;
            else if (type == "Экз")
                col = Brushes.Red;
            Style style = FindResource("TypeCol") as Style;
            style.Setters.Add(new Setter(TextBlock.ForegroundProperty, col));
        }//unused left for future purposes
        private string GetRussianWeek(DayOfWeek week)
        {
            if (week == DayOfWeek.Monday)
                return RussianWeek[0];
            if (week == DayOfWeek.Tuesday)
                return RussianWeek[1];
            if (week == DayOfWeek.Wednesday)
                return RussianWeek[2];
            if (week == DayOfWeek.Thursday)
                return RussianWeek[3];
            if (week == DayOfWeek.Friday)
                return RussianWeek[4];
            if (week == DayOfWeek.Saturday)
                return RussianWeek[5];
            if (week == DayOfWeek.Sunday)
                return RussianWeek[6];
            return null;
        }//c# inner enum of days of week has english days of week, so we have to get russian ones by ourselves
    }
}
