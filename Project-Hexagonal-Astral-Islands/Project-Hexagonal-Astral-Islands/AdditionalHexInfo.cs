using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using NHibernate;

namespace Project_Hexagonal_Astral_Islands
{
    
    public enum Race
{
    Human,
    Beastfolk,
    Undead,
    Chaotic,
    Lawful,
    Animal
}

    public class Building
{
    long iD;
    string name;
    string displayed_name;
    Ress income_per_turn;
    public virtual long ID { get => iD; set => iD = value; }
    public virtual string Name { get => name; set => name = value; }
    public virtual Ress Income_per_turn { get => income_per_turn; set => income_per_turn = value; }
    public virtual string Displayed_name { get => displayed_name; set => displayed_name = value; }
        public virtual Settlement Settlement { get; set; }

        public static Building FromTypes(int typeId) {
            Building b = new Building();
            XmlDocument types = new XmlDocument();
            types.Load("./BuildingTypes.xml");
            XmlNode type = types.SelectSingleNode($"BuildingTypes/Btype[id = {typeId}]");
            b.Income_per_turn = Ress.FromXml(type.SelectSingleNode("./income"));
            b.Name = type.SelectSingleNode("./name").InnerText;
            b.Displayed_name = type.SelectSingleNode("./displayed").InnerText;
            return b;
        }

        public static Ress GetCost(int typeId) {
            
            XmlDocument types = new XmlDocument();
            types.Load("./BuildingTypes.xml");
            XmlNode type = types.SelectSingleNode($"BuildingTypes/Btype[id = {typeId}]");
            Ress cost = Ress.FromXml(type.SelectSingleNode("./cost"));
            return cost;
        }

        public Building() {
            //Console.WriteLine("Building() constructor started");
            ID = 0;
            Name = "";
            Displayed_name = "";
            Income_per_turn = Ress.Zero;
            //Console.WriteLine("Building() constructor finished");
        }
}


    public class Ress
    {
        long iD;
        int food, wood, stone, iron, gold, mana;
        Building building;
        Unit unit;


        public virtual int Food { get => food; set => food = value; }
        public virtual int Wood { get => wood; set => wood = value; }
        public virtual int Stone { get => stone; set => stone = value; }
        public virtual int Iron { get => iron; set => iron = value; }
        public virtual int Gold { get => gold; set => gold = value; }
        public virtual int Mana { get => mana; set => mana = value; }
        public virtual long ID { get => iD; set => iD = value; }
        public virtual Building Building { get => building; set => building = value; }
        public virtual Unit Unit { get => unit; set => unit = value; }
        public virtual Settlement Settlement { get; set; }
        public virtual Dungeon Dungeon { get; set; }

        public static Ress FromXml(XmlNode xmlNode) {
            return new Ress
            {
                Food = Convert.ToInt32(xmlNode.SelectSingleNode("./Food").InnerText),
                Wood = Convert.ToInt32(xmlNode.SelectSingleNode("./Wood").InnerText),
                Gold = Convert.ToInt32(xmlNode.SelectSingleNode("./Gold").InnerText),
                Stone = Convert.ToInt32(xmlNode.SelectSingleNode("./Stone").InnerText),
                Iron = Convert.ToInt32(xmlNode.SelectSingleNode("./Iron").InnerText),
                Mana = Convert.ToInt32(xmlNode.SelectSingleNode("./Mana").InnerText)
            };
        }

        public static Ress operator *(Ress res, int number)
        {
            Ress result = new Ress();
            result.food = res.food * number;
            result.wood = res.wood * number;
            result.stone = res.stone * number;
            result.iron = res.iron * number;
            result.gold = res.gold * number;
            result.mana = res.mana * number;
            return result;

        }
        public static Ress operator +(Ress res1, Ress res2)
        {
            Ress result = new Ress();
            result.food = res1.food + res2.food;
            result.wood = res1.wood + res2.wood;
            result.stone = res1.stone + res2.stone;
            result.iron = res1.iron + res2.iron;
            result.gold = res1.gold + res2.gold;
            result.mana = res1.mana + res2.mana;
            return result;

        }

        public static Ress operator -(Ress res1, Ress res2)
        {
            Ress result = new Ress();
            result.food = res1.food - res2.food;
            result.wood = res1.wood - res2.wood;
            result.stone = res1.stone - res2.stone;
            result.iron = res1.iron - res2.iron;
            result.gold = res1.gold - res2.gold;
            result.mana = res1.mana - res2.mana;
            return result;

        }

        public static bool Enough(Ress Have, Ress Req)
        {
            if (Have == null)
                return false;
            return Have.Food >= Req.Food && Have.Wood >= Req.Wood && Have.Stone >= Req.Stone && Have.Iron >= Req.Iron && Have.Gold >= Req.Gold && Have.Mana >= Req.Mana;
        }

