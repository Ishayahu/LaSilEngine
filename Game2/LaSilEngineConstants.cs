using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game2
{
    public class LaSilEngineConstants
    {
        public enum Direction { North, East, South, West };
        public enum BorderTypes { Free, Wal, Trees, Water };
        public const int MAP_X_SIZE = 3;
        public const int MAP_Y_SIZE = 3;
    }
    
}
