using System.Collections.Generic;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using SceduleLoader.Core;

namespace SceduleLoader.SceduleMephi
{
    //hmlt parser to get schedule
    class MephiParser : IParser<List<Lesson>>
    {
        public List<Lesson> Parse(IHtmlDocument doc)
        {
            List<Lesson> result = new List<Lesson>();
            var items = doc.QuerySelectorAll("div");//selecting all div and filtering ones that contains lesson info
            Lesson lesson = new Lesson();
            int n;
            foreach (var item in items)
            {
                if(item.ClassName != null && item.ClassName == "list-group")
                {
                    n = 1;
                    foreach(var child in item.Children)
                    {
                        lesson = new Lesson();
                        if (child.ClassName != null && child.ClassName.Contains("list-group-item"))//any lesson in page contains this in his class name
                        {
                            var tmp = child.QuerySelectorAll("span");//week period info is being held in span tag
                            foreach (var t in tmp)
                                if (t.ClassName != null && t.ClassName.Contains("lesson-square lesson-square-"))//so we search it there
                                    lesson.Week += t.GetAttribute("Title") + "\n";
                            var nodes = child.GetNodes<IText>();//all other info is being held in div tags, but name of lesson is being stuck in bunch of other useless info
                                                               //so we observe text nodes of block
                            foreach (var node in nodes)//and there data science begins
                            {
                                //each time has special symbols that we check in func and ':' char
                                if (node.Text != null && (node.Text.Contains(':') || IsDate(node.Text)))
                                    lesson.Time += node.Text + "\n";
                                //each place also has special symbols and three straight coming digits and its parent is anchor 
                                else if (node.Text != null && (node.Text.Contains("каф.") || IsPlace(node.Text)) && node.Parent.NodeName == "A")
                                    lesson.Place += node.Text + "\n";
                                //each lesson type can be 2 or 3 chars long and does not contain '\n' char
                                else if (node.Text != null && (node.Text.Length == 3 || node.Text.Length == 2) && !node.Text.Contains("\n"))
                                    lesson.Type += node.Text + "\n";
                                //each teacher name has two dots and a char beetwen them so we check it
                                else if (node.Text != null && IsTeacher(node.Text))
                                    lesson.Teacher += node.Text + "\n";
                                //and each name of lesson at least 6 chars long but i'm not big sure about that
                                else if (node.Text != null && node.Text.Length > 6)
                                    lesson.Name += ReplaceToSpace(node.Text + " \n");
                            }
                            lesson.Name = MakeNormalString(lesson.Name + "\n");
                            lesson.Type = MakeNormalString(lesson.Type);
                            lesson.Week = MakeNormalString(lesson.Week);
                            lesson.Teacher = MakeNormalString(lesson.Teacher);
                            lesson.Time = MakeNormalString(lesson.Time);
                            lesson.Place = MakeNormalString(lesson.Place);
                            lesson.Num = n;
                            result.Add(lesson);
                            n++;
                        }
                    }
                }
            }
            return result;
        }

        private bool IsDate(string text)
        {
            if (text.Contains("Пн,"))
                return true;
            if (text.Contains("Вт,"))
                return true;
            if (text.Contains("Ср,"))
                return true;
            if (text.Contains("Чт,"))
                return true;
            if (text.Contains("Пт,"))
                return true;
            if (text.Contains("Сб,"))
                return true;
            if (text.Contains("Вс,"))
                return true;
            return false;
        }

        //-why you check nodes for info not div blocks where is all main info is being held?
        //i do it to make less unused lists for div/span/node/anchor tags
        //so yes, its just basically easier to make some data science
        private bool IsTeacher(string text)
        {
            for (int i = 0; i + 2 < text.Length; i++)
                if (text[i] == '.' && text[i + 2] == '.')
                    return true;
            return false;
        }//teacher name chenck
        private bool IsPlace(string str)
        {
            int c = 0;
            foreach (char ch in str)
            {
                if (char.IsDigit(ch))
                    c++;
                else
                    c = 0;
            }
            if (c >= 3)
                return true;
            foreach (char ch in str)
            {
                if (ch >= 'А' && ch <= 'Я')
                    c++;
                else
                    return false;
                if (c == 3)
                    return true;
            }
            return false;
        }//place check
        private string MakeNormalString(string v)
        {
            string res = "";
            for (int i = 0; i < v.Length - 1; i++)
            {
                if (v[i] != '\n')
                    res += v[i];
                else if (v[i + 1] != '\n')
                    res += v[i];
            }
            return res.Trim();
        }//making string look normal just by deleting extra '\n'
        private string ReplaceToSpace(string v)
        {
            string res = "";
            for (int i = 0; i + 1 < v.Length - 1; i++)
            {
                if (v[i] == '\n' && v[i + 1] != '\n')
                    res += ' ';
                else if (v[i] != '\n')
                    res += v[i];
            }
            res = res.Trim();
            //and more data science
            //there is also can be date period in name of lesson that we should display. And this date period doesn't have '\n' char at the end of it.
            //so we have to put it there manually
            if (res[res.Length - 1] == ')')
                res += '\n';
            else if (res[0] >= 'А' && res[0] <= 'Я')//but if its just name of it, we've got to add '\n' to start, if it wouldn't be neccesary it will be deleted
                res = '\n' + res;
            return res;
            //some commentaries about this block
            //in fact, i don't know exactly why we should add extra '\n' chars, i was to lazy to find out that
            //so i just "randomly" added '\n' to string, and if it is not neccesary it will be deleted
            //blame the guy who wrote frontend to this fucking site
        }//replacing all '\n' to space
    }
}
