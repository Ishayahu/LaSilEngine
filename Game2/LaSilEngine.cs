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
        Random rand;
        Boolean startRotatingRight, startRotatingLeft, startMovingForward, startMovingBackward = false;
        Camera camera;

        public LaSilEngine(Random rand)
            : base()
        {
            gameObjects = new List<GameObject>();
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
        public void AddCamera(Vector2 pos)
        {
            camera = new Camera(pos, LaSilEngineConstants.Direction.North, LaSilEngineConstants.MAP_X_SIZE,
                LaSilEngineConstants.MAP_Y_SIZE);
        }
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
                camera.Turn(true);
            }
            if (keyboardState.IsKeyUp(Keys.Left) && startRotatingLeft)
            {
                startRotatingLeft = false;
                camera.Turn(false);
            }
            if (keyboardState.IsKeyUp(Keys.Up) && startMovingForward)
            {
                startMovingForward = false;
                camera.Move(1);
            }
            if (keyboardState.IsKeyUp(Keys.Down) && startMovingBackward)
            { 
                startMovingBackward = false;
                camera.Move(-1);
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
            Vector2 cameraPosition = camera.Position;
            Vector2 cameraMapPosition = map.GetCellCenter((int)cameraPosition.X, (int)cameraPosition.Y, 400, 400);
            spriteBatch.DrawString(font, "" + (int)camera.Direction, cameraMapPosition, Color.Red);
            //рисуем, что видим
            spriteBatch.DrawString(font, "" + (int)map.GetCellSide((int)cameraPosition.X, (int)cameraPosition.Y, camera.Direction), new Vector2(500, 200), Color.Silver);
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
    /// Класс, отвечающий за хранение и работу с картой
    /// </summary>
    public class Map
    {
        private MapCell[,] map;
        private Random rand;
        private const int margin = 20;
        public int x = LaSilEngineConstants.MAP_X_SIZE, y = LaSilEngineConstants.MAP_Y_SIZE;
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
        public Border.BorderTypes GetCellSide(int x, int y, LaSilEngineConstants.Direction side)
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
        public BorderTypes GetSide(LaSilEngineConstants.Direction side)
        {
            BorderTypes res=BorderTypes.Free;
            switch (side)
            {
                case LaSilEngineConstants.Direction.East:
                    {
                        res= east;
                        break;
                    }
                case LaSilEngineConstants.Direction.North:
                    {
                        res= north;
                        break;
                    }
                case LaSilEngineConstants.Direction.South:
                    {
                        res= south;
                        break;
                    }
                case LaSilEngineConstants.Direction.West:
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
