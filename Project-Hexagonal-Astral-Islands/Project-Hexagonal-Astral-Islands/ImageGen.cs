using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing.Drawing;
using SixLabors.ImageSharp.Processing.Transforms;
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
        Image<Rgba32> image = new Image<Rgba32>(320,320);
        Point center = new Point();
        center.X = 160 - isizex;
        center.Y = 160 - isizey;
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
                        biome_name = "forest_no_civ"; //add civilisation level later

                    }
                    else
                    {
                        if (lp.Plantlife<Constants.DesertPlBound)
                        {
                            biome_name = "desert_no_civ";
                        }
                        else
                        {
                            biome_name = "field_no_civ";
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

            biome_name = "./wwwroot/images/Mapgen/"+biome_name + ".png";
            Image<Rgba32> biome = Image.Load(biome_name);

            image.Mutate(i => i.DrawImage(biome, 1f, target));
                
        }
        ResizeOptions resizeOptions = new ResizeOptions();
        resizeOptions.Size = new Size(320 * resizeFactor, 320 * resizeFactor);
        resizeOptions.Sampler = KnownResamplers.NearestNeighbor;
        image.Mutate(i => i.Resize(resizeOptions));
        image.Save(result);
        return $"map{map.ID}.png";
    }
    }

