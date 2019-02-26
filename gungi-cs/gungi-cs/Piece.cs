using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gungi_cs
{
    class Piece
    {
        private int player_color, type, elevating_tier;
        private int[] location;
        private int[,,] moveset, moves, attacks, drops;
        private int[,] line_of_sight, attack_line_of_sight;
        private bool in_hand, on_board, top, in_leading_sight;

        public Piece(int _player_color, int _piece_number)
        {
            player_color = _player_color;
            type = _piece_number;

            elevating_tier = 0;

            location = new int[3];
            moveset = new int[P.TM, P.RM, P.FM];
            moves = new int[P.TM, P.RM, P.FM];
            attacks = new int[P.TM, P.RM, P.FM];
            drops = new int[P.TM, P.RM, P.FM];

            line_of_sight = new int[P.RM, P.FM];
            attack_line_of_sight = new int[P.RM, P.FM];
            in_leading_sight = false;
            top = false;

            PlaceInHand();
        }

        private void SetLocation(int _tier, int _rank, int _file)
        {
            location[P.Ti] = _tier;
            location[P.Ri] = _rank;
            location[P.Fi] = _file;

            if (_tier >= 0 && _tier < P.TM && _rank >= 0 && _rank < P.RM && _file >= 0 && _file < P.FM)
            {
                PlaceOnBoard();
                SetMoveset();
            }
        }

        private void PlaceInHand()
        {
            in_hand = true;
            on_board = false;

            SetLocation(P.HAND, P.HAND, P.HAND);
        }

        private void PlaceOnBoard()
        {
            in_hand = false;
            on_board = true;
        }

        public void GetsAttacked()
        {
            in_hand = false;
            on_board = false;

            SetLocation(P.CAPTURED, P.CAPTURED, P.CAPTURED);
        }

        public bool CanDropTo(int[] _location)
        {
            return (in_hand && drops[_location[P.Ti], _location[P.Ri], _location[P.Fi]] == 1);
        }

        public bool CanMoveTo(int[] _location)
        {
            return (on_board && top && moves[_location[P.Ti], _location[P.Ri], _location[P.Fi]] == 1);
        }

        public bool CanAttackTo(int[] _location)
        {
            return (on_board && top && attacks[_location[P.Ti], _location[P.Ri], _location[P.Fi]] == 1);
        }

        public void MoveTo(int[] _location)
        {
            SetLocation(_location[P.Ti], _location[P.Ri], _location[P.Fi]);
        }

        public int PlayerColor()
        {
            return player_color;
        }

        public int Type()
        {
            return type;
        }

        public int Sym()
        {
            return type * (player_color == P.BLACK ? -1 : 1);
        }

        public int[] Location()
        {
            return location;
        }

        public bool IsInLocation(int _t, int _r, int _f)
        {
            return (location[P.Ti] == _t && location[P.Ri] == _r && location[P.Fi] == _f);
        }

        public void SetLeadingPieceInSight(bool _in_sight)
        {
            in_leading_sight = _in_sight;
        }

        public bool LeadingPieceInSight()
        {
            return in_leading_sight;
        }

        public void SetElevatingTier(int _elevating_tier)
        {
            elevating_tier = _elevating_tier;
        }

        public int ElevatedTier()
        {
            return Math.Max(elevating_tier, T());
        }

        public int T()
        {
            return location[P.Ti];
        }

        public int R()
        {
            return location[P.Ri];
        }

        public int F()
        {
            return location[P.Fi];
        }

        private void SetMoveset()
        {
            if (!on_board) return;

            int[,,] ext_moveset = new int[P.TM, P.RM_EXT, P.FM_EXT];

            int shift_r = (P.RM_EXT - 1) / 2 - location[P.Ri];
            int shift_f = (P.RM_EXT - 1) / 2 - location[P.Fi];

            for (int t = 0; t < P.TM; t++)
            {
                
                for (int r = 0; r < P.RM_EXT; r++)
                {
                    for (int f = 0; f < P.FM_EXT; f++)
                    {
                        int flipped_r = (player_color == P.WHITE) ? (P.RM_EXT - 1 - r) : r;
                        ext_moveset[t, r, f] = Constants.GetMoves(type)[t, flipped_r, f];
                    }
                }

                for (int r = 0; r < P.RM; r++)
                {
                    for (int f = 0; f < P.FM; f++)
                    {
                        moveset[t, r, f] = ext_moveset[t, r + shift_r, f + shift_f];
                    }
                }
            }
        }

        public int[,,] Moveset()
        {
            return moveset;
        }

        public void SetLineOfSight(int[,] _line_of_sight)
        {
            line_of_sight = _line_of_sight;
        }

        public void SetAttackLineOfSight(int[,] _attack_line_of_sight)
        {
            attack_line_of_sight = _attack_line_of_sight;
        }

        public int[,] LineOfSight()
        {
            return line_of_sight;
        }

        public int[,] AttackLineOfSight()
        {
            if (!JumpAttacks())
            {
                return line_of_sight;
            }
            return attack_line_of_sight;
        }

        public void SetMoves(int[,,] _moves)
        {
            moves = _moves;
        }

        public int[,,] Moves()
        {
            if (top) return moves;
            else return new int[P.TM, P.RM, P.FM];
        }

        public int[,,] WouldBeMoves()
        {
            return moves;
        }

        public void SetAttacks(int[,,] _attacks)
        {
            attacks = _attacks;
        }

        public int[,,] Attacks()
        {
            if (top) return attacks;
            else return new int[P.TM, P.RM, P.FM];
        }

        public int[,,] WouldBeAttacks()
        {
            return attacks;
        }

        public void SetDrops(int[,,] _drops)
        {
            drops = _drops;
        }

        public int[,,] Drops()
        {
            return drops;
        }

        public bool InHand()
        {
            return in_hand;
        }

        public bool OnBoard()
        {
            return on_board;
        }

        public void SetTop(bool _top)
        {
            top = _top;
        }

        public bool Top()
        {
            return top;
        }

        public bool Alive()
        {
            return in_hand || on_board;
        }

        public bool Leads()
        {
            return (type == P.LIE);
        }

        public bool Elevates()
        {
            return (type == P.FOR);
        }

        public bool JumpAttacks()
        {
            return (type == P.CAN);
        }

        public bool Teleports()
        {
            return (type == P.ARC || type == P.KNI);
        }

        public char Char()
        {
            return P.ConvertPiece(Sym());
        }
    }
}
