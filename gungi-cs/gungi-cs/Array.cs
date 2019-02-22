using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gungi_cs
{
    class Array
    {

        public static int[,] Top(int[,,] _board)
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

        public static int StackHeight(int _r, int _f, int[,,] _board)
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

        public static int[,] LineOfSight(int[] _location, int[,,] _board, out bool _lt_gen_in_sight, out int _elevated_tier, bool _jump)
        {
            int[,] los = new int[9, 9];
            int[,] top_board = Top(_board);

            int curr_t = _location[0];
            int curr_r = _location[1];
            int curr_f = _location[2];

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
                if (top_board[r_, f_] != 0)
                {
                    _lt_gen_in_sight |= (top_board[r_, f_] == P.LIE);
                    if (_jumped) break;
                    else _jumped = true;
                }
                los[r_, f_] = 1;
            }
            _jumped = !_jump;
            for (int i = 1; i <= uu; i++)                           // Up
            {
                int r_ = curr_r - i;
                int f_ = curr_f;
                if (top_board[r_, f_] != 0)
                {
                    _lt_gen_in_sight |= (top_board[r_, f_] == P.LIE);
                    if (_jumped) break;
                    else _jumped = true;
                }
                los[r_, f_] = 1;
            }
            _jumped = !_jump;
            for (int i = 1; i <= ur; i++)                           // Up-Right
            {
                int r_ = curr_r - i;
                int f_ = curr_f + i;
                if (top_board[r_, f_] != 0)
                {
                    _lt_gen_in_sight |= (top_board[r_, f_] == P.LIE);
                    if (_jumped) break;
                    else _jumped = true;
                }
                los[r_, f_] = 1;
            }
            _jumped = !_jump;
            for (int i = 1; i <= ll; i++)                           // Left
            {
                int r_ = curr_r;
                int f_ = curr_f - i;
                if (top_board[r_, f_] != 0)
                {
                    _lt_gen_in_sight |= (top_board[r_, f_] == P.LIE);
                    if (_jumped) break;
                    else _jumped = true;
                }
                los[r_, f_] = 1;
            }
            _jumped = !_jump;
            for (int i = 1; i <= rr; i++)                           // Right
            {
                int r_ = curr_r;
                int f_ = curr_f + i;
                if (top_board[r_, f_] != 0)
                {
                    _lt_gen_in_sight |= (top_board[r_, f_] == P.LIE);
                    if (_jumped) break;
                    else _jumped = true;
                }
                los[r_, f_] = 1;
            }
            _jumped = !_jump;
            for (int i = 1; i <= dl; i++)                           // Down-Left
            {
                int r_ = curr_r + i;
                int f_ = curr_f - i;
                if (top_board[r_, f_] != 0)
                {
                    _lt_gen_in_sight |= (top_board[r_, f_] == P.LIE);
                    if (_jumped) break;
                    else _jumped = true;
                }
                los[r_, f_] = 1;
            }
            _jumped = !_jump;
            for (int i = 1; i <= dd; i++)                           // Down
            {
                int r_ = curr_r + i;
                int f_ = curr_f;
                if (top_board[r_, f_] != 0)
                {
                    _lt_gen_in_sight |= (top_board[r_, f_] == P.LIE);
                    if (_jumped) break;
                    else _jumped = true;
                }
                los[r_, f_] = 1;
            }
            _jumped = !_jump;
            for (int i = 1; i <= dr; i++)                           // Down-Right
            {
                int r_ = curr_r + i;
                int f_ = curr_f + i;
                if (top_board[r_, f_] != 0)
                {
                    _lt_gen_in_sight |= (top_board[r_, f_] == P.LIE);
                    if (_jumped) break;
                    else _jumped = true;
                }
                los[r_, f_] = 1;
            }

            if (!_lt_gen_in_sight)
            {
                _elevated_tier = ElevatedTier(_location, _board, top_board);
            }

            return los;
        }

        public static int ElevatedTier(int[] _location, int[,,] _board, int[,] _top_board)
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
                if (_top_board[curr_r - 1, curr_f] == P.FOR)
                {
                    e_tier = Math.Max(e_tier, StackHeight(curr_r - 1, curr_f, _board) + 1);
                    if (e_tier == 2) return e_tier;
                }
                if (not_left)
                {
                    if (_top_board[curr_r, curr_f - 1] == P.FOR)
                    {
                        e_tier = Math.Max(e_tier, StackHeight(curr_r, curr_f - 1, _board) + 1);
                        if (e_tier == 2) return e_tier;
                    }
                    if (_top_board[curr_r - 1, curr_f - 1] == P.FOR)
                    {
                        e_tier = Math.Max(e_tier, StackHeight(curr_r - 1, curr_f - 1, _board) + 1);
                        if (e_tier == 2) return e_tier;
                    }
                }
                if (not_right)
                {
                    if (_top_board[curr_r, curr_f + 1] == P.FOR)
                    {
                        e_tier = Math.Max(e_tier, StackHeight(curr_r, curr_f + 1, _board) + 1);
                        if (e_tier == 2) return e_tier;
                    }
                    if (_top_board[curr_r - 1, curr_f + 1] == P.FOR)
                    {
                        e_tier = Math.Max(e_tier, StackHeight(curr_r - 1, curr_f + 1, _board) + 1);
                        if (e_tier == 2) return e_tier;
                    }
                }
            }
            if (not_bottom)
            {
                if (_top_board[curr_r + 1, curr_f] == P.FOR)
                {
                    e_tier = Math.Max(e_tier, StackHeight(curr_r + 1, curr_f, _board) + 1);
                    if (e_tier == 2) return e_tier;
                }
                if (not_left)
                {
                    if (_top_board[curr_r + 1, curr_f - 1] == P.FOR)
                    {
                        e_tier = Math.Max(e_tier, StackHeight(curr_r + 1, curr_f - 1, _board) + 1);
                        if (e_tier == 2) return e_tier;
                    }
                }
                if (not_right)
                {
                    if (_top_board[curr_r + 1, curr_f + 1] == P.FOR)
                    {
                        e_tier = Math.Max(e_tier, StackHeight(curr_r + 1, curr_f + 1, _board) + 1);
                        if (e_tier == 2) return e_tier;
                    }
                }
            }

            return e_tier;
        }

        public static void Combine(out int[,,] _valid_moves, out int[,,] _valid_captures, int[,,] _move_set, int[] _location, int[,,] _board, bool _jump)
        {
            _valid_moves = new int[3, 9, 9];
            _valid_captures = new int[3, 9, 9];

            // Need los exceptions for LtGen3, Archer2/3, Knight
            int[,] los = LineOfSight(_location, _board, out bool lt_gen_in_sight, out int e_tier, _jump);

            for (int t = 0; t < 3; t++)
            {
                for (int r = 0; r < 9; r++)
                {
                    for (int f = 0; f < 9; f++)
                    {
                        _valid_moves[t, r, f] = _board[t, r, f] & _move_set[e_tier, r, f] & los[r, f] & (_board[t, r, f] == 0 ? 1 : 0);
                        _valid_captures[t, r, f] = _board[t, r, f] & _move_set[e_tier, r, f] & los[r, f] & (_board[t, r, f] < 0 ? 1 : 0);

                        if (lt_gen_in_sight)
                        {
                            for (int m_t = 0; m_t < 3; m_t++)
                            {
                                _valid_moves[t, r, f] |= _board[t, r, f] & _move_set[m_t, r, f] & los[r, f] & (_board[t, r, f] == 0 ? 1 : 0);
                                _valid_captures[t, r, f] |= _board[t, r, f] & _move_set[m_t, r, f] & los[r, f] & (_board[t, r, f] < 0 ? 1 : 0);
                            }
                        }
                    }
                }
            }
            return;
        }

    }
}
