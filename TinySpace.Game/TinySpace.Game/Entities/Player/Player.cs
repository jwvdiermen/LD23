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
using Microsoft.Xna.Framework.Graphics;

using TinySpace.Entity;
using TinySpace.Sprite;
using TinySpace.Scene;
using TinySpace.Core;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Common;
using FarseerPhysics.Common.PolygonManipulation;
using FarseerPhysics.Common.Decomposition;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework.Audio;

namespace TinySpace.Game.Entities
{
	/// <summary>
	/// This class represents the player.
	/// </summary>
	public class Player : PhysicalEntityBase
	{
		#region Constructors

		public Player(StaticSprite sprite, string name = "Player")
			: base(name)
		{
			m_sprite = sprite;
			m_maxSpeedSqr = m_maxSpeed * m_maxSpeed;
		}

		#endregion

		#region Fields

		private World m_physicsWorld;

		private StaticSprite m_sprite;
		private Texture2D m_missleTexture;

		private Vector2 m_movement = Vector2.Zero;

		private float m_maxSpeed = 50.0f;
		private float m_maxSpeedSqr;

		private Vector2 m_force = Vector2.Zero;
		private float m_torque = 0.0f;

		private SoundEffect m_soundEngineStart;
		private SoundEffectInstance m_soundEngineStartInstance;

		private SoundEffect m_soundEngineConstant;
		private SoundEffectInstance m_soundEngineConstantInstance;

		private SoundEffect m_soundEngineStop;
		private SoundEffectInstance m_soundEngineStopInstance;

		private SoundEffect m_soundMissleLaunch;

		private bool m_engineStarted = false;
		private bool m_engineStopped = false;

		#endregion

		#region Methods

		public override void LoadContent(IServiceProvider services)
		{
			var content = (ContentManager)services.GetService(typeof(ContentManager));

			// Load the textures.
			m_missleTexture = content.Load<Texture2D>(@"Sprites\Missle");

			// Load the sounds.
			m_soundEngineStart = content.Load<SoundEffect>(@"Sounds\EngineStart");
			m_soundEngineStartInstance = m_soundEngineStart.CreateInstance();
			m_soundEngineStartInstance.Volume = 0.6f;

			m_soundEngineConstant = content.Load<SoundEffect>(@"Sounds\EngineConstant");
			m_soundEngineConstantInstance = m_soundEngineConstant.CreateInstance();
			m_soundEngineConstantInstance.IsLooped = true;
			m_soundEngineConstantInstance.Volume = 0.6f;

			m_soundEngineStop = content.Load<SoundEffect>(@"Sounds\EngineStop");
			m_soundEngineStopInstance = m_soundEngineStop.CreateInstance();
			m_soundEngineStopInstance.Volume = 0.6f;

			m_soundMissleLaunch = content.Load<SoundEffect>(@"Sounds\RocketShoot");

			// Create the player.
			this.SceneNode = this.World.Scene.Root.CreateChild(this.Name + "_Node");
			this.SceneNode.Depth = 0.5f; // Put the player in the middle.
			this.SceneNode.Attach(m_sprite);			
		}

		public override void UnloadContent()
		{
			this.SceneNode.Dispose();
			this.SceneNode = null;

			m_missleTexture.Dispose();

			m_soundEngineStartInstance.Dispose();
			m_soundEngineStart.Dispose();

			m_soundEngineConstantInstance.Dispose();
			m_soundEngineConstant.Dispose();

			m_soundEngineStopInstance.Dispose();
			m_soundEngineStop.Dispose();

			m_soundMissleLaunch.Dispose();
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
			
			// Create our shape based on our sprite.
			var texture = m_sprite.Texture;
			var data = new uint[texture.Width * texture.Height];

			texture.GetData(data);

			var textureVertices = PolygonTools.CreatePolygon(data, texture.Width, false);
		
			var centroid = -textureVertices.GetCentroid();
			textureVertices.Translate(ref centroid);

			var origin = -centroid;

			textureVertices = SimplifyTools.ReduceByDistance(textureVertices, 4.0f);

			var verticesList = BayazitDecomposer.ConvexPartition(textureVertices);

			var scale = new Vector2(m_sprite.Size.X / texture.Width);
			foreach (var vertices in verticesList)
			{
				vertices.Scale(ref scale);
			}

			// Create the body.
			this.Body = BodyFactory.CreateCompoundPolygon(m_physicsWorld, verticesList, 1.0f, BodyType.Dynamic);
			this.Body.BodyType = BodyType.Dynamic;
		}

