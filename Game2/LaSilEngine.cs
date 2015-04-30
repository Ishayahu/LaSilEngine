using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework; // for GameTime
using Microsoft.Xna.Framework.Graphics; // for SpriteBatch
//using Microsoft.Xna.Framework.Input;

namespace Game2
{
    public class LaSilEngine
    {
        List<GameObject> gameObjects;
        Map map;
        Vector2 position;
        enum Direction { North, East, South, West};
        Direction direction;

        public LaSilEngine()
            : base()
        {
            gameObjects = new List<GameObject>();
            position = new Vector2(1,1);
            direction = Direction.North;
        }
        public void LoadMap()
        {
            map = new Map();
        }
        public void AddObject(GameObject newObject)
        {
            gameObjects.Add(newObject);
        }
        public void AddCamera()
        { }
        public void Update(GameTime gameTime)
        {
            foreach (GameObject gameObject in gameObjects)
            {
                gameObject.Update(gameTime);
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (GameObject gameObject in gameObjects)
            {
                gameObject.Draw(spriteBatch);
            }
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

    public enum BorderTypes { Free, Wal, Trees, Water };

    

    /// <summary>
    /// Класс, отвечающий за хранение и работу с картой
    /// </summary>
    public class Map
    {
        private MapCell[,] map;
        private Random rand = new Random();
        private BorderTypes GetRandomBorderType()
        {
            int typeNumber = rand.Next(Enum.GetNames(typeof(BorderTypes)).Length);
            return (BorderTypes)typeNumber;
        }
        public Map():base()
        {
            map = new MapCell[3, 3];
            for (int x=0; x<3;x++)
            {
                for (int y = 0; y<3;y++)
                {
                    map[x, y] = new MapCell(GetRandomBorderType(),
                        GetRandomBorderType(),
                        GetRandomBorderType(),
                        GetRandomBorderType());
                }
            }
        }
        public void LoadMap()
        { }
        /// <summary>
        /// Возвращает клеткy карты:
        /// </summary>
        /// <returns></returns>
        public MapCell GetCell(int x, int y)
        {
            if (x >= map.GetLength(0) || y >= map.GetLength(1) || x<0 || y<0)
            {
                throw new MapCellException("Cell index out of range!");
            }
            
            return map[x, y];
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

    public class MapCell
    {
        BorderTypes left, right, forward, backward;
        public MapCell(BorderTypes left, BorderTypes right, BorderTypes forward, BorderTypes backward):base()
        {
            this.left = left;
            this.right = right;
            this.forward = forward;
            this.backward = backward;
        }
        public BorderTypes Left()
        {
            return this.left;
        }
        public BorderTypes Right()
        {
            return this.right;
        }
        public BorderTypes Forward()
        {
            return this.forward;
        }
        public BorderTypes Backward()
        {
            return this.backward;
        }
    }

    
}
