/*
 *  TinySpace by Crealuz - TinySpace.State
 * 
 *  @original authors: jwvdiermen
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TinySpace.State
{
	internal class ScreenContext
	{
		#region Constructors

		public ScreenContext(IScreen screen, IEnumerable<IScreen> underlayingScreens)
		{
			this.Screen = screen;
			this.UnderlayingScreens = underlayingScreens;
		}

		#endregion

		#region Properties

		public IScreen Screen
		{
			get;
			private set;
		}

		public IEnumerable<IScreen> UnderlayingScreens
		{
			get;
			private set;
		}

		#endregion
	}
}
