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
        #region fields
        List<GameObject> gameObjects;
        Map map;
        Random rand;
        Boolean startRotatingRight, startRotatingLeft, startMovingForward, startMovingBackward = false;
        Camera camera;

        // для рисования поля обзора
        private int cellsToEnd;
        private string[] directionsChars = { "N", "E", "S", "W" };
        #endregion fields

        #region constructor
        public LaSilEngine(Random rand)
        {
            gameObjects = new List<GameObject>();
            this.rand = rand;
        }
        #endregion

        #region methods
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
                camera.Move(1, map);
            }
            if (keyboardState.IsKeyUp(Keys.Down) && startMovingBackward)
            { 
                startMovingBackward = false;
                camera.Move(-1, map);
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
            
            spriteBatch.DrawString(font, "" + directionsChars[(int)camera.Direction], cameraMapPosition, Color.Red);
            //рисуем, что видим

            //рассчитываем, что видим, проходясь по всем клеткам в направлении обзора
            //пока без учёта освещённости

            // считаем, сколько клеток осталось
            switch (camera.Direction)
            {
                case LaSilEngineConstants.Direction.North:
                    {
                        cellsToEnd = (int)camera.Position.Y + 1;
                        break;
                    }
                case LaSilEngineConstants.Direction.South:
                    {
                        cellsToEnd = map.Y - (int)camera.Position.Y;
                        break;
                    }
                case LaSilEngineConstants.Direction.West:
                    {
                        cellsToEnd = (int)camera.Position.X + 1;
                        break;
                    }
                case LaSilEngineConstants.Direction.East:
                    {
                        cellsToEnd = map.X - (int)camera.Position.X;
                        break;
                    }
                default:
                    {
                        cellsToEnd = -1;
                        break;
                    }
            }
            
            spriteBatch.DrawString(font, ""+cellsToEnd, new Vector2(500, 200), Color.Silver);
        }
        private Vector2 GetRandomLocation()
        {
            return Vector2.Zero;
        }
        #endregion methods
    }


    public class GameObject
    {
        #region constructor
        public GameObject(): base()
        {

        }
        #endregion 

        #region methods
        public void Update(GameTime gameTime)
        {}
        public void Draw(SpriteBatch spriteBatch)
        { }
        #endregion
    }





    


    
}
