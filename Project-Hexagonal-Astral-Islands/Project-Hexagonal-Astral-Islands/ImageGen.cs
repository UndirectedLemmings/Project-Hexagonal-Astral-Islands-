using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing.Drawing;
using SixLabors.ImageSharp.Processing.Transforms;
using SixLabors.ImageSharp.Formats;
using SixLabors.Primitives;
using Project_Hexagonal_Astral_Islands;
using SixLabors.ImageSharp.Processing;

public class ImageGen
    {
    public static string GenerateImage(Map map,int xdelta=8,int ydelta=12,int isizex=16,int isizey=16, int resizeFactor=4) {

        if (map == null) {
            throw new ArgumentNullException("map");
        }
        string result ="./wwwroot/generatedmaps/map" + map.ID.ToString() + ".png";
        int Width = (map.Radius*2+1)*isizex;
        int Height = (map.Radius * 2 + 1) * isizey;
        Image<Rgba32> image = new Image<Rgba32>(Width, Height);
        Point center = new Point();
        center.X = Width/2-xdelta;
        center.Y = Height/2 - isizey;
        foreach (var kvp in map.Hcd) {
            Point target = center;
            target.X += (kvp.Key.X * xdelta*2)+(kvp.Key.Y*xdelta);
            target.Y += (kvp.Key.Y * ydelta);
            string biome_name = "";
            LandProperties lp = kvp.Value.Land_properties;
            if (lp.Height > Constants.MountainsHBound)
            {
                biome_name = "mountains";
            }
            else {
                if (lp.Height < Constants.OceanHBound && lp.Water > Constants.OceanWBound)
                {
                    biome_name = "sea";
                }
                else {
                    if (lp.Plantlife > Constants.ForestPlBound) {
                        biome_name = "forest"; //add civilisation level later

                    }
                    else
                    {
                        if (lp.Plantlife<Constants.DesertPlBound)
                        {
                            biome_name = "desert";
                        }
                        else
                        {
                            biome_name = "field";
                        }
                    }
                    if (lp.Temperature > Constants.HotTBound) {
                        biome_name = "hot_" + biome_name;
                    }
                    if (lp.Temperature < Constants.ColdTBound) {
                        biome_name = "cold_" + biome_name;
                    }
                }
            }

            biome_name = "./wwwroot/images/Mapgen/biomes/"+biome_name + ".png";
            Image<Rgba32> biome = Image.Load(biome_name);

            image.Mutate(i => i.DrawImage(biome, 1f, target));
            biome.Dispose();
            Settlement s = kvp.Value.Settlement;
            if (s != null) {
                string c = "./wwwroot/images/Mapgen/civs/";
                if (s.Housing < Constants.MediumCivBound) {
                    c = c + "small_civ.png";
                }
                else if (s.Housing < Constants.BigCivBound&&s.Housing>=Constants.MediumCivBound) {
                    c = c + "medium_civ.png";
                }
                else {
                    c = c + "big_civ.png";
                }
                Image<Rgba32> set = Image.Load(c);
                image.Mutate(i => i.DrawImage(set, 1f, target));
                set.Dispose();
                foreach (Building b in s.Buildings) {
                    if (b != null)
                    {
                        string bname = $"./wwwroot/images/Mapgen/buildings/{b.Name}.png";
                        Image<Rgba32> dun = Image.Load(bname);
                        image.Mutate(i => i.DrawImage(dun, 1f, target));
                        dun.Dispose();
                    }
                }
                
            }
            Dungeon d = kvp.Value.Dungeon;
            if (d != null) {
                string c = $"./wwwroot/images/Mapgen/dungeons/{d.Name}.png";
                Image<Rgba32> dun = Image.Load(c);
                image.Mutate(i => i.DrawImage(dun,1f,target));
                dun.Dispose();
            }
            Unit u = kvp.Value.Unit;
            if (u != null)
            {
                string c = $"./wwwroot/images/Mapgen/units/{u.Name}.png";
                Image<Rgba32> dun = Image.Load(c);
                image.Mutate(i => i.DrawImage(dun, 1f, target));
                dun.Dispose();
            }

        }
        ResizeOptions resizeOptions = new ResizeOptions();
        resizeOptions.Size = new Size(Width * resizeFactor, Height * resizeFactor);
        resizeOptions.Sampler = KnownResamplers.NearestNeighbor;
        image.Mutate(i => i.Resize(resizeOptions));
        System.IO.Stream stream = new System.IO.FileStream(result, System.IO.FileMode.OpenOrCreate,System.IO.FileAccess.ReadWrite,System.IO.FileShare.Read);
        IImageEncoder imageEncoder = new SixLabors.ImageSharp.Formats.Png.PngEncoder();
        image.Save(stream,imageEncoder);
        return $"map{map.ID}.png";
    }
    }