        public static readonly Ress Zero = new Ress()
        {
            food = 0,
            wood = 0,
            stone = 0,
            iron = 0,
            gold = 0,
            mana = 0
        };
    }

    public class Unit
    {
        long iD;
        Hex my_hex;
        string name;
        Coords origin;
        Coords target;
        string displayed_name;
        Race race;
        int alive;
        int wounded;
        bool statsRetrieved = false;

        Ress upkeep_per_person;
        Ress carrying;
        public virtual Race Race { get => race; set => race = value; }
        public virtual string Displayed_name { get => displayed_name; set => displayed_name = value; }
        public virtual string Name { get => name; set => name = value; }
        public virtual int Alive { get => alive; set => alive = value; }
        public virtual int Wounded { get => wounded; set => wounded = value; }
        public virtual Ress Upkeep_per_person { get => upkeep_per_person; set => upkeep_per_person = value; }
        public virtual long ID { get => iD; set => iD = value; }
        public virtual Ress Carrying { get => carrying; set => carrying = value; }

        public virtual Coords Origin { get => origin; set => origin = value; }
        public virtual Coords Target { get => target; set => target = value; }
        public virtual Intent Intention { get; set; }

        public virtual Hex My_hex { get => my_hex; set => my_hex = value; }

        public virtual string Team { get; set; }

        public virtual int RangedAliveAttack { get; set; }
        public virtual int RangedWoundedAttack { get; set; }
        public virtual int MeleeAliveAttack { get; set; }
        public virtual int MeleeWoundedAttack { get; set; }

        public virtual int RangedAliveDef { get; set; }
        public virtual int RangedWoundedDef { get; set; }
        public virtual int MeleeAliveDef { get; set; }
        public virtual int MeleeWoundedDef { get; set; }

        public virtual int Type { get; set; }

        public virtual int RangedAttackTotal { get => RangedAliveAttack * Alive + RangedWoundedAttack * Wounded; }
        public virtual int MeleeAttackTotal { get => MeleeAliveAttack * Alive + MeleeWoundedAttack * Wounded; }

        public virtual int RangedDefTotal { get => RangedAliveDef * Alive + RangedWoundedDef * RangedWoundedDef * Wounded; }
        public virtual int MeleeDefTotal { get => MeleeAliveDef * Alive + MeleeWoundedDef * Wounded; }
        public virtual bool StatsRetrieved { get => statsRetrieved; set => statsRetrieved = value; }

        public virtual void Update(ISession session) {
            if (TakeUpkeep() == FightEnd.Defeat) {
                My_hex.Unit = null;
                //session.Delete(this);
                return;
            }
            Unit en = SearchForEnemy();
            if (en != null)
            {
                switch (Fight(en))
                {
                    case FightEnd.Defeat:
                        session.Update(en);
                        My_hex.Unit = null;
                        //session.Delete(this);
                        break;
                    case FightEnd.Victory:
                        en.My_hex.Unit = null;
                        //session.Delete(en);
                        session.Update(this);
                        break;
                    case FightEnd.Draw:
                        session.Update(en);
                        session.Update(this);
                        break;

                }
            }
            else {
                if (my_hex.coords == target) {
                    switch (Intention) {
                        case Intent.GuardHex:
                            break;
                        case Intent.GoIntoDungeon:
                            Dungeon d = GoIntoDungeon();
                            if (d != null)
                            {
                                session.Update(d);
                                my_hex.Unit = null;
                                //session.Delete(this);
                            }
                            break;

                        case Intent.ClearDungeon:
                            var dun = ClearDungeon();
                            if (dun != null) {
                                My_hex.Dungeon = null;
                                //session.Delete(dun);
                            }
                            
                            target = origin;
                            Intention = Intent.Settle;
                            session.Update(this);
                            break;
                        case Intent.Settle:
                            session.Update(Settle());
                            //session.Delete(this);
                            break;
                        case Intent.RaidSettlementFD:
                            Settlement rs = RaidSettlement();
                            if (rs != null)
                            {
                                session.Update(rs);
                            }
                            target = origin;
                            Intention = Intent.GoIntoDungeon;
                            session.Update(this);
                            break;
                        case Intent.RaidSettlementFS:
                            Settlement rs1 = RaidSettlement();
                            if (rs1 != null)
                            {
                                session.Update(rs1);
                            }
                            target = origin;
                            Intention = Intent.Settle;
                            session.Update(this);
                            break;



                    }
                }
                if (wounded < alive) {
                    if (My_hex.coords != target) {
                        Move(FindClosestFree());
                    }
                    
                }
                Heal();
                session.Update(this);
            }
        }

        public enum Intent {
            ClearDungeon,
            RaidSettlementFD,
            RaidSettlementFS,
            Settle,
            GoIntoDungeon,
            GuardHex
        }


