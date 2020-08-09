using System;
using System.Collections.Generic;
using System.IO;

namespace Temperature_Controller.Model
{
    public class DataListView
    {
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public string Time { get; set; }

        public static List<DataListView> GetHistory()
        {
            List<DataListView> listData = new List<DataListView>();
            string[] content = {};
            try
            {
                content = File.ReadAllLines("data.txt");
            }
            catch (Exception e)
            {
                //ignored
            }
            for (int i = 0; i < content.Length; i++)
            {
                try
                {
                    if (content[i].Contains("|"))
                    {
                        string[] data = content[i].Split('|');
                        listData.Add(new DataListView()
                        {
                            Temperature = Convert.ToDouble(data[0]), Humidity = Convert.ToDouble(data[1]),
                            Time = data[2]
                        });
                    }
                }
                catch
                {
                    //ignored
                }
            }
            return listData;
        }
    }
}
