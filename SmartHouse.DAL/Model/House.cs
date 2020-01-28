using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHouse.BLL.Model;

namespace SmartHouse.DAL.Model
{
    [Serializable]
    public class House
    {
        public List<Floor> Floors { set; get; }

        // Areas calculations.
        public double GroundArea => Floors[0].Rooms.Sum(x => x.Area);

        public double RoofArea => GroundArea * 1.5;

        public double OutsideWallsArea => Floors.SelectMany(f => f.Rooms)
            .Sum(r => r.Walls.Where(w => w.NeirRoom == null)
            .Sum(wall => wall.Area));

        public double WindowsArea => Floors.SelectMany(f => f.Rooms).Sum(r => r.WindowsArea);

        // Temperature of house for common scheme of heat.
        public double Temperature { set; get; }

        public List<HouseThermalParam> HouseParams { set; get; }

        public HydroElectricLoad Hel { set; get; }
        

        public House()
        {
            Floors = new List<Floor>();
            Wall wall = new Wall(0, null);
            Room room = new Room("Кімната 1", new List<Wall>(){wall}, 0, 0 );
            Floors.Add(new Floor("Поверх 1", new List<Room>(){room}));
            Temperature = 0;
            HouseParams = new List<HouseThermalParam>();
            Script();
        }

        public House(List<Floor> floors, double temperature, List<HouseThermalParam> houseParams)
        {
            Floors = floors;
            Temperature = temperature;
            HouseParams = houseParams;
        }

        public void Script()
        {
            Temperature = 20;
            Room r1, r2, r3, r4, r5, r6, r7;
            r1 = new Room("Тамбур", new List<Wall>(), 3.2, 1.75, 18);
            r2 = new Room("Хол", new List<Wall>(), 5.7, 0, 18);
            r3 = new Room("Кухня", new List<Wall>(), 7.5, 1.75, 19);
            r4 = new Room("Гостьова", new List<Wall>(), 21.2, 3.5, 20);
            r5 = new Room("Кімната 1", new List<Wall>(), 10.6, 2.5, 20);
            r6 = new Room("Ванна", new List<Wall>(), 4, 0, 20);
            r7 = new Room("Кімната 2", new List<Wall>(), 9.2, 1.8, 20);

            r1.Walls.Add(new Wall(4.5, null));
            r1.Walls.Add(new Wall(4.5, r2));
            r1.Walls.Add(new Wall(2.5, r7));
            r1.Walls.Add(new Wall(2.5, r3));

            r2.Walls.Add(new Wall(4.5, r1));
            r2.Walls.Add(new Wall(7, r3));
            r2.Walls.Add(new Wall(10.9, r4));
            r2.Walls.Add(new Wall(2.6, r5));
            r2.Walls.Add(new Wall(4.7, r6));
            r2.Walls.Add(new Wall(2, r7));

            r3.Walls.Add(new Wall(14.4, null));
            r3.Walls.Add(new Wall(2.5, r1));
            r3.Walls.Add(new Wall(7, r2));

            r4.Walls.Add(new Wall(18, null));
            r4.Walls.Add(new Wall(10.9, r2));
            r4.Walls.Add(new Wall(7.6, r5));

            r5.Walls.Add(new Wall(17.3, null));
            r5.Walls.Add(new Wall(2.6, r2));
            r5.Walls.Add(new Wall(7.6, r4));
            r5.Walls.Add(new Wall(2.5, r6));

            r6.Walls.Add(new Wall(4.7, null));
            r6.Walls.Add(new Wall(4.7, r2));
            r6.Walls.Add(new Wall(7.6, r5));
            r6.Walls.Add(new Wall(7.6, r7));

            r7.Walls.Add(new Wall(16.1, null));
            r7.Walls.Add(new Wall(2.5, r1));
            r7.Walls.Add(new Wall(2, r2));
            r7.Walls.Add(new Wall(7.6, r6));


            Floors.Clear();
            Floors.Add(new Floor("Поверх 1", new List<Room>
            {
                r1, r2, r3, r4, r5, r6, r7
            }));
            
            r1 = new Room("Хол", new List<Wall>(), 4, 0, 18);
            r2 = new Room("Кімната 3", new List<Wall>(), 9.9, 2.2, 19);
            r3 = new Room("Ванна", new List<Wall>(), 3, 0, 20);
            r4 = new Room("Кімната 4", new List<Wall>(), 9.8, 1.9, 21);
            
            r1.Walls.Add(new Wall(7.2, r2));
            r1.Walls.Add(new Wall(5.8, r3));
            r1.Walls.Add(new Wall(7.2, r4));

            r2.Walls.Add(new Wall(42.2, null));
            r2.Walls.Add(new Wall(7.2, r1));
            r2.Walls.Add(new Wall(6.2, r3));

            r3.Walls.Add(new Wall(42.2, null));
            r3.Walls.Add(new Wall(7.2, r1));
            r3.Walls.Add(new Wall(6.2, r3));

            r4.Walls.Add(new Wall(5.8, null));
            r4.Walls.Add(new Wall(7.2, r3));
            r4.Walls.Add(new Wall(7.2, r4));

            Floors.Add(new Floor("Поверх 2", new List<Room>
            {
                r1, r2, r3, r4
            }));

            HouseParams.Add(new HouseThermalParam(Constants.OutsideWalls, 7.41, 0.13));
            HouseParams.Add(new HouseThermalParam(Constants.InsideWalls, 0.37, 2.74));
            HouseParams.Add(new HouseThermalParam(Constants.Floor, 2.83, 0.35));
            HouseParams.Add(new HouseThermalParam(Constants.RoofGround, 3.66, 0.27));
            HouseParams.Add(new HouseThermalParam(Constants.Ground, 1.06, 0.94));
            HouseParams.Add(new HouseThermalParam(Constants.Roof, 1.45, 0.69));
            HouseParams.Add(new HouseThermalParam(Constants.Window, 0.24, 4.24));

            Hel = new HydroElectricLoad
            {
                NumberOfBathUsers = 1,
                NumberOfShowerUsers = 3,
                TemperatureOfBath = 45,
                TemperatureOfShower = 40,
                TemperatureOfOutput = 60,
                TemperatureOfInput = 5,
                WaterQuantityForBath = 100,
                WaterQuantityForShower = 25,
                Time = 5
            };
        }

        
    }
}
