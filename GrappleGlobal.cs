using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria.Audio;
using static Humanizer.In;

namespace CollisionLib
{
    public class GrappleGlobal : GlobalProjectile
    {
        public bool GrappledToCollsionSurface = false;
        public Vector2 CollisionSurfaceGrapplePoint = Vector2.Zero;
		public float CollisionSurfaceGrappleDistance = -1;
		public float CollisionSufraceGrappleAngle = 0;
		public int CollisionSufraceGrappleSide = 0;

		public Ref<CollisionSurface> GrappledSurface = null;

		public override bool InstancePerEntity => true;

        public override bool PreAI(Projectile projectile)
        {

			if (projectile.aiStyle == 7)
			{
				AI_007_GrapplingHooks_Altered(projectile);

				return false;

			}

            return base.PreAI(projectile);


        }

		public void AI_007_GrapplingHooks_Altered(Projectile proj)
		{
			if (Main.player[proj.owner].dead || Main.player[proj.owner].stoned || Main.player[proj.owner].webbed || Main.player[proj.owner].frozen)
			{
				proj.Kill();
				return;
			}
			Vector2 mountedCenter = Main.player[proj.owner].MountedCenter;
			Vector2 vector = new Vector2(proj.position.X + (float)proj.width * 0.5f, proj.position.Y + (float)proj.height * 0.5f);
			float num = mountedCenter.X - vector.X;
			float num2 = mountedCenter.Y - vector.Y;
			float num3 = (float)Math.Sqrt(num * num + num2 * num2);
			proj.rotation = (float)Math.Atan2(num2, num) - 1.57f;
			if (proj.ai[0] == 2f && proj.type == 865)
			{
				float num4 = (float)Math.PI / 2f;
				int num5 = (int)Math.Round(proj.rotation / num4);
				proj.rotation = (float)num5 * num4;
			}
			if (Main.myPlayer == proj.owner)
			{
				int num6 = (int)(proj.Center.X / 16f);
				int num7 = (int)(proj.Center.Y / 16f);
				if (num6 > 0 && num7 > 0 && num6 < Main.maxTilesX && num7 < Main.maxTilesY && Main.tile[num6, num7].HasUnactuatedTile && Main.tile[num6, num7].TileType >= 481 && Main.tile[num6, num7].TileType <= 483 && Main.rand.Next(16) == 0)
				{
					WorldGen.KillTile(num6, num7);
					if (Main.netMode != 0)
					{
						NetMessage.SendData(17, -1, -1, null, 0, num6, num7);
					}
				}
			}
			if (num3 > 2500f)
			{
				proj.Kill();
			}
			if (proj.type == 256)
			{
				proj.rotation = (float)Math.Atan2(num2, num) + 3.92500019f;
			}
			if (proj.type == 446)
			{
				Lighting.AddLight(mountedCenter, 0f, 0.4f, 0.3f);
				proj.localAI[0] += 1f;
				if (proj.localAI[0] >= 28f)
				{
					proj.localAI[0] = 0f;
				}
				DelegateMethods.v3_1 = new Vector3(0f, 0.4f, 0.3f);
				Utils.PlotTileLine(proj.Center, mountedCenter, 8f, DelegateMethods.CastLightOpen);
			}
			if (proj.type == 652 && ++proj.frameCounter >= 7)
			{
				proj.frameCounter = 0;
				if (++proj.frame >= Main.projFrames[proj.type])
				{
					proj.frame = 0;
				}
			}
			if (proj.type >= 646 && proj.type <= 649)
			{
				Vector3 vector2 = Vector3.Zero;
				switch (proj.type)
				{
					case 646:
						vector2 = new Vector3(0.7f, 0.5f, 0.1f);
						break;
					case 647:
						vector2 = new Vector3(0f, 0.6f, 0.7f);
						break;
					case 648:
						vector2 = new Vector3(0.6f, 0.2f, 0.6f);
						break;
					case 649:
						vector2 = new Vector3(0.6f, 0.6f, 0.9f);
						break;
				}
				Lighting.AddLight(mountedCenter, vector2);
				Lighting.AddLight(proj.Center, vector2);
				DelegateMethods.v3_1 = vector2;
				Utils.PlotTileLine(proj.Center, mountedCenter, 8f, DelegateMethods.CastLightOpen);
			}
			if (proj.ai[0] == 0f)
			{
				if ((num3 > 300f && proj.type == 13) || (num3 > 400f && proj.type == 32) || (num3 > 440f && proj.type == 73) || (num3 > 440f && proj.type == 74) || (num3 > 300f && proj.type == 165) || (num3 > 350f && proj.type == 256) || (num3 > 500f && proj.type == 315) || (num3 > 550f && proj.type == 322) || (num3 > 400f && proj.type == 331) || (num3 > 550f && proj.type == 332) || (num3 > 400f && proj.type == 372) || (num3 > 300f && proj.type == 396) || (num3 > 550f && proj.type >= 646 && proj.type <= 649) || (num3 > 600f && proj.type == 652) || (num3 > 300f && proj.type == 865) || (num3 > 500f && proj.type == 935) || (num3 > 480f && proj.type >= 486 && proj.type <= 489) || (num3 > 500f && proj.type == 446))
				{
					proj.ai[0] = 1f;
				}
				else if (proj.type >= 230 && proj.type <= 235)
				{
					int num8 = 300 + (proj.type - 230) * 30;
					if (num3 > (float)num8)
					{
						proj.ai[0] = 1f;
					}
				}
				else if (proj.type == 753)
				{
					int num9 = 420;
					if (num3 > (float)num9)
					{
						proj.ai[0] = 1f;
					}

				}
                else if (ProjectileLoader.GrappleOutOfRange(num3, proj))
                {
                    proj.ai[0] = 1f;
                }
                Vector2 value = proj.Center - new Vector2(5f);
				Vector2 value2 = proj.Center + new Vector2(5f);
				Point point = (value - new Vector2(16f)).ToTileCoordinates();
				Point point2 = (value2 + new Vector2(32f)).ToTileCoordinates();
				int num10 = point.X;
				int num11 = point2.X;
				int num12 = point.Y;
				int num13 = point2.Y;
				if (num10 < 0)
				{
					num10 = 0;
				}
				if (num11 > Main.maxTilesX)
				{
					num11 = Main.maxTilesX;
				}
				if (num12 < 0)
				{
					num12 = 0;
				}
				if (num13 > Main.maxTilesY)
				{
					num13 = Main.maxTilesY;
				}
				Player player = Main.player[proj.owner];
				List<Point> list = new List<Point>();
				for (int i = 0; i < player.grapCount; i++)
				{
					Projectile projectile = Main.projectile[player.grappling[i]];
					if (projectile.aiStyle != 7 || projectile.ai[0] != 2f)
					{
						continue;
					}
					Point pt = projectile.Center.ToTileCoordinates();
					Tile tileSafely = Framing.GetTileSafely(pt);
					if (tileSafely.TileType != 314 && !TileID.Sets.Platforms[tileSafely.TileType])
					{
						continue;
					}
					for (int j = -2; j <= 2; j++)
					{
						for (int k = -2; k <= 2; k++)
						{
							Point point3 = new Point(pt.X + j, pt.Y + k);
							Tile tileSafely2 = Framing.GetTileSafely(point3);
							if (tileSafely2.TileType == 314 || TileID.Sets.Platforms[tileSafely2.TileType])
							{
								list.Add(point3);
							}
						}
					}
				}
				Vector2 vector3 = default(Vector2);
				for (int l = num10; l < num11; l++)
				{
					for (int m = num12; m < num13; m++)
					{
						
						vector3.X = l * 16;
						vector3.Y = m * 16;
						if (!(value.X + 10f > vector3.X) || !(value.X < vector3.X + 16f) || !(value.Y + 10f > vector3.Y) || !(value.Y < vector3.Y + 16f))
						{
							continue;
						}
						Tile tile = Main.tile[l, m];
						if (!tile.HasUnactuatedTile || !AI_007_GrapplingHooks_CanTileBeLatchedOnTo(tile, proj) || list.Contains(new Point(l, m)) || (proj.type == 403 && tile.TileType != 314) || Main.player[proj.owner].IsBlacklistedForGrappling(new Point(l, m)))
						{
							continue;
						}
						if (Main.player[proj.owner].grapCount < 10)
						{
							Main.player[proj.owner].grappling[Main.player[proj.owner].grapCount] = proj.whoAmI;
							Main.player[proj.owner].grapCount++;
						}
						if (Main.myPlayer != proj.owner)
						{
							continue;
						}
						int num14 = 0;
						int num15 = -1;
						int num16 = 100000;
						if (proj.type == 73 || proj.type == 74)
						{
							for (int n = 0; n < 1000; n++)
							{
								if (n != proj.whoAmI && Main.projectile[n].active && Main.projectile[n].owner == proj.owner && Main.projectile[n].aiStyle == 7 && Main.projectile[n].ai[0] == 2f)
								{
									Main.projectile[n].Kill();
								}
							}
						}
						else
						{
							int num17 = 3;
							if (proj.type == 165)
							{
								num17 = 8;
							}
							if (proj.type == 256)
							{
								num17 = 2;
							}
							if (proj.type == 372)
							{
								num17 = 2;
							}
							if (proj.type == 652)
							{
								num17 = 1;
							}
							if (proj.type >= 646 && proj.type <= 649)
							{
								num17 = 4;
							}
                            ProjectileLoader.NumGrappleHooks(proj, Main.player[proj.owner], ref num17);
                            for (int num18 = 0; num18 < 1000; num18++)
							{
								if (Main.projectile[num18].active && Main.projectile[num18].owner == proj.owner && Main.projectile[num18].aiStyle == 7)
								{
									if (Main.projectile[num18].timeLeft < num16)
									{
										num15 = num18;
										num16 = Main.projectile[num18].timeLeft;
									}
									num14++;
								}
							}
							if (num14 > num17)
							{
								Main.projectile[num15].Kill();
							}
						}
						WorldGen.KillTile(l, m, fail: true, effectOnly: true);
						SoundEngine.PlaySound(SoundID.Dig, new Vector2( l * 16, m * 16));
						proj.velocity.X = 0f;
						proj.velocity.Y = 0f;
						proj.ai[0] = 2f;
						proj.position.X = l * 16 + 8 - proj.width / 2;
						proj.position.Y = m * 16 + 8 - proj.height / 2;
						Rectangle? tileVisualHitbox = WorldGen.GetTileVisualHitbox(l, m);
						if (tileVisualHitbox.HasValue)
						{
							proj.Center = tileVisualHitbox.Value.Center.ToVector2();
						}
						proj.damage = 0;
						proj.netUpdate = true;
						if (Main.myPlayer == proj.owner)
						{
							if (proj.type == 935)
							{
								Main.player[proj.owner].DoQueenSlimeHookTeleport(proj.Center);
							}
							NetMessage.SendData(13, -1, -1, null, proj.owner);
						}
						break;
					}
					if (proj.ai[0] == 2f)
					{
						break;
					}
				}
			}
			else if (proj.ai[0] == 1f)
			{
				float num19 = 11f;
				if (proj.type == 32)
				{
					num19 = 15f;
				}
				if (proj.type == 73 || proj.type == 74)
				{
					num19 = 17f;
				}
				if (proj.type == 315)
				{
					num19 = 20f;
				}
				if (proj.type == 322)
				{
					num19 = 22f;
				}
				if (proj.type >= 230 && proj.type <= 235)
				{
					num19 = 11f + (float)(proj.type - 230) * 0.75f;
				}
				if (proj.type == 753)
				{
					num19 = 15f;
				}
				if (proj.type == 446)
				{
					num19 = 20f;
				}
				if (proj.type >= 486 && proj.type <= 489)
				{
					num19 = 18f;
				}
				if (proj.type >= 646 && proj.type <= 649)
				{
					num19 = 24f;
				}
				if (proj.type == 652)
				{
					num19 = 24f;
				}
				if (proj.type == 332)
				{
					num19 = 17f;
				}
                ProjectileLoader.GrappleRetreatSpeed(proj, Main.player[proj.owner], ref num19);
                
                if (num3 < 24f)
				{
					proj.Kill();
				}
				num3 = num19 / num3;
				num *= num3;
				num2 *= num3;
				proj.velocity.X = num;
				proj.velocity.Y = num2;
			}
			else if (proj.ai[0] == 2f)
			{
				Point point4 = proj.Center.ToTileCoordinates();
				/*if (Main.tile[point4.X, point4.Y] == null)
				{
					Main.tile[point4.X, point4.Y] = new Tile();
				}*/
				bool flag = true;

                if (GrappledToCollsionSurface&& proj.timeLeft < 36000 - 3)
                {
					//proj.position;

					flag = false;
                }

				if (Main.tile[point4.X, point4.Y].HasUnactuatedTile && AI_007_GrapplingHooks_CanTileBeLatchedOnTo(Main.tile[point4.X, point4.Y], proj))
				{
					flag = false;
				}
				if (flag)
				{
					proj.ai[0] = 1f;
				}
				else if (Main.player[proj.owner].grapCount < 10)
				{
					/*Terraria.Player player = Terraria.Main.player[proj.owner];
					int numHooks = 3;
					//time to replicate retarded vanilla hardcoding, wheee
					if (proj.type == 165) numHooks = 8;
					if (proj.type == 256) numHooks = 2;
					if (proj.type == 372) numHooks = 2;
					if (proj.type == 652) numHooks = 1;
					if (proj.type >= 646 && proj.type <= 649) numHooks = 4;
					//end vanilla zoink

					ProjectileLoader.NumGrappleHooks(proj, player, ref numHooks);
					if (player.grapCount > numHooks) { Terraria.Main.projectile[player.grappling.OrderBy(n => (Terraria.Main.projectile[n].active ? 0 : 999999) + Terraria.Main.projectile[n].timeLeft).ToArray()[0]].Kill(); }*/

					Main.player[proj.owner].grappling[Main.player[proj.owner].grapCount] = proj.whoAmI;
					Main.player[proj.owner].grapCount++;


				}
			}
		}

		public bool AI_007_GrapplingHooks_CanTileBeLatchedOnTo(Tile theTile, Projectile proj)
		{
			return Main.tileSolid[theTile.TileType] | (theTile.TileType == 314) | (proj.type == 865 && TileID.Sets.IsATreeTrunk[theTile.TileType]) | (proj.type == 865 && theTile.TileType == 323);
		}


	}
}
