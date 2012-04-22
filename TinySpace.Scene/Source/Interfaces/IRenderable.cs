/*
 *  IRenderable.cs
 *  TinySpace by Crealuz - TinySpace.Scene
 *
 *  © copyright 2010 Crealuz All rights reserved.
 *  The computer program(s) is the proprietary information of the original authors. 
 *  and provided under the relevant License Agreement containing restrictions 
 *  on use and disclosure. Use is subject to the License Agreement.
 *
 *  No part of this package may be reproduced and/or published by print, 
 *  photoprint, microfilm, audiotape, electronically, mechanically or any other means, 
 *  or stored in an information retrieval system, without prior permission from Crealuz.
 * 
 *  @original authors: jwvdiermen
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TinySpace.Scene
{
	/// <summary>
	/// This interface represents an object which can be rendered.
	/// </summary>
	public interface IRenderable
	{
		#region Methods

		/// <summary>
		/// This method is called when graphics resources need to be loaded.
		/// </summary>
		/// <param name="services">The service provider.</param>
		void LoadContent(IServiceProvider services);

		/// <summary>
		/// This method is called when graphics resources need to be unloaded.
		/// </summary>
		void UnloadContent();

		/// <summary>
		/// This method renders the object to the given sprite batch.
		/// </summary>
		/// <param name="gameTime">The game time.</param>
		/// <param name="camera">The camera.</param>
		void Render(GameTime gameTime, ICamera camera);

		#endregion
	}
}
