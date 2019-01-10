﻿// +------------------------------------------------------------------------------+
// |                                                                              |
// |     MadChess is developed by Erik Madsen.  Copyright 2018.                   |
// |     MadChess is free software.  It is distributed under the GNU General      |
// |     Public License Version 3 (GPLv3).  See License.txt for details.          |
// |                                                                              |
// +------------------------------------------------------------------------------+


namespace ErikTheCoder.MadChess.Engine
{
    public sealed class KillerMove
    {
        public int Piece;
        public int ToSquare;


        public KillerMove(int Piece, int ToSquare)
        {
            this.Piece = Piece;
            this.ToSquare = ToSquare;
        }
    }
}
