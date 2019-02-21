using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gungi_cs
{
    public class P
    {
        public static int
            EMP = 0,
            MAR = 1,
            SPY = 2,
            LIE = 3,
            MAJ = 4,
            GEN = 5,
            ARC = 6,
            KNI = 7,
            SAM = 8,
            CAN = 9,
            COU = 10,
            FOR = 11,
            MUS = 12,
            PAW = 13,
            ANY = 14;
    }

    public enum Type
    {
        General,
        Lt_General,
        Mj_General,
        Fortress,
        Counsel,
        Cannon,
        Musketeer,
        Samurai,
        Knight,
        Spy,
        Archer,
        Pawn,
        Marshal
    }

    public enum Property
    {
        AttackStyle,
        LeadStyle,
        StackTop,
        StackBottom,
        DropCheck,
        PawnStart
    }

    public enum AttackStyle { Default, Teleport, Jump };
    public enum LeadStyle { Default, Lead, Elevate };

    public class Constants
    {
        private static Dictionary<Tuple<Type, Property>, int> table = new Dictionary<Tuple<Type, Property>, int>();
        private static Dictionary<Type, int[,,]> movements = new Dictionary<Type, int[,,]>();
        private static int[,,] moveset = new int[3,9,9];

        public Constants()
        {
            table.Add(new Tuple<Type, Property>(Type.General, Property.AttackStyle), (int)AttackStyle.Default);
            table.Add(new Tuple<Type, Property>(Type.General, Property.LeadStyle), (int)LeadStyle.Lead);
            table.Add(new Tuple<Type, Property>(Type.General, Property.StackTop), 1);
            table.Add(new Tuple<Type, Property>(Type.General, Property.StackBottom), 1);
            table.Add(new Tuple<Type, Property>(Type.General, Property.DropCheck), 1);
            table.Add(new Tuple<Type, Property>(Type.General, Property.PawnStart), 0);

            table.Add(new Tuple<Type, Property>(Type.Lt_General, Property.AttackStyle), (int)AttackStyle.Default);
            table.Add(new Tuple<Type, Property>(Type.Lt_General, Property.LeadStyle), (int)LeadStyle.Default);
            table.Add(new Tuple<Type, Property>(Type.Lt_General, Property.StackTop), 1);
            table.Add(new Tuple<Type, Property>(Type.Lt_General, Property.StackBottom), 1);
            table.Add(new Tuple<Type, Property>(Type.Lt_General, Property.DropCheck), 1);
            table.Add(new Tuple<Type, Property>(Type.Lt_General, Property.PawnStart), 0);

            table.Add(new Tuple<Type, Property>(Type.Mj_General, Property.AttackStyle), (int)AttackStyle.Default);
            table.Add(new Tuple<Type, Property>(Type.Mj_General, Property.LeadStyle), (int)LeadStyle.Default);
            table.Add(new Tuple<Type, Property>(Type.Mj_General, Property.StackTop), 1);
            table.Add(new Tuple<Type, Property>(Type.Mj_General, Property.StackBottom), 1);
            table.Add(new Tuple<Type, Property>(Type.Mj_General, Property.DropCheck), 1);
            table.Add(new Tuple<Type, Property>(Type.Mj_General, Property.PawnStart), 0);

            table.Add(new Tuple<Type, Property>(Type.Fortress, Property.AttackStyle), (int)AttackStyle.Default);
            table.Add(new Tuple<Type, Property>(Type.Fortress, Property.LeadStyle), (int)LeadStyle.Elevate);
            table.Add(new Tuple<Type, Property>(Type.Fortress, Property.StackTop), 1);                              //0
            table.Add(new Tuple<Type, Property>(Type.Fortress, Property.StackBottom), 1);
            table.Add(new Tuple<Type, Property>(Type.Fortress, Property.DropCheck), 1);
            table.Add(new Tuple<Type, Property>(Type.Fortress, Property.PawnStart), 0);

            table.Add(new Tuple<Type, Property>(Type.Counsel, Property.AttackStyle), (int)AttackStyle.Default);
            table.Add(new Tuple<Type, Property>(Type.Counsel, Property.LeadStyle), (int)LeadStyle.Default);
            table.Add(new Tuple<Type, Property>(Type.Counsel, Property.StackTop), 1);
            table.Add(new Tuple<Type, Property>(Type.Counsel, Property.StackBottom), 1);
            table.Add(new Tuple<Type, Property>(Type.Counsel, Property.DropCheck), 1);
            table.Add(new Tuple<Type, Property>(Type.Counsel, Property.PawnStart), 0);

            table.Add(new Tuple<Type, Property>(Type.Cannon, Property.AttackStyle), (int)AttackStyle.Jump);
            table.Add(new Tuple<Type, Property>(Type.Cannon, Property.LeadStyle), (int)LeadStyle.Default);
            table.Add(new Tuple<Type, Property>(Type.Cannon, Property.StackTop), 1);
            table.Add(new Tuple<Type, Property>(Type.Cannon, Property.StackBottom), 1);
            table.Add(new Tuple<Type, Property>(Type.Cannon, Property.DropCheck), 1);
            table.Add(new Tuple<Type, Property>(Type.Cannon, Property.PawnStart), 0);

            table.Add(new Tuple<Type, Property>(Type.Musketeer, Property.AttackStyle), (int)AttackStyle.Default);
            table.Add(new Tuple<Type, Property>(Type.Musketeer, Property.LeadStyle), (int)LeadStyle.Default);
            table.Add(new Tuple<Type, Property>(Type.Musketeer, Property.StackTop), 1);
            table.Add(new Tuple<Type, Property>(Type.Musketeer, Property.StackBottom), 1);
            table.Add(new Tuple<Type, Property>(Type.Musketeer, Property.DropCheck), 1);
            table.Add(new Tuple<Type, Property>(Type.Musketeer, Property.PawnStart), 0);

            table.Add(new Tuple<Type, Property>(Type.Samurai, Property.AttackStyle), (int)AttackStyle.Default);
            table.Add(new Tuple<Type, Property>(Type.Samurai, Property.LeadStyle), (int)LeadStyle.Default);
            table.Add(new Tuple<Type, Property>(Type.Samurai, Property.StackTop), 1);
            table.Add(new Tuple<Type, Property>(Type.Samurai, Property.StackBottom), 1);
            table.Add(new Tuple<Type, Property>(Type.Samurai, Property.DropCheck), 1);
            table.Add(new Tuple<Type, Property>(Type.Samurai, Property.PawnStart), 0);

            table.Add(new Tuple<Type, Property>(Type.Knight, Property.AttackStyle), (int)AttackStyle.Teleport);
            table.Add(new Tuple<Type, Property>(Type.Knight, Property.LeadStyle), (int)LeadStyle.Default);
            table.Add(new Tuple<Type, Property>(Type.Knight, Property.StackTop), 1);
            table.Add(new Tuple<Type, Property>(Type.Knight, Property.StackBottom), 1);
            table.Add(new Tuple<Type, Property>(Type.Knight, Property.DropCheck), 1);
            table.Add(new Tuple<Type, Property>(Type.Knight, Property.PawnStart), 0);

            table.Add(new Tuple<Type, Property>(Type.Spy, Property.AttackStyle), (int)AttackStyle.Default);
            table.Add(new Tuple<Type, Property>(Type.Spy, Property.LeadStyle), (int)LeadStyle.Default);
            table.Add(new Tuple<Type, Property>(Type.Spy, Property.StackTop), 1);
            table.Add(new Tuple<Type, Property>(Type.Spy, Property.StackBottom), 1);
            table.Add(new Tuple<Type, Property>(Type.Spy, Property.DropCheck), 1);
            table.Add(new Tuple<Type, Property>(Type.Spy, Property.PawnStart), 0);

            table.Add(new Tuple<Type, Property>(Type.Archer, Property.AttackStyle), (int)AttackStyle.Teleport);
            table.Add(new Tuple<Type, Property>(Type.Archer, Property.LeadStyle), (int)LeadStyle.Default);
            table.Add(new Tuple<Type, Property>(Type.Archer, Property.StackTop), 1);
            table.Add(new Tuple<Type, Property>(Type.Archer, Property.StackBottom), 1);
            table.Add(new Tuple<Type, Property>(Type.Archer, Property.DropCheck), 1);
            table.Add(new Tuple<Type, Property>(Type.Archer, Property.PawnStart), 0);

            table.Add(new Tuple<Type, Property>(Type.Pawn, Property.AttackStyle), (int)AttackStyle.Default);
            table.Add(new Tuple<Type, Property>(Type.Pawn, Property.LeadStyle), (int)LeadStyle.Default);
            table.Add(new Tuple<Type, Property>(Type.Pawn, Property.StackTop), 1);
            table.Add(new Tuple<Type, Property>(Type.Pawn, Property.StackBottom), 1);
            table.Add(new Tuple<Type, Property>(Type.Pawn, Property.DropCheck), 0);
            table.Add(new Tuple<Type, Property>(Type.Pawn, Property.PawnStart), 1);

            table.Add(new Tuple<Type, Property>(Type.Marshal, Property.AttackStyle), (int)AttackStyle.Default);
            table.Add(new Tuple<Type, Property>(Type.Marshal, Property.LeadStyle), (int)LeadStyle.Default);
            table.Add(new Tuple<Type, Property>(Type.Marshal, Property.StackTop), 1);
            table.Add(new Tuple<Type, Property>(Type.Marshal, Property.StackBottom), 0);
            table.Add(new Tuple<Type, Property>(Type.Marshal, Property.DropCheck), 1);
            table.Add(new Tuple<Type, Property>(Type.Marshal, Property.PawnStart), 0);
        }

        public static int GetP(Type _type, Property _property)
        {
            return table[new Tuple<Type, Property>(_type, _property)];
        }

        public static int[,,] GetM(Type _type)
        {
            return movements[_type];
        }
    }
}
