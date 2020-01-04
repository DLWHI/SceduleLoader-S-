namespace SceduleLoader.Core
{
    interface IConf
    {
        string Read(string data);
        void Save(string Key, string Value);
    }
}
