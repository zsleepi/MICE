using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MICE.scripts.lib
{
    public class WorldCoordinates(Vector2I room, Vector2I tile)
    {
        public Vector2I room = room;
        public Vector2I tile = tile;
    }
}