        public virtual Settlement RaidSettlement() {
            Random random = new Random();
            Settlement s = my_hex.Settlement;
            if (s == null) {
                return null;
            }
            Ress robbed = new Ress();
            

            
            Population killed = new Population();
            switch (s.Population.PrimaryRace) {
                case Race.Human:
                    killed.Humans=random.Next(MeleeAttackTotal / 2, MeleeAttackTotal);
                    s.Population = s.Population - killed;
                    if (s.Population.Humans < 0)
                        s.Population.Humans = 0;
                    break;
                case Race.Beastfolk:
                    killed.Beastfolk= random.Next(MeleeAttackTotal / 2, MeleeAttackTotal);
                    s.Population = s.Population - killed;
                    if (s.Population.Beastfolk < 0)
                        s.Population.Beastfolk = 0;
                    break;
                case Race.Undead:
                    killed.Undead = random.Next(MeleeAttackTotal / 2, MeleeAttackTotal);
                    s.Population = s.Population - killed;
                    if (s.Population.Undead < 0)
                        s.Population.Undead = 0;
                    break;
                case Race.Lawful:
                    killed.Lawful= random.Next(MeleeAttackTotal / 2, MeleeAttackTotal);
                    s.Population = s.Population - killed;
                    if (s.Population.Lawful < 0)
                        s.Population.Lawful = 0;
                    break;
                case Race.Chaotic:
                    killed.Chaotic= random.Next(MeleeAttackTotal / 2, MeleeAttackTotal);
                    s.Population = s.Population - killed;
                    if (s.Population.Chaotic < 0)
                        s.Population.Chaotic = 0;
                    break;
            }
            if (s.Population.Total > 0)
            {
                robbed.Gold = random.Next(s.Resources.Gold / 4, (s.Resources.Gold / 4) * 3);
                robbed.Food = random.Next(s.Resources.Food / 4, (s.Resources.Food / 4) * 3);
                robbed.Wood = random.Next(s.Resources.Wood / 4, (s.Resources.Wood / 4) * 3);
                robbed.Stone = random.Next(s.Resources.Stone / 4, (s.Resources.Stone / 4) * 3);
                robbed.Iron = random.Next(s.Resources.Iron / 4, (s.Resources.Iron / 4) * 3);
            }
            else {
                robbed = s.Resources;
            }
            s.Resources = s.Resources - robbed;
            carrying = carrying + robbed;
            return s;

        }
        public virtual Unit SearchForEnemy() {
            Unit result = null;
            List<Coords> cs = My_hex.coords.Adjacent;
            foreach (Coords c in cs) {
                Hex hex = My_hex.Map.GetHex(c);
                if (hex.Unit != null) {
                    if (hex.Unit.Team != Team) {
                        result = hex.Unit;
                        return result;
                    }
                }
            }

            return result;
        }

        public virtual Settlement Settle() {
           
           if (My_hex.Settlement == null) {
                my_hex.Settlement = new Settlement();
                my_hex.Settlement.MyHex = My_hex;
            } 
            
                
            
            switch (Race) {
                case Race.Human:
                    My_hex.Settlement.Population.Humans += (Alive + Wounded);
                    break;
                case Race.Beastfolk:
                    My_hex.Settlement.Population.Beastfolk += (Alive + Wounded);
                    break;
                case Race.Undead:
                    My_hex.Settlement.Population.Undead += (Alive + Wounded);
                    break;
                case Race.Lawful:
                    My_hex.Settlement.Population.Lawful += (Alive + Wounded);
                    break;
                case Race.Chaotic:
                    My_hex.Settlement.Population.Chaotic += (Alive + Wounded);
                    break;

            }
            My_hex.Settlement.Resources =My_hex.Settlement.Resources + carrying+GetCost(Type)*(Alive+Wounded);
            my_hex.Unit = null;
            return My_hex.Settlement;
        }

        public virtual Dungeon ClearDungeon() {
            Dungeon dungeon = my_hex.Dungeon;
            my_hex.Dungeon.Loot = null;
            my_hex.Dungeon = null;
            carrying = carrying + dungeon.Loot;
            return dungeon;

        }

        public virtual Dungeon GoIntoDungeon() {
            Dungeon d = my_hex.Dungeon;
            if (d == null)
            {
                return null;
            }
            else {
                d.Garrison += (Alive + Wounded);
                d.Loot = d.Loot + Carrying;
                return d;
            }
        }
        

        public virtual void Heal() {
            if (wounded <= 0)
            {
                return;
            }
            else {
                int Healed = wounded / 10;
                if (Healed <= 0) {
                    Healed = 1;
                }
                wounded -= Healed;
                Alive += Healed;
                return;
            }
        }

        public enum FightEnd {
            Victory,
            Defeat,
            Draw
        }

