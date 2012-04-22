/*
 *  TinySpace by Crealuz - TinySpace.Core
 * 
 *  @original authors: jwvdiermen
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TinySpace.Core
{
	/// <summary>
	/// Used by type which can have a name.
	/// </summary>
	public interface INamable
	{
		#region Properties

		/// <summary>
		/// Gets the name.
		/// </summary>
		string Name
		{
			get;
		}

		#endregion
	}
}
