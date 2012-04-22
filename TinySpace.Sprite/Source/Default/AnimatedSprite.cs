/*
 *  TinySpace by Crealuz - TinySpace.Sprite
 * 
 *  @original authors: jwvdiermen
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TinySpace.Scene;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TinySpace.Sprite
{
	///// <summary>
	///// This class implements the <see cref="IAnimatedSprite" /> interface and provides default functionality.
	///// </summary>
	//public class AnimatedSprite : Movable, IAnimatedSprite
	//{
	//    #region Nested types

	//    private class Animation : IAnimation
	//    {
	//        #region Constructors

	//        public Animation(string name, float duration, IEnumerable<Frame> frames)
	//        {
	//            this.Name = name;
	//            this.Duration = duration;
	//            m_frameList.AddRange(frames);
	//        }

	//        #endregion

	//        #region Fields

	//        private List<Frame> m_frameList = new List<Frame>();

	//        #endregion

	//        #region IAnimation Members

	//        public string Name
	//        {
	//            get;
	//            private set;
	//        }

	//        public IEnumerable<IFrame> Frames
	//        {
	//            get { return m_frameList; }
	//        }

	//        public float Duration
	//        {
	//            get;
	//            private set;
	//        }

	//        public IFrame GetFrame(GameTime gameTime)
	//        {
	//            var animationTime = (float)gameTime.TotalGameTime.TotalSeconds % this.Duration;
	//            return m_frameList.First(e => animationTime >= e.Time && animationTime < e.EndTime);
	//        }

	//        #endregion
	//    }

	//    private class Frame : IFrame
	//    {
	//        #region Constructors

	//        public Frame(Texture2D texture, Point source, float time, float endTime)
	//        {
	//            this.Texture = texture;
	//            this.Source = source;
	//            this.Time = time;
	//            this.EndTime = endTime;
	//        }

	//        #endregion

	//        #region Properties

	//        public float EndTime
	//        {
	//            get;
	//            private set;
	//        }

	//        #endregion

	//        #region IFrame Members

	//        public Texture2D Texture
	//        {
	//            get;
	//            private set;
	//        }

	//        public Point Source
	//        {
	//            get;
	//            private set;
	//        }

	//        public float Time
	//        {
	//            get;
	//            private set;
	//        }

	//        #endregion
	//    }

	//    #endregion

	//    #region Constructors

	//    /// <summary>
	//    /// Public constructor.
	//    /// </summary>
	//    /// <param name="size">The size of a single frame.</param>
	//    /// <param name="name">The name.</param>
	//    public AnimatedSprite(Vector2 frameSize, string name = null)
	//        : base(name)
	//    {
	//        m_frameSize = frameSize;
	//    }

	//    #endregion

	//    #region Fields

	//    private Vector2 m_frameSize;

	//    private List<Animation> m_animationList = new List<Animation>();

	//    #endregion

	//    #region Methods

	//    /// <summary>
	//    /// This method creates an animation.
	//    /// </summary>
	//    /// <param name="name">The name of the animation.</param>
	//    /// <param name="texture">The texture which contains all the frames.</param>
	//    /// <param name="timePerFrame">The duration of a single frame in seconds.</param>
	//    /// <param name="start">The position of the first frame.</param>
	//    /// <param name="frameCount">The number of frames in the animation.</param>
	//    /// <returns>The created animation.</returns>
	//    public IAnimation CreateAnimation(string name, Texture2D texture, float timePerFrame, Point start, int frameCount)
	//    {
	//        var frames = new List<Frame>();

	//        var currentPoint = start;
	//        var currentTime = 0.0f;
	//        for (int i = 0; i < frameCount; ++i)
	//        {
	//            frames.Add(new Frame(texture, currentPoint, currentTime, currentTime + timePerFrame));

	//            currentPoint.X += (int)m_frameSize.X;
	//            if (currentPoint.X + (int)m_frameSize.X > texture.Width)
	//            {
	//                currentPoint.X = 0;
	//                currentPoint.Y += (int)m_frameSize.Y;
	//            }

	//            currentTime += timePerFrame;
	//        }

	//        var animation = new Animation(name, timePerFrame * frameCount, frames);
	//        m_animationList.Add(animation);

	//        return animation;
	//    }

	//    #endregion

	//    #region ISprite Members

	//    public SpriteEffects Effects
	//    {
	//        get;
	//        set;
	//    }

	//    #endregion

	//    #region IAnimatedSprite Members

	//    public IEnumerable<IAnimation> Animations
	//    {
	//        get { return m_animationList; }
	//    }

	//    public IAnimation ActiveAnimation
	//    {
	//        get;
	//        private set;
	//    }

	//    public IAnimation PlayAnimation(string name)
	//    {
	//        var animation = m_animationList.FirstOrDefault(e => e.Name == name);
	//        if (animation == null)
	//        {
	//            throw new KeyNotFoundException("No animation found with the name '" + name + "'.");
	//        }

	//        this.ActiveAnimation = animation;
	//        return animation;
	//    }

	//    #endregion

	//    #region IUpdatable Members

	//    public void Update(GameTime time)
	//    {			
	//    }

	//    #endregion

	//    #region IRenderable Members

	//    public void Render(GameTime gameTime, SpriteBatch spriteBatch)
	//    {
	//        if (this.SceneNode != null && this.ActiveAnimation != null)
	//        {
	//            var frame = this.ActiveAnimation.GetFrame(gameTime);

	//            spriteBatch.Draw(
	//                frame.Texture, 
	//                this.SceneNode.Position, 
	//                new Rectangle(frame.Source.X, frame.Source.Y, (int)m_frameSize.X, (int)m_frameSize.Y), 
	//                Color.White, 
	//                this.SceneNode.Rotation, 
	//                Vector2.Zero, 
	//                this.SceneNode.Scale, 
	//                this.Effects, 
	//                this.SceneNode.Depth);
	//        }
	//    }

	//    #endregion
	//}
}
