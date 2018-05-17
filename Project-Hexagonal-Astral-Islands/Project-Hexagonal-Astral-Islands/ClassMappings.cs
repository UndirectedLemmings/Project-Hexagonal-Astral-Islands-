using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NHibernate;
using FluentNHibernate.Mapping;
using Project_Hexagonal_Astral_Islands;

namespace Project_Hexagonal_Astral_Islands.ClassMappings
{
    public class MapMap : ClassMap<Map>
    {

        public MapMap()
        {
            //Console.WriteLine("MapMap()");
            Id(x => x.ID);
            Map(x => x.Radius);
            Map(x => x.Domain);
            Map(x => x.Tier);
            HasMany(x => x.Hcd)
                .AsEntityMap("CoordsID")
                .KeyColumn("MapID")
                .Cascade.All();
            //Console.WriteLine("MapMap() finished");
        }

    }

    public class HexMap : ClassMap<Hex>
    {
        public HexMap()
        {
            //Console.WriteLine("HexMap()");
            Id(x => x.ID);
            Map(x => x.Unchangable);
            Map(x => x.IsVoid);
            References(x => x.Map);
            HasOne(x => x.Land_properties)
                .Cascade.All();
            References(x => x.coords);
            HasOne(x => x.Unit)
                .Cascade.All();
            HasOne(x => x.Settlement)
                .Cascade.All();
            HasOne(x => x.Dungeon)
                .Cascade.All();
            //Console.WriteLine("HexMap() finished");
        }
    }

    public class LPMAP : ClassMap<LandProperties>
    {
        public LPMAP()
        {
            //Console.WriteLine("LPMAP()");
            Id(x => x.ID);
            Map(x => x.Temperature);
            Map(x => x.Height);
            Map(x => x.Water);
            Map(x => x.Plantlife);
            References(x => x.My_hex).Unique();
            //Console.WriteLine("LPMAP() finished");
        }
    }

    public class CMap : ClassMap<Coords>
    {
        public CMap()
        {
            //Console.WriteLine(" CMap()");
            Id(c => c.ID);
            Map(c => c.X);
            Map(c => c.Y);
        }
    }

    public class RessMap : ClassMap<Ress> {
        public RessMap() {
            //Console.WriteLine("RessMap()");
            Id(x => x.ID);
            Map(x => x.Food);
            Map(x => x.Wood);
            Map(x => x.Stone);
            Map(x => x.Iron);
            Map(x => x.Gold);
            Map(x => x.Mana);
            References(x => x.Building);
            References(x => x.Unit);
            References(x => x.Settlement);
            References(x => x.Dungeon);
            //Console.WriteLine("RessMap() finished");
        }
    }

    public class BuildMap : ClassMap<Building> {
        public BuildMap() {
            //Console.WriteLine("BuildMap()");
            Id(x => x.ID);
            HasOne(x => x.Income_per_turn)
                .Cascade.All();
            Map(x => x.Name);
            Map(x => x.Displayed_name);
            References(x=>x.Settlement);
            //Console.WriteLine("BuildMap() finished");
        }

    }


    public class UnitMap : ClassMap<Unit> {
        public UnitMap() {
            //Console.WriteLine("UnitMap()");
            Id(x => x.ID);
            References(x => x.My_hex);
            Map(x => x.Type);
            //Map(x => x.Name);
            References(x => x.Origin);
            References(x => x.Target);
            //Map(x => x.Displayed_name);
            Map(x => x.Alive);
            Map(x => x.Wounded);
            //Map(x => x.Race);
            Map(x => x.Intention);
           /* Map(x => x.RangedAliveAttack);
            Map(x => x.RangedWoundedAttack);
            Map(x => x.RangedAliveDef);
            Map(x => x.RangedWoundedDef);
            Map(x => x.MeleeAliveAttack);
            Map(x => x.MeleeWoundedAttack);
            Map(x => x.MeleeAliveDef);
            Map(x => x.MeleeWoundedDef);*/
            Map(x => x.Team);
            HasOne(x => x.Upkeep_per_person)
                .Cascade.All();
            HasOne(x => x.Carrying)
                .Cascade.All();
            //Console.WriteLine("UnitMap() finished");
        }
    }

    public class PopMap : ClassMap<Population> {
        public PopMap()
        {
            //Console.WriteLine("PopMap()");
            Id(x => x.ID);
            Map(x => x.Humans);
            Map(x => x.Beastfolk);
            Map(x => x.Undead);
            Map(x => x.Chaotic);
            Map(x => x.Lawful);
            References(x => x.Settlement);
            //Console.WriteLine("PopMap() finished");
        }
    }

    public class SettlMap : ClassMap<Settlement> {
        public SettlMap() {
            //Console.WriteLine("SettlMap()");
            Id(x => x.ID);
            HasOne(x => x.Population)
                .Cascade.All();
            HasOne(x => x.Resources)
                .Cascade.All();
            Map(x => x.Housing);
            Map(x => x.Team);
            Map(x => x.ProducedUnitId);
            HasMany(x => x.Buildings)
                .Cascade.All();
            References(x => x.MyHex);
            //Console.WriteLine("SettlMap() finished");
        }
    }

    public class DunggMap : ClassMap<Dungeon> {
        public DunggMap() {
            //Console.WriteLine("DunggMap()");
            Id(x => x.ID);
            References(x => x.My_hex);
            Map(x => x.Team);
            HasOne(x => x.Loot)
                .Cascade.All();
            Map(x => x.UnitTypeId);
            Map(x => x.Raider_party_size);
            Map(x => x.Garrison);
            Map(x => x.GarrisonIncome);
            Map(x => x.Turns_before_income);
            Map(x => x.Income_interval);
            Map(x => x.Displayed_name);
            Map(x => x.Name);
            Map(x => x.Threat);
            //Console.WriteLine("DunggMap() finished");
        }

    }

}
