using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHouse.DAL.Model
{
    [Serializable]
    public class Floor
    {
        public string Name { set; get; }
        public List<Room> Rooms { set; get; }
        
        public Floor(string name, List<Room> rooms)
        {
            Name = name;
            Rooms = rooms;
        }

        public Floor(string name)
        {
            Name = name;
            Rooms = new List<Room>();
        }

        public void AddRoom(Room room)
        {
            if(Rooms.Exists(x => x.Name.Equals(room.Name))) throw new Exception("Порушення унікальності імен");
            Rooms.Add(room);
        }

        public void UpdateRoomName(string newName)
        {
            if (Rooms.Exists(x => x.Name.Equals(newName))) throw new Exception("Порушення унікальності імен");
        }
    }
}
