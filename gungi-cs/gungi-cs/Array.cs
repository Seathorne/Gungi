using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gungi_cs
{
    class Array
    {
        // Game State
        int player_color;

        // Board State
        int[,,] board, board_threatened;
        int[,] board_top;
        int[][,,] board_open;

        // Modifiers
        int[,] lt_gen_sight, fort_range;

        // Valid Moves
        HashSet<Piece> pieces, board_pieces, top_pieces,
            unstackable_pieces, leading_pieces, elevating_pieces, jumping_pieces, teleporting_pieces;

        public Array()
        {
            Clear();
        }

        public void Clear()
        {
            player_color = P.EMPTY;

            board = new int[P.TM, P.RM, P.FM];
            board_threatened = new int[P.TM, P.RM, P.FM];
            board_top = new int[P.RM, P.FM];
            board_open = new int[][,,] { new int[P.TM, P.RM, P.FM], new int[P.TM, P.RM, P.FM], new int[P.TM, P.RM, P.FM] };

            lt_gen_sight = new int[P.RM, P.FM];
            fort_range = new int[P.RM, P.FM];

            pieces = new HashSet<Piece>();
            board_pieces = new HashSet<Piece>();
            top_pieces = new HashSet<Piece>();
            unstackable_pieces = new HashSet<Piece>();
            leading_pieces = new HashSet<Piece>();
            elevating_pieces = new HashSet<Piece>();
            jumping_pieces = new HashSet<Piece>();
            teleporting_pieces = new HashSet<Piece>();
        }

        public void Update(int _player_color, HashSet<Piece> _pieces, HashSet<Piece> _board_pieces)
        {
            Clear();

            player_color = _player_color;
            pieces = _pieces;
            board_pieces = _board_pieces;

            UpdateBoardStates();
            UpdateMoves();

            Print("Board", board);
            Print("Top", board_top);
            Print("Open", board_open[P.EMPTY]);
        }

        private void UpdateBoardStates()
        {
            foreach (Piece p in board_pieces)
            {
                board[p.T(), p.R(), p.F()] = p.Sym();                   // Construct the board array.

                if (p.IsUnstackable())                                  // Add special pieces to their own sets.
                {
                    unstackable_pieces.Add(p);
                }
                if (p.Leads())
                {
                    leading_pieces.Add(p);
                }
                if (p.Elevates())
                {
                    elevating_pieces.Add(p);
                }
                if (p.JumpAttacks())
                {
                    jumping_pieces.Add(p);
                }
                if (p.Teleports())
                {
                    teleporting_pieces.Add(p);
                }
            }

            for (int r = 0; r < P.RM; r++)
            {
                for (int f = 0; f < P.RM; f++)
                {
                    bool unstackable = false;
                    foreach (Piece p in unstackable_pieces)
                    {
                        unstackable = (r == p.R() && f == p.F());
                        if (unstackable)
                        {
                            board_top[r, f] = p.Sym();                  // If a marshal is found, it is the top piece, and the rest of the stack is not open.
                            break;
                        }
                    }

                    for (int t = P.TM - 1; t >= 0; t--)
                    {
                        int cell = board[t, r, f];

                        if (!unstackable && !Empty(cell))
                        {
                            board_top[r, f] = cell;                     // If a piece is found, it is the top piece.
                            if (t < P.TM - 1)
                            {
                                board_open[P.EMPTY][t + 1, r, f] = 1;            // If this piece is in tier 1 or 2, make the cell above it open.
                            }
                            break;
                        }
                        else if (Empty(cell) && t == 0)
                        {
                            board_open[P.EMPTY][t, r, f] = 1;                    // If a spot in tier 1 is empty, make it open.
                        }
                    }
                }
            }

            foreach (Piece p in board_pieces)
            {
                if (p.Sym() == board_top[p.R(), p.F()])
                {
                    top_pieces.Add(p);                                  // Add top pieces to their own set.
                    p.SetTop(true);
                    board_open[p.Color()][p.T(), p.R(), p.F()] = 1;     // Add piece to black or white board.
                }
                else
                {
                    p.SetTop(false);
                }
            }
        }

        private void UpdateMoves()
        {
            foreach (Piece p in pieces)
            {
                if (p.OnBoard())
                {
                    UpdateLineOfSight(p);
                    UpdateInLeadingSight(p);
                }

                UpdateX(p);

                if (p.OnBoard())
                {
                    Print("MOVES", p.Moves());
                    Print("ATTACKS", p.Attacks());
                }
            }
        }

        private void UpdateX(Piece _piece)
        {
            int[,,] moves = new int[P.TM, P.RM, P.FM];
            int[,,] attacks = new int[P.TM, P.RM, P.FM];

            if (_piece.OnBoard())
            {
                for (int t = 0; t < P.TM; t++)
                {
                    for (int r = 0; r < P.RM; r++)
                    {
                        for (int f = 0; f < P.FM; f++)
                        {
                            if (_piece.LeadingPieceInSight())
                            {
                                for (int m_t = 0; m_t < P.TM; m_t++)
                                {
                                    moves[t, r, f] |= _piece.Moveset()[m_t, r, f];
                                    attacks[t, r, f] |= _piece.Moveset()[P.TM - 1, r, f];
                                }
                            }
                            else
                            {
                                moves[t, r, f] = _piece.Moveset()[_piece.T(), r, f];
                                attacks[t, r, f] = _piece.Moveset()[P.TM - 1, r, f];
                            }
                            moves[t, r, f] &= board_open[P.EMPTY][t, r, f] & (_piece.Teleports() ? 1 : _piece.LineOfSight()[r, f]);
                            attacks[t, r, f] &= board_open[1 - _piece.Color()][t, r, f] & (_piece.Teleports() ? 1 : _piece.AttackLineOfSight()[r, f]);
                        }
                    }
                }
            }

            _piece.SetMoves(moves);
            _piece.SetAttacks(attacks);
        }

        private void UpdateLineOfSight(Piece _piece)
        {
            int[,] line_of_sight = new int[P.RM, P.FM];
            int[,] attack_line_of_sight = new int[P.RM, P.FM];

            for (int dir = 0; dir < P.NUM_DIR; dir++)
            {
                bool seen_piece = false;
                int jump_count = 0;

                int sight_length = SightLength(dir, _piece.R(), _piece.F(), out int r_sign, out int f_sign);
                for (int i = 1; i <= sight_length; i++)
                {
                    int r_ = _piece.R() + r_sign * i;
                    int f_ = _piece.F() + f_sign * i;

                    if (!seen_piece)
                    {
                        line_of_sight[r_, f_] = 1;
                        if (!Empty(board_top[r_, f_]))
                        {
                            seen_piece = true;
                        }
                    }

                    if (_piece.JumpAttacks())
                    {
                        if (jump_count == 1)
                        {
                            attack_line_of_sight[r_, f_] = 1;
                        }
                        if (!Empty(board_top[r_, f_]))
                        {
                            jump_count++;
                        }
                    }
                }
            }

            _piece.SetLineOfSight(line_of_sight);
            _piece.SetAttackLineOfSight(attack_line_of_sight);
        }

        private void UpdateInLeadingSight(Piece _piece)
        {
            bool in_sight = false;
            foreach (Piece l_p in leading_pieces)
            {
                in_sight |= (l_p.LineOfSight()[_piece.R(), _piece.F()] == 1 && l_p.Color() == _piece.Color());
                //crashes on second lt gen placement
            }
            _piece.SetLeadingPieceInSight(in_sight);
        }

        private int SightLength(int _dir, int _rank, int _file, out int _r, out int _f)
        {
            _r = 0;
            _f = 0;
            switch (_dir)
            {
                case P.UP_LEFT:
                    _r = -1; _f = -1;
                    return Math.Min(_rank, _file);
                case P.UP:
                    _r = -1;
                    return _rank;
                case P.UP_RIGHT:
                    _r = -1; _f = 1;
                    return Math.Min(_rank, P.FM - 1 - _file);
                case P.LEFT:
                    _f = -1;
                    return _file;
                case P.RIGHT:
                    _f = 1;
                    return P.FM - 1 - _file;
                case P.DOWN_LEFT:
                    _r = 1; _f = -1;
                    return Math.Min(P.RM - 1 - _rank, _file);
                case P.DOWN:
                    _r = 1;
                    return P.RM - 1 - _rank;
                case P.DOWN_RIGHT:
                    _r = 1; _f = 1;
                    return Math.Min(P.RM - 1 - _rank, P.FM - 1 - _file);
                default:
                    return 0;
            }
        }

        private int Color(int _piece)
        {
            if (_piece < 0) return P.BLACK;
            else if (_piece > 0) return P.WHITE;
            else return P.EMPTY;
        }

        private int Type(int _piece)
        {
            return Math.Abs(_piece);
        }

        private bool Empty(int _piece)
        {
            return (Color(_piece) == P.EMPTY);
        }

        private bool Friendly(int _piece)
        {
            return (!Empty(_piece) && Color(_piece) == player_color);
        }

        private bool IsPiece(int _piece, bool _friendly, int _desired_piece_type)
        {
            return Type(_piece) == _desired_piece_type && Friendly(_piece) == _friendly;
        }

        public void Print(String _word, int[,,] _array)
        {
            String ret = _word + "\n";
            for (int t = 0; t < _array.GetLength(0); t++)
            {
                for (int r = 0; r < _array.GetLength(1); r++)
                {
                    for (int f = 0; f < _array.GetLength(2); f++)
                    {
                        if (_array[t, r, f] > 9)
                        {
                            ret += " ";
                        }
                        else if (_array[t, r, f] >= 0)
                        {
                            ret += "  ";
                        }
                        else if (_array[t, r, f] < -9)
                        {
                        }
                        else if (_array[t, r, f] < 0)
                        {
                            ret += " ";
                        }
                        if (_array[t, r, f] == 0)
                        {
                            ret += "· ";
                        }
                        else
                        {
                            ret += _array[t, r, f] + " ";
                        }
                    }
                    ret += '\n';
                }
                ret += '\n';
            }
            Console.WriteLine(ret);
        }

        public void Print(String _word, int[,] _array)
        {
            String ret = _word + "\n";
            for (int r = 0; r < _array.GetLength(0); r++)
            {
                for (int f = 0; f < _array.GetLength(1); f++)
                {
                    if (_array[r, f] > 9)
                    {
                        ret += " ";
                    }
                    else if (_array[r, f] >= 0)
                    {
                        ret += "  ";
                    }
                    else if (_array[r, f] < -9)
                    {
                    }
                    else if (_array[r, f] < 0)
                    {
                        ret += " ";
                    }
                    if (_array[r, f] == 0)
                    {
                        ret += "· ";
                    }
                    else
                    {
                        ret += _array[r, f] + " ";
                    }
                }
                ret += '\n';
            }
            Console.WriteLine(ret);
        }
        /*
        public int StackHeight(int _r, int _f, int[,,] _board)
        {
            for (int t = 2; t >= 0; t--)
            {
                if (_board[t, _r, _f] != 0)
                {
                    return t;
                }
            }
            return 0;
        }

        public int ElevatedTier(int[] _location, int[,,] _board, int[,] _top_board)
        {
            int curr_t = _location[0];
            int curr_r = _location[1];
            int curr_f = _location[2];

            int e_tier = curr_t;
            if (e_tier == 2) return e_tier;

            bool not_top = (curr_r - 1 >= 0);
            bool not_bottom = (curr_r + 1 < 9);
            bool not_left = (curr_f - 1 >= 0);
            bool not_right = (curr_f + 1 < 9);

            if (not_top)
            {
                if (IsPiece(_top_board[curr_r - 1, curr_f], true, P.FOR))
                {
                    e_tier = Math.Min(2, Math.Max(e_tier, StackHeight(curr_r - 1, curr_f, _board) + 1));
                    if (e_tier == 2) return e_tier;
                }
                if (not_left)
                {
                    if (IsPiece(_top_board[curr_r, curr_f - 1], true, P.FOR))
                    {
                        e_tier = Math.Min(2, Math.Max(e_tier, StackHeight(curr_r, curr_f - 1, _board) + 1));
                        if (e_tier == 2) return e_tier;
                    }
                    if (IsPiece(_top_board[curr_r - 1, curr_f - 1], true, P.FOR))
                    {
                        e_tier = Math.Min(2, Math.Max(e_tier, StackHeight(curr_r - 1, curr_f - 1, _board) + 1));
                        if (e_tier == 2) return e_tier;
                    }
                }
                if (not_right)
                {
                    if (IsPiece(_top_board[curr_r, curr_f + 1], true, P.FOR))
                    {
                        e_tier = Math.Min(2, Math.Max(e_tier, StackHeight(curr_r, curr_f + 1, _board) + 1));
                        if (e_tier == 2) return e_tier;
                    }
                    if (IsPiece(_top_board[curr_r - 1, curr_f + 1], true, P.FOR))
                    {
                        e_tier = Math.Min(2, Math.Max(e_tier, StackHeight(curr_r - 1, curr_f + 1, _board) + 1));
                        if (e_tier == 2) return e_tier;
                    }
                }
            }
            if (not_bottom)
            {
                if (IsPiece(_top_board[curr_r + 1, curr_f], true, P.FOR))
                {
                    e_tier = Math.Min(2, Math.Max(e_tier, StackHeight(curr_r + 1, curr_f, _board) + 1));
                    if (e_tier == 2) return e_tier;
                }
                if (not_left)
                {
                    if (IsPiece(_top_board[curr_r + 1, curr_f - 1], true, P.FOR))
                    {
                        e_tier = Math.Min(2, Math.Max(e_tier, StackHeight(curr_r + 1, curr_f - 1, _board) + 1));
                        if (e_tier == 2) return e_tier;
                    }
                }
                if (not_right)
                {
                    if (IsPiece(_top_board[curr_r + 1, curr_f + 1], true, P.FOR))
                    {
                        e_tier = Math.Min(2, Math.Max(e_tier, StackHeight(curr_r + 1, curr_f + 1, _board) + 1));
                        if (e_tier == 2) return e_tier;
                    }
                }
            }

            return e_tier;
        }*/
    }
}
