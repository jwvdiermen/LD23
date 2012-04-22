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

using TinySpace.Entity;
using Microsoft.Xna.Framework.Input;
using TinySpace.Scene;
using TinySpace.Core;

namespace TinySpace.Game.Controllers
{
	/// <summary>
	/// Controls the movement of the player.
	/// </summary>
	public class PlayerMovement : EntityControllerBase
	{
		#region Constructors

		/// <summary>
		/// Public camera.
		/// </summary>
		/// <param name="camera">The camera.</param>
		public PlayerMovement(ICamera camera)
		{
			// TODO: get active camera from the scene.
			m_camera = camera;
		}

		#endregion

		#region Fields

		private ICamera m_camera;

		private List<Keys> m_pressedKeys = new List<Keys>();
		private ButtonState m_leftButton = ButtonState.Released;
		private ButtonState m_rightButton = ButtonState.Released;

		#endregion

		#region Methods

		public override void Update(GameTime time, ICamera camera)
		{
			var player = this.Entity as Game.Entities.Player;
			if (player != null)
			{
				var kbs = Keyboard.GetState();

				// Check if we need to thrust forward.
				if (kbs.IsKeyDown(Keys.W) || kbs.IsKeyDown(Keys.Up))
				{
					player.Thrust(Vector2.UnitY * -2500.0f);
				}
				if (kbs.IsKeyDown(Keys.S) || kbs.IsKeyDown(Keys.Down))
				{
					player.Thrust(Vector2.UnitY * 2500.0f);
				}

				// Check if we need to thrust sidewards.
				if (kbs.IsKeyDown(Keys.E))
				{
					player.Thrust(Vector2.UnitX * 2000.0f);
				}
				if (kbs.IsKeyDown(Keys.Q))
				{
					player.Thrust(Vector2.UnitX * -2000.0f);
				}

				// Check if we need to rotate.
				if (kbs.IsKeyDown(Keys.D) || kbs.IsKeyDown(Keys.Right))
				{
					player.Rotate(2500.0f);
				}
				if (kbs.IsKeyDown(Keys.A) || kbs.IsKeyDown(Keys.Left))
				{
					player.Rotate(-2500.0f);
				}

				// Get the mouse position on the screen.
				var mouseState = Mouse.GetState();

				// Check if we need to fire a missle.
				if (mouseState.LeftButton == ButtonState.Pressed && m_leftButton == ButtonState.Released)
				{
					player.FireMissle();
					m_leftButton = ButtonState.Pressed;
				}
				else if (mouseState.LeftButton == ButtonState.Released)
				{
					m_leftButton = ButtonState.Released;
				}

				// Translate the mouse position to the scene.
				var mousePosition3D = new Vector3(mouseState.X, mouseState.Y, 0.0f);

				Matrix projectionMatrix, viewMatrix;
				m_camera.GetMatrices(out projectionMatrix, out viewMatrix);

				mousePosition3D = m_camera.Viewport.Unproject(mousePosition3D, projectionMatrix, viewMatrix, Matrix.Identity);
				var mousePosition = new Vector2(mousePosition3D.X, mousePosition3D.Y) - player.SceneNode.Position;

				player.AimAt(mousePosition);

				//// Determine the angle between the player and the mouse.
				//var angle = MathHelper.ToDegrees((float)Math.Atan2(mousePosition.X, -mousePosition.Y));
				//var angleDiff = MathCore.WrapAngle(angle - player.SceneNode.Rotation);

				//player.Rotate(angleDiff * 100.0f);
			}
		}

		#endregion
	}
}
