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
        private int[,,] move_set, move_curr, atk_set, atk_curr;

        public General(int _player)
        {
            player = _player;

            location = new int[3];
            MoveTo(-1, -1, -1);

            move_set = new int[3,9,9];
            SetMovements();

            move_curr = new int[3,9,9];
        }

        public void Main()
        {
            SetMovements();
            Console.Write(move_set);
            Console.Write(move_set);
        }

        public void SetMovements()
        {
            //move_set must be complete moveset for this tile, but have all tiers available
            move_set = Constants.GetM(Type.General);
        }

        public void SetProperties()
        {

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
            return move_curr;
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

        public void CalcMoves(int[,,] _board, int _elevated_tier)    //using int representation for pieces, negative if enemy, 0 if empty
        {//get rid of _elevated_tier by making that calculate in here later
            int[,,] possible = new int[3, 9, 9];
            bool lt_lead = false;

            int curr_t = Math.Max(location[0], _elevated_tier);
            int[] elevated_location = new int[3] { curr_t, location[1], location[2] };

            int[,,] los = Array.LineOfSight(move_set, elevated_location, _board, out lt_lead);//do stuff with this

            for (int t = 0; t < 3; t++)
            {
                for (int r = 0; r < 9; r++)
                {
                    for (int f = 0; f < 9; f++)
                    {
                        if (lt_lead)
                        {
                            for (int m_t = 0; m_t < 3; m_t++)          //if _lt_lead: cover all three movement tiers
                            {
                                possible[t, r, f] |= move_set[m_t, r, f] & ((_board[t, r, f] == 0) ? 1: 0);
                            }
                        }
                        else
                        {
                            possible[t, r, f] |= move_set[curr_t, r, f] & ((_board[t, r, f] == 0) ? 1 : 0);
                        }
                    }
                }
            }

            //deal with line of sight; for all except ltgeneral, extend out in 8 directions. When any piece is encountered, stop that direction

            move_curr = possible;
        }

        public void CalcAttacks(int[,,] _open)
        {

        }
    }
}