		public override void UpdatePhysics(GameTime gameTime, ICamera camera)
		{
			base.UpdatePhysics(gameTime, camera);

			// Play sounds when force is applied.
			if (m_force.LengthSquared() > 0.0f)
			{
				if (m_engineStarted == false &&
					m_soundEngineStartInstance.State != SoundState.Playing &&
					m_soundEngineConstantInstance.State != SoundState.Playing)
				{
					m_soundEngineStartInstance.Play();
					m_engineStarted = true;
					m_engineStopped = false;
				}
				else if (m_engineStarted == true &&
					m_soundEngineStartInstance.State == SoundState.Stopped &&
					m_soundEngineConstantInstance.State != SoundState.Playing)
				{
					m_soundEngineConstantInstance.Play();
				}
			}
			else
			{
				if (m_engineStarted == true &&
					m_soundEngineConstantInstance.State == SoundState.Playing)
				{
					m_soundEngineConstantInstance.Stop(false);
					m_engineStarted = false;
					m_engineStopped = true;
				}
				else if (m_engineStarted == true && m_engineStopped == false &&
				   m_soundEngineStartInstance.State == SoundState.Stopped &&
				   m_soundEngineConstantInstance.State != SoundState.Playing)
				{
					m_soundEngineStopInstance.Play();
					m_engineStarted = false;
					m_engineStopped = false;
				}
				else if (m_engineStopped == true &&
					m_soundEngineConstantInstance.State != SoundState.Playing &&
					m_soundEngineStopInstance.State != SoundState.Playing)
				{
					m_soundEngineStopInstance.Play();
					m_engineStopped = false;
				}
			}

			// Apply the force and torque.
			this.Body.ApplyForce(ref m_force);
			m_force = Vector2.Zero;

			this.Body.ApplyTorque(m_torque);
			m_torque = 0.0f;
		}

		#endregion

		#region Controller methods

		/// <summary>
		/// Adds thurst in the given direction.
		/// </summary>
		/// <param name="dir">The direction.</param>
		public void Thrust(Vector2 dir)
		{
			// Rotate the direction.
			var rotatedForce = new Vector2(
				dir.X * MathCore.Cos(this.SceneNode.Rotation) + -dir.Y * MathCore.Sin(this.SceneNode.Rotation),
				dir.Y * MathCore.Cos(this.SceneNode.Rotation) + dir.X * MathCore.Sin(this.SceneNode.Rotation));

			// Add the thrust as acceleration.
			var velocity = this.Body.LinearVelocity;

			if (velocity != Vector2.Zero)
			{
				var currentSpeed = velocity.Length();
				var maxSpeed = 50.0f;

				var velocityDir = Vector2.Normalize(velocity);
				var forceDir = Vector2.Normalize(rotatedForce);

				float dotProduct;
				Vector2.Dot(ref velocityDir, ref forceDir, out dotProduct);

				var damping = 1.0f - (1.0f / maxSpeed * currentSpeed) * dotProduct;

				m_force += rotatedForce * damping;
			}
			else
			{
				m_force += rotatedForce;
			}
		}

		/// <summary>
		/// Adds rotation.
		/// </summary>
		/// <param name="acc">The angular acceleration.</param>
		public void Rotate(float acc)
		{
			var velocity = this.Body.AngularVelocity;

			if ((velocity < 0.0f && acc < 0.0f) ||
				(velocity > 0.0f && acc > 0.0f))
			{
				var currentSpeed = Math.Abs(velocity);
				var maxSpeed = MathHelper.PiOver2 * 1.5f;

				var damping = 1.0f - (1.0f / maxSpeed * currentSpeed);
				m_torque += acc * damping;
			}
			else
			{
				m_torque += acc;
			}
		}

		/// <summary>
		/// Fires a missle.
		/// </summary>
		public void FireMissle()
		{
			// Spawn a missle entity.
			var sprite = new StaticSprite(Vector2.One * 2.0f, m_missleTexture, "MISC_Missle", new Rectangle(4, 2, 24, 28));
			var missle = new Missle(sprite);

			var direction = -Vector2.UnitY;
			direction = new Vector2(
				direction.X * MathCore.Cos(this.SceneNode.Rotation) + -direction.Y * MathCore.Sin(this.SceneNode.Rotation),
				direction.Y * MathCore.Cos(this.SceneNode.Rotation) + direction.X * MathCore.Sin(this.SceneNode.Rotation));

			this.World.Add(missle);

			missle.Position = this.SceneNode.Position + (direction * 8.0f);
			missle.Rotation = this.SceneNode.Rotation;
			missle.Body.LinearVelocity = this.Body.LinearVelocity + (direction * 50.0f);

			// Play the sound.
			m_soundMissleLaunch.Play();
		}

		/// <summary>
		/// Aims the cannon at the target.
		/// </summary>
		/// <param name="target">The target, relative from the player</param>
		public void AimAt(Vector2 target)
		{
		}

		#endregion
	}
}
