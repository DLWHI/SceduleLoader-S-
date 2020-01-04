using SceduleLoader.Core;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace SceduleLoader.SceduleMephi
{
    class Configurator : IConf
    {
        Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        public List<string> GetSchedule(string dayOfWeek)
        {
            throw new NotImplementedException();
        }

        public string Read(string data)
        {
            ConfigurationManager.RefreshSection("appSettings");
            foreach (KeyValueConfigurationElement info in config.AppSettings.Settings)
                if (info.Key == data)
                    return info.Value;
            return "Нет таких данных";
        }

        public void Save(string Key, string Value)
        {
            config.AppSettings.Settings[Key].Value = Value;
            config.Save();
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}
