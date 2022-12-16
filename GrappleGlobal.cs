using CollisionLib14.Utilities;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;

namespace CollisionLib
{
	public sealed class GrappleGlobal : GlobalProjectile
    {
        public bool GrappledToCollsionSurface = false;
        public Vector2 CollisionSurfaceGrapplePoint = Vector2.Zero;
		public float CollisionSurfaceGrappleDistance = -1;
		public float CollisionSufraceGrappleAngle = 0;
		public int CollisionSufraceGrappleSide = 0;

		public Ref<CollisionSurface> GrappledSurface = null;

		public override bool InstancePerEntity => true;

		public override void Load()
		{
			IL.Terraria.Projectile.AI_007_GrapplingHooks += GrapplingHookAIInjection;
		}

		private static void GrapplingHookAIInjection(ILContext context)
		{
			var il = new ILCursor(context)
			{
				// Start from the bottom.
				Index = context.Instrs.Count - 1
			};

			// Match the last 'ai[0] = 1f;'.
			il.GotoPrev(
				MoveType.Before,
				i => i.MatchLdarg(0), // 'this'
				i => i.MatchLdfld(typeof(Projectile), nameof(Projectile.ai)),
				i => i.MatchLdcI4(0),
				i => i.MatchLdcR4(1f),
				i => i.MatchStelemR4()
			);

			// Match the above 'if (flag)' check.
			int flagLocalId = -1;
			
			il.GotoPrev(
				MoveType.Before,
				i => i.MatchLdloc(out flagLocalId),
				i => i.MatchBrfalse(out _)
			);

			// Ensures that our following code isn't jumped over.
			il.HijackIncomingLabels();

			// Insert code that modifies 'flag'.
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ldloca, flagLocalId);
			il.EmitDelegate(ModifyFlag);
		}

		private static void ModifyFlag(Projectile projectile, ref bool flag)
		{
			if (!projectile.TryGetGlobalProjectile(out GrappleGlobal grappleData))
			{
				return;
			}

			if (grappleData.GrappledToCollsionSurface && projectile.timeLeft < 36000 - 3)
			{
				//projectile.position;
				flag = false;
			}
		}
	}
}
