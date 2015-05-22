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
    /// Класс, отвечающий за камеру. Он обеспечивает отслеживание её координат, перемещение и направление
    /// Видимость обрабатывается в движке
    /// </summary>
    public class Camera
    {
        #region fields
        /// <summary>
        /// Координаты камеры
        /// </summary>
        private Vector2 pos;
        /// <summary>
        /// Размеры карты
        /// </summary>
        private int x, y;
        /// <summary>
        /// Направление камеры
        /// </summary>
        private LaSilEngineConstants.Direction direction;
        private int directionCount = Enum.GetNames(typeof(LaSilEngineConstants.Direction)).Length;

        #endregion fields

        #region constructor
        /// <summary>
        /// Создаём объект камеры. Если камера расположена за пределами карты - помещаем её в 0,0
        /// </summary>
        /// <param name="pos">Положение камеры</param>
        /// <param name="direction">Направление камеры</param>
        /// <param name="x">Размер карты по горизонтали</param>
        /// <param name="y">Размер карты по вертикали</param>
        public Camera(Vector2 pos, LaSilEngineConstants.Direction direction, int x, int y)
            : base()
        {
            this.pos = pos;
            this.direction = direction;
            this.x = x;
            this.y = y;
            /// Проверяем, что место камеры не находится вне карты. Если находится - ставим в 0,0
            if (pos.X < 0 || pos.X >= x || pos.Y < 0 || pos.Y >= y)
            {
                pos.X = 0;
                pos.Y = 0;
            }
        }
        #endregion constructor

        #region properties
        public Vector2 Position
        {
            get
            {
                return pos;
            }
            set
            {
                if (value.X < 0 || value.X >= x || value.Y < 0 || value.Y >= y)
                {
                    pos.X = 0;
                    pos.Y = 0;
                }
                else
                {
                    pos = value;
                }
            }
        }
        public LaSilEngineConstants.Direction Direction
        {
            get
            {
                return direction;
            }
            set
            {
                direction = value;
            }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Перемещение камеры вперёд/назад
        /// </summary>
        /// <param name="steps">Насколько передвинуть камеру. + - вперёд, - - назад</param>
        public void Move(int steps, Map map)
        {
            int newpos;
            Vector2 oldpos = pos;
            switch (direction)
            {
                case LaSilEngineConstants.Direction.North:
                    {
                        newpos = (int)pos.Y - steps;
                        if (newpos >= 0 && newpos < y)
                        {
                            pos.Y=newpos;
                        }
                        break;
                    }
                case LaSilEngineConstants.Direction.East:
                    {
                        newpos = (int)pos.X + steps;
                        if (newpos >= 0 && newpos < x)
                        {
                            pos.X = newpos;
                        }
                        break;
                    }
                case LaSilEngineConstants.Direction.South:
                    {
                        newpos = (int)pos.Y + steps;
                        if (newpos >= 0 && newpos < y)
                        {
                            pos.Y = newpos;
                        }
                        break;
                    }
                case LaSilEngineConstants.Direction.West:
                    {
                        newpos = (int)pos.X - steps;
                        if (newpos >= 0 && newpos < x)
                        {
                            pos.X = newpos;
                        }
                        break;
                    }
            }
            // Если пройти нельзя
            if (map.IsTraversal(oldpos,pos,this)<0)
            {
                pos = oldpos;
            }
        }
        /// <summary>
        /// Повернуться
        /// </summary>
        /// <param name="cw">По часовой стрелке (вправо) = true, или нет = false</param>
        public void Turn(bool cw)
        { 
            if (cw)
            {
                direction++;
                if ((int)direction >= directionCount)
                { direction = (LaSilEngineConstants.Direction)0; }
            }
            else
            {
                direction--;
                if ((int)direction < 0)
                { direction = (LaSilEngineConstants.Direction)(directionCount - 1); }
            }
        }
        #endregion methods
    }
}
