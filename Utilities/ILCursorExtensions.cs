using System.Linq;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace CollisionLib14.Utilities
{
	internal static class ILCursorExtensions
	{
		/// <summary>
		/// Grabs incoming labels and redirects them to an inserted no-op, allowing insertion of code between it and next operation.
		/// </summary>
		public static ILCursor HijackIncomingLabels(this ILCursor cursor)
		{
			var incomingLabels = cursor.IncomingLabels.ToArray();

			cursor.Emit(OpCodes.Nop);

			foreach (var incomingLabel in incomingLabels) {
				incomingLabel.Target = cursor.Prev;
			}

			return cursor;
		}
	}
}
