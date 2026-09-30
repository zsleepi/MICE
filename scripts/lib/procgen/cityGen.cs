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
        static int minDistrictSize = 200;
        static int maxDistrictSize = 600;

        static int minHouseSize = 33;
        static int maxHouseSize = 70;
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
            List<Vector2I> selection = RectToSelection(area);

            PlaceBed(selection, TagSet, terrain);

            PlaceTable(selection, TagSet, terrain);

            PlaceChair(selection, TagSet, terrain);
            PlaceChair(selection, TagSet, terrain);

            PlaceBookcase(selection, TagSet, terrain);

            PlaceTorch(selection, TagSet, terrain);

        }

        public static void PlaceBed(List<Vector2I> selection, Dictionary<Vector2I, List<string>> TagSet, TileMapLayer terrain)
        {
            List<string> tags = ["corner", "edge"];
            List<string> excludedTags = ["inapplicable"];
            List<Vector2I> applicableCells = selection.Where(cell => HasAnyTag(TagSet, cell, tags) && !HasAnyTag(TagSet, cell, excludedTags)).ToList();
            if (applicableCells.Count == 0) return;
            Vector2I coords = RandomCellFromSelection(applicableCells);
            terrain.SetCell(coords, 5, new Vector2I(7, 1));
            AddTagtoSet(TagSet, coords, "inapplicable");
            AddTagtoSet(TagSet, coords, "bed");
            AddTagtoAdjacent(TagSet, coords, "bedAdjacent");
            AddTagtoAdjacent(TagSet, coords, "inapplicable");
        }
        public static void PlaceTable(List<Vector2I> selection, Dictionary<Vector2I, List<string>> TagSet, TileMapLayer terrain)
        {
            List<string> tags = ["open", "edge"];
            List<string> excludedTags = ["inapplicable"];
            List<Vector2I> applicableCells = selection.Where(cell => HasAnyTag(TagSet, cell, tags) && !HasAnyTag(TagSet, cell, excludedTags)).ToList();
            if (applicableCells.Count == 0) return;
            Vector2I coords = RandomCellFromSelection(applicableCells);
            terrain.SetCell(coords, 5, new Vector2I(0, 1));
            AddTagtoSet(TagSet, coords, "inapplicable");
            AddTagtoSet(TagSet, coords, "table");
            AddTagtoAdjacent(TagSet, coords, "chairApplicable");
        }

        public static void PlaceChair(List<Vector2I> selection, Dictionary<Vector2I, List<string>> TagSet, TileMapLayer terrain)
        {
            List<string> allowedTags = ["open", "edge"];
            List<string> requiredTags = ["chairApplicable"];
            List<string> excludedTags = ["inapplicable"];
            List<Vector2I> applicableCells = selection.Where(cell => HasAnyTag(TagSet, cell, allowedTags) && !HasAnyTag(TagSet, cell, excludedTags) && HasAllTags(TagSet, cell, requiredTags)).ToList();
            if (applicableCells.Count == 0) return;
            Vector2I coords = RandomCellFromSelection(applicableCells);
            terrain.SetCell(coords, 5, new Vector2I(7, 3));
            AddTagtoSet(TagSet, coords, "inapplicable");
            AddTagtoSet(TagSet, coords, "chair");
        }
        public static void PlaceBookcase(List<Vector2I> selection, Dictionary<Vector2I, List<string>> TagSet, TileMapLayer terrain)
        {
            List<string> allowedTags = ["edge"];
            List<string> excludedTags = ["inapplicable"];
            List<Vector2I> applicableCells = selection.Where(cell => HasAnyTag(TagSet, cell, allowedTags) && !HasAnyTag(TagSet, cell, excludedTags)).ToList();
            if (applicableCells.Count == 0) return;
            Vector2I coords = RandomCellFromSelection(applicableCells);
            terrain.SetCell(coords, 5, new Vector2I(0, 5));
            AddTagtoSet(TagSet, coords, "inapplicable");
            AddTagtoSet(TagSet, coords, "bookcase");
        }
        public static void PlaceTorch(List<Vector2I> selection, Dictionary<Vector2I, List<string>> TagSet, TileMapLayer terrain)
        {
            List<string> allowedTags = ["open"];
            List<string> excludedTags = ["inapplicable"];
            List<Vector2I> applicableCells = selection.Where(cell => HasAnyTag(TagSet, cell, allowedTags) && !HasAnyTag(TagSet, cell, excludedTags)).ToList();
            if (applicableCells.Count == 0) return;
            Vector2I coords = RandomCellFromSelection(applicableCells);
            terrain.SetCell(coords, 3, new Vector2I(0, 1));
            AddTagtoSet(TagSet, coords, "inapplicable");
            AddTagtoSet(TagSet, coords, "torch");
        }

        public static void AddTagtoSet(Dictionary<Vector2I, List<string>> TagSet, Vector2I cell, string tag)
        {
            TagSet.TryGetValue(cell, out List<string> tagList);
            if (!tagList.Contains(tag)) tagList.Add(tag);
            TagSet[cell] = tagList;
        }

        public static void AddTagtoAdjacent(Dictionary<Vector2I, List<string>> TagSet, Vector2I cell, string tag)
        {
            List<Vector2I> adjacentTiles = GetAdjacentCells(cell);
            foreach (Vector2I adjacentTile in adjacentTiles)
            {
                AddTagtoSet(TagSet, adjacentTile, tag);
            }
        }

        public static bool HasAnyTag(Dictionary<Vector2I, List<string>> TagSet, Vector2I cell, List<string> allowedTags)
        {
            bool allowed = false;
            TagSet.TryGetValue(cell, out List<string> tagList);
            foreach (string tag in allowedTags)
            {
                if (tagList.Contains(tag)) allowed = true;
            }
            return allowed;
        }

        public static bool HasAllTags(Dictionary<Vector2I, List<string>> TagSet, Vector2I cell, List<string> requiredTags)
        {
            TagSet.TryGetValue(cell, out List<string> tagList);
            foreach (string tag in requiredTags)
            {
                if (!tagList.Contains(tag)) return false;
            }
            return true;
        }

        public static Dictionary<Vector2I, List<string>> CreateTagSet(Rect2I area, TileMapLayer terrain)
        {
            Dictionary<Vector2I, List<string>> TagSet = new Dictionary<Vector2I,List<string>>();
            List<Vector2I> selection = RectToSelection(area);
            foreach (Vector2I cell in selection)
            {
                TagSet.Add(cell, new List<string>());
            }

            // first pass (walls)
            foreach (Vector2I cell in SelectRectCircumference(area))
            {
                if (terrain.GetCellTileData(cell) != null && !terrain.GetCellTileData(cell).GetCustomData("Walkable").AsBool())
                {
                    AddTagtoSet(TagSet, cell, "wall");
                    AddTagtoSet(TagSet, cell, "inapplicable");
                } else
                {
                    AddTagtoSet(TagSet, cell, "doorHole");
                    AddTagtoSet(TagSet, cell, "inapplicable");
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
                        AddTagtoSet(TagSet, cell,"open");
                        break;
                    case 1:
                        AddTagtoSet(TagSet, cell,"edge");
                        break;
                    case 2:
                        AddTagtoSet(TagSet, cell, "corner");
                        break;
                    case 3:
                        AddTagtoSet(TagSet, cell, "end");
                        break;
                    case 4:
                        AddTagtoSet(TagSet, cell, "enclosed");
                        break;
                }
                if (adjacentDoorhole) { AddTagtoSet(TagSet, cell, "inapplicable"); }
                if (adjacentDoorhole && !tagList.Contains("wall")) {
                    AddTagtoSet(TagSet, cell, "entry");
                    foreach (Vector2I tile in GetAdjacentCells(cell))
                    {
                        TagSet.TryGetValue(tile, out List<string> adjacentTileTags);
                        if (adjacentTileTags.Contains("wall")) continue;
                        AddTagtoSet(TagSet, tile, "entryAdjacent");
                        AddTagtoSet(TagSet, tile, "inapplicable");
                    }
                }
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



        public static List<Vector2I> RectToSelection(Rect2I rect)
        {
            List<Vector2I> _selection = [];
            for (int x = 0; x < rect.Size.X; x++)
            {
                for (int y = 0; y < rect.Size.Y; y++)
                {
                    _selection.Add(new Vector2I(rect.Position.X + x, rect.Position.Y + y));
                }
            }
            return _selection;
        }

        public static Vector2I RandomCellFromSelection(List<Vector2I> selection)
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
