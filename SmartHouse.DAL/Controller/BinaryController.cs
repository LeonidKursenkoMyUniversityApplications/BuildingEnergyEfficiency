using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using SmartHouse.DAL.Model;

namespace SmartHouse.DAL.Controller
{
    public class BinaryController
    {
        public static void WriteDataToBinary<T>(string file, List<T> dataList)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream(file + ".dat", FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, dataList);
                fs.Close();
            }
        }

        public static List<T> ReadDataFromBinary<T>(string file)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                using (FileStream fs = new FileStream(file + ".dat", FileMode.Open))
                {
                    var dataList = (List<T>)formatter.Deserialize(fs);
                    fs.Close();
                    return dataList;
                }
            }
            catch
            {
                return new List<T>();
            }
            
        }

        public static List<List<Weather>> ReadAllWeatherFromBinary(List<string> files)
        {
            List<List<Weather>> weathersList = new List<List<Weather>>();
            foreach(var file in files)
            {
                weathersList.Add(ReadDataFromBinary<Weather>(file));
            }
            return weathersList;
        }

        public static void WriteAllWeatherToBinary(List<string> files, List<List<Weather>> weathersList)
        {
            for (int i = 1; i <= 12; i++)
            {
                WriteDataToBinary(files[i - 1], weathersList[i - 1]);
            }
        }
    }
}
