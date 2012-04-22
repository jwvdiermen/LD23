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

using TinySpace.Scene;

namespace TinySpace.Sprite
{
	/// <summary>
	/// This interface represents a sprite.
	/// </summary>
	public interface ISprite : IDisposable, IMovable, IUpdatable, IRenderable
	{
		#region Properties
		
		/// <summary>
		/// Gets the size of the sprite.
		/// </summary>
		Vector2 Size
		{
			get;
		}

		/// <summary>
		/// Gets the pivot of the sprite.
		/// </summary>
		Vector2 Pivot
		{
			get;
		}

		/// <summary>
		/// Gets or sets the sprite effects.
		/// </summary>
		SpriteEffects Effects
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the blend state to use when rendering.
		/// </summary>
		BlendState BlendState
		{
			get;
		}

		#endregion
	}
}
