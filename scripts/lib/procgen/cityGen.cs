using Godot;
using MICE.scripts.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace MICE.scripts.lib.procgen
{
    public static class cityGen
    {
        static int minDistrictSize = 400;
        static int maxDistrictSize = 1200;

        static int minHouseSize = 35;
        static int maxHouseSize = 80;
        public static TileMapLayer CreateCityTerrain(Rect2I roomArea)
        {
            TileMapLayer _terrain = new TileMapLayer();
            _terrain.TileSet = (TileSet)GD.Load("res://assets/terrain/outpost_tiles.tres");
            Random r = new Random();
            List<Rect2I> _districts = SubdivideRect(roomArea, minDistrictSize, maxDistrictSize, 2f);
            foreach (Rect2I rect in _districts)
            {
                Rect2I district = GridUtils.ShrinkRectEven(rect, 2);
                district = GridUtils.ShrinkRectRandom(district, 1);
                district = GridUtils.ShrinkRectUneven(district, 1);
                List<Rect2I> _plots = SubdivideRect(district, minHouseSize, maxHouseSize, 2f);
                foreach (Rect2I plot in _plots)
                {
                    Vector2I houseDirection = RandomSide(GetDirectionOutsideContainer(plot, district.Grow(-1)));
                    if (houseDirection == Vector2I.Zero || r.NextDouble() > 0.9) continue;
                    CreateHouse(plot, _terrain, houseDirection);
                }
            }
            return _terrain;
        }

        public static void CreateHouse(Rect2I area, TileMapLayer terrain, Vector2I direction)
        {
            Godot.Collections.Array<Vector2I> wallFill = [];
            foreach (Vector2I cell in SelectRectCircumference(area))
            {
                wallFill.Add(cell);
            }
            Vector2I doorHole = RandomCellFromSelection(RectToSelection(GetRectEdge(area, direction)));
            wallFill.Remove(doorHole);
            terrain.SetCellsTerrainConnect(wallFill, 0, 1, true);
            FurnishHouse(area, terrain);
        }

        public static void FurnishHouse(Rect2I area, TileMapLayer terrain)
        {
            Dictionary<Vector2I, List<string>> TagSet = CreateTagSet(area, terrain);
            Godot.Collections.Array<Vector2I> selection = RectToSelection(area);
            foreach (Vector2I cell in selection.Where(a => TagSet.GetValueOrDefault(a).Contains("edge") && !TagSet.GetValueOrDefault(a).Contains("inapplicable")))
            {
                GD.Print(TilesheetLookup.GetSpriteLocation("bed"));
                terrain.SetCell(cell, TilesheetLookup.GetSpriteLocation("bed").atlasID, TilesheetLookup.GetSpriteLocation("bed").positionInSheet);
            }
        }

        public static Dictionary<Vector2I, List<string>> CreateTagSet(Rect2I area, TileMapLayer terrain)
        {
            Dictionary<Vector2I, List<string>> TagSet = new Dictionary<Vector2I,List<string>>();
            Godot.Collections.Array<Vector2I> selection = RectToSelection(area);
            foreach (Vector2I cell in selection)
            {
                TagSet.Add(cell, new List<string>());
            }

            // first pass (walls)
            foreach (Vector2I cell in SelectRectCircumference(area))
            {
                if (terrain.GetCellTileData(cell) != null && !terrain.GetCellTileData(cell).GetCustomData("Walkable").AsBool())
                {
                    TagSet.TryGetValue(cell, out List<string> tagList);
                    tagList.Add("wall");
                    tagList.Add("inapplicable");
                    TagSet[cell] = tagList;
                } else
                {
                    TagSet.TryGetValue(cell, out List<string> tagList);
                    tagList.Add("doorHole");
                    tagList.Add("inapplicable");
                    TagSet[cell] = tagList;
                }
            }
            // second pass (edges and corners)
            foreach (Vector2I cell in selection)
            {
                TagSet.TryGetValue(cell, out List<string> tagList);
                if (tagList.Count > 0) { continue; }
                List<Vector2I> adjacentTiles = GetAdjacentCells(cell);
                int adjacentWalls = 0;
                bool adjacentDoorhole = false;
                foreach (Vector2I tile in adjacentTiles)
                {
                    TagSet.TryGetValue(tile, out List<string> adjacentTileTags);
                    if (adjacentTileTags.Contains("wall")) { adjacentWalls++; }
                    if (adjacentTileTags.Contains("doorHole")) { adjacentDoorhole = true; }
                }
                switch (adjacentWalls) {
                    case 0:
                        tagList.Add("open");
                        break;
                    case 1:
                        tagList.Add("edge");
                        break;
                    case 2:
                        tagList.Add("corner");
                        break;
                    case 3:
                        tagList.Add("end");
                        break;
                    case 4:
                        tagList.Add("enclosed");
                        break;
                }
                if (adjacentDoorhole) { tagList.Add("inapplicable"); }
                if (adjacentDoorhole) { tagList.Add("entry"); }
                TagSet[cell] = tagList;
            }
            return TagSet;
        }

        public static List<Vector2I> GetAdjacentCells(Vector2I cell)
        {
            List<Vector2I> list = [cell+new Vector2I(1,0), cell + new Vector2I(-1, 0), cell + new Vector2I(0, 1), cell + new Vector2I(0, -1)];
            return list;
        }

        public static Rect2I GetRectEdge(Rect2I rect, Vector2I direction)
        {
            Rect2I rectEdge = rect;
            if (direction.X == 1) { rectEdge = rectEdge.GrowIndividual(-(rect.Size.X - 1), -1, 0, -1); }
            if (direction.X == -1) { rectEdge = rectEdge.GrowIndividual(0, -1, -(rect.Size.X - 1), -1); }
            if (direction.Y == 1) { rectEdge = rectEdge.GrowIndividual(-1, -(rect.Size.Y - 1), -1, 0); }
            if (direction.Y == -1) { rectEdge = rectEdge.GrowIndividual(-1, 0, -1, -(rect.Size.Y - 1)); }
            return rectEdge;
        }

        public static Vector2I RotateVector(Vector2I vector, int clockwiseRotations)
        {
            List<Vector2I> rotations = [
                new Vector2I(vector.X,vector.Y),
                new Vector2I(-vector.Y, vector.X),
                new Vector2I(-vector.X, -vector.Y),
                new Vector2I(vector.Y, -vector.X),
            ];
            return rotations[clockwiseRotations % 4];
        }



        public static Godot.Collections.Array<Vector2I> RectToSelection(Rect2I rect)
        {
            Godot.Collections.Array<Vector2I> _selection = [];
            for (int x = 0; x < rect.Size.X; x++)
            {
                for (int y = 0; y < rect.Size.Y; y++)
                {
                    _selection.Add(new Vector2I(rect.Position.X + x, rect.Position.Y + y));
                }
            }
            return _selection;
        }

        public static Vector2I RandomCellFromSelection(Godot.Collections.Array<Vector2I> selection)
        {
            Random r = new Random();
            Vector2I cell = selection[r.Next(0, selection.Count-1)];
            return cell;
        }

        public static Vector2I GetDirectionOutsideContainer(Rect2I rect, Rect2I container)
        {
            Vector2I direction = new Vector2I(0,0);
            if (!container.HasPoint(new Vector2I(rect.Position.X, rect.Position.Y + rect.Size.Y/2))) direction.X = -1;
            if (!container.HasPoint(new Vector2I(rect.Position.X + rect.Size.X/2, rect.Position.Y))) direction.Y = -1;
            if (!container.HasPoint(new Vector2I(rect.Position.X + rect.Size.X/2, rect.Position.Y + rect.Size.Y))) direction.Y = 1;
            if (!container.HasPoint(new Vector2I(rect.Position.X + rect.Size.X, rect.Position.Y + rect.Size.Y/2))) direction.X = 1;
            return direction;
        }

        public static Vector2I RandomSide(Vector2I direction)
        {
            List<Vector2I> eligibleSides = [];
            if (direction.X != 0) {eligibleSides.Add(new Vector2I(direction.X, 0)); }
            if (direction.Y != 0) {eligibleSides.Add(new Vector2I(0, direction.Y)); }
            if (eligibleSides.Count() == 0) return Vector2I.Zero;
            Random r = new Random();
            return eligibleSides[r.Next(0, eligibleSides.Count)];
        }

        public static List<Rect2I> SubdivideRect(Rect2I input, int minSize, int maxSize, float maxLengthRatio)
        {
            List<Rect2I> _rectangles = [input];
            while (_rectangles.Exists(contender => contender.Size.X * contender.Size.Y > maxSize))
            {
                int index = _rectangles.FindIndex(contender => contender.Size.X * contender.Size.Y > maxSize);
                Rect2I _rect = _rectangles[index];
                List<Rect2I> _slicedRect = GridUtils.SliceRectangle(_rect, minSize, maxLengthRatio);
                _rectangles.RemoveAt(index);
                foreach (Rect2I item in _slicedRect)
                {
                    _rectangles.Add(item);
                }
            }
            return _rectangles;
        }

        public static HashSet<Vector2I> SelectRectCircumference(Rect2I input)
        {
            HashSet<Vector2I> selection = [];
            for (int x = 0; x < input.Size.X; x++)
            {
                selection.Add(new Vector2I(input.Position.X + x, input.Position.Y));
                selection.Add(new Vector2I(input.Position.X + x, input.Position.Y + input.Size.Y-1));
            }
            for (int y = 0; y < input.Size.Y; y++)
            {
                selection.Add(new Vector2I(input.Position.X, input.Position.Y + y));
                selection.Add(new Vector2I(input.Position.X + input.Size.X-1, input.Position.Y + y));
            }
            return selection;
        }
    }
}
