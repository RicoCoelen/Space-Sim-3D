using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace SpaceSim
{
    class Bullet : Sphere
    {
        public Vector3 Position;
        public Vector3 Velocity;

        public Bullet(Vector3 position, Vector3 velocity) : base(Matrix.CreateScale(0.005f) * Matrix.CreateTranslation(position), Color.White, 20)
        {
            Position = position;
            Velocity = velocity;
        }
        public void Update(float gametime)
        {
            // change position over time using velocity
            Position += this.Velocity * gametime;
            Transform = Matrix.CreateScale(0.005f) * Matrix.CreateTranslation(Position);
        }
    }
}
