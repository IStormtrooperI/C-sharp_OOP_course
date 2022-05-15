using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inheritance.MapObjects
{
    public class Dwelling : IOwner
    {
        public int Owner { get; set; }
    }

    public class Mine : IArmy, ITreasure, IOwner
    {
        public int Owner { get; set; }
        public Army Army { get; set; }
        public Treasure Treasure { get; set; }
    }

    public class Creeps : IArmy, ITreasure
    {
        public Army Army { get; set; }
        public Treasure Treasure { get; set; }
    }

    public class Wolves : IArmy
    {
        public Army Army { get; set; }
    }

    public class ResourcePile : ITreasure
    {
        public Treasure Treasure { get; set; }
    }

    public interface IArmy
    {
        Army Army { get; set; }
    }

    public interface ITreasure
    {
        Treasure Treasure { get; set; }
    }

    public interface IOwner
    {
        int Owner { get; set; }
    }

    public static class Interaction
    {
        public static void ResultOfFight(Player player)
        {
            player.Die();
        }
        
        public static void ResultOfGetTreasure(Player player, ITreasure treasure)
        {
            player.Consume(treasure.Treasure);
        }

        public static void ResultOfGetOwn(Player player, IOwner owner)
        {
            owner.Owner = player.Id;
        }

        public static void Make(Player player, object mapObject)
        {
            if (mapObject is IArmy army) 
            { 
                if (!player.CanBeat(army.Army))
                {
                    ResultOfFight(player);
                    return;
                }
            }
            if (mapObject is ITreasure treasure)
            {
                ResultOfGetTreasure(player, treasure);
            }
            if (mapObject is IOwner owner)
            {
                ResultOfGetOwn(player, owner);
            }
        }
    }
}
