using System;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xen2D
{
    /// <summary>
    /// This class contains extensions to the XNA SpriteBatch that allow it to draw lines, polygons, sprites, and text
    /// </summary>
    public static class SpriteBatchEx
    {
        private static string[] digits = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
        private static string[] charBuffer = new string[ 10 ];
        private static float[] xposBuffer = new float[ 10 ];
        private static readonly string minValue = Int32.MinValue.ToString( CultureInfo.InvariantCulture );
        private static Rectangle _destinationRect;
        private static readonly uint _defaultDecimalPlaces = 5;
        private static Vector2[] _extentTraceVertices = new Vector2[ ShapePolygon.MaxSides ];
        private static Vector2[] _boundingBoxVertices = new Vector2[ 4 ];
		public static Matrix ScreenSizeTransform = Matrix.Identity;

        public static void DrawLine( this SpriteBatch spriteBatch, Color color, IBasicLine2D line, I2DDisplayModifiers attributes )
		{
            DrawLine( spriteBatch, color, line, attributes, Matrix.Identity );
        }

        public static void DrawLine( this SpriteBatch spriteBatch, Color color, IBasicLine2D line, I2DDisplayModifiers attributes, Matrix transformFromWorldToCamera )
        {
            DrawLine( spriteBatch, color, line.Start, line.Stop, line.Thickness, attributes, transformFromWorldToCamera );
        }

        public static void DrawLine( this SpriteBatch spriteBatch, Color color, Vector2 start, Vector2 stop )
        {
            DrawLine( spriteBatch, color, start, stop, 1 );
        }

        public static void DrawLine( this SpriteBatch spriteBatch, Color color, Vector2 start, Vector2 stop, Matrix transformFromWorldToCamera )
        {
            DrawLine( spriteBatch, color, start, stop, 1, transformFromWorldToCamera );
        }

        public static void DrawLine( this SpriteBatch spriteBatch, Color color, Vector2 start, Vector2 stop, int thickness )
        {
            DrawLine( spriteBatch, color, start, stop, thickness, SpriteDisplayAttributeDefaults.Default );
        }

        public static void DrawLine( this SpriteBatch spriteBatch, Color color, Vector2 start, Vector2 stop, int thickness, Matrix transformFromWorldToCamera )
        {
            DrawLine( spriteBatch, color, start, stop, thickness, SpriteDisplayAttributeDefaults.Default, transformFromWorldToCamera );
        }

        public static void DrawLine( this SpriteBatch spriteBatch, Color color, Vector2 start, Vector2 stop, I2DDisplayModifiers attributes )
		{
			DrawLine( spriteBatch, color, start, stop, attributes, ScreenSizeTransform );
        }

        public static void DrawLine( this SpriteBatch spriteBatch, Color color, Vector2 start, Vector2 stop, I2DDisplayModifiers attributes, Matrix transformFromWorldToCamera )
        {
            DrawLine( spriteBatch, color, start, stop, 1, attributes, transformFromWorldToCamera );
        }

        public static void DrawLine( this SpriteBatch spriteBatch, Color color, Vector2 start, Vector2 stop, int thickness, I2DDisplayModifiers attributes )
        {
			DrawLine( spriteBatch, color, start, stop, thickness, attributes, ScreenSizeTransform );
        }

        public static void DrawLine( this SpriteBatch spriteBatch, Color color, Vector2 start, Vector2 stop, int thickness, I2DDisplayModifiers attributes, Matrix transformFromWorldToCamera )
        {
            //float scale = Camera.GetScale( 1, Camera.Zoom, attributes.ParallaxDepth );
            //Vector2 position = Camera.TranslateAbsoluteVectorToCamera( start, attributes.ParallaxDepth );
            float originalLength = Vector2.Distance( start, stop );
            start = Vector2.Transform( start, transformFromWorldToCamera );
            stop = Vector2.Transform( stop, transformFromWorldToCamera );
            float newLength = Vector2.Distance( start, stop );
            float scale = newLength / originalLength;   //originalLength might be 0
            thickness = (int)Math.Max( 1, thickness * scale );
            Vector2 position = start;

            SinglePixel pixel = SinglePixel.WhitePixel;
            float angle = XenMath.GetAngleFloat( stop - start );

            Vector2 offsetVector = XenMath.CreateNormalVectorFromAngleFloat( angle );
            _destinationRect.X = (int)( position.X + offsetVector.X * (float)thickness * 0.5f );
            _destinationRect.Y = (int)( position.Y + offsetVector.Y * (float)thickness * 0.5f );
            _destinationRect.Width = (int)( Math.Ceiling( Vector2.Distance( start, stop ) ) );
            if( _destinationRect.Width == 0 )
            {
                _destinationRect.Width = 1;
            }
            _destinationRect.Height = thickness;

            spriteBatch.Draw(
                pixel.Asset,
                _destinationRect,
                pixel.SourceRectangle,
#if SILVERLIGHT
                new Color( color, attributes.OpacityFinal ),
#else
                color * attributes.OpacityFinal,
#endif
                angle,
                Vector2.UnitY / 2.0f,
                attributes.SpriteEffects,
                attributes.LayerDepth );
        }

        public static void DrawPixel( this SpriteBatch spriteBatch, Color color, Vector2 position )
        {
            DrawPixel( spriteBatch, color, position, 1 );
        }

        public static void DrawPixel( this SpriteBatch spriteBatch, Color color, Vector2 position, Matrix transformFromWorldToCamera )
        {
            DrawPixel( spriteBatch, color, position, 1, transformFromWorldToCamera );
        }

        public static void DrawPixel( this SpriteBatch spriteBatch, Color color, Vector2 position, int thickness )
        {
            DrawPixel( spriteBatch, color, position, thickness, SpriteDisplayAttributeDefaults.Default );
        }

        public static void DrawPixel( this SpriteBatch spriteBatch, Color color, Vector2 position, int thickness, Matrix transformFromWorldToCamera )
        {
            DrawPixel( spriteBatch, color, position, thickness, SpriteDisplayAttributeDefaults.Default, transformFromWorldToCamera );
        }

        public static void DrawPixel( this SpriteBatch spriteBatch, Color color, Vector2 position, I2DDisplayModifiers attributes )
        {
            DrawPixel( spriteBatch, color, position, attributes, Matrix.Identity );
        }

        public static void DrawPixel( this SpriteBatch spriteBatch, Color color, Vector2 position, I2DDisplayModifiers attributes, Matrix transformFromWorldToCamera )
        {
            DrawPixel( spriteBatch, color, position, 1, attributes, transformFromWorldToCamera );
        }

        public static void DrawPixel( this SpriteBatch spriteBatch, Color color, Vector2 position, int thickness, I2DDisplayModifiers attributes )
        {
            DrawPixel( spriteBatch, color, position, thickness, attributes, Matrix.Identity );
        }

        public static void DrawPixel( this SpriteBatch spriteBatch, Color color, Vector2 position, int thickness, I2DDisplayModifiers attributes, Matrix transformFromWorldToCamera )
        {
            //float scale = Camera.GetScale( 1, Camera.Zoom, attributes.ParallaxDepth );
            //Vector2 position = Camera.TranslateAbsoluteVectorToCamera( start, attributes.ParallaxDepth );
            position = Vector2.Transform( position, transformFromWorldToCamera );
            thickness = (int)Math.Max( 1, thickness );

            _destinationRect.X = (int)( position.X - (float)thickness / 2.0f );
            _destinationRect.Y = (int)( position.Y + (float)thickness / 2.0f );
            _destinationRect.Width = thickness;
            if( _destinationRect.Width == 0 )
            {
                _destinationRect.Width = 1;
            }
            _destinationRect.Height = thickness;

            SinglePixel pixel = SinglePixel.WhitePixel;

            spriteBatch.Draw(
                pixel.Asset,
                _destinationRect,
                pixel.SourceRectangle,
#if SILVERLIGHT
                new Color( color, attributes.Opacity ),
#else
                color * attributes.OpacityFinal,
#endif
                0.0f,
                Vector2.UnitY / 2.0f,
                attributes.SpriteEffects,
                attributes.LayerDepth );
        }

        public static void DrawPixelFast( this SpriteBatch spriteBatch, Color color, Vector2 position )
        {
            int thickness = 1;
            _destinationRect.X = (int)( position.X - (float)thickness / 2.0f );
            _destinationRect.Y = (int)( position.Y + (float)thickness / 2.0f );
            _destinationRect.Width = thickness;
            if( _destinationRect.Width == 0 )
            {
                _destinationRect.Width = 1;
            }
            _destinationRect.Height = thickness;

            SinglePixel pixel = SinglePixel.WhitePixel;

            spriteBatch.Draw(
                pixel.Asset,
                _destinationRect,
                pixel.SourceRectangle,
                color,
                0.0f,
                Vector2.UnitY / 2.0f,
                SpriteEffects.None,
                0.0f );
        }

        public static void DrawPolygon( this SpriteBatch spriteBatch, IPolygon2D polygon, Color color )
        {
            DrawPolygon( spriteBatch, polygon.Vertices, polygon.NumSides, color, Matrix.Identity );
        }

        public static void DrawPolygon( this SpriteBatch spriteBatch, IPolygon2D polygon, Color color, Matrix transformFromWorldToCamera )
        {
            DrawPolygon( spriteBatch, polygon.Vertices, polygon.NumSides, color, transformFromWorldToCamera );
        }

        public static void DrawPolygon( this SpriteBatch spriteBatch, Vector2[] points, int numPoints, Color color )
        {
            DrawPolygon( spriteBatch, points, numPoints, color, 1, Matrix.Identity );
        }

        public static void DrawPolygon( this SpriteBatch spriteBatch, Vector2[] points, int numPoints, Color color, int thickness )
        {
            DrawPolygon( spriteBatch, points, numPoints, color, thickness, Matrix.Identity );
        }

        public static void DrawPolygon( this SpriteBatch spriteBatch, Vector2[] points, int numPoints, Color color, Matrix transformFromWorldToCamera )
        {
            DrawPolygon( spriteBatch, points, numPoints, color, 1, transformFromWorldToCamera );
        }

        public static void DrawPolygon( this SpriteBatch spriteBatch, Vector2[] points, int numPoints, Color color, int thickness, Matrix transformFromWorldToCamera )
        {
            DrawLine( spriteBatch, color, points[ 0 ], points[ numPoints - 1 ], transformFromWorldToCamera );
            for( int i = 0; i < ( numPoints - 1 ); i++ )
            {
                DrawLine( spriteBatch, color, points[ i ], points[ i + 1 ], transformFromWorldToCamera );
            }
        }

        public static void DrawCircle( this SpriteBatch spriteBatch, ICircle2D circle, Color color )
        {
            DrawCircle( spriteBatch, circle.Center, circle.Radius, color, Matrix.Identity );
        }

        public static void DrawCircle( this SpriteBatch spriteBatch, ICircle2D circle, Color color, Matrix transformFromWorldToCamera )
        {
            DrawCircle( spriteBatch, circle.Center, circle.Radius, color, transformFromWorldToCamera );
        }

        public static void DrawCircle( this SpriteBatch spriteBatch, Vector2 center, float radius, Color color )
        {
            DrawCircle( spriteBatch, center, radius, color, 1, Matrix.Identity );
        }

        public static void DrawCircle( this SpriteBatch spriteBatch, Vector2 center, float radius, Color color, int thickness )
        {
            DrawCircle( spriteBatch, center, radius, color, thickness, Matrix.Identity );
        }

        public static void DrawCircle( this SpriteBatch spriteBatch, Vector2 center, float radius, Color color, Matrix transformFromWorldToCamera )
        {
            DrawCircle( spriteBatch, center, radius, color, 1, transformFromWorldToCamera );
        }

        public static void DrawCircle( this SpriteBatch spriteBatch, Vector2 center, float radius, Color color, int thickness, Matrix transformFromWorldToCamera )
        {
            int numCirclePoints = Math.Max( 4, (int)(radius * 2.0f / 5.0f) );
            float angleInterval = MathHelper.TwoPi / (float)numCirclePoints;
            for( int i = 0; i < numCirclePoints; i++ )
            {
                Vector2 startPoint = center + new Vector2( (float)Math.Cos( angleInterval * i ), (float)Math.Sin( angleInterval * i ) ) * radius;
                Vector2 endPoint = center + new Vector2( (float)Math.Cos( angleInterval * ( i + 1 ) ), (float)Math.Sin( angleInterval * ( i + 1 ) ) ) * radius;
                DrawLine( spriteBatch, color, startPoint, endPoint, thickness, SpriteDisplayAttributeDefaults.Default, transformFromWorldToCamera );
            }
        }

        public static void DrawKeyPoints( this SpriteBatch spriteBatch, IRectangularExtent extent, RenderParams renderParams, Matrix transformFromWorldToCamera )
        {
            ISprite marker;
            if( null != renderParams.GetTexture_MarkCenter )
            {
                marker = renderParams.GetTexture_MarkCenter();
                marker.RenderingExtent.Anchor = Vector2.Transform( extent.ReferenceRegion.Center, extent.TranslateFrom );
                DrawSprite( spriteBatch, marker, transformFromWorldToCamera );
            }
            if( null != renderParams.GetTexture_MarkOrigin )
            {
                marker = renderParams.GetTexture_MarkOrigin();
                marker.RenderingExtent.Anchor = Vector2.Transform( extent.Origin, extent.TranslateFrom );
                DrawSprite( spriteBatch, marker, transformFromWorldToCamera );
            }
            if( null != renderParams.GetTexture_MarkTopLeft )
            {
                marker = renderParams.GetTexture_MarkTopLeft();
                marker.RenderingExtent.Anchor = Vector2.Transform( extent.ReferenceTopLeft, extent.TranslateFrom );
                DrawSprite( spriteBatch, marker, transformFromWorldToCamera );
            }
        }

        public static void DrawKeyPoints( this SpriteBatch spriteBatch, IPolygonExtent extent, RenderParams renderParams, Matrix transformFromWorldToCamera )
        {
            ISprite marker;
            if( null != renderParams.GetTexture_MarkCenter )
            {
                marker = renderParams.GetTexture_MarkCenter();
                marker.RenderingExtent.Anchor = Vector2.Transform( extent.ReferenceRegion.Center, extent.TranslateFrom );
                DrawSprite( spriteBatch, marker, transformFromWorldToCamera );
            }
            if( null != renderParams.GetTexture_MarkOrigin )
            {
                marker = renderParams.GetTexture_MarkOrigin();
                marker.RenderingExtent.Anchor = Vector2.Transform( extent.Origin, extent.TranslateFrom );
                DrawSprite( spriteBatch, marker, transformFromWorldToCamera );
            }
        }

        public static void DrawSprite( this SpriteBatch spriteBatch, ISprite sprite )
        {
            DrawSprite( spriteBatch, sprite, Matrix.Identity, RenderParams.Default );
        }

        public static void DrawSprite( this SpriteBatch spriteBatch, ISprite sprite, RenderParams renderParams )
        {
            DrawSprite( spriteBatch, sprite, Matrix.Identity, renderParams );
        }

        public static void DrawSprite( this SpriteBatch spriteBatch, ISprite sprite, Matrix transformFromWorldToCamera )
        {
            DrawSprite( spriteBatch, sprite, transformFromWorldToCamera, RenderParams.Default );
        }

        public static void DrawSprite( this SpriteBatch spriteBatch, ISprite sprite, Matrix transformFromWorldToCamera, RenderParams renderParams )
        {
            if( RenderMode.Texture == ( renderParams.Mode & RenderMode.Texture ) )
                DrawSpriteInternal( spriteBatch, sprite, transformFromWorldToCamera, renderParams );

            DrawPolygonExtent( spriteBatch, sprite.RenderingExtent, transformFromWorldToCamera, renderParams );
        }

        private static void DrawSpriteInternal( this SpriteBatch spriteBatch, ISprite sprite, Matrix transformFromWorldToCamera, RenderParams renderParams )
        {
            Matrix transformFromSpriteToWorld = sprite.RenderingExtent.TranslateFrom;
            Matrix fromSpriteToCamera = transformFromSpriteToWorld * transformFromWorldToCamera;
            Vector2 position = Vector2.Transform( sprite.RenderingExtent.ReferenceTopLeft, fromSpriteToCamera );

            Vector3 scale, translate;
            Quaternion rotate;
            fromSpriteToCamera.Decompose( out scale, out rotate, out translate );
            fromSpriteToCamera.Translation = Vector3.Zero;
            
            float angle = XenMath.GetAngleFloat( Vector2.Transform( Vector2.UnitX, fromSpriteToCamera ) );

            spriteBatch.Draw(
                sprite.TextureInfo.Asset,               // The sprite texture.
                position,                               // The location, in screen coordinates, where the sprite will be drawn.
                sprite.TextureInfo.SourceRectangle,     // A rectangle specifying, in texels, which section of the rectangle to draw.  Use null to draw the entire texture.
                sprite.ModulationColorWithOpacity,      // The color channel modulation to use. Use Color.White for full color with no tinting. 
                angle,                                  // The angle, in radians, to rotate the sprite around the origin.
                Vector2.Zero,                           // The origin of the sprite. Specify (0,0) for the upper-left corner.
                new Vector2( scale.X, scale.Y ),        // Vector containing separate scalar multiples for the x- and y-axes of the sprite.
                sprite.SpriteEffects,                   // Rotations to apply before rendering.
                sprite.LayerDepth );                    // The sorting depth of the sprite, between 0 (front) and 1 (back). You must 
                                                        // specify either SpriteSortMode.FrontToBack or SpriteSortMode.BackToFront for
                                                        // this parameter to affect sprite drawing. 
		}

        public static void Draw( this SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth, Matrix transformFromWorldToCamera, RenderParams renderParams )
        {
            transformFromWorldToCamera *= SpriteBatchEx.ScreenSizeTransform;
            position = Vector2.Transform( position, transformFromWorldToCamera );

            Vector3 vecScale, translate;
            Quaternion rotate;
            transformFromWorldToCamera.Decompose( out vecScale, out rotate, out translate );
            transformFromWorldToCamera.Translation = Vector3.Zero;

            float angle = rotation + XenMath.GetAngleFloat( Vector2.Transform( Vector2.UnitX, transformFromWorldToCamera ) );

            spriteBatch.Draw(
                texture,                                // The sprite texture.
                position,                               // The location, in screen coordinates, where the sprite will be drawn.
                sourceRectangle,                        // A rectangle specifying, in texels, which section of the rectangle to draw.  Use null to draw the entire texture.
                color,                                  // The color channel modulation to use. Use Color.White for full color with no tinting. 
                angle,                                  // The angle, in radians, to rotate the sprite around the origin.
                origin,                                 // The origin of the sprite. Specify (0,0) for the upper-left corner.
                new Vector2( vecScale.X, vecScale.Y ) * scale,  // Vector containing separate scalar multiples for the x- and y-axes of the sprite.
                effects,                                // Rotations to apply before rendering.
                layerDepth );                           // The sorting depth of the sprite, between 0 (front) and 1 (back). You must 
                                                        // specify either SpriteSortMode.FrontToBack or SpriteSortMode.BackToFront for
                                                        // this parameter to affect sprite drawing.
		}

		public static void Draw( this SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth, Matrix transformFromWorldToCamera, RenderParams renderParams )
		{
            transformFromWorldToCamera *= SpriteBatchEx.ScreenSizeTransform;
            position = Vector2.Transform( position, transformFromWorldToCamera );

			Vector3 vecScale, translate;
			Quaternion rotate;
			transformFromWorldToCamera.Decompose( out vecScale, out rotate, out translate );
			transformFromWorldToCamera.Translation = Vector3.Zero;

			float angle = rotation + XenMath.GetAngleFloat( Vector2.Transform( Vector2.UnitX, transformFromWorldToCamera ) );

			spriteBatch.Draw(
				texture,                                // The sprite texture.
				position,                               // The location, in screen coordinates, where the sprite will be drawn.
				sourceRectangle,                        // A rectangle specifying, in texels, which section of the rectangle to draw.  Use null to draw the entire texture.
				color,                                  // The color channel modulation to use. Use Color.White for full color with no tinting. 
				angle,                                  // The angle, in radians, to rotate the sprite around the origin.
				origin,                                 // The origin of the sprite. Specify (0,0) for the upper-left corner.
				new Vector2( vecScale.X * scale.X, vecScale.Y * scale.Y ),  // Vector containing separate scalar multiples for the x- and y-axes of the sprite.
				effects,                                // Rotations to apply before rendering.
				layerDepth );                           // The sorting depth of the sprite, between 0 (front) and 1 (back). You must 
			// specify either SpriteSortMode.FrontToBack or SpriteSortMode.BackToFront for
			// this parameter to affect sprite drawing.
		}

		public static void Draw( this SpriteBatch spriteBatch, Texture2D texture, Rectangle destRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth, Matrix transformFromWorldToCamera, RenderParams renderParams )
        {
            transformFromWorldToCamera *= SpriteBatchEx.ScreenSizeTransform;
			Vector2 position = Vector2.Transform( new Vector2( destRectangle.X, destRectangle.Y ), transformFromWorldToCamera );

			Vector3 vecScale, translate;
			Quaternion rotate;
			transformFromWorldToCamera.Decompose( out vecScale, out rotate, out translate );
			transformFromWorldToCamera.Translation = Vector3.Zero;
			destRectangle = new Rectangle( (int)position.X, (int)position.Y, (int)(destRectangle.Width * vecScale.X), (int)(destRectangle.Height * vecScale.Y) );

			float angle = rotation + XenMath.GetAngleFloat( Vector2.Transform( Vector2.UnitX, transformFromWorldToCamera ) );

			spriteBatch.Draw(
				texture,                                // The sprite texture.
				destRectangle,                          // The location, in screen coordinates, where the sprite will be drawn.
				sourceRectangle,                        // A rectangle specifying, in texels, which section of the rectangle to draw.  Use null to draw the entire texture.
				color,                                  // The color channel modulation to use. Use Color.White for full color with no tinting. 
				angle,                                  // The angle, in radians, to rotate the sprite around the origin.
				origin,                                 // The origin of the sprite. Specify (0,0) for the upper-left corner.
				effects,                                // Rotations to apply before rendering.
				layerDepth );                           // The sorting depth of the sprite, between 0 (front) and 1 (back). You must 
			// specify either SpriteSortMode.FrontToBack or SpriteSortMode.BackToFront for
			// this parameter to affect sprite drawing.
		}

        public static void DrawPolygonExtent( this SpriteBatch spriteBatch, IPolygonExtent extent )
        {
            DrawPolygonExtent( spriteBatch, extent, RenderParams.Default );
        }

        public static void DrawPolygonExtent( this SpriteBatch spriteBatch, IPolygonExtent extent, RenderParams renderParams )
        {
            DrawPolygonExtent( spriteBatch, extent, Matrix.Identity, renderParams );
        }

        public static void DrawPolygonExtent( this SpriteBatch spriteBatch, IPolygonExtent extent, Matrix transformFromWorldToCamera )
        {
            DrawPolygonExtent( spriteBatch, extent, transformFromWorldToCamera, RenderParams.Default );
        }

        public static void DrawPolygonExtent( this SpriteBatch spriteBatch, IPolygonExtent extent, Matrix transformFromWorldToCamera, RenderParams renderParams )
        {
            if( RenderMode.TraceBoundingBox == ( renderParams.Mode & RenderMode.TraceBoundingBox ) )
            {
                DrawBoundingBox( spriteBatch, extent, renderParams.TraceBoundingBoxColor, transformFromWorldToCamera );
                DrawCircle( spriteBatch, extent.ActualCenter, extent.InnerRadius, renderParams.TraceBoundingBoxColor, transformFromWorldToCamera );
                DrawCircle( spriteBatch, extent.ActualCenter, extent.OuterRadius, renderParams.TraceBoundingBoxColor, transformFromWorldToCamera );
            }

            if( RenderMode.TraceRenderingExtent == ( renderParams.Mode & RenderMode.TraceRenderingExtent ) )
            {
                DrawPolygon( spriteBatch, extent, renderParams.TraceRenderingExtentColor, transformFromWorldToCamera );
                DrawKeyPoints( spriteBatch, extent, renderParams, transformFromWorldToCamera );
            }
        }

        private static void DrawExtentBox( this SpriteBatch spriteBatch, IRectangularExtent extent, Color color )
        {
            DrawExtentBox( spriteBatch, extent, color, Matrix.Identity );
        }

        private static void DrawExtentBox( this SpriteBatch spriteBatch, IRectangularExtent extent, Color color, Matrix transformFromWorldToCamera )
        {
            for( int i = 0; i < extent.ReferenceRegion.NumSides; i++ )
            {
                _extentTraceVertices[ i ] = extent.ReferenceRegion.Vertices[ i ];
            }

            for( int i = 0; i < extent.ReferenceRegion.NumSides; i++ )
            {
                _extentTraceVertices[ i ] = Vector2.Transform( _extentTraceVertices[ i ], extent.TranslateFrom );
            }
            spriteBatch.DrawPolygon( _extentTraceVertices, 4, color, transformFromWorldToCamera );
        }

        private static void DrawPolygon( this SpriteBatch spriteBatch, IPolygonExtent extent, Color color )
        {
            DrawPolygon( spriteBatch, extent, color, Matrix.Identity );
        }

        private static void DrawPolygon( this SpriteBatch spriteBatch, IPolygonExtent extent, Color color, Matrix transformFromWorldToCamera )
        {
            for( int i = 0; i < extent.ReferenceRegion.NumSides; i++ )
            {
                _extentTraceVertices[ i ] = Vector2.Transform( extent.ReferenceRegion.Vertices[ i ], extent.TranslateFrom );
            }
            spriteBatch.DrawPolygon( _extentTraceVertices, extent.NumSides, color, transformFromWorldToCamera );
        }

        private static void DrawBoundingBox( this SpriteBatch spriteBatch, IExtent extent, Color color )
        {
            DrawBoundingBox( spriteBatch, extent, color, Matrix.Identity );
        }

        private static void DrawBoundingBox( this SpriteBatch spriteBatch, IExtent extent, Color color, Matrix transformFromWorldToCamera )
        {
            _boundingBoxVertices[ 0 ] = new Vector2( extent.LowestX, extent.LowestY );
            _boundingBoxVertices[ 1 ] = new Vector2( extent.HighestX, extent.LowestY );
            _boundingBoxVertices[ 2 ] = new Vector2( extent.HighestX, extent.HighestY);
            _boundingBoxVertices[ 3 ] = new Vector2( extent.LowestX, extent.HighestY );

            spriteBatch.DrawPolygon( _boundingBoxVertices, 4, color, transformFromWorldToCamera );
        }

        private static void DrawCompositeExtent( this SpriteBatch spriteBatch, ICompositeExtent extent, Color color )
        {
            DrawCompositeExtent( spriteBatch, extent, color, Matrix.Identity );
        }

        private static void DrawCompositeExtent( this SpriteBatch spriteBatch, ICompositeExtent extent, Color color, Matrix transformFromWorldToCamera )
        {
            //for( int i = 0; i < extent.ReferenceRegion.NumSides; i++ )
            //{
            //    _extentTraceVertices[ i ] = Vector2.Transform( extent.ReferenceRegion.Vertices[ i ], extent.TranslateFrom );
            //}
            //spriteBatch.DrawPolygon( _extentTraceVertices, extent.NumSides, color, transformFromWorldToCamera );
            foreach( var child in extent.Items )
            {
                DrawExtent( spriteBatch, child, color, transformFromWorldToCamera );
            }
        }

        public static void DrawExtent( this SpriteBatch spriteBatch, IExtent extent, Color color )
        {
            DrawExtent( spriteBatch, extent, color, Matrix.Identity );
        }

        public static void DrawExtent( this SpriteBatch spriteBatch, IExtent extent, Color color, Matrix transformFromWorldToCamera )
        {
            ICompositeExtent composite = extent as ICompositeExtent;
            if( null != composite )
            {
                DrawCompositeExtent( spriteBatch, composite, color, transformFromWorldToCamera );
                return;
            }

            IPolygonExtent poly = extent as IPolygonExtent;
            if( null != poly )
            {
                DrawPolygon( spriteBatch, poly, color, transformFromWorldToCamera );
                return;
            }

            ICircularExtent circle = extent as ICircularExtent;
            if( null != circle )
            {
                DrawCircle( spriteBatch, circle, color, transformFromWorldToCamera );
                return;
            }

            throw new NotImplementedException( "can only draw extents that are either polygons or circles" );
        }

        /// <summary>
        /// Extension method for SpriteBatch that draws a string without allocating
        /// any memory. This function avoids garbage collections that are normally caused
        /// by creating strings
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch instance whose DrawString method will be invoked.</param>
        /// <param name="text">The text to draw.</param>
        public static void DrawString(
           this SpriteBatch spriteBatch,
           XenString text
           )
        {
            DrawString( spriteBatch, null, text, Matrix.Identity );
        }

        /// <summary>
        /// Extension method for SpriteBatch that draws a string without allocating
        /// any memory. This function avoids garbage collections that are normally caused
        /// by creating strings
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch instance whose DrawString method will be invoked.</param>
        /// <param name="spriteFont">The SpriteFont to draw the text with.</param>
        /// <param name="text">The text to draw.</param>
        public static void DrawString(
            this SpriteBatch spriteBatch,
            SpriteFont spriteFont,
            XenString text
            )
        {
            DrawString( spriteBatch, spriteFont, text, Matrix.Identity );
        }

        /// <summary>
        /// Extension method for SpriteBatch that draws a string without allocating
        /// any memory. This function avoids garbage collections that are normally caused
        /// by creating strings
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch instance whose DrawString method will be invoked.</param>
        /// <param name="spriteFont">The SpriteFont to draw the text with.</param>
        /// <param name="text">The text to draw.</param>
        /// <param name="transformFromWorldToCamera">The matrix transformation to bring the world to camera-space.</param>
        public static void DrawString(
            this SpriteBatch spriteBatch,
            SpriteFont spriteFont,
            XenString text,
            Matrix transformFromWorldToCamera
            )
        {
            SpriteFont font = ( spriteFont != null ? spriteFont : text.SpriteFont );
            if( font == null ) throw new ArgumentNullException( "spriteFont" );

            Matrix transformFromTextToWorld = text.RenderingExtent.TranslateFrom;
            Matrix fromTextToCamera = transformFromTextToWorld * transformFromWorldToCamera;
            Vector2 position = Vector2.Transform( text.RenderingExtent.ReferenceTopLeft, fromTextToCamera );

            Vector3 scale, translate;
            Quaternion rotate;
            fromTextToCamera.Decompose( out scale, out rotate, out translate );
            fromTextToCamera.Translation = Vector3.Zero;

            float angle = XenMath.GetAngleFloat( Vector2.Transform( Vector2.UnitX, fromTextToCamera ) );

            Vector2 nextPosition = position;
            switch( text._type )
            {
                case XenString.XenStringType.String:
                    spriteBatch.DrawString( 
                        font, 
                        text.Text, 
                        position, 
                        text.ModulationColorWithOpacity, 
                        angle, 
                        Vector2.Zero, 
                        new Vector2( scale.X, scale.Y ), 
                        text.SpriteEffects, 
                        text.LayerDepth );
                    nextPosition += font.MeasureString( text.Text ) * Vector2.Transform( Vector2.UnitX, Matrix.CreateRotationZ( angle ) );
                    break;

                case XenString.XenStringType.Integer:
                    nextPosition = spriteBatch.DrawInteger( 
                        font, 
                        text.ValueInt, 
                        position, 
                        text.ModulationColorWithOpacity, 
                        angle, 
                        Vector2.Zero, 
                        new Vector2( scale.X, scale.Y ), 
                        text.SpriteEffects, 
                        text.LayerDepth );
                    break;

                case XenString.XenStringType.Float:
                    nextPosition = spriteBatch.DrawDecimal( 
                        font, 
                        text.ValueFloat, 
                        text.FloatDecimalPlaces, 
                        position, 
                        text.ModulationColorWithOpacity, 
                        angle, 
                        Vector2.Zero, 
                        new Vector2( scale.X, scale.Y ), 
                        text.SpriteEffects, 
                        text.LayerDepth );
                    break;
            }
            if( text._nextString != null )
            {
                spriteBatch.DrawString( 
                    spriteFont, 
                    text._nextString, 
                    nextPosition, 
                    text.ModulationColorWithOpacity, 
                    angle, 
                    Vector2.Zero, 
                    new Vector2( scale.X, scale.Y ), 
                    text.SpriteEffects, text.LayerDepth );
            }
            if( text.IsTemporary )
            {
                text.Release();
            }
        }

        /// <summary>
        /// Extension method for SpriteBatch that draws a string without allocating
        /// any memory. This function avoids garbage collections that are normally caused
        /// by creating strings
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch instance whose DrawString method will be invoked.</param>
        /// <param name="spriteFont">The SpriteFont to draw the text with.</param>
        /// <param name="text">The text to draw.</param>
        /// <param name="position">The screen position specifying where to draw the text.</param>
        /// <param name="color">The color of the text drawn.</param>
        /// <remarks>The parameters override the parameters defined inside the XenString class</remarks>
        public static void DrawString(
            this SpriteBatch spriteBatch,
            SpriteFont spriteFont,
            XenString text,
            Vector2 position,
            Color color
            )
        {
            DrawString( spriteBatch, spriteFont, text, position, color, 0.0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.0f );
        }
        
        /// <summary>
        /// Extension method for SpriteBatch that draws a string without allocating
        /// any memory. This function avoids garbage collections that are normally caused
        /// by creating strings
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch instance whose DrawString method will be invoked.</param>
        /// <param name="spriteFont">The SpriteFont to draw the text with.</param>
        /// <param name="text">The text to draw.</param>
        /// <param name="position">The screen position specifying where to draw the text.</param>
        /// <param name="color">The color of the text drawn.</param>
        /// <param name="rotation">The rotation angle of the text.</param>
        /// <param name="origin">The origin of the text image</param>
        /// <param name="scale">The scale factor of the text.</param>
        /// <param name="effects">The sprite effects applied to the text.</param>
        /// <param name="layerDepth">The layer depth of the text.</param>
        /// <remarks>The parameters override the parameters defined inside the XenString class</remarks>
        public static void DrawString(
            this SpriteBatch spriteBatch,
            SpriteFont spriteFont,
            XenString text,
            Vector2 position,
            Color color,
            float rotation,
            Vector2 origin,
            float scale,
            SpriteEffects effects,
            float layerDepth
            )
        {
            DrawString( spriteBatch, spriteFont, text, position, color, rotation, origin, Vector2.One * scale, effects, layerDepth );
        }

        /// <summary>
        /// Extension method for SpriteBatch that draws a string without allocating
        /// any memory. This function avoids garbage collections that are normally caused
        /// by creating strings
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch instance whose DrawString method will be invoked.</param>
        /// <param name="spriteFont">The SpriteFont to draw the text with.</param>
        /// <param name="text">The text to draw.</param>
        /// <param name="position">The screen position specifying where to draw the text.</param>
        /// <param name="color">The color of the text drawn.</param>
        /// <param name="rotation">The rotation angle of the text.</param>
        /// <param name="origin">The origin of the text image</param>
        /// <param name="scale">The scale factor of the text.</param>
        /// <param name="effects">The sprite effects applied to the text.</param>
        /// <param name="layerDepth">The layer depth of the text.</param>
        /// <returns>The next position on the line to draw text.</returns>
        /// <remarks>The parameters override the parameters defined inside the XenString class</remarks>
        public static void DrawString(
            this SpriteBatch spriteBatch,
            SpriteFont spriteFont,
            XenString text,
            Vector2 position,
            Color color,
            float rotation,
            Vector2 origin,
            Vector2 scale,
            SpriteEffects effects,
            float layerDepth
            )
        {
            SpriteFont font = ( spriteFont != null ? spriteFont : text.SpriteFont );
            if( font == null ) throw new ArgumentNullException( "spriteFont" );

            Vector2 nextPosition = position;
            switch( text._type )
            {
                case XenString.XenStringType.String:
                    spriteBatch.DrawString( 
                        font, 
                        text.Text, 
                        position, 
                        color, 
                        rotation, 
                        origin, 
                        scale, 
                        effects, 
                        layerDepth );
                    nextPosition += font.MeasureString( text.Text ) * Vector2.Transform( Vector2.UnitX, Matrix.CreateRotationZ( rotation ) );
                    break;
                case XenString.XenStringType.Integer:
                    nextPosition = spriteBatch.DrawInteger( 
                        font, 
                        text.ValueInt, 
                        position + Vector2.Transform( text.RenderingExtent.ReferenceTopLeft, text.Transform.TranslateFrom ), 
                        color );
                    break;
                case XenString.XenStringType.Float:
                    nextPosition = spriteBatch.DrawDecimal( 
                        font, 
                        text.ValueFloat, 
                        text.FloatDecimalPlaces, 
                        position + Vector2.Transform( text.RenderingExtent.ReferenceTopLeft, text.Transform.TranslateFrom ), 
                        color );
                    break;
            }
            if( text._nextString != null )
            {
                spriteBatch.DrawString( 
                    spriteFont, 
                    text._nextString, 
                    nextPosition, 
                    color, 
                    rotation, 
                    origin, 
                    scale, 
                    effects, 
                    layerDepth );
            }
            if( text.IsTemporary )
            {
                text.Release();
            }
        }

		public static void DrawString( this SpriteBatch spriteBatch, SpriteFont spriteFont, string value, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth, Matrix transformFromWorldToCamera, RenderParams renderParams )
		{
			if( spriteBatch == null ) throw new ArgumentNullException( "spriteBatch" );
			if( spriteFont == null ) throw new ArgumentNullException( "spriteFont" );

			position = Vector2.Transform( position, transformFromWorldToCamera );

			Vector3 vecScale, translate;
			Quaternion rotate;
			transformFromWorldToCamera.Decompose( out vecScale, out rotate, out translate );
			transformFromWorldToCamera.Translation = Vector3.Zero;

			float angle = rotation + XenMath.GetAngleFloat( Vector2.Transform( Vector2.UnitX, transformFromWorldToCamera ) );
			
			spriteBatch.DrawString( 
					               spriteFont, 
					               value, 
					               position, 
					               color, 
					               angle, 
					               origin, 
					               new Vector2( scale.X * vecScale.X, scale.Y * vecScale.Y ), 
					               effects, 
					               layerDepth
			                       );
		}

        /// <summary>
        /// Extension method for SpriteBatch that draws an integer without allocating   
        /// any memory. This function avoids garbage collections that are normally caused   
        /// by calling Int32.ToString or String.Format.   
        /// </summary>   
        /// <param name="spriteBatch">The SpriteBatch instance whose DrawString method will be invoked.</param>   
        /// <param name="spriteFont">The SpriteFont to draw the integer value with.</param>   
        /// <param name="value">The integer value to draw.</param>   
        /// <param name="position">The screen position specifying where to draw the value.</param>   
        /// <param name="color">The color of the text drawn.</param>   
        /// <returns>The next position on the line to draw text. This value uses position.Y and position.X plus the equivalent of calling spriteFont.MeasureString on value.ToString(CultureInfo.InvariantCulture).</returns>   
        public static Vector2 DrawInteger( this SpriteBatch spriteBatch, SpriteFont spriteFont, int value, Vector2 position, Color color )
        {
            return DrawInteger( spriteBatch, spriteFont, value, position, color, 0.0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.0f );
        }

        public static Vector2 DrawInteger( this SpriteBatch spriteBatch, SpriteFont spriteFont, int value, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth )
        {
            if( spriteBatch == null ) throw new ArgumentNullException( "spriteBatch" );
            if( spriteFont == null ) throw new ArgumentNullException( "spriteFont" );

            Vector2 rightVector = Vector2.Transform( Vector2.UnitX, Matrix.CreateRotationZ( rotation ) );
            Vector2 nextPosition = position;

            if( value == Int32.MinValue )
            {
                nextPosition = nextPosition + rightVector * spriteFont.MeasureString( minValue ).X;
                spriteBatch.DrawString( spriteFont, minValue, position, color, rotation, origin, scale, effects, layerDepth );
                position = nextPosition;
            }
            else
            {
                if( value < 0 )
                {
                    nextPosition = nextPosition + rightVector * spriteFont.MeasureString( "-" ).X;
                    spriteBatch.DrawString( spriteFont, "-", position, color, rotation, origin, scale, effects, layerDepth );
                    value = -value;
                    position = nextPosition;
                }

                int index = 0;

                do
                {
                    int modulus = value % 10;
                    value = value / 10;

                    charBuffer[ index ] = digits[ modulus ];
                    xposBuffer[ index ] = spriteFont.MeasureString( digits[ modulus ] ).X;
                    index += 1;
                }
                while( value > 0 );

                for( int i = index - 1; i >= 0; --i )
                {
                    nextPosition = nextPosition + rightVector * xposBuffer[ i ];
                    spriteBatch.DrawString( spriteFont, charBuffer[ i ], position, color );
                    position = nextPosition;
                }
            }
            return position;
        }

		public static Vector2 DrawInteger( this SpriteBatch spriteBatch, SpriteFont spriteFont, int value, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth, Matrix transformFromWorldToCamera, RenderParams renderParams )
		{
			if( spriteBatch == null ) throw new ArgumentNullException( "spriteBatch" );
			if( spriteFont == null ) throw new ArgumentNullException( "spriteFont" );

			Vector2 originalPosition = position;
			position = Vector2.Transform( position, transformFromWorldToCamera );

			Vector3 vecScale, translate;
			Quaternion rotate;
			transformFromWorldToCamera.Decompose( out vecScale, out rotate, out translate );
			transformFromWorldToCamera.Translation = Vector3.Zero;

			float angle = rotation + XenMath.GetAngleFloat( Vector2.Transform( Vector2.UnitX, transformFromWorldToCamera ) );

			Vector2 rightVector = Vector2.Transform( Vector2.UnitX, Matrix.CreateRotationZ( angle ) );
			rightVector = new Vector2 (rightVector.X * vecScale.X, rightVector.Y * vecScale.Y);
			Vector2 nextPosition = position;

			if( value == Int32.MinValue )
			{
				nextPosition = nextPosition + rightVector * spriteFont.MeasureString( minValue ).X;
				spriteBatch.DrawString( spriteFont, minValue, originalPosition, color, rotation, origin, scale, effects, layerDepth, transformFromWorldToCamera, renderParams );
				position = nextPosition;
			}
			else
			{
				if( value < 0 )
				{
					nextPosition = nextPosition + rightVector * spriteFont.MeasureString( "-" ).X;
					spriteBatch.DrawString( spriteFont, "-", originalPosition, color, rotation, origin, scale, effects, layerDepth, transformFromWorldToCamera, renderParams );
					value = -value;
					position = nextPosition;
				}

				int index = 0;

				do
				{
					int modulus = value % 10;
					value = value / 10;

					charBuffer[ index ] = digits[ modulus ];
					xposBuffer[ index ] = spriteFont.MeasureString( digits[ modulus ] ).X;
					index += 1;
				}
				while( value > 0 );

				for( int i = index - 1; i >= 0; --i )
				{
					nextPosition = nextPosition + rightVector * xposBuffer[ i ];
					spriteBatch.DrawString(
						spriteFont,
						charBuffer[ i ],
						position,
						color,
						angle,
						origin,
						new Vector2( scale.X * vecScale.X, scale.Y * vecScale.Y ),
						effects,
						layerDepth
						);
					position = nextPosition;
				}
			}
			return position;
		}

        internal static Vector2 MeasureIntegerString( SpriteFont spriteFont, int value )
        {
            if( spriteFont == null ) throw new ArgumentNullException( "spriteFont" );

            Vector2 size = Vector2.Zero;

            if( value == Int32.MinValue )
            {
                size = spriteFont.MeasureString( minValue );
            }
            else
            {
                if( value < 0 )
                {
                    Vector2 strVec = spriteFont.MeasureString( "-" );
                    size.X += strVec.X;
                    size.Y = Math.Max( size.Y, strVec.Y );
                }

                do
                {
                    int modulus = value % 10;
                    value = value / 10;


                    Vector2 strVec = spriteFont.MeasureString( digits[ modulus ] );
                    size.X += strVec.X;
                    size.Y = Math.Max( size.Y, strVec.Y );
                }
                while( value > 0 );
            }
            return size;
        }

        public static Vector2 DrawDecimal( this SpriteBatch spriteBatch, SpriteFont spriteFont, float value, Vector2 position, Color color )
        {
            return DrawDecimal( spriteBatch, spriteFont, value, _defaultDecimalPlaces, position, color, 0.0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.0f );
        }

        public static Vector2 DrawDecimal( this SpriteBatch spriteBatch, SpriteFont spriteFont, float value, uint decimalPlaces, Vector2 position, Color color )
        {
            return DrawDecimal( spriteBatch, spriteFont, value, decimalPlaces, position, color, 0.0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.0f );
        }

        public static Vector2 DrawDecimal( this SpriteBatch spriteBatch, SpriteFont spriteFont, float value, uint decimalPlaces, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth )
        {
            if( spriteBatch == null ) throw new ArgumentNullException( "spriteBatch" );
            if( spriteFont == null ) throw new ArgumentNullException( "spriteFont" );

            // Draw the integer value
            Vector2 nextPosition = spriteBatch.DrawInteger( spriteFont, XenMath.RoundFloatToInt( value ), position, color, rotation, origin, scale, effects, layerDepth );
            if( decimalPlaces > 0 )
            {
                spriteBatch.DrawString( spriteFont, ".", nextPosition, color, rotation, origin, scale, effects, layerDepth );
                nextPosition += spriteFont.MeasureString( "." ) * Vector2.UnitX;
                // Draw each decimal digit afterwards one-by-one because otherwise the leading-0's disappear
                for( int i = 0; i < decimalPlaces; i++ )
                {
                    value = ( value - (int)value ) * 10;
                    nextPosition = spriteBatch.DrawInteger( spriteFont, (int)value, nextPosition, color, rotation, origin, scale, effects, layerDepth );
                }
            }
            return nextPosition;
        }

        internal static Vector2 MeasureDecimalString( SpriteFont spriteFont, float value, uint decimalPlaces )
        {
            if( spriteFont == null ) throw new ArgumentNullException( "spriteFont" );

            Vector2 size = SpriteBatchEx.MeasureIntegerString( spriteFont, XenMath.RoundFloatToInt( value ) );

            if( decimalPlaces > 0 )
            {
                Vector2 strVec = spriteFont.MeasureString( "." );
                size.X += strVec.X;
                size.Y = Math.Max( size.Y, strVec.Y );

                // Draw each decimal digit afterwards one-by-one because otherwise the leading-0's disappear
                for( int i = 0; i < decimalPlaces; i++ )
                {
                    value = ( value - (int)value ) * 10;
                    strVec = SpriteBatchEx.MeasureIntegerString( spriteFont, (int)value );
                    size.X += strVec.X;
                    size.Y = Math.Max( size.Y, strVec.Y );
                }
            }
            return size;
        }
    }
}
