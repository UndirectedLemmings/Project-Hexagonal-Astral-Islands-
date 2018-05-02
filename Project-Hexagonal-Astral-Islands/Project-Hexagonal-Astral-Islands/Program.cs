using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;






namespace Project_Hexagonal_Astral_Islands
{
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

    internal struct LandProperties {
        public int Temperature;
        public int Height;
        public int Water;
        public int Plantlife;

        public LandProperties(int temp = 0, int h = 0, int water = 0, int pl = 0) : this()
        {
            Temperature = temp;
            Height = h;
            Water = water;
            Plantlife = pl;
        }
    }


    internal struct LandCategoires {
        private TempCat tempCat;
        private HeightCat heightCat;
        private WaterCat waterCat;
        private PlantCat plantCat;

    }
        /* public enum LandType {
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
         */

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


        public class Hex {
            private LandProperties land_properties;
            //private LandCategoires landCategoires;
            private bool unchangable=true;
            private bool isVoid = true;

            public static void NudgeParameters(Hex first, Hex second) {
                

                if (first.isVoid || second.isVoid) {
                    return;
                }
                LandProperties lp = first.land_properties;
                if (!first.unchangable) {
                    first.land_properties.Temperature += (second.land_properties.Temperature - first.land_properties.Temperature) / Constants.TemperatureDifferenceDivider;
                    first.land_properties.Height += (second.land_properties.Height - first.land_properties.Height) / Constants.HeightDifferenceDivider;
                    first.land_properties.Water += (second.land_properties.Water - first.land_properties.Water) / Constants.WaterDifferenceDivider;
                    first.land_properties.Plantlife += (second.land_properties.Plantlife - first.land_properties.Plantlife) / Constants.PlantlifeDifferenceDivider;

                }
                if (!second.unchangable) {
                    second.land_properties.Temperature += (lp.Temperature - second.land_properties.Temperature) / Constants.TemperatureDifferenceDivider;
                    second.land_properties.Height += (lp.Height - second.land_properties.Height) / Constants.HeightDifferenceDivider;
                    second.land_properties.Water += (lp.Water - second.land_properties.Water) / Constants.WaterDifferenceDivider;
                    second.land_properties.Plantlife += (lp.Plantlife - second.land_properties.Plantlife) / Constants.PlantlifeDifferenceDivider;
                }


            }
            public void ChangeTemperature(int change) {
                land_properties.Temperature += change;
            }
            public void ChangeHeight(int change)
            {
                land_properties.Height += change;
            }
            public void ChangeWater(int change)
            {
                land_properties.Water += change;
            }
            public void ChangePlantlife(int change)
            {
                land_properties.Plantlife += change;
            }
            public void Randomize() {
                Random random = new Random();
                int t = random.Next(Constants.RandomTemperatureLowerBound, Constants.RandomTemperatureUpperBound);
                int h= random.Next(Constants.RandomHeightLowerBound, Constants.RandomHeightUpperBound);
                int w = random.Next(Constants.RandomWaterLowerBound, Constants.RandomWaterUpperBound);
                int pl = random.Next(Constants.RandomPlantlifeLowerBound, Constants.RandomPlantlifeUpperBound);
                land_properties = new LandProperties(t, h, w, pl);
                isVoid = false;
            }
            public void Unlock() {
                unchangable = false;
            }
            public void Lock() {
                unchangable = true;    
            }

        }

    public struct Coords {
        public int x;
        public int y;

        public List<Coords> Adjacent
        {
            get
            {
                List<Coords> list = new List<Coords>();
                list.Append<Coords>(new Coords(x, y - 1));
                list.Append<Coords>(new Coords(x + 1, y - 1));
                list.Append<Coords>(new Coords(x + 1, y));
                list.Append<Coords>(new Coords(x, y + 1));
                list.Append<Coords>(new Coords(x - 1, y + 1));
                list.Append<Coords>(new Coords(x - 1, y));
                return list;
            }
        }

        public Coords(int X, int Y) {
            x = X;
            y = Y;
        }
    }


    public class Map {
        Dictionary<Coords,Hex> hexes;


        public Hex GetHex(Coords coords) {
            return hexes.GetValueOrDefault<Coords,Hex>(coords,new Hex());
        }

        public List<Coords> GetAllCoords () {
            List<Coords> coords = new List<Coords>();

            foreach (var i in hexes) {
                coords.Append<Coords>(i.Key);
            }

            return coords;
        }

        public List<Hex> GetAllHexes() {
            List<Hex> hx = new List<Hex>();
            foreach (var i in hexes) {
                hx.Append<Hex>(i.Value);
            }
            return hx;
        }

        public Dictionary<Coords, Hex> GetMap() {
            return hexes;
        }

        public void GenerateNew() {
            for (int y = -5; y < 0; y++) {
                for (int x = 5; x >=-y-5; x--) {
                    Coords c = new Coords(x, y);
                    Hex h = new Hex();
                    h.Randomize();
                    h.Unlock();
                    KeyValuePair<Coords, Hex> pair = new KeyValuePair<Coords, Hex>(c, h);
                    hexes.Append<KeyValuePair<Coords, Hex>>(pair);
                }
            }
            for (int y = 0; y < 6; y++)
            {
                for (int x = -5; x >= -y +5; x++)
                {
                    Coords c = new Coords(x, y);
                    Hex h = new Hex();
                    h.Randomize();
                    h.Unlock();
                    KeyValuePair<Coords, Hex> pair = new KeyValuePair<Coords, Hex>(c, h);
                    hexes.Append<KeyValuePair<Coords, Hex>>(pair);
                }
            }
            hexes[new Coords(0, 0)].Lock();
        }

        public void ChangeLandscape() {
            foreach (var kvp in hexes) {
                Hex center = kvp.Value;
                List<Coords> surroundings = kvp.Key.Adjacent;
                foreach (Coords c in surroundings) {
                    Hex.NudgeParameters(center, GetHex(c));
                }

            }
        }

    }

}
