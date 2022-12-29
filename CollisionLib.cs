using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.UI;
using Terraria.GameContent.UI;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using On.Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.Elements;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria.ModLoader.Config;

namespace CollisionLib
{
	public class CollisionLib : Mod
	{
		public static CollisionLib Instance;

		public CollisionLib()
        {
			Instance = this;
        }

        public override void Load()
        {
            On.Terraria.Player.Update_NPCCollision += CollisionDetour;
        }

        private void CollisionDetour(On.Terraria.Player.orig_Update_NPCCollision orig, Player self)
        {
            PhysicsPlayer physics = self.GetModPlayer<PhysicsPlayer>();

			if (physics.collisionPointBottom != Vector2.Zero && !physics.canDropThrough)
            {
                if (!self.justJumped && self.velocity.Y > 0)
                {


                    self.velocity.Y=0;
                    self.fallStart = (int)(self.position.Y / 16f);
                    if (!self.controlJump)
                    {
                        self.position.Y = physics.collisionPointBottom.Y - self.height - 2;
                    }
                    
                    //self.position.Y = npc.position.Y - self.height + 4;
                    // orig(self);
                }

            }

            if (physics.collisionPointTop != Vector2.Zero)
            {
                if (self.velocity.Y < 0)
                {
                    //self.gfxOffY = npc.gfxOffY;
                    self.velocity.Y = 0;
                    self.fallStart = (int)(self.position.Y / 16f);
                    self.position.Y = physics.collisionPointTop.Y + 4;
                    //self.position.Y = npc.position.Y - self.height + 4;
                    // orig(self);
                }

            }

            if (physics.collisionPointLeft != Vector2.Zero)
            {
                if (self.velocity.X <= 0)
                {
                    //self.gfxOffY = npc.gfxOffY;
                    self.velocity.X = 0;
                    //self.fallStart = (int)(self.position.Y / 16f);
                    self.position.X = physics.collisionPointLeft.X + 4;
                    //self.position.Y = npc.position.Y - self.height + 4; - self.width -
                    // orig(self);
                }

            }

            if (physics.collisionPointRight != Vector2.Zero)
            {
                if (self.velocity.X >= 0)
                {
                    //self.gfxOffY = npc.gfxOffY;
                    self.velocity.X = 0;
                    //self.fallStart = (int)(self.position.Y / 16f);
                    self.position.X = physics.collisionPointRight.X - self.width - 3;
                    //self.position.Y = npc.position.Y - self.height + 4; - self.width -
                    // orig(self);
                }

            }


            if (self.controlDown) physics.PlatformCounter = 5;
            if (self.controlDown || physics.PlatformCounter > 0 || self.GoingDownWithGrapple) { orig(self); return; }

            if (physics.collisionPointBottom != Vector2.Zero && physics.canDropThrough)
            {
                if (!self.justJumped && self.velocity.Y >= 0)
                {
                    //self.gfxOffY = npc.gfxOffY;
                    self.velocity.Y = 0;
                    self.fallStart = (int)(self.position.Y / 16f);
                    self.position.Y = physics.collisionPointBottom.Y - self.height - 2;
                    //self.position.Y = npc.position.Y - self.height + 4;
                    // orig(self);
                }

            }

            /*foreach (Terraria.NPC npc in Terraria.Main.npc.Where(n => n.active && n.modNPC != null && n.modNPC is NPCs.Environment.MovablePlatform))
            {
                if (new Rectangle((int)self.position.X, (int)self.position.Y + (self.height), self.width, 2).Intersects
                (new Rectangle((int)npc.position.X, (int)npc.position.Y, npc.width, 8 + (self.velocity.Y > 0 ? (int)self.velocity.Y : 0))) && self.position.Y <= npc.position.Y)
                {
                    if (!self.justJumped && self.velocity.Y >= 0)
                    {
                        self.gfxOffY = npc.gfxOffY;
                        self.velocity.Y = 0;
                        self.fallStart = (int)(self.position.Y / 16f);
                        self.position.Y = npc.position.Y - self.height + 4;
                        orig(self);
                    }
                }
            }*/
            orig(self);
        }
    }
}