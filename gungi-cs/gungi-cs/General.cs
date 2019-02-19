using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gungi_cs
{
    public class General : IPiece
    {
        private int player;
        private int[] location;
        private int[][][] move_set, atk_set;
        private int[][][] move_curr, atk_curr, drop_curr;

        public General(int _player)
        {
            player = _player;

            location = new int[3];
            MoveTo(-1, -1, -1);

            move_set = new int[3][][];
            atk_set = new int[3][][];
            SetMovements();

            move_curr = new int[3][][];
            atk_curr = new int[3][][];
        }

        public void Main()
        {
            SetMovements();
            Console.Write(move_set);
        }

        public void SetMovements()
        {
            move_set = Constants.GetM(Type.General);
            atk_set = move_set;
        }

        public void SetProperties()
        {

        }

        public int GetPlayer()
        {
            return player;
        }

        public int[][][] GetMoves()
        {
            return move_curr;
        }

        public int[][][] GetAttacks()
        {
            return atk_curr;
        }

        public int[] GetLocation()
        {
            return location;
        }

        public void MoveTo(int _rank, int _file, int _tier)
        {
            location[0] = _tier;
            location[1] = _rank;
            location[2] = _file;
        }

        public void CalcMoves(int[][][] _open_tiles)
        {

        }

        public void CalcAttacks(int[][][] _open_tiles)
        {

        }

        public void CalcDrops(int[][][] _open_tiles)
        {

        }
    }
}
