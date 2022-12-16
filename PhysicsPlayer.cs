using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Ionic.Zip;

namespace CollisionLib
{
    class PhysicsPlayer : ModPlayer
    {

        public bool collisionBottom = false;
        public bool collisionTop = false;
        public bool collisionLeft = false;
        public bool collisionRight = false;

        public bool canDropThrough = true;

        public Vector2 collisionRaycastBottom = Vector2.Zero;
        public Vector2 collisionRaycastTop = Vector2.Zero;
        public Vector2 collisionRaycastLeft = Vector2.Zero;
        public Vector2 collisionRaycastRight = Vector2.Zero;

        public Vector2 collisionPointBottom = Vector2.Zero;
        public Vector2 collisionPointTop = Vector2.Zero;
        public Vector2 collisionPointLeft = Vector2.Zero;
        public Vector2 collisionPointRight = Vector2.Zero;

        public int PlatformCounter = 0;

        public override void ResetEffects()
        {
            

        }

        public override void PreUpdate()
        {
            --PlatformCounter;



            //sets raycast points
            collisionRaycastBottom = Player.Center + new Vector2(0, (Player.height / 2) + 5);
            collisionRaycastTop = Player.Center + new Vector2(0, -((Player.height / 2) + 6));
            collisionRaycastRight = Player.Center + (new Vector2(((Player.width / 2) + 6), 0));
            collisionRaycastLeft = Player.Center + (new Vector2(-((Player.width / 2) + 6), 0));
        }


        public override void PostUpdate()
        {



           // Main.NewText("Collisions (bottom, top, left, right): " + collisionBottom + ", " + collisionTop + ", " + collisionLeft + ", " + collisionRight + ", ");
            
            collisionBottom = false;
            collisionLeft = false;
            collisionRight = false;
            collisionTop = false;
            collisionPointBottom = Vector2.Zero;
            collisionPointLeft = Vector2.Zero;
            collisionPointRight = Vector2.Zero;
            collisionPointTop = Vector2.Zero;
        }
    }


}
