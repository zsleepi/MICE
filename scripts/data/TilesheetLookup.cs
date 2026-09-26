using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MICE.scripts.data
{
    public static class TilesheetLookup
    {
        public class SpriteLocation(string spriteName, int atlasID, Vector2I positionInSheet)
        {
            public string spriteName = spriteName;
            public int atlasID = atlasID;
            public Vector2I positionInSheet = positionInSheet;
        }

        public static List<SpriteLocation> ObjectSprites = [
            new SpriteLocation( "table", 5, new Vector2I(0,1) ),
            new SpriteLocation( "chair", 5, new Vector2I(7,3) ),
            new SpriteLocation( "stool", 5, new Vector2I(1,1) ),
            new SpriteLocation( "bed", 5, new Vector2I(7,1) ),
            new SpriteLocation( "chest", 5, new Vector2I(5,1) ),
            new SpriteLocation( "bookcase", 5, new Vector2I(0,5) ),
        ];

        public static SpriteLocation GetSpriteLocation( string name )
        {
            return ObjectSprites.Find(a => a.spriteName == name);
        }
    }
}
