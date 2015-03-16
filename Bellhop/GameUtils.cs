using System;
using System.Runtime.InteropServices;
#if MONOTOUCH
using MonoTouch.CoreGraphics;
using MonoTouch.CoreImage;
using MonoTouch.UIKit;
#endif
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Xen2D;
using OpenTK.Graphics.ES11;

namespace Jubble
{
    public static class GameUtils
	{
#if MONOTOUCH
        public static UIImage CaptureScreen()
        {
            UIImage image = null;
            int imageWidth = (int)( UIScreen.MainScreen.Bounds.Width * UIScreen.MainScreen.Scale );
            int imageHeight = (int)( UIScreen.MainScreen.Bounds.Height * UIScreen.MainScreen.Scale );
            int bufferSize = imageWidth * imageHeight * 4;
            IntPtr textureBuffer = Marshal.AllocHGlobal( bufferSize );

            if( textureBuffer != IntPtr.Zero )
            {
                unsafe
                {
                    GL.ReadPixels(
                        0,
                        0,
                        imageWidth,
                        imageHeight,
                        All.Rgba,
                        All.UnsignedByte,
                        textureBuffer
                    );

                    CGDataProvider provider = new CGDataProvider( textureBuffer, bufferSize, true );

                    CGImage cgImage = new CGImage(
                                imageWidth,
                                imageHeight,
                                8,
                                32,
                                4 * imageWidth,
                                CGColorSpace.CreateDeviceRGB( ),
                                CGBitmapFlags.ByteOrderDefault,
                                provider,
                                null,
                                false,
                                CGColorRenderingIntent.Default
                                );

//                    if (Globals.GraphicsDevice.Scaler.Orientation == ExEnInterfaceOrientation.LandscapeLeft)
//                    {
//                        image = new UIImage( cgImage, 1.0f / UIScreen.MainScreen.Scale, UIImageOrientation.Left | UIImageOrientation.UpMirrored );
//                    }
//                    else
//                    {
//                        image = new UIImage( cgImage, 1.0f / UIScreen.MainScreen.Scale, UIImageOrientation.Left | UIImageOrientation.DownMirrored );
//                    }
                    image = UIImage.FromImage( cgImage );
                }
            }

            GameLog.Log( "Image Captured" );

            UIGraphics.BeginImageContext( new System.Drawing.Size( imageWidth, imageHeight ) );
            GameLog.Log( "Began Context" );
            CGContext context = UIGraphics.GetCurrentContext();
            GameLog.Log( "Got Context" );
            context.TranslateCTM( imageWidth / 2, imageHeight / 2 );
            if( Globals.GraphicsDevice.Scaler.Orientation == ExEnInterfaceOrientation.Portrait )
            {
                context.ScaleCTM( 1, -1 );
            }
            else
            {
                context.ScaleCTM( -1, 1 );
            }
            GameLog.Log( "transforms" );

            image.Draw( new System.Drawing.RectangleF( -imageWidth/2, -imageHeight/2, imageWidth, imageHeight ) );
            GameLog.Log( "Image drawn" );
            UIImage newImage = UIGraphics.GetImageFromCurrentImageContext();
            GameLog.Log( "new image retrieved" );
            UIGraphics.EndImageContext();
            GameLog.Log( "Context finished" );

            // Force a GC to avoid memory warnings from the screen capture
            GC.Collect();
            return newImage;

            /*
            // Maybe use this in the future...
            CGImage cgImage = CGImage.ScreenImage;
            UIImage image = UIImage.FromImage( cgImage );
            return image;*/
        }
#elif ANDROID
		public static Android.Graphics.Bitmap CaptureScreen()
		{
			Android.Graphics.Bitmap bitmap = null;
			int imageWidth = (int)( Globals.GraphicsDevice.Viewport.Width );
			int imageHeight = (int)( Globals.GraphicsDevice.Viewport.Height );
			int textureSize = imageWidth * imageHeight;
			int[] textureBuffer = new int[textureSize];

			if( textureBuffer != null )
			{
				unsafe
				{
					// create bitmap screen captureint screenshotSize = width * height;
					GL.ReadPixels(
						0,
						0,
						imageWidth,
						imageHeight,
						All.Rgba,
						All.UnsignedByte,
						textureBuffer
						);
					for (int i = 0; i < textureSize; i++)
					{
						// The alpha and green channels' positions are preserved while the red and blue are swapped
						textureBuffer[i] = (int)(((textureBuffer[i] & 0xff00ff00)) | ((textureBuffer[i] & 0x000000ff) << 16) | ((textureBuffer[i] & 0x00ff0000) >> 16));
					}
					bitmap = Android.Graphics.Bitmap.CreateBitmap(imageWidth, imageHeight, Android.Graphics.Bitmap.Config.Argb8888);
					bitmap.SetPixels(textureBuffer, textureSize - imageWidth, -imageWidth, 0, 0, imageWidth, imageHeight);
					textureBuffer = null;
				}
			}

			// Force a GC to avoid memory warnings from the screen capture
			GC.Collect();

			return bitmap;
		}
#endif
    }
}

