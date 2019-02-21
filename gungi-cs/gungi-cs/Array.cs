using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gungi_cs
{
    class Array
    {
        public static int[,,] Not(int[,,] _array)
        {
            for (int t = 0; t < _array.GetLength(0); t++)
            {
                for (int r = 0; r < _array.GetLength(1); r++)
                {
                    for (int f = 0; f < _array.GetLength(2); f++)
                    {
                        _array[t, r, f] = 1 - _array[t, r, f];
                    }
                }
            }
            return _array;
        }

        public static int[,,] And(int[,,] _array1, int[,,] _array2)
        {
            for (int t = 0; t < _array1.GetLength(0); t++)
            {
                for (int r = 0; r < _array1.GetLength(1); r++)
                {
                    for (int f = 0; f < _array2.GetLength(2); f++)
                    {
                        _array1[t, r, f] &= _array2[t, r, f];
                    }
                }
            }
            return _array1;
        }

        public static int[,,] And(int[,,] _array1, int[,,] _array2, int[,,] _array3)
        {
            for (int t = 0; t < _array1.GetLength(0); t++)
            {
                for (int r = 0; r < _array1.GetLength(1); r++)
                {
                    for (int f = 0; f < _array1.GetLength(2); f++)
                    {
                        _array1[t, r, f] &= _array2[t, r, f] & _array3[t, r, f];
                    }
                }
            }
            return _array1;
        }

        public static int[,,] Or(int[,,] _array1, int[,,] _array2)
        {
            for (int t = 0; t < _array1.GetLength(0); t++)
            {
                for (int r = 0; r < _array1.GetLength(1); r++)
                {
                    for (int f = 0; f < _array1.GetLength(2); f++)
                    {
                        _array1[t, r, f] |= _array2[t, r, f];
                    }
                }
            }
            return _array1;
        }

        public static int[,,] Or(int[,,] _array1, int[,,] _array2, int[,,] _array3)
        {
            for (int t = 0; t < _array1.GetLength(0); t++)
            {
                for (int r = 0; r < _array1.GetLength(1); r++)
                {
                    for (int f = 0; f < _array1.GetLength(2); f++)
                    {
                        _array1[t, r, f] |= _array2[t, r, f] | _array3[t, r, f];
                    }
                }
            }
            return _array1;
        }

        public static int[,,] Enemy(int[,,] _board)
        {
            for (int t = 0; t < 3; t++)
            {
                for (int r = 0; r < 9; r++)
                {
                    for (int f = 0; f < 9; f++)
                    {
                        _board[t, r, f] = (_board[t, r, f] < 0) ? 1 : 0;
                    }
                }
            }
            return _board;
        }

        public static int[,,] Friend(int[,,] _board)
        {
            for (int t = 0; t < 3; t++)
            {
                for (int r = 0; r < 9; r++)
                {
                    for (int f = 0; f < 9; f++)
                    {
                        _board[t, r, f] = (_board[t, r, f] > 0) ? 1 : 0;
                    }
                }
            }
            return _board;
        }

        public static int[,,] Full(int[,,] _board)
        {
            for (int t = 0; t < 3; t++)
            {
                for (int r = 0; r < 9; r++)
                {
                    for (int f = 0; f < 9; f++)
                    {
                        _board[t, r, f] = (_board[t, r, f] != 0) ? 1 : 0;
                    }
                }
            }
            return _board;
        }

        public static int[,,] NotFull(int[,,] _board)
        {
            for (int t = 0; t < 3; t++)
            {
                for (int r = 0; r < 9; r++)
                {
                    for (int f = 0; f < 9; f++)
                    {
                        _board[t, r, f] = (_board[t, r, f] == 0) ? 1 : 0;
                    }
                }
            }
            return _board;
        }

        public static int[,,] LineOfSight(int[,,] _move_set, int[] _location, int[,,] _board, out bool _lt_gen_in_sight)
        {
            int[,,] los = new int[3, 9, 9];
            _lt_gen_in_sight = false;

            int c_t = _location[0];
            int c_r = _location[1];
            int c_f = _location[2];

            int ul = Math.Min(c_r, c_f);
            int uu = c_r;
            int ur = Math.Min(c_r, 8 - c_f);
            int ll = c_f;
            int rr = 8 - c_f;
            int dl = Math.Min(8 - c_r, c_f);
            int dd = 8 - c_r;
            int dr = Math.Min(8 - c_r, 8 - c_f);

            //LT GENERAL MUST BE ON TOP OF STACK, currently only looks for lt general
            ///make function that grabs top piece only and smushes into 1x9x9 array
            ///then use that array with los to make a 1x9x9 los
            ///then copy the 1x9x9 los array to a 3x9x9 los to return
            for (int t = 0; t < 3; t++)
            {
                los[t, c_r, c_f] = 1;                                   // Center
                for (int i = 1; i <= ul; i++)                           // Up-Left
                {
                    int r_ = c_r - i;
                    int f_ = c_f - i;
                    if (_board[t, r_, f_] != 0)
                    {
                        _lt_gen_in_sight |= (_board[t, r_, f_] == P.LIE);
                        break;
                    }
                    los[0, r_, f_] = 1;
                    los[1, r_, f_] = 1;
                    los[2, r_, f_] = 1;
                }
                for (int i = 1; i <= uu; i++)                           // Up
                {
                    int r_ = c_r - i;
                    int f_ = c_f;
                    if (_board[t, r_, f_] != 0)
                    {
                        _lt_gen_in_sight |= (_board[t, r_, f_] == P.LIE);
                        break;
                    }
                    los[t, r_, f_] = 1;
                }
                for (int i = 1; i <= ur; i++)                           // Up-Right
                {
                    int r_ = c_r - i;
                    int f_ = c_f + i;
                    if (_board[t, r_, f_] != 0)
                    {
                        _lt_gen_in_sight |= (_board[t, r_, f_] == P.LIE);
                        break;
                    }
                    los[t, r_, f_] = 1;
                }
                for (int i = 1; i <= ll; i++)                           // Left
                {
                    int r_ = c_r;
                    int f_ = c_f - i;
                    if (_board[t, r_, f_] != 0)
                    {
                        _lt_gen_in_sight |= (_board[t, r_, f_] == P.LIE);
                        break;
                    }
                    los[t, r_, f_] = 1;
                }
                for (int i = 1; i <= rr; i++)                           // Right
                {
                    int r_ = c_r;
                    int f_ = c_f + i;
                    if (_board[t, r_, f_] != 0)
                    {
                        _lt_gen_in_sight |= (_board[t, r_, f_] == P.LIE);
                        break;
                    }
                    los[t, r_, f_] = 1;
                }
                for (int i = 1; i <= dl; i++)                           // Down-Left
                {
                    int r_ = c_r + i;
                    int f_ = c_f - i;
                    if (_board[t, r_, f_] != 0)
                    {
                        _lt_gen_in_sight |= (_board[t, r_, f_] == P.LIE);
                        break;
                    }
                    los[t, r_, f_] = 1;
                }
                for (int i = 1; i <= dd; i++)                           // Down
                {
                    int r_ = c_r + i;
                    int f_ = c_f;
                    if (_board[t, r_, f_] != 0)
                    {
                        _lt_gen_in_sight |= (_board[t, r_, f_] == P.LIE);
                        break;
                    }
                    los[t, r_, f_] = 1;
                }
                for (int i = 1; i <= dr; i++)                           // Down-Right
                {
                    int r_ = c_r + i;
                    int f_ = c_f + i;
                    if (_board[t, r_, f_] != 0)
                    {
                        _lt_gen_in_sight |= (_board[t, r_, f_] == P.LIE);
                        break;
                    }
                    los[t, r_, f_] = 1;
                }
            }

            return los;
        }

        public static int[,,] Dupe(int[,,] _move_set, int _tier_to_dupe)
        {
            for (int t = 0; t < _move_set.GetLength(0); t++)
            {
                for (int r = 0; r < _move_set.GetLength(1); r++)
                {
                    for (int f = 0; f < _move_set.GetLength(2); f++)
                    {
                        _move_set[t, r, f] = _move_set[_tier_to_dupe, r, f];
                    }
                }
            }
            return _move_set;
        }
    }
}
