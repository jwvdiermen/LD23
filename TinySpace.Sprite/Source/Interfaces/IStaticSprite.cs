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
	/// This interface represents a static sprite.
	/// </summary>
	public interface IStaticSprite : ISprite
	{
		#region Properties

		/// <summary>
		/// Gets the texture that contains the sprite.
		/// </summary>
		Texture2D Texture
		{
			get;
		}

		/// <summary>
		/// Gets the area in the texture that represents the sprite.
		/// </summary>
		Rectangle Source
		{
			get;
		}

		#endregion
	}
}
