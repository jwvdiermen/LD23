/*
 *  TinySpace by Crealuz - TinySpace.State
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
	/// This class implements the <see cref="IStateManager" /> interface and provides default functionality.
	/// </summary>
	public class DefaultStateManager : DrawableGameComponent, IStateManager
	{
		#region Constructors

		public DefaultStateManager(Game game)
			: base(game)
		{
		}

		#endregion

		#region Fields

		private Stack<ScreenContext> m_screenStack = new Stack<ScreenContext>();
		private bool m_contentLoaded = false;

		#endregion

		#region Methods

		public override void Initialize()
		{
			base.Initialize();
		}

		protected override void LoadContent()
		{
			foreach (var screenContext in m_screenStack)
			{
				screenContext.Screen.LoadContent();
			}

			m_contentLoaded = true;
			base.LoadContent();
		}

		protected override void UnloadContent()
		{
			foreach (var screenContext in m_screenStack)
			{
				screenContext.Screen.UnloadContent();
			}

			m_contentLoaded = false;
			base.UnloadContent();
		}

		public override void Update(GameTime gameTime)
		{
			if (m_screenStack.Count > 0)
			{
				var activeContext = m_screenStack.Peek();

				foreach (var screen in activeContext.UnderlayingScreens)
				{
					screen.Update(gameTime, false, true);
				}

				activeContext.Screen.Update(gameTime, true, false);
			}
		}

		public override void Draw(GameTime gameTime)
		{
			if (m_screenStack.Count > 0)
			{
				var activeContext = m_screenStack.Peek();

				// TODO: handle transisions.
				foreach (var screen in activeContext.UnderlayingScreens)
				{
					screen.Draw(gameTime);
				}

				activeContext.Screen.Draw(gameTime);
			}
			else
			{
				this.GraphicsDevice.Clear(Color.Black);
			}

			base.Draw(gameTime);
		}

		#endregion

		#region IStateManager Members

		public IScreen ActiveScreen
		{
			get { return m_screenStack.Count > 0 ? m_screenStack.Peek().Screen : null; }
		}

		public void PushScreen(IScreen screen)
		{
			ScreenContext activeContext = null;
			if (m_screenStack.Count > 0)
			{
				activeContext = m_screenStack.Peek();
			}

			var underlayingScreens = new List<IScreen>();
			if (screen.IsPopup == true && activeContext != null)
			{
				underlayingScreens.AddRange(activeContext.UnderlayingScreens);
				underlayingScreens.Add(activeContext.Screen);
			}

			if (m_contentLoaded == true)
			{
				screen.LoadContent();
			}

			m_screenStack.Push(new ScreenContext(screen, underlayingScreens));
		}

		public IScreen PopScreen()
		{
			ScreenContext activeContext = null;
			if (m_screenStack.Count > 0)
			{
				activeContext = m_screenStack.Pop();

				if (m_contentLoaded == true)
				{
					activeContext.Screen.UnloadContent();
				}
			}			

			return activeContext != null ? activeContext.Screen : null;
		}

		public void ActivateScreen(IScreen screen)
		{
			m_screenStack.Clear();
			m_screenStack.Push(new ScreenContext(screen, new IScreen[0]));
		}

		#endregion
	}
}
