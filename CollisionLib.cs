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

        private void CollisionDetour(On.Terraria.Player.orig_Update_NPCCollision orig, Terraria.Player self)
        {

            PhysicsPlayer physics = self.GetModPlayer<PhysicsPlayer>();

            if (self.GetModPlayer<PhysicsPlayer>().collisionPointBottom != Vector2.Zero && !self.GetModPlayer<PhysicsPlayer>().canDropThrough)
            {
                if (!self.justJumped && self.velocity.Y > 0)
                {


                    self.velocity.Y=0;
                    self.fallStart = (int)(self.position.Y / 16f);
                    if (!self.controlJump)
                    {
                        self.position.Y = self.GetModPlayer<PhysicsPlayer>().collisionPointBottom.Y - self.height - 2;
                    }
                    
                    //self.position.Y = npc.position.Y - self.height + 4;
                    // orig(self);
                }

            }

            if (self.GetModPlayer<PhysicsPlayer>().collisionPointTop != Vector2.Zero)
            {
                if (self.velocity.Y < 0)
                {
                    //self.gfxOffY = npc.gfxOffY;
                    self.velocity.Y = 0;
                    self.fallStart = (int)(self.position.Y / 16f);
                    self.position.Y = self.GetModPlayer<PhysicsPlayer>().collisionPointTop.Y + 4;
                    //self.position.Y = npc.position.Y - self.height + 4;
                    // orig(self);
                }

            }

            if (self.GetModPlayer<PhysicsPlayer>().collisionPointLeft != Vector2.Zero)
            {
                if (self.velocity.X <= 0)
                {
                    //self.gfxOffY = npc.gfxOffY;
                    self.velocity.X = 0;
                    //self.fallStart = (int)(self.position.Y / 16f);
                    self.position.X = self.GetModPlayer<PhysicsPlayer>().collisionPointLeft.X + 4;
                    //self.position.Y = npc.position.Y - self.height + 4; - self.width -
                    // orig(self);
                }

            }

            if (self.GetModPlayer<PhysicsPlayer>().collisionPointRight != Vector2.Zero)
            {
                if (self.velocity.X >= 0)
                {
                    //self.gfxOffY = npc.gfxOffY;
                    self.velocity.X = 0;
                    //self.fallStart = (int)(self.position.Y / 16f);
                    self.position.X = self.GetModPlayer<PhysicsPlayer>().collisionPointRight.X - self.width - 3;
                    //self.position.Y = npc.position.Y - self.height + 4; - self.width -
                    // orig(self);
                }

            }


            if (self.controlDown) self.GetModPlayer<PhysicsPlayer>().PlatformCounter = 5;
            if (self.controlDown || self.GetModPlayer<PhysicsPlayer>().PlatformCounter > 0 || self.GoingDownWithGrapple) { orig(self); return; }

            if (self.GetModPlayer<PhysicsPlayer>().collisionPointBottom != Vector2.Zero && self.GetModPlayer<PhysicsPlayer>().canDropThrough)
            {
                if (!self.justJumped && self.velocity.Y >= 0)
                {
                    //self.gfxOffY = npc.gfxOffY;
                    self.velocity.Y = 0;
                    self.fallStart = (int)(self.position.Y / 16f);
                    self.position.Y = self.GetModPlayer<PhysicsPlayer>().collisionPointBottom.Y - self.height - 2;
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