        public virtual FightEnd Fight(Unit enemy) {
            //Ranged Phase
            int dmg_to_e = RangedAttackTotal - enemy.RangedDefTotal;
            if (dmg_to_e < 0)
            {
                if (alive + wounded >= enemy.Alive + enemy.Wounded)
                {
                    dmg_to_e = 0;
                }
                else
                {
                    int a = Alive + Wounded;
                    int w = 0;
                    if (enemy.Alive < a) {
                        w = a - enemy.Alive;
                        a = enemy.Alive;
                    }
                        dmg_to_e = (RangedAttackTotal - (a * enemy.RangedAliveDef + w * enemy.RangedWoundedDef))/2;
                    if (dmg_to_e < 0)
                    {
                        dmg_to_e = 0;
                    }
                }
            }
            int dmg_to_me = enemy.RangedAttackTotal - RangedDefTotal;
            if (dmg_to_me < 0)
            {
                if (enemy.Alive + enemy.Wounded >= Alive + Wounded)
                {
                    dmg_to_me = 0;
                }
                else {
                    int a = enemy.Alive + enemy.Wounded;
                    int w = 0;
                    if (Alive < a)
                    {
                        w = a - Alive;
                        a = Alive;
                    }
                    dmg_to_me = (enemy.RangedAttackTotal - (a * RangedAliveDef + w * RangedWoundedDef)) / 2; ;
                    if (dmg_to_me < 0) {
                        dmg_to_me = 0;
                    }
                }
            }
            Hurt(dmg_to_me);
            enemy.Hurt(dmg_to_e);

            if (enemy.My_hex.Unit == null) {
                carrying =carrying+ enemy.Carrying;
                return FightEnd.Victory;
            }
            if (my_hex.Unit == null) {
                enemy.Carrying = enemy.Carrying+carrying;
                return FightEnd.Defeat;
            }

            //Melee Phase

             dmg_to_e = MeleeAttackTotal - enemy.MeleeDefTotal;
            if (dmg_to_e < 0)
            {
                if (alive + wounded >= enemy.Alive + enemy.Wounded)
                {
                    dmg_to_e = 0;
                }
                else
                {
                    int a = Alive + Wounded;
                    int w = 0;
                    if (enemy.Alive < a)
                    {
                        w = a - enemy.Alive;
                        a = enemy.Alive;
                    }
                    dmg_to_e = (MeleeAttackTotal - (a * enemy.MeleeAliveDef + w * enemy.MeleeWoundedDef)) / 2;
                    if (dmg_to_e < 0)
                    {
                        dmg_to_e = 0;
                    }
                }
            }
            dmg_to_me = enemy.MeleeAttackTotal - MeleeDefTotal;
            if (dmg_to_me < 0)
            {
                if (enemy.Alive + enemy.Wounded >= Alive + Wounded)
                {
                    dmg_to_me = 0;
                }
                else
                {
                    int a = enemy.Alive + enemy.Wounded;
                    int w = 0;
                    if (Alive < a)
                    {
                        w = a - Alive;
                        a = Alive;
                    }
                    dmg_to_me = (enemy.MeleeAttackTotal - (a * MeleeAliveDef + w * MeleeWoundedDef)) / 2; ;
                    if (dmg_to_me < 0)
                    {
                        dmg_to_me = 0;
                    }
                }
            }
            Hurt(dmg_to_me);
            enemy.Hurt(dmg_to_e);

            if (enemy.My_hex.Unit == null)
            {
                carrying = carrying + enemy.Carrying;
                return FightEnd.Victory;
            }
            if (my_hex.Unit == null)
            {
                enemy.Carrying = enemy.Carrying + carrying;
                return FightEnd.Defeat;
            }

            return FightEnd.Draw;
        }
            
        

        public virtual void Hurt(int dmg) {
            wounded -= dmg;
            if (wounded < 0) {
                wounded = 0;
            }alive -= dmg;
            if(alive<0){
                dmg += alive*2;
                alive = 0;
            }
            wounded += dmg;
            if (alive + wounded >= 0) {
                My_hex.Unit = null;
            }
        }

        public static Ress GetCost(int typeId)
        {
            
            XmlDocument types = new XmlDocument();
            types.Load("./UnitTypes.xml");
            XmlNode type = types.SelectSingleNode($"UnitTypes/Unit[id = {typeId}]");
            Ress cost = Ress.FromXml(type.SelectSingleNode("./cost"));
            return cost;
        }

