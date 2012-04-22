/*
 *  TinySpace by Crealuz - TinySpace.Sprite
 * 
 *  @original authors: jwvdiermen
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TinySpace.Sprite
{
	/// <summary>
	/// This interface represents an animated sprite.
	/// </summary>
	public interface IAnimatedSprite : ISprite
	{
		#region Properties

		/// <summary>
		/// Gets an enumerable containing the animations.
		/// </summary>
		IEnumerable<IAnimation> Animations
		{
			get;
		}

		/// <summary>
		/// Gets the active animation.
		/// </summary>
		IAnimation ActiveAnimation
		{
			get;
		}

		#endregion

		#region Methods

		/// <summary>
		/// This method plays the given animation.
		/// </summary>
		/// <param name="name">The name of the animation.</param>
		/// <returns>The animation being played.</returns>
		/// <exception cref="KeyNotFoundException">No animation was found with the given name.</exception>
		IAnimation PlayAnimation(string name);		

		#endregion
	}
}
