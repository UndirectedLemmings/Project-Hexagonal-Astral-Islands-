using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using FluentNHibernate.Mapping;
using FluentNHibernate;
using NHibernate;

namespace Project_Hexagonal_Astral_Islands
{
    public class Program
    {
        public static void Main(string[] args)
        {
            
            var startTimeSpan = TimeSpan.Zero;
            var periodTimeSpan = TimeSpan.FromSeconds(Constants.MapUpdateInterval);

            var timer = new System.Threading.Timer(
                e =>
                {
                    DateTime start = DateTime.Now;
                    using (ISession session = NHibernateHelper.OpenSession())
                    {
                        
                            IQueryable<Map> maps = session.Query<Map>();
                            Console.WriteLine("Map updating started");
                            foreach (Map m in maps)
                            {
                                Console.WriteLine($"Updating map {m.ID}...");
                                m.UpdateAll(session);
                            ImageGen.GenerateImage(m);
                                session.Update(m);
                            }
                          

                    }
                    TimeSpan diff = DateTime.Now - start;
                    Console.WriteLine($"=========Maps updated in {diff.TotalSeconds} seconds========");
                },
                null,
                startTimeSpan,
                periodTimeSpan
                );

            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }

    public static class Constants
    {

        public static readonly int MapUpdateInterval = 15;

        public static readonly int TemperatureDifferenceDivider = 5;
        public static readonly int HeightDifferenceDivider = 20;
        public static readonly int WaterDifferenceDivider = 10;
        public static readonly int PlantlifeDifferenceDivider = 5;

        public static readonly int RandomTemperatureUpperBound = 70;
        public static readonly int RandomTemperatureLowerBound = -40;

        public static readonly int RandomHeightUpperBound = 100;
        public static readonly int RandomHeightLowerBound = -50;

        public static readonly int RandomWaterUpperBound = 100;
        public static readonly int RandomWaterLowerBound = 0;

        public static readonly int RandomPlantlifeUpperBound = 100;
        public static readonly int RandomPlantlifeLowerBound = 0;

        public static readonly int HotTBound = 33;
        public static readonly int ColdTBound = -15;

        public static readonly int MountainsHBound = 60;
        public static readonly int OceanHBound = -25;

        public static readonly int OceanWBound = 70;

        public static readonly int ForestPlBound = 60;
        public static readonly int DesertPlBound = 30;


        public static readonly int MediumCivBound = 50;
        public static readonly int BigCivBound = 100;

        public static IReadOnlyList<int> DungeonsTier0 = new List<int>(new[]{0,1});

        public static readonly int DungeonGenerationPercent = 3;
        public static readonly int SendColonisatorsPercent = 75; //turn down later
    }

    public class LandProperties
    {
        private long  iD;
        private Hex my_hex;
        private int temperature;
        private int height;
        private int water;
        private int plantlife;

        public virtual long ID { get => iD; set => iD = value; }
        public virtual Hex My_hex { get => my_hex; set => my_hex = value; }
        public virtual int Temperature { get => temperature; set => temperature = value; }
        public virtual int Height { get => height; set => height = value; }
        public virtual int Water { get => water; set => water = value; }
        public virtual int Plantlife { get => plantlife; set => plantlife = value; }

        public LandProperties() {
            iD = 0;
            my_hex = null;
            temperature = 0;
            height = 0;
            water = 0;
            plantlife = 0;
        }

        public LandProperties(int temp = 0, int h = 0, int water = 0, int pl = 0, Hex hex = null) : this()

        {
            My_hex = hex;
            Temperature = temp;
            Height = h;
            Water = water;
            Plantlife = pl;
        }

        public override string ToString() {
            return $"Tempeature: {Temperature}\nHeight: {Height}\nWater: {Water}\nPlantlife: {Plantlife}";
        }
    }


    /*internal struct LandCategoires
    {
        private TempCat tempCat;
        private HeightCat heightCat;
        private WaterCat waterCat;
        private PlantCat plantCat;

    }
     public enum LandType {
         Void,
         HighMountains,
         LowMountains,
         TundraHills,
         TaigaHills,
         Hills,
         StepHills,
         JungleHills,
         SwampHills,
         DesertHills,
         Tundra,
         Plain,
         Step,
         Jungle,
         Swamp,
         FrigidSea,
         Sea,
         TropicalSea
     }
     

    public enum HeightCat
    {
        HighMountains,
        LowMountains,
        Hills,
        Plains,
        Lowlands,
        Sealevel
    }

    public enum TempCat
    {
        Freezing,
        Cold,
        Temperate,
        Warm,
        Scorching
    }


    public enum WaterCat
    {
        Flooded,
        Lots,
        Normal,
        Dry,
        VeryDry
    }

    public enum PlantCat
    {
        HeavilyForested,
        Forests,
        GrassOnly,
        None
    }
    */

