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

        public General(int _player, int _t, int _r, int _f)
        {
            player = _player;

            location = new int[3];
            move_set = new int[3, 9, 9];
            move_curr = new int[3, 9, 9];
            atk_curr = new int[3, 9, 9];

            MoveTo(_t, _r, _f);
            SetMoves();
        }

        public void SetMoves()
        {
            int orig_r = 8, orig_f = 8;
            int shift_r = orig_r - location[1];
            int shift_f = orig_f - location[2];

            for (int t = 0; t < 3; t++)
            {
                for (int r = 0; r < 9; r++)
                {
                    for (int f = 0; f < 9; f++)
                    {
                        move_set[t, r, f] = Constants.GetMoves(P.SPY)[t, r + shift_r, f + shift_f];
                    }
                }
            }
        }

        public int GetPlayer()
        {
            return player;
        }

        public int[,,] GetMoveSet()
        {
            return move_set;
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

        public void MoveTo(int _tier, int _rank, int _file)
        {
            location[0] = _tier;
            location[1] = _rank;
            location[2] = _file;
        }

        public void CalcMoves(int[,,] _board)
        {
            Array array = new Array();
            array.Combine(out move_curr, out atk_curr, move_set, location, _board, JUMP);
        }
    }
}
