/*
 *  TinySpace by Crealuz - TinySpace.Game
 * 
 *  @original authors: jwvdiermen
 */

using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using TinySpace.State;

namespace TinySpace.Game
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class XnaGame : Microsoft.Xna.Framework.Game
	{
		#region Constructors

		public XnaGame()
		{
			m_graphics = new GraphicsDeviceManager(this);
			m_graphics.PreferredBackBufferWidth = 1280;
			m_graphics.PreferredBackBufferHeight = 720;
			m_graphics.PreferMultiSampling = true;

			this.Window.Title = "Ludum Dare #23: TINY WORLD";
			this.IsMouseVisible = true;

			this.IsFixedTimeStep = true;
			this.TargetElapsedTime = TimeSpan.FromSeconds(1.0 / 60.0);

			this.Content.RootDirectory = "Content";
			this.Services.AddService(typeof(ContentManager), this.Content);
		}

		#endregion

		#region Fields

		private GraphicsDeviceManager m_graphics;
		private IStateManager m_stateManager;

		#endregion

		#region Methods

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			m_stateManager = new DefaultStateManager(this);
			m_stateManager.ActivateScreen(new Screens.StarSystemScreen(this));

			this.Components.Add(m_stateManager);

			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// all content.
		/// </summary>
		protected override void UnloadContent()
		{
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			// Allows the game to exit
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape) == true)
			{
				this.Exit();
			}
			
			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			base.Draw(gameTime);
		}

		#endregion
	}
}
