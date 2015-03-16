using System;
using System.Diagnostics;

namespace XenAspects
{
    /// <summary>
    /// Execution Time Performance Measurement Class
    /// </summary>
    public class PerformanceTimer
    {
        private struct PerformanceSample
        {
            public float BeginTime;
            public float EndTime;

            public float Duration
            {
                get { return EndTime - BeginTime; }
            }

            public PerformanceSample( float beginTime, float endTime )
            {
                BeginTime = beginTime;
                EndTime = endTime;
            }
        };

        const int MaxSamples = 16;

        #region Singleton-Related

        // Singleton stopwatch to Tick at each frame
        static Stopwatch stopwatch = new Stopwatch();
        static float[] frameSamples = new float[ MaxSamples ];
        static int frameSampleCounter = 0;
        static float frameCurrent = 0;
        static float frameMin = 0, frameMax = 0, frameAverage = 0;

        /// <summary>
        /// Gets the most recent frame duration sample in milliseconds
        /// </summary>
        public static float CurrentFrameDuration
        {
            get { return frameCurrent; }
        }

        /// <summary>
        /// Gets the minimum frame duration sample in milliseconds within the sample window
        /// </summary>
        public static float MinFrameDuration
        {
            get { return frameMin; }
        }

        /// <summary>
        /// Gets the maximum frame duration sample in milliseconds within the sample window
        /// </summary>
        public static float MaxFrameDuration
        {
            get { return frameMax; }
        }

        /// <summary>
        /// Gets the average frame duration sample in milliseconds within the sample window
        /// </summary>
        public static float AverageFrameDuration
        {
            get { return frameAverage; }
        }

        /// <summary>
        /// Gets the most recent framerate (fps) sample
        /// </summary>
        public static float CurrentFramerate
        {
            get { return 1000.0f / frameCurrent; }
        }

        /// <summary>
        /// Gets the minimum framerate (fps) sample within the sample window
        /// </summary>
        public static float MinFramerate
        {
            get { return 1000.0f / frameMax; } // Max-duration is the minimum framerate
        }

        /// <summary>
        /// Gets the maximum framerate (fps) sample within the sample window
        /// </summary>
        public static float MaxFramerate
        {
            get { return 1000.0f / frameMin; } // Min-duration is the maximum framerate
        }

        /// <summary>
        /// Gets the average framerate (fps) sample within the sample window
        /// </summary>
        public static float AverageFramerate
        {
            get { return 1000.0f / frameAverage; }
        }

        /// <summary>
        /// Indicate the start of a frame of execution
        /// </summary>
        public static void FrameTick()
        {
            lock( typeof( PerformanceTimer ) )
            {
                stopwatch.Reset();
                stopwatch.Start();
            }
        }

        /// <summary>
        /// Indicate the finalization of a frame of execution
        /// </summary>
        public static void FrameRefresh()
        {
            lock( typeof( PerformanceTimer ) )
            {
                // Refresh the full frame properties
                frameSamples[ frameSampleCounter ] = (float)stopwatch.Elapsed.TotalMilliseconds;
                frameCurrent = frameSamples[ frameSampleCounter ];
                frameSampleCounter++;
                if( frameSampleCounter >= MaxSamples )
                {
                    frameSampleCounter = 0;
                }
                frameAverage = 0;
                frameMin = frameMax = frameCurrent;
                for( int i = 0; i < MaxSamples; i++ )
                {
                    float duration = frameSamples[ i ];
                    frameMin = Math.Min( frameMin, duration );
                    frameMax = Math.Max( frameMax, duration );
                    frameAverage += duration;
                }
                frameAverage /= MaxSamples;
            }
        }

        #endregion

        PerformanceSample _currentSample;
        float[] _samples = null;
        int _sampleCounter = 0;
        bool _isRefreshed = false;
        float _current = 0;
        float _min = 0, _max = 0, _average = 0;

        public PerformanceTimer()
        {
            _currentSample = new PerformanceSample( 0, 0 );
            _samples = new float[ MaxSamples ];
            _sampleCounter = 0;
            CountEachExecution = false;
        }

        /// <summary>
        /// Gets or Sets the flag to indicate whether or not sample on each Begin(),End() pair or over the full frame
        /// </summary>
        /// <remarks>Default value is false</remarks>
        public bool CountEachExecution
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the most recent duration sample in milliseconds
        /// </summary>
        public float Current
        {
            get { return _current; }
        }

        /// <summary>
        /// Gets the minimum duration sample in milliseconds within the sample window
        /// </summary>
        public float Min
        {
            get { return _min; }
        }

        /// <summary>
        /// Gets the maximum duration sample in milliseconds within the sample window
        /// </summary>
        public float Max
        {
            get { return _max; }
        }

        /// <summary>
        /// Gets the average duration sample in milliseconds within the sample window
        /// </summary>
        public float Average
        {
            get { return _average; }
        }

        /// <summary>
        /// Marks the start of this execution sample
        /// </summary>
        public void Begin()
        {
            _currentSample.BeginTime = (float)stopwatch.Elapsed.TotalMilliseconds;
        }

        /// <summary>
        /// Marks the end of this execution sample
        /// </summary>
        public void End()
        {
            _currentSample.EndTime = (float)stopwatch.Elapsed.TotalMilliseconds;
            if( _isRefreshed || CountEachExecution )
            {
                // Set the current sample
                _isRefreshed = false;
                _samples[ _sampleCounter ] = _currentSample.Duration;
                if( CountEachExecution )
                {
                    // Increment the sample counter
                    _sampleCounter++;
                    if( _sampleCounter >= MaxSamples )
                    {
                        _sampleCounter = 0;
                    }
                }
            }
            else
            {
                // Add to the current sample since we haven't received the end-of-frame refresh yet
                _samples[ _sampleCounter ] += _currentSample.Duration;
            }
        }

        /// <summary>
        /// Updates the properties with the latest values
        /// </summary>
        /// <remarks>
        /// This method should be called after FrameRefresh() or just prior to the following FrameTick() so that it does not add to the performance sample timing.
        /// </remarks>
        public void Refresh()
        {
            // Refresh the values
            _current = _samples[ _sampleCounter ];
            _average = 0;
            _min = _max = _current;
            for( int i = 0; i < MaxSamples; i++ )
            {
                float totalDuration = _samples[ i ];
                _min = Math.Min( _min, totalDuration );
                _max = Math.Max( _max, totalDuration );
                _average += totalDuration;
            }
            _average /= MaxSamples;

            // Increment the sample counter
            _sampleCounter++;
            if( _sampleCounter >= MaxSamples )
            {
                _sampleCounter = 0;
            }
            _isRefreshed = true;
        }
    }
}