    public class Hex
    {
        private long iD;
        private Map _map;
        private LandProperties land_properties;
        //private LandCategoires landCategoires;
        private bool unchangable = true;
        private bool isVoid = true;
        private Coords _coords;

        

        public virtual LandProperties Land_properties { get => land_properties; set => land_properties = value; }
        public virtual long ID { get => iD; set => iD = value; }
        public virtual Map Map { get => _map; set => _map = value; }
        public virtual bool Unchangable { get => unchangable; set => unchangable = value; }
        public virtual bool IsVoid { get => isVoid; set => isVoid = value; }
        public virtual Coords coords { get => _coords; set => _coords = value; }
        public virtual Unit Unit { get; set; }
        public virtual Dungeon Dungeon { get; set; }
        public virtual Settlement Settlement { get; set; }

        public Hex() {
            Land_properties = new LandProperties();
            Land_properties.My_hex = this;
            ID = 0;
            Map = null;
            Unchangable = true;
            IsVoid = true;
            coords = new Coords();
            Unit = null;
            Dungeon = null;
            Settlement = null;
        }

        public static void NudgeParameters(Hex first, Hex second)
        {


            if (first.isVoid || second.isVoid)
            {
                return;
            }
            LandProperties lp = first.Land_properties;
            if (!first.unchangable)
            {
                
                int t = first.Land_properties.Temperature + (second.Land_properties.Temperature - first.Land_properties.Temperature) / Constants.TemperatureDifferenceDivider;
                int h = first.Land_properties.Height + (second.Land_properties.Height - first.Land_properties.Height) / Constants.HeightDifferenceDivider;
                int w = first.Land_properties.Water + (second.Land_properties.Water - first.Land_properties.Water) / Constants.WaterDifferenceDivider;
                int pl = first.Land_properties.Plantlife + (second.Land_properties.Plantlife - first.Land_properties.Plantlife) / Constants.PlantlifeDifferenceDivider;
                first.Land_properties = new LandProperties(t, h, w, pl, first);
            }
            if (!second.unchangable)
            {
                int t = second.Land_properties.Temperature + (lp.Temperature - second.Land_properties.Temperature) / Constants.TemperatureDifferenceDivider;
                int h = second.Land_properties.Height + (lp.Height - second.Land_properties.Height) / Constants.HeightDifferenceDivider;
                int w = second.Land_properties.Water + (lp.Water - second.Land_properties.Water) / Constants.WaterDifferenceDivider;
                int pl = second.Land_properties.Plantlife + (lp.Plantlife - second.Land_properties.Plantlife) / Constants.PlantlifeDifferenceDivider;
                second.Land_properties = new LandProperties(t, h, w, pl, second);
            }


        }
        public virtual void ChangeTemperature(int change)
        {
            land_properties.Temperature += change;
        }
        public virtual void ChangeHeight(int change)
        {
            land_properties.Height += change;
        }
        public virtual void ChangeWater(int change)
        {
            land_properties.Water += change;
        }
        public virtual void ChangePlantlife(int change)
        {
            land_properties.Plantlife += change;
        }
        public virtual void Randomize()
        {
            Random random = new Random();
            int t = random.Next(Constants.RandomTemperatureLowerBound, Constants.RandomTemperatureUpperBound);
            int h = random.Next(Constants.RandomHeightLowerBound, Constants.RandomHeightUpperBound);
            int w = random.Next(Constants.RandomWaterLowerBound, Constants.RandomWaterUpperBound);
            int pl = random.Next(Constants.RandomPlantlifeLowerBound, Constants.RandomPlantlifeUpperBound);
            Land_properties = new LandProperties(t, h, w, pl, this);
            isVoid = false;
        }
        public virtual void Unlock()
        {
            unchangable = false;
        }
        public virtual void Lock()
        {
            unchangable = true;
        }

        public override string ToString() {
            return "Land Properties:\n" + Land_properties.ToString() + $"\nUnchangable:{unchangable}\nisVoid:{isVoid}";
        }

