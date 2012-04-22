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
using Microsoft.Xna.Framework.Content;

namespace TinySpace.Game
{
	/// <summary>
	/// A debugging utility.
	/// </summary>
	public class Debug
	{
		#region Constructors

		/// <summary>
		/// Creates a new instance of the Debug class.
		/// </summary>
		/// <param name="game">The Game instance.</param>
		public Debug(XnaGame game)
		{
			m_content = game.Content;
			m_graphics = game.GraphicsDevice;
		}

		#endregion

		#region Fields
		
		private ContentManager m_content;
		private GraphicsDevice m_graphics;

		private SpriteFont m_debugFont;
		private SpriteBatch m_spriteBatch;

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the debug text.
		/// </summary>
		public static string Text
		{
			get;
			set;
		}

		#endregion

		#region Methods

		public void LoadContent()
		{
			m_spriteBatch = new SpriteBatch(m_graphics);
			m_debugFont = m_content.Load<SpriteFont>(@"Fonts\Debug");
		}

		public void UnloadContent()
		{
			m_spriteBatch.Dispose();
			m_spriteBatch = null;
		}

		public void Draw()
		{
			if (String.IsNullOrEmpty(Debug.Text) == false)
			{
				m_spriteBatch.Begin();
				m_spriteBatch.DrawString(m_debugFont, Debug.Text, Vector2.One * 10.0f, Color.White);
				m_spriteBatch.End();
			}
		}

		#endregion
	}
}
