/*
 *  IUpdatable.cs
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

using Microsoft.Xna.Framework;

namespace TinySpace.Scene
{
	/// <summary>
	/// This interface represents an object which can be updated.
	/// </summary>
	public interface IUpdatable
	{
		#region Methods

		/// <summary>
		/// This method updates the object.
		/// </summary>
		/// <param name="time">The game time.</param>
		/// <param name="camera">The camera.</param>
		void Update(GameTime time, ICamera camera);

		#endregion
	}
}
