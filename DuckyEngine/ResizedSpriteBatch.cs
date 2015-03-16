using System;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Xen2D;

namespace DuckyEngine
{
	public class ResizedSpriteBatch : SpriteBatch
	{
		public ResizedSpriteBatch( GraphicsDevice device ) :
			base( device )
		{ }

		public new void Draw( Texture2D texture, Vector2 position, Color color )
		{
			this.Draw (
				texture,
				position,
				null,
				color,
				0.0f,
				Vector2.Zero,
				1.0f,
				SpriteEffects.None,
				0.0f,
				Matrix.Identity,//SpriteBatchEx.ScreenSizeTransform,
				RenderParams.Default
				);
		}

		public new void Draw( Texture2D texture, Rectangle destRectangle, Color color )
		{
			this.Draw (
				texture,
				destRectangle,
				null,
				color,
				0.0f,
				Vector2.Zero,
				SpriteEffects.None,
                0.0f,
                Matrix.Identity,//SpriteBatchEx.ScreenSizeTransform,
				RenderParams.Default
				);
		}

		public new void Draw( Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth )
		{
			this.Draw (
				texture,
				position,
				sourceRectangle,
				color,
				rotation,
				origin,
				scale,
				effects,
                layerDepth,
                Matrix.Identity,//SpriteBatchEx.ScreenSizeTransform,
				RenderParams.Default
				);
		}

		public new void Draw( Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth )
		{
			this.Draw (
				texture,
				position,
				sourceRectangle,
				color,
				rotation,
				origin,
				scale,
				effects,
                layerDepth,
                Matrix.Identity,//SpriteBatchEx.ScreenSizeTransform,
				RenderParams.Default
				);
		}

		public new void Draw( Texture2D texture, Rectangle destRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth )
		{
			this.Draw (
				texture,
				destRectangle,
				sourceRectangle,
				color,
				rotation,
				origin,
				effects,
                layerDepth,
                Matrix.Identity,//SpriteBatchEx.ScreenSizeTransform,
				RenderParams.Default
				);
		}

		public new void DrawString( SpriteFont spriteFont, string value, Vector2 position, Color color )
		{
			this.DrawString(
				spriteFont,
				value,
				position,
				color,
				0.0f,
				Vector2.Zero,
				Vector2.One,
				SpriteEffects.None,
				0.0f,
				SpriteBatchEx.ScreenSizeTransform,
				RenderParams.Default
				);
		}

		public new void DrawString( SpriteFont spriteFont, string value, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth )
		{
			this.DrawString(
				spriteFont,
				value,
				position,
				color,
				rotation,
				origin,
				new Vector2( scale, scale ),
				effects,
				layerDepth,
				SpriteBatchEx.ScreenSizeTransform,
				RenderParams.Default
				);
		}

		public new void DrawString( SpriteFont spriteFont, string value, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth )
		{
			this.DrawString(
				spriteFont,
				value,
				position,
				color,
				rotation,
				origin,
				scale,
				effects,
				layerDepth,
				SpriteBatchEx.ScreenSizeTransform,
				RenderParams.Default
				);
		}

		public Vector2 DrawInteger( SpriteFont spriteFont, int value, Vector2 position, Color color )
		{
			return this.DrawInteger( spriteFont, value, position, color, 0.0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.0f, SpriteBatchEx.ScreenSizeTransform, RenderParams.Default );
		}

		public Vector2 DrawInteger( SpriteFont spriteFont, int value, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth )
		{
			return this.DrawInteger( spriteFont, value, position, color, rotation, origin, scale, effects, layerDepth, SpriteBatchEx.ScreenSizeTransform, RenderParams.Default );
		}
	}
}

