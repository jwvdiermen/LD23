/*
 *  TinySpace by Crealuz - TinySpace.Game
 * 
 *  @original authors: jwvdiermen
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TinySpace.Scene
{
	/// <summary>
	/// This interface represents a camera.
	/// </summary>
	public interface ICamera : IMovable
	{
		#region Properties

		/// <summary>
		/// Gets the render target.
		/// </summary>
		RenderTarget2D RenderTarget
		{
			get;
		}

		/// <summary>
		/// Gets the viewport.
		/// </summary>
		Viewport Viewport
		{
			get;
		}

		/// <summary>
		/// Gets the size of the screen, adjusted by the width to height ratio:
		/// ratio = width / height, screen = ratio * 100, 100. 
		/// </summary>
		Vector2 ScreenSize
		{
			get;
		}

		/// <summary>
		/// Gets the transformation matrix that represents the camera.
		/// </summary>
		Matrix Transformation
		{
			get;
		}

		#endregion

		#region Methods

		/// <summary>
		/// This method gets the camera matrices.
		/// </summary>
		/// <param name="projectionMatrix">The projection matrix.</param>
		/// <param name="viewMatrix">The view matrix.</param>
		void GetMatrices(out Matrix projectionMatrix, out Matrix viewMatrix);

		/// <summary>
		/// This method applies the camera's render target and viewport to the graphics device.
		/// </summary>
		/// <param name="graphics">The graphics device.</param>
		void Apply(GraphicsDevice graphics);

		#endregion
	}
}
