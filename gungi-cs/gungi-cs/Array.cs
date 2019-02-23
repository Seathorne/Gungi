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
        int[,,] board, board_open, board_threatened;
        int[,] board_top;
        int[][,,] board_;

        // Modifiers
        int[,] lt_gen_sight, fort_range;

        // Valid Moves
        HashSet<Piece> pieces, board_pieces, top_pieces,
            unstackable_pieces, leading_pieces, elevating_pieces, jumping_pieces, teleporting_pieces, quickstart_pieces;
        
        public Array()
        {
            Clear();
        }

        public void Clear()
        {
            player_color = P.NA;

            board = new int[P.TM, P.RM, P.FM];
            board_open = new int[P.TM, P.RM, P.FM];
            board_threatened = new int[P.TM, P.RM, P.FM];
            board_top = new int[P.RM, P.FM];
            board_ = new int[][,,] { new int[P.TM, P.RM, P.FM], new int[P.TM, P.RM, P.FM], new int[P.TM, P.RM, P.FM] };

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
            quickstart_pieces = new HashSet<Piece>();
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
            //Print("Black", board_[P.BL]);
            //Print("White", board_[P.WH]);
            //Print("Empty", board_[P.NA]);
            Print("Open", board_open);
            Print("Top", board_top);
        }

        private void UpdateBoardStates()
        {
            Console.WriteLine("UPDATING BOARD STATES");
            Print("PIECES", pieces);
            Print("BOARD PIECES", board_pieces);

            foreach (Piece p in board_pieces)
            {
                board[p.T(), p.R(), p.F()] = p.Sym();                   // Construct the board array.
                board_[p.Color()][p.T(), p.R(), p.F()] = 1;             // Add piece to black, white, or empty board.

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
                if (p.QuickStarts())
                {
                    quickstart_pieces.Add(p);
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
                                board_open[t + 1, r, f] = 1;            // If this piece is in tier 1 or 2, make the cell above it open.
                            }
                            break;
                        }
                        else if (Empty(cell) && t == 0)
                        {
                            board_open[t, r, f] = 1;                    // If a spot in tier 1 is empty, make it open.
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
                }
                else
                {
                    p.SetTop(false);
                }
            }
        }

        private void UpdateMoves()
        {
            int[,,] moveset = new int[P.TM, P.RM, P.FM];
            int[,,] attackset = new int[P.TM, P.RM, P.FM];

            foreach (Piece p in pieces)
            {
                if (p.OnBoard())
                {
                    if (p.JumpAttacks())
                    {
                        UpdateLineOfSight(p, !p.JumpAttacks());
                    }
                    UpdateLineOfSight(p, p.JumpAttacks());
                    Console.WriteLine(p.Sym() + " " + p.LeadingPieceInSight().ToString());

                }
                p.SetMoves(moveset);
                p.SetAttacks(attackset);
            }
            for (int t = 0; t < P.TM; t++)
            {
                for (int r = 0; r < P.RM; r++)
                {
                    for (int f = 0; f < P.FM; f++)
                    {
                        //Combine(out valid_moves[player_color][_location], out valid_attacks[player_color][_location], moveset, _location, board, jump);




                        //valid_moves[player_color][piece.Location()] = moveset;
                        //valid_attacks[player_color][piece.Location()] = attackset;
                    }
                }
            }
        }

        private void UpdateLineOfSight(Piece _piece, bool _jump_attacks)
        {
            int[,] line_of_sight = new int[P.RM, P.FM];
            bool leading_piece_in_sight = false;

            for(int dir = 0; dir < P.NUM_DIR; dir++)
            {
                bool jumped = false;

                int sight_length = SightLength(dir, _piece.R(), _piece.F(), out int r_sign, out int f_sign);
                for (int i = 1; i <= sight_length; i++)
                {
                    int r_ = _piece.R() + r_sign * i;
                    int f_ = _piece.F() + f_sign * i;

                    if (!_jump_attacks || jumped)
                    {
                        line_of_sight[r_, f_] = 1;
                    }

                    if (!_jump_attacks)
                    {
                        foreach (Piece l_p in leading_pieces)
                        {
                            leading_piece_in_sight |= ((l_p.Color() == _piece.Color()) && (l_p.R() == r_) && (l_p.F() == f_));
                        }
                    }

                    if (!Empty(board_top[r_, f_]))
                    {
                        if (!_jump_attacks || jumped) break;

                        line_of_sight[r_, f_] = 0;
                        jumped = true;
                    }
                }
            }

            if (_jump_attacks)
            {
                _piece.SetAttackLineOfSight(line_of_sight);
            }
            else
            {
                _piece.SetLineOfSight(line_of_sight);
                _piece.SetLeadingPieceInSight(leading_piece_in_sight);
            }
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
            if (_piece < 0) return P.BL;
            else if (_piece > 0) return P.WH;
            else return P.NA;
        }

        private int Type(int _piece)
        {
            return Math.Abs(_piece);
        }

        private bool Empty(int _piece)
        {
            return (Color(_piece) == P.NA);
        }

        private bool Friendly(int _piece)
        {
            return (!Empty(_piece) && Color(_piece) == player_color);
        }

        private bool IsPiece(int _piece, bool _friendly, int _desired_piece_type)
        {
            return Type(_piece) == _desired_piece_type && Friendly(_piece) == _friendly;
        }

        public void Print(String _word, HashSet<Piece> _pieces)
        {
            String ret = _word + "\n";
            foreach (Piece p in _pieces)
            {
                ret += p.ToString() + ", ";
            }
            Console.WriteLine(ret);
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

        public int[,] Top(int[,,] _board)
        {
            int[,] ret = new int[9, 9];

            for (int r = 0; r < 9; r++)
            {
                for (int f = 0; f < 9; f++)
                {
                    for (int t = 2; t >= 0; t--)
                    {
                        if (_board[t, r, f] != 0)
                        {
                            ret[r, f] = _board[t, r, f];
                            break;
                        }
                    }
                }
            }

            return ret;
        }

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

        public int[,] LineOfSight(int[] _location, int[,,] _board, out bool _lt_gen_in_sight, out int _elevated_tier, bool _jump)
        {
            int[,] los = new int[9, 9];
            int[,] top_board = Top(_board);

            int curr_t = _location[0];
            int curr_r = _location[1];
            int curr_f = _location[2];
            // TODO Enemy ltgen and fort
            _lt_gen_in_sight = false;
            _elevated_tier = curr_t;

            int ul = Math.Min(curr_r, curr_f);
            int uu = curr_r;
            int ur = Math.Min(curr_r, 8 - curr_f);
            int ll = curr_f;
            int rr = 8 - curr_f;
            int dl = Math.Min(8 - curr_r, curr_f);
            int dd = 8 - curr_r;
            int dr = Math.Min(8 - curr_r, 8 - curr_f);

            los[curr_r, curr_f] = 1;                                // Center

            bool _jumped = !_jump;
            for (int i = 1; i <= ul; i++)                           // Up-Left
            {
                int r_ = curr_r - i;
                int f_ = curr_f - i;
                los[r_, f_] = 1;
                if (top_board[r_, f_] != 0)
                {
                    _lt_gen_in_sight |= (IsPiece(top_board[r_, f_], true, P.LIE));
                    if (_jumped) break;
                    else
                    {
                        los[r_, f_] = 0;
                        _jumped = true;
                    }
                }
            }
            _jumped = !_jump;
            for (int i = 1; i <= uu; i++)                           // Up
            {
                int r_ = curr_r - i;
                int f_ = curr_f;
                los[r_, f_] = 1;
                if (top_board[r_, f_] != 0)
                {
                    _lt_gen_in_sight |= (IsPiece(top_board[r_, f_], true, P.LIE));
                    if (_jumped) break;
                    else
                    {
                        los[r_, f_] = 0;
                        _jumped = true;
                    }
                }
            }
            _jumped = !_jump;
            for (int i = 1; i <= ur; i++)                           // Up-Right
            {
                int r_ = curr_r - i;
                int f_ = curr_f + i;
                los[r_, f_] = 1;
                if (top_board[r_, f_] != 0)
                {
                    _lt_gen_in_sight |= (IsPiece(top_board[r_, f_], true, P.LIE));
                    if (_jumped) break;
                    else
                    {
                        los[r_, f_] = 0;
                        _jumped = true;
                    }
                }
            }
            _jumped = !_jump;
            for (int i = 1; i <= ll; i++)                           // Left
            {
                int r_ = curr_r;
                int f_ = curr_f - i;
                los[r_, f_] = 1;
                if (top_board[r_, f_] != 0)
                {
                    _lt_gen_in_sight |= (IsPiece(top_board[r_, f_], true, P.LIE));
                    if (_jumped) break;
                    else
                    {
                        los[r_, f_] = 0;
                        _jumped = true;
                    }
                }
            }
            _jumped = !_jump;
            for (int i = 1; i <= rr; i++)                           // Right
            {
                int r_ = curr_r;
                int f_ = curr_f + i;
                los[r_, f_] = 1;
                if (top_board[r_, f_] != 0)
                {
                    _lt_gen_in_sight |= (IsPiece(top_board[r_, f_], true, P.LIE));
                    if (_jumped) break;
                    else
                    {
                        los[r_, f_] = 0;
                        _jumped = true;
                    }
                }
            }
            _jumped = !_jump;
            for (int i = 1; i <= dl; i++)                           // Down-Left
            {
                int r_ = curr_r + i;
                int f_ = curr_f - i;
                los[r_, f_] = 1;
                if (top_board[r_, f_] != 0)
                {
                    _lt_gen_in_sight |= (IsPiece(top_board[r_, f_], true, P.LIE));
                    if (_jumped) break;
                    else
                    {
                        los[r_, f_] = 0;
                        _jumped = true;
                    }
                }
            }
            _jumped = !_jump;
            for (int i = 1; i <= dd; i++)                           // Down
            {
                int r_ = curr_r + i;
                int f_ = curr_f;
                los[r_, f_] = 1;
                if (top_board[r_, f_] != 0)
                {
                    _lt_gen_in_sight |= (IsPiece(top_board[r_, f_], true, P.LIE));
                    if (_jumped) break;
                    else
                    {
                        los[r_, f_] = 0;
                        _jumped = true;
                    }
                }
            }
            _jumped = !_jump;
            for (int i = 1; i <= dr; i++)                           // Down-Right
            {
                int r_ = curr_r + i;
                int f_ = curr_f + i;
                los[r_, f_] = 1;
                if (top_board[r_, f_] != 0)
                {
                    _lt_gen_in_sight |= (IsPiece(top_board[r_, f_], true, P.LIE));
                    if (_jumped) break;
                    else
                    {
                        los[r_, f_] = 0;
                        _jumped = true;
                    }
                }
            }

            if (!_lt_gen_in_sight)
            {
                _elevated_tier = ElevatedTier(_location, _board, top_board);
            }

            return los;
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
        }

        public void OpenBoard(int[,,] _board, out int[,,] _open_empty, out int[,,] _open_enemy)
        {
            _open_empty = new int[3, 9, 9];
            _open_enemy = new int[3, 9, 9];

            for (int r = 0; r < 9; r++)
            {
                for (int f = 0; f < 9; f++)
                {
                    for (int t = 2; t >= 0; t--)
                    {
                        if (IsPiece(_board[t, r, f], false, P.MAR))    //enemy marshal
                        {
                            _open_enemy[t, r, f] = 1;
                            break;
                        }
                        else if (IsPiece(_board[t, r, f], true, P.MAR))    //friendly marshal
                        {
                            break;
                        }
                        else if (_board[t, r, f] != 0)
                        {
                            if (!Friendly(_board[t, r, f]))      //enemy
                            {
                                _open_enemy[t, r, f] = 1;
                            }
                            if(t < 2)
                            {
                                _open_empty[t + 1, r, f] = 1;
                            }
                            break;
                        }
                        else if (t == 0)
                        {
                            _open_empty[t, r, f] = 1;
                            break;
                        }
                    }
                }
            }
            return;
        }

        public void Combine(out int[,,] _valid_moves, out int[,,] _valid_captures, int[,,] _move_set, int[] _location, int[,,] _board, bool _jump)
        {
            _valid_moves = new int[3, 9, 9];
            _valid_captures = new int[3, 9, 9];

            // Need los exceptions for LtGen3, Archer2/3, Knight
            int[,] los = LineOfSight(_location, _board, out bool lt_gen_in_sight, out int e_tier, _jump);

            OpenBoard(_board, out int[,,] board_open, out int[,,] board_enemy);

            for (int t = 0; t < 3; t++)
            {
                for (int r = 0; r < 9; r++)
                {
                    for (int f = 0; f < 9; f++)
                    {
                        _valid_moves[t, r, f] = board_open[t, r, f] & _move_set[e_tier, r, f] & los[r, f];
                        _valid_captures[t, r, f] = board_enemy[t, r, f] & _move_set[e_tier, r, f] & los[r, f];

                        if (lt_gen_in_sight)
                        {
                            for (int m_t = 0; m_t < 3; m_t++)
                            {
                                _valid_moves[t, r, f] |= board_open[t, r, f] & _move_set[m_t, r, f] & los[r, f];
                                _valid_captures[t, r, f] |= board_enemy[t, r, f] & _move_set[m_t, r, f] & los[r, f];
                            }
                        }
                    }
                }
            }
            _valid_moves[_location[0], _location[1], _location[2]] = 1;
            return;
        }

    }
}
