/*
 *  TinySpace by Crealuz - TinySpace.Core
 * 
 *  @original authors: jwvdiermen
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TinySpace.Core
{
	/// <summary>
	/// Helper methods with mathh.
	/// </summary>
	public class MathCore
	{
		#region Methods
		
		/// <summary>
		/// Returns the sine of the specified angle.
		/// </summary>
		/// <param name="a">An angle, measured in degrees.</param>
		/// <returns>An angle, measured in degrees.</returns>
		public static float Sin(float a)
		{
			return (float)Math.Sin((double)MathHelper.ToRadians(a));
		}
		
		/// <summary>
		/// Returns the cosine of the specified angle.
		/// </summary>
		/// <param name="d">An angle, measured in degrees.</param>
		/// <returns>An angle, measured in degrees.</returns>
		public static float Cos(float d)
		{
			return (float)Math.Cos((double)MathHelper.ToRadians(d));
		}

		/// <summary>
		/// Wraps the given angle between 180 and -180 degrees.
		/// </summary>
		/// <param name="a">The angle.</param>
		/// <returns>The wrapped angle.</returns>
		public static float WrapAngle(float a)
		{
			while (a > 180.0f)
			{
				a = -180.0f + (a - 180.0f);
			}
			while (a < -180.0f)
			{
				a = 180.0f + (a + 180.0f);
			}
			return a;
		}

		#endregion
	}
}
