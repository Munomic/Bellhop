using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Xen2D;
using XenAspects;

namespace DuckyEngine
{
    public class DuckyScreen : IDuckyElement
    {
        PerformanceTimer _updateLoopPerf;
        PerformanceTimer _drawLoopPerf;
        Stack<DuckyScene> _scenes = null;
        internal static Dictionary<string, SpriteFont> Fonts = new Dictionary<string, SpriteFont>();

        public Color BackgroundColor { get; set; }
        public DuckyScene Scene { get { return _scenes.Peek(); } }
        public int Count { get { return _scenes.Count; } }

        public DuckyScreen()
        {
            _updateLoopPerf = new PerformanceTimer();
            _drawLoopPerf = new PerformanceTimer();
            _scenes = new Stack<DuckyScene>();
        }

        public virtual void Load( XElement scene )
        {
            Unload();
            Push( scene );
        }

        public virtual void Push( XElement scene )
        {
            DuckyScene newScene = new DuckyScene();
            newScene.Load( scene );
            _scenes.Push( newScene );
        }

        public virtual void Pop()
        {
            _scenes.Pop().Unload();
        }

        public virtual void Unload()
        {
            while( _scenes.Count > 0 )
            {
                Pop();
            }
            //Fonts.Clear();
        }

        public virtual void Update( float timeDiff )
        {
            _updateLoopPerf.Begin();
            if( _scenes.Count > 0 )
            {
                Scene.Update( timeDiff );
            }
            _updateLoopPerf.End();
            _updateLoopPerf.Refresh();
        }

        public virtual void Render( ResizedSpriteBatch sb )
        {
            _drawLoopPerf.Begin();
            Globals.GraphicsDevice.Clear( this.BackgroundColor );
            if( _scenes.Count > 0 )
            {
                sb.Begin( SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise );
                Scene.Render( sb );
                sb.End();
            }
            _drawLoopPerf.End();
            PerformanceTimer.FrameRefresh();
            _drawLoopPerf.Refresh();

            // Let the next 'frame' begin now because Draw() is throttled internally
            PerformanceTimer.FrameTick();
        }
    }
}
