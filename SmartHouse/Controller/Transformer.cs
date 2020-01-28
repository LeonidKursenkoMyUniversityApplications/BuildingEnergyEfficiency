using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SmartHouse.Controller
{
    public class Transformer
    {
        public static double ToDouble(string s)
        {
            try
            {
                return double.Parse(s);
            }
            catch (Exception e)
            {
                MessageBox.Show("Недопустимі значення параметру: " + s);
                throw e;
            }
        }

        public static int ToInt(string s)
        {
            try
            {
                return int.Parse(s);
            }
            catch (Exception e)
            {
                MessageBox.Show("Недопустимі значення параметру: " + s);
                throw e;
            }
        }
    }
}
