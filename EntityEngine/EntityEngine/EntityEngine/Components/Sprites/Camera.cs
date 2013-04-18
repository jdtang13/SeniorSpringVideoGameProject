using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using EntityEngine.Components.Component_Parents;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngine.Components.Sprites
{
    public static class Camera
    {
        static Matrix transformMatrix;

        static float zoom= 1.0f;
        static Vector2 position = Vector2.Zero;
        static float rotation = 0.0f;
        public static float Zoom
        {
            get
            {
                return zoom;
            }
            set
            {
                zoom = value; if (zoom < 0.1f) zoom = 0.1f;
            } // Negative zoom will flip image
        }
        public static Vector2 Pos
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }
        public static float Rotation
        {
            get
            {
                return rotation;
            }
            set
            {
                rotation = value;
            }
        }

        public static void Move(Vector2 amount)
        {
            position += amount;
        }
        public static void MoveTo(Vector2 myVector)
        {
            position = myVector;
        }

        public static Matrix GetTransformation(GraphicsDevice graphicsDevice)
        {
            transformMatrix =       
              Matrix.CreateTranslation(new Vector3(-position.X, -position.Y, 0)) *
                                         Matrix.CreateRotationZ(Rotation) *
                                         Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                                         Matrix.CreateTranslation(new Vector3(graphicsDevice.Viewport.Width * 0.5f, graphicsDevice.Viewport.Height * 0.5f, 0));
            return transformMatrix;
        }
    }

}