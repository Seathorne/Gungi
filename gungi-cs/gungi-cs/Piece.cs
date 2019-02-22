using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gungi_cs
{
    class Piece
    {
        private int color, type;
        private int[] location;
        private int[,,] moveset, curr_moves, curr_attacks;
        private bool in_hand, on_board;

        public Piece(int _player_color, int _piece_number)
        {
            Init();

            color = _player_color;
            type = _piece_number;

            SetLocation(-1, -1, -1);
        }

        private void Init()
        {
            color = P.NA;
            type = P.EMP;

            moveset = new int[P.TM, P.RM, P.FM];
            location = new int[3];

            in_hand = true;
            on_board = false;
        }

        private void SetLocation(int _tier, int _rank, int _file)
        {
            if (_tier == -1 || _rank == -1 || _file == -1)
            {
                in_hand = true;
                on_board = false;
            }

            location[P.T] = _tier;
            location[P.R] = _rank;
            location[P.F] = _file;

            SetMoveset();
        }

        public void Drop(int _tier, int _rank, int _file)
        {
            if (in_hand)
            {
                in_hand = false;
                on_board = true;
                MoveTo(_tier, _rank, _file);
            }
        }

        public void MoveTo(int _tier, int _rank, int _file)
        {
            if (on_board)
            {
                SetLocation(_tier, _rank, _file);
            }
        }

        public void Capture()
        {
            in_hand = false;
            on_board = false;
            SetLocation(-2, -2, -2);
        }

        public int Color()
        {
            return color;
        }

        public int Type()
        {
            return type;
        }

        public int[] Location()
        {
            return location;
        }

        private void SetMoveset()
        {
            if (location[P.T] < 0 || location[P.R] < 0 || location[P.F] < 0) return;

            int shift_r = P.OC_R - location[P.R];
            int shift_f = P.OC_F - location[P.F];

            for (int t = 0; t < P.TM; t++)
            {
                for (int r = 0; r < P.RM; r++)
                {
                    for (int f = 0; f < P.FM; f++)
                    {
                        moveset[t, r, f] = Constants.GetMoves(type)[t, r + shift_r, f + shift_f];
                    }
                }
            }
        }

        public int[,,] Moveset()
        {
            return moveset;
        }

        public void SetCurrMoves(int[,,] _moveset)
        {
            curr_moves = _moveset;
        }

        public int[,,] CurrMoves()
        {
            return curr_moves;
        }

        public void SetCurrAttacks(int[,,] _attackset)
        {
            curr_attacks = _attackset;
        }

        public int[,,] CurrAttacks()
        {
            return curr_attacks;
        }

        public bool InHand()
        {
            return in_hand;
        }

        public bool OnBoard()
        {
            return on_board;
        }

        public bool Alive()
        {
            return in_hand || on_board;
        }

        public bool Unstackable()
        {
            return (type == P.MAR);
        }

        public bool Leads()
        {
            return (type == P.LIE);
        }

        public bool Elevates()
        {
            return (type == P.FOR);
        }

        public bool Jumps()
        {
            return (type == P.CAN);
        }

        public bool Teleports()
        {
            return (type == P.ARC || type == P.KNI);
        }

        public bool PawnStarts()
        {
            return (type == P.PAW);
        }
    }
}
