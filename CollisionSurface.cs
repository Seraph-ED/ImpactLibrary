using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace CollisionLib
{

    /// <summary>
    /// Collision Surfaces are orientable "wall" objects which have their positions determined by two end points. To use collision surfaces, they must be defined and updated from another class which is updated ingame, such as an NPC or Projectile.
    /// </summary>
    public class CollisionSurface
    {
        

        public Vector2[] endPoints = new Vector2[2];

        public Vector2[] oldEndPoints = new Vector2[2];

        public Vector2[] velocities = new Vector2[2];

        public float rotation = 0;

        public Vector2 playerCollisionPoint = Vector2.Zero;

        public float collidePointRelative = 0;

        public bool grappleable = false;


        public int[] collisionStyles = new int[4];


        public bool currentlyColliding = false;
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
        public CollisionSurface(Vector2 point1, Vector2 point2, int[] styles = null, bool canGrapple = false)
        {
            endPoints[0] = point1;
            endPoints[1] = point2;

            grappleable = canGrapple;
            if (styles == null || styles.Length != 4)
            {
                collisionStyles = new int[] { 2, 0, 0, 0 };
            }
            else
            {
                collisionStyles = styles;
            }
        }


        /// <summary>
        /// Calls the first part of collision surface updates, which handles detecting player collisions. Call this in a mid update method like NPC/Projectile.AI();
        /// To make the collision surface move, update it's endpoint positions AFTER this method is called but BEFORE PostUpdate() is called
        /// </summary>
        public void Update()//first updates
        {

            oldEndPoints[0] = endPoints[0];
            oldEndPoints[1] = endPoints[1];
            rotation = (endPoints[1] - endPoints[0]).ToRotation() - MathHelper.PiOver2;

            DetectPlayerCollisions();
            if (grappleable)
            {
                DetectGrappleHookCollision();
            }

        }

        public static Vector2 LineIntersection(Vector2 l1p1, Vector2 l1p2, Vector2 l2p1, Vector2 l2p2) // Thanks to OS for help making this methods
        {
            float q = (l1p1.Y - l2p1.Y) * (l2p2.X - l2p1.X) - (l1p1.X - l2p1.X) * (l2p2.Y - l2p1.Y);
            float d = (l1p2.X - l1p1.X) * (l2p2.Y - l2p1.Y) - (l1p2.Y - l1p1.Y) * (l2p2.X - l2p1.X);
            if (d == 0)
                return Vector2.Zero;
            float r = q / d;
            q = (l1p1.Y - l2p1.Y) * (l1p2.X - l1p1.X) - (l1p1.X - l2p1.X) * (l1p2.Y - l1p1.Y);
            float s = q / d;
            if (r < 0 || r > 1 || s < 0 || s > 1)
                return Vector2.Zero;
            return Vector2.Lerp(l1p1, l1p2, r);
        }

        public static float GetLineIntersectionLerp(Vector2 l1p1, Vector2 l1p2, Vector2 l2p1, Vector2 l2p2) // Thanks to OS for help making this methods
        {
            float q = (l1p1.Y - l2p1.Y) * (l2p2.X - l2p1.X) - (l1p1.X - l2p1.X) * (l2p2.Y - l2p1.Y);
            float d = (l1p2.X - l1p1.X) * (l2p2.Y - l2p1.Y) - (l1p2.Y - l1p1.Y) * (l2p2.X - l2p1.X);
            if (d == 0)
                return -1;
            float r = q / d;
            q = (l1p1.Y - l2p1.Y) * (l1p2.X - l1p1.X) - (l1p1.X - l2p1.X) * (l1p2.Y - l1p1.Y);
            float s = q / d;
            if (r < 0 || r > 1 || s < 0 || s > 1)
                return -1;
            return r;
        }

        public Vector2 GetPlanarUnitVector(bool whichside = false)
        {
            float d1 = endPoints[1].X - endPoints[0].X;

            float d2 = endPoints[1].Y - endPoints[0].Y;

            double angle = Math.Atan2(d2, d1);

            if (whichside)
            {
                return Vector2.UnitX.RotatedBy(angle + MathHelper.PiOver2);
            }
            else
            {
                return Vector2.UnitX.RotatedBy(angle - MathHelper.PiOver2);

            }

        }

        public float GetPlanarAngle(int whichside = 1)
        {
            float d1 = endPoints[1].X - endPoints[0].X;

            float d2 = endPoints[1].Y - endPoints[0].Y;

            double angle = Math.Atan2(d2, d1);

            if (whichside==1)
            {
                return (float)angle + MathHelper.PiOver2;
            }
            else if (whichside == -1)
            {
                return (float)angle - MathHelper.PiOver2;

            }
            else
            {
                return 0;
            }

        }

        public void DetectGrappleHookCollision()
        {
            for(int i = 0; i < Main.maxProjectiles; ++i)
            {
                if (Main.projectile[i].aiStyle != 7 || !Main.projectile[i].active || Main.projectile[i].ai[0] == 1 || Main.projectile[i].timeLeft >= 35997)
                {
                    continue;
                }
                Projectile proj = Main.projectile[i];
                //Collision.canth
                if(Collision.CheckAABBvLineCollision(proj.position, proj.Size, endPoints[0], endPoints[1])|| Collision.CheckAABBvLineCollision(proj.GetGlobalProjectile<GrappleGlobal>().CollisionSurfaceGrapplePoint, proj.Size, endPoints[0], endPoints[1]))
                {

                    if (proj.GetGlobalProjectile<GrappleGlobal>().GrappledSurface == null)
                    {
                        proj.GetGlobalProjectile<GrappleGlobal>().GrappledSurface = new Ref<CollisionSurface>(this);

                    }
                    else if (proj.GetGlobalProjectile<GrappleGlobal>().GrappledSurface.Value != this)
                    {
                        continue;
                    }
                    
                    
                    
                    proj.ai[0] = 2;
                    
                    proj.netUpdate = true;
                    Main.player[proj.owner].wingTime = Main.player[proj.owner].wingTimeMax;
                    proj.GetGlobalProjectile<GrappleGlobal>().GrappledToCollsionSurface = true;




                   
                    if (proj.GetGlobalProjectile<GrappleGlobal>().CollisionSurfaceGrapplePoint == Vector2.Zero)
                    {
                        
                        
                        
                        
                        proj.GetGlobalProjectile<GrappleGlobal>().CollisionSurfaceGrapplePoint = proj.Center;

                        Vector2 p1 = LineIntersection(endPoints[0], endPoints[1], proj.position, proj.position + proj.Size);

                        if (p1 == Vector2.Zero)
                        {
                            //proj.GetGlobalProjectile<GrappleGlobal>().CollisionSurfaceGrapplePoint = LineIntersection(endPoints[0], endPoints[1], proj.position + new Vector2(proj.width, 0), proj.position + new Vector2(0, proj.height));
                            proj.GetGlobalProjectile<GrappleGlobal>().CollisionSurfaceGrappleDistance = GetLineIntersectionLerp(endPoints[0], endPoints[1], proj.position + new Vector2(proj.width, 0), proj.position + new Vector2(0, proj.height));
                        }
                        else
                        {
                            //proj.GetGlobalProjectile<GrappleGlobal>().CollisionSurfaceGrapplePoint = p1;
                            proj.GetGlobalProjectile<GrappleGlobal>().CollisionSurfaceGrappleDistance = GetLineIntersectionLerp(endPoints[0], endPoints[1], proj.position, proj.position + proj.Size);
                        }

                        if(proj.GetGlobalProjectile<GrappleGlobal>().CollisionSurfaceGrappleDistance > 0.95f)
                        {
                            proj.GetGlobalProjectile<GrappleGlobal>().CollisionSurfaceGrappleDistance = 0.95f;
                        }
                        else if(proj.GetGlobalProjectile<GrappleGlobal>().CollisionSurfaceGrappleDistance < 0.05f)
                        {
                            proj.GetGlobalProjectile<GrappleGlobal>().CollisionSurfaceGrappleDistance = 0.05f;
                        }

                        proj.GetGlobalProjectile<GrappleGlobal>().CollisionSurfaceGrapplePoint = Vector2.Lerp(endPoints[0], endPoints[1], proj.GetGlobalProjectile<GrappleGlobal>().CollisionSurfaceGrappleDistance);

                        if (Math.Abs(proj.AngleTo(Main.player[proj.owner].Center)- (GetPlanarAngle(-1)%MathHelper.TwoPi)) <= Math.Abs(proj.AngleTo(Main.player[proj.owner].Center) - (GetPlanarAngle(1) % MathHelper.TwoPi)))
                        {
                            proj.GetGlobalProjectile<GrappleGlobal>().CollisionSufraceGrappleSide = -1;
                        }
                        else
                        {
                            proj.GetGlobalProjectile<GrappleGlobal>().CollisionSufraceGrappleSide = 1;
                        }

                        proj.GetGlobalProjectile<GrappleGlobal>().CollisionSufraceGrappleAngle = GetPlanarAngle(proj.GetGlobalProjectile<GrappleGlobal>().CollisionSufraceGrappleSide);
                    }
                    else
                    {
                        proj.GetGlobalProjectile<GrappleGlobal>().CollisionSurfaceGrapplePoint = Vector2.Lerp(endPoints[0], endPoints[1], proj.GetGlobalProjectile<GrappleGlobal>().CollisionSurfaceGrappleDistance);
                        proj.GetGlobalProjectile<GrappleGlobal>().CollisionSufraceGrappleAngle = GetPlanarAngle(proj.GetGlobalProjectile<GrappleGlobal>().CollisionSufraceGrappleSide);
                        //proj.GetGlobalProjectile<GrappleGlobal>().CollisionSurfaceGrapplePoint += Vector2.Lerp(velocities[0], velocities[1], proj.GetGlobalProjectile<GrappleGlobal>().CollisionSurfaceGrappleDistance);
                        proj.Center = proj.GetGlobalProjectile<GrappleGlobal>().CollisionSurfaceGrapplePoint + Vector2.UnitX.RotatedBy(proj.GetGlobalProjectile<GrappleGlobal>().CollisionSufraceGrappleAngle) * 8;
                    }


                    



                }


            }



        }


        public void DetectPlayerCollisions()
        {

            for (int i = 0; i < Main.maxPlayers; ++i)
            {
                Player player = Main.player[i];

                if (!player.active)
                {
                    continue;
                }

                PhysicsPlayer modPlayer = player.GetModPlayer<PhysicsPlayer>();

                currentlyColliding = false;

                if (collisionStyles[0] > 0)
                {
                    if (LineIntersection(endPoints[0], endPoints[1], player.Center, modPlayer.collisionRaycastBottom) != Vector2.Zero)
                    {

                        if (collisionStyles[0] > 1&&player.controlDown)
                        {

                        }
                        else
                        {
                            modPlayer.collisionBottom = true;
                           
                            if (modPlayer.collisionBottom)
                            {
                                Vector2 currentPoint = LineIntersection(endPoints[0], endPoints[1], player.Center, modPlayer.collisionRaycastBottom);
                                float lerpValue = Vector2.Distance(currentPoint, endPoints[0]) / Vector2.Distance(endPoints[0], endPoints[1]);

                                if (Collision.CanHit(player.position, player.width, player.height, player.position + Vector2.Lerp(velocities[0], velocities[1], lerpValue), player.width, player.height))
                                {
                                    player.position += Vector2.Lerp(velocities[0], velocities[1], lerpValue);
                                } //(LineIntersection(endPoints[0], endPoints[1], player.Center, modPlayer.collisionRaycastBottom) - modPlayer.collisionPointBottom);
                            }
                            modPlayer.collisionPointBottom = LineIntersection(endPoints[0], endPoints[1], player.Center, modPlayer.collisionRaycastBottom);

                            if (collisionStyles[0] > 1)
                            {
                                modPlayer.canDropThrough = true;
                            }
                            else
                            {
                                modPlayer.canDropThrough = false;
                            }
                            currentlyColliding = true;
                        }
                        
                        



                        
                    }
                }
                if (collisionStyles[1] > 0)
                {
                    if (LineIntersection(endPoints[0], endPoints[1], player.Center, modPlayer.collisionRaycastTop) != Vector2.Zero)
                    {


                        modPlayer.collisionTop = true;
                        if (modPlayer.collisionTop)
                        {
                            Vector2 currentPoint = LineIntersection(endPoints[0], endPoints[1], player.Center, modPlayer.collisionRaycastTop);
                            float lerpValue = Vector2.Distance(currentPoint, endPoints[0]) / Vector2.Distance(endPoints[0], endPoints[1]);
                            if (Collision.CanHit(player.position, player.width, player.height, player.position + Vector2.Lerp(velocities[0], velocities[1], lerpValue), player.width, player.height))
                            {
                                player.position += Vector2.Lerp(velocities[0], velocities[1], lerpValue);
                            }
                            //(LineIntersection(endPoints[0], endPoints[1], player.Center, modPlayer.collisionRaycastBottom) - modPlayer.collisionPointBottom);
                        }
                        modPlayer.collisionPointTop = LineIntersection(endPoints[0], endPoints[1], player.Center, modPlayer.collisionRaycastTop);
                        /*if (collisionStyles[0] > 1)
                        {
                            modPlayer.canDropThrough = true;
                        }*/
                        currentlyColliding = true;
                    }
                }
                if (collisionStyles[2] > 0)
                {
                    if (LineIntersection(endPoints[0], endPoints[1], player.Center, modPlayer.collisionRaycastLeft) != Vector2.Zero)
                    {


                        modPlayer.collisionLeft = true;
                        if (modPlayer.collisionLeft)
                        {
                            Vector2 currentPoint = LineIntersection(endPoints[0], endPoints[1], player.Center, modPlayer.collisionRaycastLeft);
                            float lerpValue = Vector2.Distance(currentPoint, endPoints[0]) / Vector2.Distance(endPoints[0], endPoints[1]);

                            if (Collision.CanHit(player.position, player.width, player.height, player.position + Vector2.Lerp(velocities[0], velocities[1], lerpValue), player.width, player.height))
                            {
                                player.position += Vector2.Lerp(velocities[0], velocities[1], lerpValue);
                            } //(LineIntersection(endPoints[0], endPoints[1], player.Center, modPlayer.collisionRaycastBottom) - modPlayer.collisionPointBottom);
                        }
                        modPlayer.collisionPointLeft = LineIntersection(endPoints[0], endPoints[1], player.Center, modPlayer.collisionRaycastLeft);
                        /*if (collisionStyles[0] > 1)
                        {
                            modPlayer.canDropThrough = true;
                        }*/
                        currentlyColliding = true;
                    }
                }
                if (collisionStyles[3] > 0)
                {
                    if (LineIntersection(endPoints[0], endPoints[1], player.Center, modPlayer.collisionRaycastRight) != Vector2.Zero)
                    {

                        modPlayer.collisionRight = true;
                        if (modPlayer.collisionRight)
                        {
                            Vector2 currentPoint = LineIntersection(endPoints[0], endPoints[1], player.Center, modPlayer.collisionRaycastRight);
                            float lerpValue = Vector2.Distance(currentPoint, endPoints[0]) / Vector2.Distance(endPoints[0], endPoints[1]);

                            if (Collision.CanHit(player.position, player.width, player.height, player.position + Vector2.Lerp(velocities[0], velocities[1], lerpValue), player.width, player.height))
                            {
                                player.position += Vector2.Lerp(velocities[0], velocities[1], lerpValue);
                            } //(LineIntersection(endPoints[0], endPoints[1], player.Center, modPlayer.collisionRaycastBottom) - modPlayer.collisionPointBottom);
                        }

                        modPlayer.collisionPointRight = LineIntersection(endPoints[0], endPoints[1], player.Center, modPlayer.collisionRaycastRight);
                        /*if (collisionStyles[0] > 1)
                        {
                            modPlayer.canDropThrough = true;
                        }*/
                        currentlyColliding = true;
                    }
                }


            }
        }

        /// <summary>
        /// Calls the second part of collision surface updates, which handles defining the relative velocities of endpoints. Call this in a post update method like NPC/Projectile.PostAI();
        /// /// To make the collision surface move, update it's endpoint positions AFTER Update() is called but BEFORE this method is called
        /// </summary>
        public void PostUpdate()//second updates, think postupdate
        {
            velocities[0] = endPoints[0] - oldEndPoints[0];
            velocities[1] = endPoints[1] - oldEndPoints[1];
        }


    }
}