        public virtual void RetreiveStatsFromType() {
            Unit example = NewFromTypes(Type);
            Upkeep_per_person = example.Upkeep_per_person;
            //Ranged
            RangedAliveAttack = example.RangedAliveAttack;
            RangedWoundedAttack = example.RangedWoundedAttack;
            RangedAliveDef = example.RangedAliveDef;
            RangedWoundedDef = example.RangedWoundedDef;
            //Melee
            MeleeAliveAttack = example.MeleeAliveAttack;
            MeleeWoundedAttack = example.MeleeWoundedAttack;
            MeleeAliveDef = example.MeleeAliveDef;
            MeleeWoundedDef = example.MeleeWoundedDef;

            Race = example.Race;
            { }
            Name = example.Name;
            Displayed_name = example.Displayed_name;

            StatsRetrieved = true;
        }

        public static Unit NewFromTypes(int typeId) {
            Unit res = new Unit();

            res.Type = typeId;
            XmlDocument types = new XmlDocument();
            types.Load("./UnitTypes.xml");
            XmlNode node = types.SelectSingleNode($"UnitTypes/Unit[id = {typeId}]");
            XmlNode n = node.SelectSingleNode($"./name");
            res.Name = n.InnerText;
            n= node.SelectSingleNode($"./displayed");
            res.Displayed_name = n.InnerText;
            
            XmlNode upkeep = node.SelectSingleNode($"./Upkeep");
            res.Upkeep_per_person = Ress.FromXml(upkeep);
            XmlNode D = node.SelectSingleNode($"./RangedAttack");
            res.RangedAliveAttack = Convert.ToInt32(D.SelectSingleNode("./alive").InnerText);
            res.RangedWoundedAttack = Convert.ToInt32(D.SelectSingleNode("./wounded").InnerText);
            D = node.SelectSingleNode($"./MeleeAttack");
            res.MeleeAliveAttack = Convert.ToInt32(D.SelectSingleNode("./alive").InnerText);
            res.MeleeWoundedAttack = Convert.ToInt32(D.SelectSingleNode("./wounded").InnerText);
            D = node.SelectSingleNode($"./MeleeDef");
            res.MeleeAliveDef = Convert.ToInt32(D.SelectSingleNode("./alive").InnerText);
            res.MeleeWoundedDef = Convert.ToInt32(D.SelectSingleNode("./wounded").InnerText);
            D = node.SelectSingleNode($"./RangedDef");
            res.MeleeAliveDef = Convert.ToInt32(D.SelectSingleNode("./alive").InnerText);
            res.MeleeWoundedDef = Convert.ToInt32(D.SelectSingleNode("./wounded").InnerText);
            string race = node.SelectSingleNode($"./race").InnerText;
            switch (race.ToUpper()) {
                case "HUMAN":
                    res.Race = Race.Human;
                    break;
                case "BEASTFOLK":
                    res.Race = Race.Beastfolk;
                    break;
                case "UNDEAD":
                    res.Race = Race.Undead;
                    break;
                case "CHAOTIC":
                    res.Race = Race.Chaotic;
                    break;
                case "LAWFUL":
                    res.Race = Race.Lawful;
                    break;
                case "ANIMAL":
                    res.Race = Race.Animal;
                    break;
                default:
                    throw new Exception($"Wrong race in UnitTypes.xml for UnitID {typeId}");
                    

            }


            return res;
        }




        public virtual FightEnd TakeUpkeep() {
            bool leaveCondition = !Ress.Enough(carrying, upkeep_per_person * (wounded + alive));
            
            
            if (leaveCondition) {
                Carrying = null;
                My_hex.Unit = null;
                return FightEnd.Defeat;
            }
            carrying = carrying - upkeep_per_person * (wounded + alive);
            return FightEnd.Victory;

        }

        public virtual void Move(Coords trt) {
            if (trt == null) {
                return;
            }
            Hex where = my_hex.Map.GetHex(trt);
            if (where.IsVoid || where.Unit != null) {
                return;
            }
            where.Unit = this;
            my_hex.Unit = null;
            my_hex = where;
        }

        public virtual Coords FindClosestFree() {
            List<Coords> coords = my_hex.coords.Adjacent;
            Coords result = null;
            int distance = 10000;
            foreach (Coords c in coords) {
                Hex hex = My_hex.Map.GetHex(c);
                int d = Coords.Distance(c, target);
                if (!hex.IsVoid && hex.Unit==null && d < distance) {
                    result = c;
                    distance = d;
                }
            }
            return result;

        }
}

    public class Population
{
    long iD;
    int humans;
    int beastfolk;
    int undead;
    int chaotic;
    int lawful;
    public virtual long ID { get => iD; set => iD = value; }

        public virtual Settlement Settlement { get; set; }

    public virtual int Total { get => humans + beastfolk + undead + chaotic + lawful; }
    public virtual int Humans { get => humans; set => humans = value; }
    public virtual int Beastfolk { get => beastfolk; set => beastfolk = value; }
    public virtual int Undead { get => undead; set => undead = value; }
    public virtual int Chaotic { get => chaotic; set => chaotic = value; }
    public virtual int Lawful { get => lawful; set => lawful = value; }


