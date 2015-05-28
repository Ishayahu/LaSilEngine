using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework; // for GameTime
using Microsoft.Xna.Framework.Graphics; // for SpriteBatch
using Microsoft.Xna.Framework.Input;

namespace Game2
{
    /// <summary>
    /// Класс, отвечающий за хранение и работу с картой
    /// </summary>
    public class Map
    {
        #region fields
        //карта
        private MapCell[,] map;
        // карта источников света
        private LightSource[,] lightSourceMap;
        private Random rand;
        private const int margin = 20;
        private int x = LaSilEngineConstants.MAP_X_SIZE, y = LaSilEngineConstants.MAP_Y_SIZE;
        #endregion
        
        #region properties
        public int X { get { return x; } }
        public int Y { get { return y; } }
        #endregion

        #region constructor
        public Map(Random rand)
        {
            this.rand = rand;
            map = new MapCell[x, y];
            lightSourceMap = new LightSource[x, y];
            for (int dx = 0; dx < x; dx++)
            {
                for (int dy = 0; dy < y; dy++)
                {
                    map[dx, dy] = new MapCell(rand);
                    // пока нет источников света
                    lightSourceMap[dx, dy] = null;
                }
            }
        }
        #endregion

        #region methods
        /// <summary>
        /// Добавляем источник света
        /// </summary>
        /// <param name="pos">координаты, Vector2</param>
        /// <param name="strength">сила, float</param>
        public void AddLightSource(Vector2 pos, float strength)
        {
            lightSourceMap[(int)pos.X, (int)pos.Y] = new LightSource(strength);
        }
        public float GetLightSourceStrength(Vector2 pos)
        {
            try
            {
                return lightSourceMap[(int)pos.X, (int)pos.Y].Strength;
            }
            catch (Exception e)
            {
                return 0F;
            }
        }
        public void LoadMap()
        { }
        /// <summary>
        /// Возвращает клеткy карты:
        /// </summary>
        /// <returns></returns>
        public Vector2 GetCellCenter(int x, int y, int width, int height)
        {
            if (x >= map.GetLength(0) || y >= map.GetLength(1) || x < 0 || y < 0)
            {
                throw new MapCellException("Cell index out of range!");
            }
            int cellWidth = (width - margin * map.GetLength(0)) / map.GetLength(0);
            int cellHeight = (height - margin * map.GetLength(1)) / map.GetLength(1);
            return new Vector2((cellWidth + margin) / 2 + x * (cellWidth + margin), (cellHeight + margin) / 2 + y * (cellHeight + margin));
        }
        public string GetCellChar(int x, int y)
        {
            if (x >= map.GetLength(0) || y >= map.GetLength(1) || x < 0 || y < 0)
            {
                throw new MapCellException("Cell index out of range!");
            }
            return map[x, y].Char;
        }
        public LaSilEngineConstants.BorderTypes GetCellSide(int x, int y, LaSilEngineConstants.Direction side)
        {
            MapCell cell = map[x, y];
            return cell.GetSide(side);
        }
        /// <summary>
        /// Рисует карту
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="font"></param>
        /// <param name="x">верхний левый угол - х</param>
        /// <param name="y">верхний левый угол - у</param>
        /// <param name="width">ширина</param>
        /// <param name="height">высота</param>
        public void Draw(SpriteBatch spriteBatch, SpriteFont font, int x, int y, int width, int height)
        {
            //Draw(SpriteBatch spriteBatch, SpriteFont font, int x, int y, int width, int height)
            int cellWidth = (width - margin * map.GetLength(0)) / map.GetLength(0);
            int cellHeight = (height - margin * map.GetLength(1)) / map.GetLength(1);
            for (int xCell = 0; xCell < map.GetLength(0); xCell++)
            {
                for (int yCell = 0; yCell < map.GetLength(1); yCell++)
                {
                    map[xCell, yCell].Draw(spriteBatch, font, x + (cellWidth + margin) * xCell, y + (cellHeight + margin) * yCell, cellWidth, cellHeight);
                }
            }
        }

        /// <summary>
        /// Можно ли пройти с одной клетки на другую и сколько это займёт ходов
        /// </summary>
        /// <param name="from">С какой клетки идём</param>
        /// <param name="to">На какую клетку идём</param>
        /// <param name="camera">Камера (игрок, например) (для учёта заклинаний)</param>
        /// <returns>Количество затраченных ходов или -1, если пройти не возможно</returns>
        public int IsTraversal(Vector2 from, Vector2 to, Camera camera)
        {
            int dy = (int)(from.Y - to.Y);
            int dx = (int)(from.X - to.X);
            if (dy < 0 && dx == 0)
            {
                return map[(int)to.X, (int)to.Y].IsTraversal(camera, LaSilEngineConstants.Direction.North);
            }
            else if (dy > 0 && dx == 0)
            {
                return map[(int)to.X, (int)to.Y].IsTraversal(camera, LaSilEngineConstants.Direction.South);
            }
            else if (dy == 0 && dx < 0)
            {
                return map[(int)to.X, (int)to.Y].IsTraversal(camera, LaSilEngineConstants.Direction.West);
            }
            else if (dy == 0 && dx > 0)
            {
                return map[(int)to.X, (int)to.Y].IsTraversal(camera, LaSilEngineConstants.Direction.East);
            }
            // Если какой-то странный результат
            return -1;
        }

