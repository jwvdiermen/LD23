/*
 *  TinySpace by Crealuz - TinySpace.Scene
 * 
 *  @original authors: jwvdiermen
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TinySpace.Core;

namespace TinySpace.Scene
{
	/// <summary>
	/// This interface represents a layer to which <see cref="IRenderable" /> objects are rendered.
	/// </summary>
	public interface ILayer : IDisposable
	{
		#region Properties

		/// <summary>
		/// Gets or sets the scene.
		/// </summary>
		IScene Scene
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the depth.
		/// </summary>
		float Depth
		{
			get;
		}

		/// <summary>
		/// Gets a read-only list containing the renderable in the layeer.
		/// </summary>
		ReadOnlyList<IRenderable> Renderables
		{
			get;
		}

		#endregion

		#region Methods

		///// <summary>
		///// This method updates the layer.
		///// </summary>
		///// <param name="time">The game time.</param>
		//void Update(GameTime time);

		#endregion
	}
}