        public virtual Race PrimaryRace { get {
                if (humans >= beastfolk && humans >= undead && humans >= chaotic && humans >= lawful) {
                    return Race.Human;
                }else if (Beastfolk >= humans && Beastfolk >= undead && Beastfolk >= chaotic && Beastfolk >= lawful) {
                    return Race.Beastfolk;
                }
                else if (undead >= humans && undead >= beastfolk && undead >= chaotic && undead >= lawful) {
                    return Race.Undead;

                }
                else if (chaotic >= humans && chaotic >= undead && chaotic >= beastfolk && chaotic >= lawful)
                {
                    return Race.Chaotic;
                }
                else if (lawful >= humans && lawful >= undead && lawful >= beastfolk && lawful >= chaotic)
                {
                    return Race.Chaotic;
                }
                return Race.Human;
            } }

    public static Population operator -(Population one, Population other)
    {
        Population result = new Population();
        result.humans = one.humans - other.humans;
        result.beastfolk = one.beastfolk - other.beastfolk;
        result.undead = one.undead - other.undead;
        result.chaotic = one.chaotic - other.chaotic;
        result.lawful = one.lawful - other.lawful;
        return result;
    }
    public static Population operator +(Population one, Population other)
    {
        Population result = new Population();
        result.humans = one.humans + other.humans;
        result.beastfolk = one.beastfolk + other.beastfolk;
        result.undead = one.undead + other.undead;
        result.chaotic = one.chaotic + other.chaotic;
        result.lawful = one.lawful + other.lawful;
        return result;
    }

}

    public class Settlement
    {

        public static readonly int maxBuildings = 3;

        int housing;
        Population population;
        Ress resources;
        ISet<Building> buildings;

