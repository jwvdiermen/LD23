/*
 *  TinySpace by Crealuz - TinySpace.Sprite
 * 
 *  @original authors: jwvdiermen
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TinySpace.Sprite
{
	/// <summary>
	/// This interface represents an animation of an <see cref="IAnimatedSprite"/>.
	/// </summary>
	public interface IAnimation
	{
		#region Properties

		/// <summary>
		/// Gets the name.
		/// </summary>
		string Name
		{
			get;
		}

		/// <summary>
		/// Gets an enumerable containing the frames of the animation.
		/// </summary>
		IEnumerable<IFrame> Frames
		{
			get;
		}

		/// <summary>
		/// Gets the duration in seconds.
		/// </summary>
		float Duration
		{
			get;
		}

		#endregion

		#region Methods

		/// <summary>
		/// This method gets the frame for the given game time.
		/// </summary>
		/// <param name="gameTime">The game time.</param>
		IFrame GetFrame(GameTime gameTime);

		#endregion
	}
}
