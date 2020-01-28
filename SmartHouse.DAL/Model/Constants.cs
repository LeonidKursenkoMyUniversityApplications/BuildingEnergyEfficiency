using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHouse.DAL.Model
{
    public class Constants
    {
        public static List<string> DayOfWeek = new List<string>
        {
            "Понеділок",
            "Вівторок",
            "Середа",
            "Четвер",
            "П'ятниця",
            "Субота",
            "Неділя"
        };

        public static List<string> Monthes = new List<string>
        {
            "січень", "лютий", "березень", "квітень", "травень", "червень", "липень", "серпень",
            "вересень", "жовтень", "листопад", "грудень" , "всі"
        };

        public static List<string> RatesOfConsumption = new List<string>
        {
            "Обсяги споживання (Wспож), кВт*год",
            "Пікове (Pпік.) навантаження, кВт",
            "Середнє (Pсер.) навантаження, кВт",
            "Тривалість використання максимального навантаження (Тmax),год",
            "Ступінь нерівномірності ГЕН",
            "Коефіцієнт використання встановленої потужності (kвик.)"
        };

        public static List<string> RatesOfConsumptionProps = new List<string>
        {
            "Consumption",
            "PowerMax",
            "PowerAverage",
            "DurationOfMaxPower",
            "DegreeOfUnevenness",
            "RateOfUsePower"
        };

        public static string OutsideWall = "Зовнішня";

        public static string OutsideWalls = "Зовнішні стіни";
        public static string InsideWalls = "Внутрішні стіни";
        public static string Floor = "Перекриття між поверхами";
        public static string RoofGround = "Горищне перекриття";
        public static string Ground = "Підлога";
        public static string Roof = "Дах";
        public static string Window = "Вікна";
        
    }
}
