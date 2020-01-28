using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHouse.DAL.Model
{
    [Serializable]
    public class Room
    {
        public string Name { set; get; }
        public List<Wall> Walls { set; get; }
        public double Area { set; get; }
        public double WindowsArea { set; get; }
        // Temperature of room for individual scheme of heat.
        public double Temperature { set; get; }

        public double TotalArea
        {
            get
            {
                return Walls.Sum(x => x.Area) + WindowsArea + 2 * Area;
            }
        }

        public Room(string name, List<Wall> walls, double area, double windowsArea, double temperature)
        {
            Name = name;
            Walls = walls;
            Area = area;
            WindowsArea = windowsArea;
            Temperature = temperature;
        }

        public Room(string name, List<Wall> walls, double area, double windowsArea)
        {
            Name = name;
            Walls = walls;
            Area = area;
            WindowsArea = windowsArea;
        }

        public Room(string name, double area, double windowsArea)
        {
            Name = name;
            Walls = new List<Wall>();
            Area = area;
            WindowsArea = windowsArea;
        }
        
    }
}
