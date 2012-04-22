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
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

using FarseerPhysics.Common;
using FarseerPhysics.Common.PolygonManipulation;
using FarseerPhysics.Common.Decomposition;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics.Contacts;

using TinySpace.Entity;
using TinySpace.Sprite;
using TinySpace.Core;

namespace TinySpace.Game.Entities
{
	/// <summary>
	/// Represents a missle fired a player and explodes on impact.
	/// </summary>
	public class Missle : PhysicalEntityBase
	{
		#region Constructors

		public Missle(StaticSprite sprite, string name = null)
			: base(name)
		{
			m_sprite = sprite;
		}

		#endregion

		#region Fields

		private World m_physicsWorld;
		private StaticSprite m_sprite;

		private SoundEffect m_soundExplosion;

		#endregion

		#region Methods

		public override void LoadContent(IServiceProvider services)
		{
			var content = (ContentManager)services.GetService(typeof(ContentManager));

			// Load the sounds.
			m_soundExplosion = content.Load<SoundEffect>(@"Sounds\RocketExplosion");

			// Create the scene node.
			this.SceneNode = this.World.Scene.Root.CreateChild();
			this.SceneNode.Depth = 0.1f;
			this.SceneNode.Attach(m_sprite);
		}

		public override void UnloadContent()
		{
			this.SceneNode.Dispose();
			this.SceneNode = null;

			//m_soundExplosion.Dispose();
		}

		protected override void OnWorldChanged(IEntityWorld world)
		{
			DestroyPhysics();

			if (world != null)
			{
				m_physicsWorld = ((DynamicEntityWorld)world).PhysicsWorld;
				InitializePhysics();
			}
			else
			{
				m_physicsWorld = null;
			}
		}

		private void DestroyPhysics()
		{
			if (this.Body != null)
			{
				this.Body.Dispose();
				this.Body = null;
			}
		}

		private void InitializePhysics()
		{
			// Ensure we have a physics world.
			if (m_physicsWorld == null)
			{
				throw new InvalidOperationException("No physics world available.");
			}

			// Create the body.
			this.Body = BodyFactory.CreateRectangle(m_physicsWorld, m_sprite.Size.X, m_sprite.Size.Y, 1.0f);
			this.Body.BodyType = BodyType.Dynamic;

			foreach (var fixture in this.Body.FixtureList)
			{
				fixture.OnCollision += OnCollision;
			}
		}

		public override void UpdatePhysics(GameTime gameTime, Scene.ICamera camera)
		{
			// Apply force.
			var dir = Vector2.UnitY * -500.0f;

			// Rotate the direction.
			var rotatedForce = new Vector2(
				dir.X * MathCore.Cos(this.SceneNode.Rotation) + -dir.Y * MathCore.Sin(this.SceneNode.Rotation),
				dir.Y * MathCore.Cos(this.SceneNode.Rotation) + dir.X * MathCore.Sin(this.SceneNode.Rotation));

			// Add the thrust as acceleration.
			var velocity = this.Body.LinearVelocity;

			if (velocity != Vector2.Zero)
			{
				var currentSpeed = velocity.Length();
				var maxSpeed = 200.0f;

				var velocityDir = Vector2.Normalize(velocity);
				var forceDir = Vector2.Normalize(rotatedForce);

				float dotProduct;
				Vector2.Dot(ref velocityDir, ref forceDir, out dotProduct);

				var damping = 1.0f - (1.0f / maxSpeed * currentSpeed) * dotProduct;

				this.Body.ApplyForce(rotatedForce * damping);
			}
			else
			{
				this.Body.ApplyForce(rotatedForce);
			}
		}

		private bool OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
		{
			// Get the user data.
			var userData = fixtureB.UserData;//fixtureA.Body.UserData ?? fixtureB.Body.UserData;

			// Check if we hit terrain.
			var terrainRegion = userData as TerrainRegion;
			if (terrainRegion != null)
			{
				terrainRegion.DrawCircle(this.Position, 6.0f, 1);
				terrainRegion.RegenerateTerrain();
			}

			// EXPLODE!
			m_soundExplosion.Play();
			this.Destroy();

			return true;
		}

		#endregion
	}
}