        public virtual Settlement AddStartingSettlement() {
            Settlement = new Settlement();
            Settlement.Team = $"native{Map.ID}";
            Settlement.Population = new Population
            {
                Humans = 35,
                Beastfolk = 0,
                Undead = 0,
                Lawful = 0,
                Chaotic = 0
            };
            Settlement.Housing = 35;
            Settlement.MyHex = this;
            Settlement.ID = 0;
            Lock();
            return Settlement;
        }


        public virtual Unit StartingUnit() {
            Unit unit = Unit.NewFromTypes(1);
            unit.ID = 0;
            unit.Alive = 20;
            unit.Origin = coords;
            unit.Target = coords;
            unit.Intention = Unit.Intent.Settle;
            unit.Carrying = new Ress
            {
                Food = 200,
                Wood = 200,
                Stone = 100,
                Gold = 20,
                Iron = 20
            };
            unit.My_hex = this;
            this.Unit = unit;
            return this.Unit;
        }
        public virtual Dungeon RandomizeDungeon() {
            Dungeon d = null;
            if (Settlement != null)
                return null;
            if (Land_properties.Height < Constants.OceanHBound && Land_properties.Water > Constants.OceanWBound)
            {
                return null;
            }
            Random random = new Random();
            int type = Constants.DungeonsTier0[random.Next(Constants.DungeonsTier0.Count)];
            d = Dungeon.FromTypes(type);
            d.Team = $"monsters{Map.ID}";
            d.My_hex = this;
            Dungeon = d;
            return d;
        }
    }

    public class Coords
    {
        private int x;
        private int y;
        private long iD;
       // private Hex _hex;

        public virtual List<Coords> Adjacent
        {
            get
            {
                List<Coords> list = new List<Coords>();
                list.Append<Coords>(new Coords(X, Y - 1));
                list.Append<Coords>(new Coords(X + 1, Y - 1));
                list.Append<Coords>(new Coords(X + 1, Y));
                list.Append<Coords>(new Coords(X, Y + 1));
                list.Append<Coords>(new Coords(X - 1, Y + 1));
                list.Append<Coords>(new Coords(X - 1, Y));
                return list;
            }
        }
        public static int Distance(Coords from, Coords to) {
            return Math.Abs(to.X-from.X) + Math.Abs(to.Y - from.Y);
        }
        public virtual int X { get => x; set => x = value; }
        public virtual int Y { get => y; set => y = value; }
        public virtual long ID { get => iD; set => iD = value; }
       // public virtual Hex hex { get => _hex; set => _hex = value; }

        public Coords() {
            X = 0;
            Y = 0;
        }
        public Coords(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }
     // override object.Equals
    public override bool Equals(object obj)
        {
            

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Coords c = (Coords)obj;
            return (x == c.x) && (y == c.y);
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            
            return x.GetHashCode()*17+y.GetHashCode()*31;
        }
        public override string ToString() {
            return $"({X},{Y})";
        }
    }

    public enum Domain {
        Life,
        Death,
        Chaos,
        Law
    }

    
    public class Map
    {
        IDictionary<Coords, Hex> hcd;
        private long iD;
        private int tier=0;

        public virtual long ID { get => iD; set => iD = value; }
        public virtual IDictionary<Coords, Hex> Hcd { get => hcd; set => hcd = value; }
        public virtual int Radius { get; set; }
        public virtual Domain Domain { get; set; }
        public virtual int Tier { get => tier; set => tier = value; }

        public virtual int MaxDungeonCount
        {
            get
            {
                return Hcd.Count / 15;
                }
        }

        public virtual List<Settlement> AllSettlements {
            get {
                List<Settlement> settlements = new List<Settlement>();
                foreach (Hex h in Hcd.Values) {
                    if (h.Settlement != null) {
                        settlements.Add(h.Settlement);
                    }
                }
                return settlements;
            }
        }

        public virtual List<Dungeon> AllDungeons {
            get {
                List<Dungeon> dungeons = new List<Dungeon>();
                foreach (Hex h in Hcd.Values) {
                    if (h.Dungeon != null) {
                        dungeons.Add(h.Dungeon);
                    }
                }
                return dungeons;
            }
        }

        public virtual List<Unit> AllUnits {
            get {
                List<Unit> units = new List<Unit>();
                foreach (Hex h in Hcd.Values) {
                    if (h.Unit != null) {
                        units.Add(h.Unit);
                    }
                }
                return units;
            }
        }

        

