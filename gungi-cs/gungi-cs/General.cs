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
        private int[,,] move_set, move_curr, atk_curr;
        private const bool JUMP = false;

        public General(int _player)
        {
            player = _player;

            location = new int[3];

            move_set = Constants.GetMoves(P.GEN);

            move_curr = new int[3, 9, 9];
            atk_curr = new int[3, 9, 9];
        }

        public void Main()
        {
            // Test
        }

        public int GetPlayer()
        {
            return player;
        }

        public int[,,] GetMoves()
        {
            return move_curr;
        }

        public int[,,] GetAttacks()
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

        public void CalcMoves(int[,,] _board)
        {
            Array.Combine(out move_curr, out atk_curr, move_set, location, _board, JUMP);
        }
    }
}