        public float VisibilityDecreasing(Vector2 mapCellCoordinates)
        {
            return map[(int)mapCellCoordinates.X, (int)mapCellCoordinates.Y].Visibility;
        }
        #endregion

        #region exceptions
        public class MapCellException : Exception
        {
            public MapCellException()
            {
            }

            public MapCellException(string message)
                : base(message)
            {
            }

            public MapCellException(string message, Exception inner)
                : base(message, inner)
            {
            }
        }
        #endregion
    }

    
    /// <summary>
    /// Класс границ клетки. Хранит типы границ и позволяет получить случайную границу
    /// </summary>
    public class Border
    {
        #region fields
        private Random rand;
        #endregion

        public Border(Random rand)
        {
            this.rand = rand;
        }

        public LaSilEngineConstants.BorderTypes GetRandomBorderType()
        {

            int typeNumber = rand.Next(Enum.GetNames(typeof(LaSilEngineConstants.BorderTypes)).Length);
            return (LaSilEngineConstants.BorderTypes)typeNumber;
        }
    }

    


    public class MapCell : Border
    {
        #region fields
        LaSilEngineConstants.BorderTypes east, west, north, south;
        Surface surface;
        #endregion

        #region constructor
        public MapCell(Random rand, LaSilEngineConstants.BorderTypes east,
            LaSilEngineConstants.BorderTypes west,
            LaSilEngineConstants.BorderTypes north,
            LaSilEngineConstants.BorderTypes south,
            Surface surface)
            : base(rand)
        {
            this.east = east;
            this.west = west;
            this.north = north;
            this.south = south;
            this.surface = surface;
        }
        public MapCell(Random rand)
            : base(rand)
        {

            east = GetRandomBorderType();
            west = GetRandomBorderType();
            north = GetRandomBorderType();
            south = GetRandomBorderType();
            surface = new Surface(rand).GetRandormSurface();

        }
        #endregion

        #region properties
        public Surface Surface
        {
            get { return this.surface; }
        }
        public float Visibility
        {
            get { return this.surface.Visibility; }
        }
        public string Char
        {
            get { return this.surface.SurfaceCode; }
        }
        #endregion

        #region methods
        /// <summary>
        /// Получить значение типа сотороны клетки
        /// </summary>
        /// <param name="side">Сторона клетки, перечисление из LaSilEngineConstants.Direction</param>
        /// <returns></returns>
        public LaSilEngineConstants.BorderTypes GetSide(LaSilEngineConstants.Direction side)
        {
            LaSilEngineConstants.BorderTypes res = LaSilEngineConstants.BorderTypes.Free;
            switch (side)
            {
                case LaSilEngineConstants.Direction.East:
                    {
                        res = east;
                        break;
                    }
                case LaSilEngineConstants.Direction.North:
                    {
                        res = north;
                        break;
                    }
                case LaSilEngineConstants.Direction.South:
                    {
                        res = south;
                        break;
                    }
                case LaSilEngineConstants.Direction.West:
                    {
                        res = west;
                        break;
                    }
            }
            return res;
        }

        /// <summary>
        /// Рисует клетку карты
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="x">верхний левый угол - x</param>
        /// <param name="y">верхний левый угол - y</param>
        /// <param name="width">ширина ячейки</param>
        /// <param name="height">высота ячейки</param>
        public void Draw(SpriteBatch spriteBatch, SpriteFont font, int x, int y, int width, int height)
        {
            //spriteBatch.DrawString(font, "" + (int)this.west, new Vector2(x, y + height / 2), Color.White);
            //spriteBatch.DrawString(font, "" + (int)this.east, new Vector2(x + width, y + height / 2), Color.White);
            //spriteBatch.DrawString(font, "" + (int)this.north, new Vector2(x + width / 2, y), Color.White);
            //spriteBatch.DrawString(font, "" + (int)this.south, new Vector2(x + width / 2, y + height), Color.White);

            spriteBatch.DrawString(font, "" + this.surface.SurfaceCode, new Vector2(x + width / 2, y + height / 2), Color.White);
        }

        /// <summary>
        /// Можно ли попасть на эту клетку
        /// Пока без варинатов с какой стороны
        /// </summary>
        /// <param name="camera">Камера, которая пытается попасть. Для учёта заклинаний</param>
        /// <returns>Сколько ходов занимает. Или -1 если пройти не возможно</returns>
        public int IsTraversal(Camera camera,LaSilEngineConstants.Direction from)
        {
            return surface.IsTraversal(camera);
        }
        #endregion
    }
}