        public Map() {
            Console.WriteLine("Map constructor satarted");
            Hcd = new Dictionary<Coords, Hex>();
            Console.WriteLine("Map constructor finished");
        }
        public virtual Hex GetHex(Coords coords)
        {
            try
            {
                return Hcd[coords];
            }
            catch (KeyNotFoundException) {
                return new Hex();
            }
        }

        public virtual List<Coords> GetAllCoords()
        {
            List<Coords> coords = new List<Coords>();

            foreach (var i in Hcd)
            {
                coords.Append<Coords>(i.Key);
            }

            return coords;
        }

        public virtual List<Hex> GetAllHexes()
        {
            List<Hex> hx = new List<Hex>();
            foreach (var i in Hcd)
            {
                hx.Append<Hex>(i.Value);
            }
            return hx;
        }

        

        public virtual void GenerateNew(ISession session, int radius)
        {
            Console.WriteLine("GenerateNew() started");
                Hcd.Clear();
            Radius = radius;
                for (int y = -radius; y < 0; y++)
                {
                    for (int x = radius; x >= -y - radius; x--)
                    {

                        Coords c;


                        c = new Coords(x, y);


                        Hex h = new Hex();
                        h.Randomize();
                        //Console.WriteLine("Hex generated: " + c.ToString());
                        h.Unlock();
                        h.coords = c;
                        h.Map = this;
                        // c.hex = h;
                        session.Save(h);
                        session.Save(c);
                        Hcd.Add(c, h);

                    }
                }
                for (int y = 0; y < radius+1; y++)
                {
                    for (int x = -radius; x <= -y + radius; x++)
                    {
                        Coords c;
                        c = new Coords(x, y);

                        Hex h = new Hex();
                        h.Randomize();
                        //Console.WriteLine("Hex generated: " + c.ToString());
                        h.Unlock();
                        h.coords = c;
                        h.Map = this;
                    //c.hex = h;
                    

                        session.Save(h);
                        session.Save(c);
                        Hcd.Add(c, h);
                    }
                }

                Console.WriteLine("Total hexes generated: " + Hcd.Count.ToString());

                Hex H = Hcd[new Coords(0, 0)];
            LandProperties lp = H.Land_properties;
            if (lp.Height < Constants.OceanHBound && lp.Water > Constants.OceanWBound)
            {
                H.ChangeWater(-lp.Water / 2);
            }
            
            
               
        }

        public virtual void ChangeLandscape(ISession session)
        {
           
                foreach (var kvp in Hcd)
                {
                    Hex center = kvp.Value;
                    List<Coords> surroundings = kvp.Key.Adjacent;
                    foreach (Coords c in surroundings)
                    {
                        Hex.NudgeParameters(center, GetHex(c));
                        session.Update(center);
                    }

                }
            
        }

        public virtual void AddDungeons(ISession session) {
            if (AllDungeons.Count >= MaxDungeonCount)
                return;
            foreach (var h in Hcd.Values) {
                Random random = new Random();
                if (random.Next(100) < Constants.DungeonGenerationPercent) {
                    var d = h.RandomizeDungeon();
                    if (d != null) {
                        session.Save(d);
                        session.Update(h);
                    }
                }
            }

        }
        public virtual void UpdateUnits(ISession session) {
            foreach (var u in AllUnits) {
                if (!u.StatsRetrieved)
                {
                    u.RetreiveStatsFromType();
                }
            }
            foreach (var u in AllUnits) {
                u.Update(session);
            }

        }

        public virtual void UpdateSettlements(ISession session) {
            foreach (var s in AllSettlements) {
                s.Update(session);
            }
        }

        public virtual void UpdateDungeons(ISession session) {
            foreach (var d in AllDungeons) {
                d.Update(session);
            }
        }

        public virtual void UpdateAll(ISession session) {
            if (AllSettlements.Count == 0) {
                Settlement s = Hcd[new Coords(0, 0)].AddStartingSettlement();
                Unit u = Hcd[new Coords(0, 0)].StartingUnit();
                session.Save(s.Resources);
                session.Save(s.Population);
                session.Save(u.Carrying);
                session.Save(u);
                foreach (Building b in s.Buildings) {
                    session.Save(b);
                }
                session.Save(s);
            }
            UpdateUnits(session);
            UpdateSettlements(session);
            UpdateDungeons(session);
            ChangeLandscape(session);
            AddDungeons(session);
        }
    }
    

    

}