        public virtual int Housing { get => housing; set => housing = value; }
        public virtual Population Population { get => population; set => population = value; }
        public virtual Ress Resources { get => resources; set => resources = value; }
        public virtual ISet<Building> Buildings { get => buildings; set => buildings = value; }
        public virtual long ID { get; set; }
        public virtual int ProducedUnitId { get; set; }
        public virtual string Team { get; set; }
        public virtual Hex MyHex { get; set; }
        public virtual bool EnemyNear
        {
            get
            {
                List<Coords> cs = MyHex.coords.Adjacent;
                foreach (Coords c in cs)
                {
                    Hex hex = MyHex.Map.GetHex(c);
                    if (hex.Unit != null)
                    {
                        if (hex.Unit.Team != Team)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        public Settlement() {
            Housing = 0;
            Population = new Population();
            Resources = Ress.Zero;
            Buildings = new SortedSet<Building>();
            ProducedUnitId = 0;
            Team = "";
            MyHex = null;

        }

        public static Ress MaterialsRequired(int buildAmount, int alreadyBuilt) {
            Ress ress = new Ress();

            for (int i = 0; i < buildAmount; i++) {
                ress.Wood += (((buildAmount + alreadyBuilt) / 50) + 1) * 3;
                ress.Stone += (buildAmount + alreadyBuilt) / 50;
            }

            return ress;
        }


        public virtual void BuildHousing(int amount) {
            Ress ress = MaterialsRequired(amount,Housing);
            if (!Ress.Enough(Resources,ress)) {
                return;
            }
            Housing += amount;
            Resources = Resources - ress;

        }

        public virtual Building AddBuilding(int typeId) {
            if (buildings.Count == maxBuildings) {
                return null;
            }
            Ress cost = Building.GetCost(typeId);
            if (Ress.Enough(Resources, cost)) {
                Building b = Building.FromTypes(typeId);
                buildings.Add(b);
                return b;
            }
            return null;
        }

        public virtual Dungeon MDD() {
            int T = 4;
            Dungeon d = null;
            foreach (var dun in MyHex.Map.AllDungeons) {
                if (dun.Team != Team) {
                    int t = dun.Threat - Coords.Distance(MyHex.coords, dun.My_hex.coords);
                    if (t > T) {
                        T = t;
                        d = dun;
                    }
                }
            }
            return d;

        }

        public virtual void Immigration() {
            if (Population.Total>=housing) {
                return;
            }
            Random random = new Random();
            int t = MyHex.Map.Tier;
            Population.Humans += random.Next((t+1),(t+1)*3);
            if (t > 2) {
                switch (MyHex.Map.Domain) {
                    case Domain.Life:
                        Population.Beastfolk += random.Next((t - 2), (t - 2) * 3);
                        break;
                    case Domain.Death:
                        Population.Undead += random.Next((t - 2), (t - 2) * 3);
                        break;
                    case Domain.Chaos:
                        Population.Chaotic += random.Next((t - 2), (t - 2) * 3);
                        break;
                    case Domain.Law:
                        Population.Lawful += random.Next((t - 2), (t - 2) * 3);
                        break;

                }
            }

        }

        public virtual Unit SendUnit(Coords Where, Unit.Intent intent, int amount, Ress additionalLoad ) {
            if (MyHex.Unit != null) { return null; }
            Unit unit = Unit.NewFromTypes(ProducedUnitId);
            unit.Alive = amount;
            Ress planned_upkeep =unit.Upkeep_per_person*(amount*Coords.Distance(MyHex.coords,Where)*4);
            unit.Carrying = planned_upkeep+additionalLoad;
            unit.Origin = MyHex.coords;
            unit.Target = Where;
            unit.Intention = intent;
            unit.My_hex = MyHex;
            Ress cost = additionalLoad + planned_upkeep + Unit.GetCost(ProducedUnitId)*amount;
            if (!Ress.Enough(Resources, cost)) {
                return null;
            }

            switch (unit.Race)
            {
                case Race.Human:
                    Population.Humans -= amount;
                    if (Population.Humans < 0)
                    {
                        Population.Humans += amount;
                        return null;
                    }
                    break;
                case Race.Beastfolk:
                    Population.Beastfolk -= amount;
                    if (Population.Beastfolk < 0)
                    {
                        Population.Beastfolk += amount;
                        return null;
                    }
                    break;
                case Race.Undead:
                    Population.Undead -= amount;
                    if (Population.Undead < 0)
                    {
                        Population.Undead += amount;
                        return null;
                    }
                    break;
                case Race.Chaotic:
                    Population.Chaotic -= amount;
                    if (Population.Chaotic < 0)
                    {
                        Population.Chaotic += amount;
                        return null;
                    }
                    break;
                case Race.Lawful:
                    Population.Lawful -= amount;
                    if (Population.Lawful < 0)
                    {
                        Population.Lawful += amount;
                        return null;
                    }
                    break;
            }
            MyHex.Unit = unit;
            Console.WriteLine($"Map:{MyHex.Map.ID}  Sent {unit.Alive} {unit.Name} from {unit.Origin} to {unit.Target}");
            return unit;
        }

        public virtual Unit SendUnit(Coords Where,Unit.Intent intent,int amount) {
            return SendUnit(Where, intent, amount, Ress.Zero);
        }

        public virtual void Update(ISession session) {
            if (Population.Total < 1) {
                Resources = null;
                Buildings.Clear();
                Buildings = null;
                Population = null;
                MyHex.Settlement = null;
                return;
                //session.Delete(this);
            }
            if (EnemyNear) {
                Unit u = SendUnit(MyHex.coords, Unit.Intent.Settle,Population.Total/4);
                if (u != null) {
                    session.Save(u);
                    session.Update(this);
                }
                return;
            }
                Immigration();
            
            Dungeon mdd = MDD();
            if (mdd != null)
            {
                Unit unit = SendUnit(mdd.My_hex.coords, Unit.Intent.ClearDungeon, Population.Total / 4);
                if (unit != null)
                {
                    session.Save(unit);
                    session.Update(this);
                }
                return;
            }
            else {
                Random random = new Random();
                if (random.Next(Population.Total+Housing)>= Housing) {
                    List<Coords> coords = MyHex.Map.Hcd.Keys.ToList();
                    Coords c= null;
                    bool unsuitable = true;
                    while (unsuitable) {
                        c = coords[random.Next(coords.Count)];
                        Hex h = MyHex.Map.GetHex(c);
                        LandProperties lp = h.Land_properties;
                        unsuitable = lp.Height > Constants.MountainsHBound; //not mountains
                        unsuitable = unsuitable || (lp.Height < Constants.OceanHBound && lp.Water > Constants.OceanWBound); //not ocean
                        unsuitable = unsuitable || h.Dungeon != null; // not atop of a dungeon
                        unsuitable = unsuitable || c == MyHex.coords; // not the same hex
                    }
                    Unit unit = SendUnit(c,Unit.Intent.Settle,Population.Total/5,MaterialsRequired(Population.Total / 5+10,0));
                    if (unit != null)
                    {
                        session.Save(unit);
                        session.Update(this);
                    }
                    else {
                        BuildHousing(Population.Total / 10);
                    }
                    return;
                }
            }
        }

    }


public class Dungeon
{
        long iD;
        Hex my_hex;
        string name;
        string displayed_name;
        int unitTypeId;
        Ress loot;
        int garrison;
        int garrisonIncome;
        int turns_before_income;
        int income_interval;
        int raider_party_size;
        int threat;

        public virtual Hex My_hex { get => my_hex; set => my_hex = value; }//mapped
        public virtual string Team { get; set; }//mapped
        public virtual long ID { get => iD; set => iD = value; }//mapped
        public virtual string Displayed_name { get => displayed_name; set => displayed_name = value; } //m
        public virtual string Name { get => name; set => name = value; } //m
        public virtual Ress Loot { get => loot; set => loot = value; } //m
        public virtual int Garrison { get => garrison; set => garrison = value; } //m
        public virtual int GarrisonIncome { get => garrisonIncome; set => garrisonIncome = value; } //m
        public virtual int Turns_before_income { get => turns_before_income; set => turns_before_income = value; }//m
        public virtual int Income_interval { get => income_interval; set => income_interval = value; }//m
        public virtual int Raider_party_size { get => raider_party_size; set => raider_party_size = value; }//m
        public virtual int UnitTypeId { get => unitTypeId; set => unitTypeId = value; }//m
        public virtual int Threat { get => threat; set => threat = value; }



        public static Dungeon FromTypes(int typeId) {
            Dungeon dungeon = new Dungeon();
            XmlDocument types = new XmlDocument();
            types.Load("./DungeonTypes.xml");
            XmlNode dun = types.SelectSingleNode($"DTypes/Dungeon[id = {typeId}]");
            dungeon.name = dun.SelectSingleNode("./name").InnerText;
            dungeon.displayed_name = dun.SelectSingleNode("./displayed").InnerText;
            dungeon.unitTypeId = Convert.ToInt32(dun.SelectSingleNode("./unitId").InnerText);
            dungeon.Garrison = Convert.ToInt32(dun.SelectSingleNode("./garrison").InnerText);
            dungeon.GarrisonIncome = Convert.ToInt32(dun.SelectSingleNode("./income").InnerText);
            dungeon.income_interval = Convert.ToInt32(dun.SelectSingleNode("./interval").InnerText);
            dungeon.Raider_party_size = Convert.ToInt32(dun.SelectSingleNode("./raiders").InnerText);
            dungeon.Threat = Convert.ToInt32(dun.SelectSingleNode("./threat").InnerText);
            dungeon.Loot = Ress.FromXml(dun.SelectSingleNode("./loot"));
            return dungeon;
        }


        public virtual Unit SendUnit(Coords Where, Unit.Intent intent, int amount) {
            if (My_hex.Unit != null)
                return null;
            if (amount > garrison)
                return null;
            Unit unit = Unit.NewFromTypes(UnitTypeId);
            unit.Alive = amount;
            Ress planned_upkeep = unit.Upkeep_per_person * (amount * Coords.Distance(My_hex.coords, Where) * 4);
            if (!Ress.Enough(loot, planned_upkeep)) {
                return null;
            }
            unit.Carrying = planned_upkeep;
            unit.Origin = My_hex.coords;
            unit.Target = Where;
            unit.Intention = intent;
            loot = loot - planned_upkeep;
            unit.My_hex = My_hex;
            my_hex.Unit = unit;
            Console.WriteLine($"Map:{My_hex.Map.ID}  Sent {unit.Alive} {unit.Name} from {unit.Origin} to {unit.Target}");
            return unit;
        }
        public virtual bool EnemyNear {
            get {
                List<Coords> cs = My_hex.coords.Adjacent;
                foreach (Coords c in cs)
                {
                    Hex hex = My_hex.Map.GetHex(c);
                    if (hex.Unit != null)
                    {
                        if (hex.Unit.Team != Team)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        public virtual Coords ClosestTarget {
            get {
                List<Settlement> settlements = my_hex.Map.AllSettlements;
                Coords coords = my_hex.coords;
                int distance = 10000;
                foreach (var s in settlements) {
                    if (Coords.Distance(s.MyHex.coords,my_hex.coords)<distance) {
                        if (s.Team != Team) {
                            distance = Coords.Distance(s.MyHex.coords, my_hex.coords);
                            coords = s.MyHex.coords;
                        }
                    }
                }
                return coords;
            }
        }

        
        public virtual void Update(ISession session) {
            
            if (EnemyNear) {
                if (My_hex.Unit == null) {
                    Unit sent = SendUnit(My_hex.coords, Unit.Intent.GoIntoDungeon, Garrison);
                    my_hex.Unit = sent;
                    session.Save(sent);
                    session.Update(this);
                }
                return;
            }
            Turns_before_income--;
            if (Turns_before_income < 1) {
                Turns_before_income = Income_interval;
                Garrison += GarrisonIncome;
            }
            Random random = new Random();
            if (random.Next(garrison) >= (raider_party_size / 4) * 3) {
                if (garrison >= raider_party_size) {
                    if (My_hex.Unit == null) {
                        Unit sent = SendUnit(ClosestTarget, Unit.Intent.RaidSettlementFD,raider_party_size);
                        if (sent != null)
                        {
                            Threat++;
                            my_hex.Unit = sent;
                            session.Save(sent);
                        }
                       
                    }
                }
            }
            session.Update(this);


        }
    }

}
