namespace SceduleLoader.SceduleMephi
{
    class Lesson
    {
        public bool IsMerged { get; set; } = false;//flag to know should we merge row or not
        public int Num { get; set; }//num of lesson in schedule, we have to count
        public string Time { get; set; } = "";
        public string Week { get; set; } = "";
        public string Place { get; set; } = "";
        public string Type { get; set; } = "";
        public string Name { get; set; } = "";
        public string Teacher { get; set; } = "";

        public Lesson()
        {
            return;
        }
        public Lesson(string name)
        {
            IsMerged = true;
            Name = name;
        }
    }
}
