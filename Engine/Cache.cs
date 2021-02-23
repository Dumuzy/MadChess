﻿// +------------------------------------------------------------------------------+
// |                                                                              |
// |     MadChess is developed by Erik Madsen.  Copyright 2020.                   |
// |     MadChess is free software.  It is distributed under the GNU General      |
// |     Public License Version 3 (GPLv3).  See LICENSE file for details.         |
// |     See https://www.madchess.net/ for user and developer guides.             |
// |                                                                              |
// +------------------------------------------------------------------------------+


using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;


namespace ErikTheCoder.MadChess.Engine
{
    public sealed class Cache
    {
        public static readonly int CapacityPerMegabyte = 1024 * 1024 / Marshal.SizeOf(typeof(CachedPosition));
        public readonly CachedPosition NullPosition;
        public int Positions;
        public byte Searches;
        private const int _buckets = 4;
        private readonly Stats _stats;
        private readonly Delegates.ValidateMove _validateMove;
        private int _indices;
        private CachedPosition[] _positions; // More memory efficient than a jagged array that has a .NET object header for each sub-array (for garbage collection tracking of reachable-from-root).
        
        
        public int Capacity
        {
            get => _positions.Length;
            set
            {
                _positions = null;
                GC.Collect();
                _positions = new CachedPosition[value];
                _indices = value / _buckets;
                Reset();
            }
        }


        public Cache(int SizeMegabyte, Stats Stats, Delegates.ValidateMove ValidateMove)
        {
            _stats = Stats;
            _validateMove = ValidateMove;
            // Set null position.
            NullPosition = new CachedPosition(0, 0);
            CachedPositionData.SetToHorizon(ref NullPosition.Data, 0);
            CachedPositionData.SetBestMoveFrom(ref NullPosition.Data, Square.Illegal); // An illegal square indicates no best move stored in cached position.
            CachedPositionData.SetBestMoveTo(ref NullPosition.Data, Square.Illegal);
            CachedPositionData.SetBestMovePromotedPiece(ref NullPosition.Data, Piece.None);
            CachedPositionData.SetScore(ref NullPosition.Data, StaticScore.NotCached);
            CachedPositionData.SetScorePrecision(ref NullPosition.Data, ScorePrecision.Unknown);
            CachedPositionData.SetLastAccessed(ref NullPosition.Data, 0);
            // Set capacity (which resets position array).
            Capacity = SizeMegabyte * CapacityPerMegabyte;
        }


        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public CachedPosition GetPosition(ulong Key)
        {
            _stats.CacheProbes++;
            var index = GetIndex(Key);
            for (var bucket = 0; bucket < _buckets; bucket++)
            {
                var bucketIndex = index + bucket;
                var cachedPosition = _positions[bucketIndex];
                if (cachedPosition.Key == Key)
                {
                    // Position is cached.
                    _stats.CacheHits++;
                    CachedPositionData.SetLastAccessed(ref cachedPosition.Data, Searches);
                    _positions[bucketIndex] = cachedPosition;
                    Debug.Assert(CachedPositionData.IsValid(cachedPosition.Data));
                    return cachedPosition;
                }
            }
            // Position is not cached.
            Debug.Assert(CachedPositionData.IsValid(NullPosition.Data));
            return NullPosition;
        }


        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public void SetPosition(CachedPosition CachedPosition)
        {
            Debug.Assert(CachedPositionData.IsValid(CachedPosition.Data));
            var index = GetIndex(CachedPosition.Key);
            CachedPositionData.SetLastAccessed(ref CachedPosition.Data, Searches);
            // Find oldest bucket.
            var earliestAccess = byte.MaxValue;
            var oldestBucketIndex = 0;
            for (var bucket = 0; bucket < _buckets; bucket++)
            {
                var bucketIndex = index + bucket;
                var cachedPosition = _positions[bucketIndex];
                if (cachedPosition.Key == CachedPosition.Key)
                {
                    // Position is cached.  Overwrite position.
                    Debug.Assert(CachedPositionData.IsValid(CachedPosition.Data));
                    _positions[bucketIndex] = CachedPosition;
                    return;
                }
                var lastAccessed = CachedPositionData.LastAccessed(cachedPosition.Data);
                if (lastAccessed < earliestAccess)
                {
                    earliestAccess = lastAccessed;
                    oldestBucketIndex = bucketIndex;
                }
            }
            if (_positions[oldestBucketIndex] == NullPosition) Positions++; // Oldest bucket has not been used.
            // Overwrite oldest bucket.
            Debug.Assert(CachedPositionData.IsValid(CachedPosition.Data));
            _positions[oldestBucketIndex] = CachedPosition;
        }


        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public ulong GetBestMove(ulong CachedPosition)
        {
            _stats.CacheBestMoveProbes++;
            Debug.Assert(CachedPositionData.IsValid(CachedPosition));
            var fromSquare = CachedPositionData.BestMoveFrom(CachedPosition);
            if (fromSquare == Square.Illegal) return Move.Null; // Cached position does not specify a best move.
            var bestMove = Move.Null;
            Move.SetFrom(ref bestMove, fromSquare);
            Move.SetTo(ref bestMove, CachedPositionData.BestMoveTo(CachedPosition));
            Move.SetPromotedPiece(ref bestMove, CachedPositionData.BestMovePromotedPiece(CachedPosition));
            Move.SetIsBest(ref bestMove, true);
            var validMove = _validateMove(ref bestMove);
            if (validMove) _stats.CacheValidBestMove++;
            else _stats.CacheInvalidBestMove++;
            Debug.Assert(Move.IsValid(bestMove));
            return validMove ? bestMove : Move.Null;
        }


        public void Reset()
        {
            for (var index = 0; index < _positions.Length; index++) _positions[index] = NullPosition;
            Positions = 0;
            Searches = 0;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetIndex(ulong Key)
        {
            // Ensure even distribution of indices by using GetHashCode method rather than raw Zobrist Key for modular division.
            var index = (Key.GetHashCode() % _indices) * _buckets; // Index may be negative.
            // Ensure index is positive using technique faster than Math.Abs().  See http://graphics.stanford.edu/~seander/bithacks.html#IntegerAbs.
            var mask = index >> 31;
            return (index ^ mask) - mask;
        }
    }
}
