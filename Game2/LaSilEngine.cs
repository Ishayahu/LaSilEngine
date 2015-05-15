using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework; // for GameTime
using Microsoft.Xna.Framework.Graphics; // for SpriteBatch
using Microsoft.Xna.Framework.Input;

namespace Game2
{
    public class LaSilEngine
    {
        List<GameObject> gameObjects;
        Map map;
        Vector2 position;
        public enum Direction { North, East, South, West};
        private int directionCount = Enum.GetNames(typeof(Direction)).Length;
        Direction direction;
        Random rand;
        Boolean startRotatingRight, startRotatingLeft, startMovingForward, startMovingBackward = false;

        public LaSilEngine(Random rand, Vector2 pos)
            : base()
        {
            gameObjects = new List<GameObject>();
            position = pos;
            direction = Direction.North;
            this.rand = rand;
        }
        public void LoadMap()
        {
            map = new Map(rand);
        }
        public void AddObject(GameObject newObject)
        {
            gameObjects.Add(newObject);
        }
        public void AddCamera()
        { }
        public void Update(GameTime gameTime, KeyboardState keyboardState)
        {
            foreach (GameObject gameObject in gameObjects)
            {
                gameObject.Update(gameTime);
            }
            // start moving
            if (keyboardState.IsKeyDown(Keys.Right))
            { startRotatingRight = true; }
            if (keyboardState.IsKeyDown(Keys.Left))
            { startRotatingLeft = true; }
            if (keyboardState.IsKeyDown(Keys.Up))
            { startMovingForward = true; }
            if (keyboardState.IsKeyDown(Keys.Down))
            { startMovingBackward = true; }
            // actually moving
            if (keyboardState.IsKeyUp(Keys.Right) && startRotatingRight)
            {
                startRotatingRight = false;
                direction++;
                if ((int)direction >= directionCount)
                { direction = (Direction)0; }
            }
            if (keyboardState.IsKeyUp(Keys.Left) && startRotatingLeft)
            {
                startRotatingLeft = false;
                direction--;
                if ((int)direction < 0)
                { direction = (Direction)(directionCount-1); }
            }
            if (keyboardState.IsKeyUp(Keys.Up) && startMovingForward)
            {
                startMovingForward = false;
                // if direction east/west
                switch (direction){
                    case Direction.North:
                        {
                            if (position.Y > 0)
                            {
                                position.Y--;
                            }
                            break;
                        }
                    case Direction.East:
                        {
                            if (position.X < (map.x - 1))
                            {
                                position.X++;
                            }
                            break;
                        }
                    case Direction.South:
                        {
                            if (position.Y < (map.y - 1))
                            {
                                position.Y++;
                            }
                            break;
                        }
                    case Direction.West:
                        {
                            if (position.X > 0)
                            {
                                position.X--;
                            }
                            break; 
                        }
                }
                
                
                
            }
            if (keyboardState.IsKeyUp(Keys.Down) && startMovingBackward)
            { 
                startMovingBackward = false;
                switch (direction)
                {
                    case Direction.South:
                        {
                            if (position.Y > 0)
                            {
                                position.Y--;
                            }
                            break;
                        }
                    case Direction.West:
                        {
                            if (position.X < (map.x - 1))
                            {
                                position.X++;
                            }
                            break;
                        }
                    case Direction.North:
                        {
                            if (position.Y < (map.y - 1))
                            {
                                position.Y++;
                            }
                            break;
                        }
                    case Direction.East:
                        {
                            if (position.X > 0)
                            {
                                position.X--;
                            }
                            break;
                        }
                }
            }
        }
        public void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            // рисуем объекты
            foreach (GameObject gameObject in gameObjects)
            {
                gameObject.Draw(spriteBatch);
            }
            // рисуем карту
            map.Draw(spriteBatch, font, 10, 10, 400, 400);
            // рисуем камеру и направление
            Vector2 cameraPosition = map.GetCellCenter((int)position.X,(int)position.Y,400,400);
            spriteBatch.DrawString(font, "" + (int)direction, cameraPosition, Color.Red);
            //рисуем, что видим
            spriteBatch.DrawString(font, "" + (int)map.GetCellSide((int)position.X, (int)position.Y,direction), new Vector2(500, 200), Color.Silver);
        }
        private Vector2 GetRandomLocation()
        {
            return Vector2.Zero;
        }
    }


    public class GameObject
    {
        public GameObject(): base()
        {

        }
        public void Update(GameTime gameTime)
        {}
        public void Draw(SpriteBatch spriteBatch)
        {}
    }



    /// <summary>
    /// Класс, отвечающий за камеру. Он обеспечивает отслеживание её координат, перемещение и направление
    /// Видимость обрабатывается в движке
    /// </summary>
    public class Camera
    { 
        public Camera():base()
        {

        }
    }

    /// <summary>
    /// Класс, отвечающий за хранение и работу с картой
    /// </summary>
    public class Map
    {
        private MapCell[,] map;
        private Random rand;
        private const int margin = 20;
        public int x = 3, y = 3;
        public Map(Random rand):base()
        {
            this.rand = rand;
            map = new MapCell[x,y];
            for (int dx=0; dx<3;dx++)
            {
                for (int dy = 0; dy<3;dy++)
                {
                    map[dx, dy] = new MapCell(rand);
                }
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
            if (x >= map.GetLength(0) || y >= map.GetLength(1) || x<0 || y<0)
            {
                throw new MapCellException("Cell index out of range!");
            }
            int cellWidth = (width - margin * map.GetLength(0)) / map.GetLength(0);
            int cellHeight = (height - margin * map.GetLength(1)) / map.GetLength(1);
            return new Vector2((cellWidth+margin)/2+x*(cellWidth+margin), (cellHeight+margin)/2+y*(cellHeight+margin));
        }
        public Border.BorderTypes GetCellSide(int x, int y, LaSilEngine.Direction side)
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
                    map[xCell, yCell].Draw(spriteBatch, font, x + (cellWidth+margin) * xCell, y + (cellHeight+margin) * yCell, cellWidth, cellHeight);
                }
            }
        }

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
    }
    
    public class Border
    {
        public Border(Random rand)
            : base()
        {
            this.rand = rand;
        }
        public enum BorderTypes { Free, Wal, Trees, Water };
        public Random rand;
        public BorderTypes GetRandomBorderType()
        {

            int typeNumber = rand.Next(Enum.GetNames(typeof(BorderTypes)).Length);
            return (BorderTypes)typeNumber;
        }
    }

    

    public class MapCell : Border
    {
        BorderTypes east, west, north, south;
        public MapCell(Random rand, BorderTypes east, BorderTypes west, BorderTypes north, BorderTypes south)
            : base(rand)
        {
            this.east = east;
            this.west = west;
            this.north = north;
            this.south = south;
        }
        public MapCell(Random rand):base(rand)
        {

            east = GetRandomBorderType();
            west = GetRandomBorderType();
            north = GetRandomBorderType();
            south = GetRandomBorderType();
            
        }
        public BorderTypes GetSide(LaSilEngine.Direction side)
        {
            BorderTypes res=BorderTypes.Free;
            switch (side)
            {
                case LaSilEngine.Direction.East:
                    {
                        res= east;
                        break;
                    }
                case LaSilEngine.Direction.North:
                    {
                        res= north;
                        break;
                    }
                case LaSilEngine.Direction.South:
                    {
                        res= south;
                        break;
                    }
                case LaSilEngine.Direction.West:
                    {
                        res= west;
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
            spriteBatch.DrawString(font, ""+(int)this.west, new Vector2(x, y+height/2), Color.White);
            spriteBatch.DrawString(font, "" + (int)this.east, new Vector2(x+width, y + height / 2), Color.White);
            spriteBatch.DrawString(font, "" + (int)this.north, new Vector2(x+width/2, y), Color.White);
            spriteBatch.DrawString(font, "" + (int)this.south, new Vector2(x + width / 2, y+height), Color.White);
        }
    }

    
}
