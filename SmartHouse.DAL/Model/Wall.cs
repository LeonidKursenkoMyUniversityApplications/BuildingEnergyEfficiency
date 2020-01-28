using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHouse.DAL.Model
{
    [Serializable]
    public class Wall
    {
        // In meters m^2
        public double Area { set; get; }
        public Room NeirRoom { set; get; }

        public string GetWallType
        {
            get { return NeirRoom == null ? Constants.OutsideWall : "Внутрішня з " + NeirRoom.Name; }
        }

        public Wall(double area, Room neirRoom)
        {
            Area = area;
            NeirRoom = neirRoom;
        }

        public Wall() : this(0, null)
        {
            
        }
    }
}
