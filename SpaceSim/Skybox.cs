using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpaceSim
{
    public class Skybox
    {
        public Matrix Transform;

        Model model;
        Texture2D texture;

        public Skybox(Matrix transform, ContentManager content)
        {
            this.Transform = transform;

            model = content.Load<Model>("SkyboxModel");
            texture = content.Load<Texture2D>("Skybox");

            foreach (BasicEffect effect in model.Meshes[0].Effects)
            {
                effect.Texture = texture;
                effect.TextureEnabled = true;
                effect.LightingEnabled = false;
            }
        }

        public void Draw()
        {
            model.Draw(Transform, SpaceSim.World.View, SpaceSim.World.Projection);
        }
    }
}
