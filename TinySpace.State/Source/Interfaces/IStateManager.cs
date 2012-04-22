/*
 *  TinySpace by Crealuz - TinySpace.Sprite
 * 
 *  @original authors: jwvdiermen
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace TinySpace.State
{
	/// <summary>
	/// This interface represents the state manager. The state manager is responsible for
	/// the states in a game, represented by screens (see <see cref="IScreen" />). By activating
	/// screens, a state is either entered or left based on the screen's properties. The
	/// state manager also handles transisions between screens if necessary.
	/// </summary>
	public interface IStateManager : IGameComponent
	{
		#region Properties

		/// <summary>
		/// Gets the current active screen.
		/// </summary>
		IScreen ActiveScreen
		{
			get;
		}

		#endregion

		#region Methods

		/// <summary>
		/// This method pushes a screen onto the stack and actives it.
		/// </summary>
		/// <param name="screen">The screen to push.</param>
		void PushScreen(IScreen screen);

		/// <summary>
		/// This method pops the current screen from the stack and activates the
		/// previous screen if there is one.
		/// </summary>
		/// <returns>The popped screen.</returns>
		IScreen PopScreen();

		/// <summary>
		/// This method actives the given screen, clearing the current stack.
		/// </summary>
		/// <param name="screen">The screen to activate.</param>
		void ActivateScreen(IScreen screen);

		#endregion
	}
}
