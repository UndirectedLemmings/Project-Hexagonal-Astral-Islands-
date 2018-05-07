﻿using System;
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
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }

    public static class Constants
    {
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


    
    public class Map
    {
        IDictionary<Coords, Hex> hcd;
        private int iD;

        public virtual int ID { get => iD; set => iD = value; }
        public virtual IDictionary<Coords, Hex> Hcd { get => hcd; set => hcd = value; }

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

        

        public virtual void GenerateNew(ISession session)
        {
            Console.WriteLine("GenerateNew() started");
                Hcd.Clear();
            
                for (int y = -5; y < 0; y++)
                {
                    for (int x = 5; x >= -y - 5; x--)
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
                for (int y = 0; y < 6; y++)
                {
                    for (int x = -5; x <= -y + 5; x++)
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
                H.Lock();
                session.UpdateAsync(H);
               
        }

        public virtual void ChangeLandscape()
        {
            using (ISession session = NHibernateHelper.OpenSession())
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
        }

    }
    public class MapMap:ClassMap<Map> {

        public MapMap() {
            Id(x => x.ID);
            HasMany(x => x.Hcd)
                .AsEntityMap("CoordsID")
                .KeyColumn("MapID")
                .Cascade.All();
        }

    }

    public class HexMap : ClassMap<Hex> {
        public HexMap() {
            Id(x => x.ID);
            Map(x => x.Unchangable);
            Map(x => x.IsVoid);
            References(x => x.Map);
            HasOne(x => x.Land_properties)
                .Cascade.All();
            References(x => x.coords);
        }
    }

    public class LPMAP : ClassMap<LandProperties> {
        public LPMAP() {
            Id(x => x.ID);
            Map(x => x.Temperature);
            Map(x => x.Height);
            Map(x => x.Water);
            Map(x => x.Plantlife);
            References(x => x.My_hex).Unique();
        }
    }

    public class CMap : ClassMap<Coords> {
        public CMap() {
            Id(c => c.ID);
            Map(c => c.X);
            Map(c => c.Y);
        }
    }

    

}
