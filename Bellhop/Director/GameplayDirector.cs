using DuckyEngine;
using Microsoft.Xna.Framework;
using Xen2D;
using System;
using System.Collections.Generic;

namespace Jubble
{
    public class GameplayDirector : DuckySceneDirector
    {
        public class ElevatorButton
        {
            public bool IsOn = false;
            DuckyGroup Group = null;
            public Action OnActivated = null;

            public ElevatorButton( DuckyGroup group, Action onActivated = null )
            {
                IsOn = false;
                Group = group;
                OnActivated = onActivated;

                Unset();

                Group.OnTouchDown = ( int id, Vector2 position ) =>
                {
                    if( !IsOn )
                    {
                        Group[ "Off" ].Visible = false;
                        Group[ "In" ].Visible = true;
                        Group[ "On" ].Visible = false;
                    }
                };

                Group.OnTouchUp = ( int id, Vector2 position, bool isInside ) =>
                {
                    if( !IsOn )
                    {
                        Group[ "Off" ].Visible = false;
                        Group[ "In" ].Visible = false;
                        Group[ "On" ].Visible = true;

                        IsOn = true;
                        if( OnActivated != null )
                        {
                            OnActivated();
                        }
                    }
                };
            }

            public void Unset()
            {
                IsOn = false;
                Group[ "Off" ].Visible = true;
                Group[ "In" ].Visible = false;
                Group[ "On" ].Visible = false;
            }
        }

        Elevator Elevator = new Elevator( 5.0f, 0.5f, 4 );
        ElevatorButton[] ElevatorButtons = new ElevatorButton[ 4 ];
        float[] FloorYPositions = new float[ 4 ];
        float currentElevatorY = 0.0f;

        public override void Initialize( DuckyScene scene )
        {
            base.Initialize( scene );

            AttachEntityFromElement( "Main", "Elevator", DuckySceneEntityType.Actor, 1.0f, 1 );
            FloorYPositions[ 3 ] = -2.0f;
            FloorYPositions[ 2 ] = 30.0f;
            FloorYPositions[ 1 ] = 62.0f;
            FloorYPositions[ 0 ] = 94.0f;
            //RectangularExtent groundExtent = new RectangularExtent();
            //groundExtent.Reset( 0.0f, (float)GameSettings.ScreenHeight, (float)GameSettings.ScreenWidth, 50.0f );
            //AttachEntity( "Ground", DuckySceneEntityType.Prop, groundExtent, 0.0f, 1 );
            //this[ "Elevator" ].Velocity = new Vector2( 0.0f, 100.0f );
            //this[ "Elevator" ].OnCollision = ( DuckySceneEntity entity, Vector2 point ) =>
            //{
            //    this[ "Elevator" ].Velocity = Vector2.Zero;
            //    this[ "Elevator" ].Position = new Vector2( this[ "Elevator" ].Position.X, this[ "Ground" ].Position.Y - this[ "Elevator" ].Element.Height );
            //};

            Elevator.OnFloorReached = ( int floor ) =>
            {
                ElevatorButtons[ floor ].Unset();
            };

            ElevatorButtons[ 0 ] = new ElevatorButton( (DuckyGroup)scene[ "Main" ][ "ElevatorF1" ], () =>
            {
                Elevator.QueueFloor( 0 );
            } );
            ElevatorButtons[ 1 ] = new ElevatorButton( (DuckyGroup)scene[ "Main" ][ "ElevatorF2" ], () =>
            {
                Elevator.QueueFloor( 1 );
            } );
            ElevatorButtons[ 2 ] = new ElevatorButton( (DuckyGroup)scene[ "Main" ][ "ElevatorF3" ], () =>
            {
                Elevator.QueueFloor( 2 );
            } );
            ElevatorButtons[ 3 ] = new ElevatorButton( (DuckyGroup)scene[ "Main" ][ "ElevatorF4" ], () =>
            {
                Elevator.QueueFloor( 3 );
            } );
        }

        public override void Update( float timeDiff )
        {
            if( Elevator.State == ElevatorState.Moving )
            {
                // Calculate how far the elevator's gotten
                int numFloorsToMove = Math.Abs( Elevator.NextFloor - Elevator.PreviousFloor );
                int numFloorsRemaining = Math.Abs( Elevator.NextFloor - Elevator.CurrentFloor );
                currentElevatorY = Interpolator.Linear( FloorYPositions[ Elevator.NextFloor ], FloorYPositions[ Elevator.PreviousFloor ], Elevator.TimeToFloor + Elevator.Speed * ( numFloorsRemaining - 1 ), Elevator.Speed * numFloorsToMove );
            }
            else
            {
                currentElevatorY = FloorYPositions[ Elevator.CurrentFloor ];
            }
            this[ "Elevator" ].Position = new Vector2( this[ "Elevator" ].Position.X, currentElevatorY );
            Scene[ "Main" ][ "ElevatorTop" ].Position = new Vector2( this[ "Elevator" ].Position.X, currentElevatorY );
            Elevator.Update( timeDiff );
            base.Update( timeDiff );
        }
    }
}
