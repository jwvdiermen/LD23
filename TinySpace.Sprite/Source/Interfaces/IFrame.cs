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
	/// This interface represents a frame in an animated sprite.
	/// </summary>
	public interface IFrame
	{
		#region Properties

		/// <summary>
		/// Gets the texture that contains the frame.
		/// </summary>
		Texture2D Texture
		{
			get;
		}

		/// <summary>
		/// Gets the source in the texture where the frame is located.
		/// </summary>
		Point Source
		{
			get;
		}

		/// <summary>
		/// Gets the time when the frame is displayed.
		/// </summary>
		float Time
		{
			get;
		}

		#endregion
	}
}
