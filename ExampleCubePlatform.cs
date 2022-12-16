using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Microsoft.Xna.Framework;
using Terraria.GameContent.Bestiary;

namespace CollisionLib { 
    public class ExampleCubePlatform : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cube Platform");
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Hide = true // Hides this NPC from the Bestiary, useful for multi-part NPCs whom you only want one entry.
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
        }

        

        public CollisionSurface[] colliders = null;  // because this object has multiple colliders, it uses a list of collision surfaces to make updating them all easier
        public override void SetDefaults()
        {
            NPC.width = 124;
            NPC.height = 124;
            NPC.lifeMax = 1;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.noGravity = true;
            NPC.knockBackResist = 0;
            NPC.aiStyle = -1;

        }

        public override bool CheckActive()
        {
            return true;
        }

        public override bool PreAI()
        {
            if (colliders == null || colliders.Length != 4) //initializes the collision surfaces if null
            {
                colliders = new CollisionSurface[] { 
                    new CollisionSurface(NPC.TopLeft, NPC.TopRight, new int[] { 2, 1, 1, 1 }, true),  // the first two vector2s are the endpoints, the int array is the data for collision styles, the boolean determines if this surface can be grappled
                    new CollisionSurface(NPC.TopLeft, NPC.BottomLeft, new int[] { 2, 1, 1, 1 }, true), 
                    new CollisionSurface(NPC.TopRight, NPC.BottomRight, new int[] { 2, 1, 1, 1 }, true),
                    new CollisionSurface(NPC.BottomLeft, NPC.BottomRight, new int[] { 2, 1, 1, 1 }, true) };
            }

        /*
         * CollisionStyles controls which sides of the player can collide with each surface,
         * index 0 = bottom collision
         * index 1 = top collision
         * index 2 = left collision
         * index 4 = right collision
         * 
         * value 0 = doesnt collide
         * value 1 = collides without the ability to drop through
         * value 2 = collides but the player can drop through, doesn't do anything different from value 1 unless the index is zero (Ie, the surface is set to collide with the bottom of the player)
         *
         */

            return base.PreAI();
        }


        public override void AI()
        {
            /* calls update and then sets new endpoints, to get collision surfaces to move you need to perform the calls in this order:
             * 
             *  Call Update() in a pre-update or mid-update method
             *  Change endpoint positions afterwards in the same method
             *  Call PostUpdate in a post-update method
             * 
             */

            ++NPC.ai[0];
            NPC.velocity.X = (float)Math.Cos(MathHelper.ToRadians(NPC.ai[0]) / 3);
            //NPC.rotation += MathHelper.ToRadians(2);

            if (colliders != null && colliders.Length == 4) // trying to update a collision surface which isn't defined will lead to a exception
            {

                colliders[0].Update();
                colliders[0].endPoints[0] = NPC.Center + (NPC.TopLeft - NPC.Center).RotatedBy(NPC.rotation);
                colliders[0].endPoints[1] = NPC.Center + (NPC.TopRight - NPC.Center).RotatedBy(NPC.rotation);

                colliders[1].Update();
                colliders[1].endPoints[0] = NPC.Center + (NPC.TopLeft - NPC.Center).RotatedBy(NPC.rotation);
                colliders[1].endPoints[1] = NPC.Center + (NPC.BottomLeft - NPC.Center).RotatedBy(NPC.rotation);

                colliders[2].Update();
                colliders[2].endPoints[0] = NPC.Center + (NPC.TopRight - NPC.Center).RotatedBy(NPC.rotation);
                colliders[2].endPoints[1] = NPC.Center + (NPC.BottomRight - NPC.Center).RotatedBy(NPC.rotation);

                colliders[3].Update();
                colliders[3].endPoints[0] = NPC.Center + (NPC.BottomLeft - NPC.Center).RotatedBy(NPC.rotation);
                colliders[3].endPoints[1] = NPC.Center + (NPC.BottomRight - NPC.Center).RotatedBy(NPC.rotation);


            }



        }

        public override void PostAI()
        {
            if (colliders != null)
            {
                foreach (CollisionSurface collider in colliders)
                {
                    collider.PostUpdate(); // calls postUpdate for every collision surface
                }
            }
        }



        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            
            
            base.SetBestiary(database, bestiaryEntry);
        }

    }
}