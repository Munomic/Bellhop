using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jubble
{
    public enum ElevatorState
    {
        Ready,
        Opening,
        Closing,
        Moving,
    }

    public class Elevator
    {
        public float Speed = 5.0f;
        public float DoorSpeed = 1.0f;
        public float FloorDelay = 3.0f; // Minimum time before door can close again
        public int CurrentFloor = 0;
        public bool[] Floors;
        public ElevatorState State = ElevatorState.Ready;

        public float TimeBeforeReady = 0.0f;
        public float TimeToFloor = 0.0f;
        public float TimeToOpenDoor = 0.0f;
        public float TimeToCloseDoor = 0.0f;

        public int PreviousFloor = 0;
        public int NextFloor = 0;
        public Action<int> OnFloorReached = null;

        public bool IsDirectionUp
        {
            get { return ( NextFloor > CurrentFloor ); }
        }

        public bool IsOpen
        {
            get
            {
                return ( State == ElevatorState.Moving );
            }
        }

        public Elevator(
            float speed = 5.0f, // Time to reach next floor in seconds
            float doorSpeed = 1.0f, // Time to open/close door
            int floorCount = 4
            )
        {
            Speed = speed;
            DoorSpeed = doorSpeed;
            Floors = new bool[ floorCount ];
            State = ElevatorState.Ready;
        }

        public void QueueFloor( int floor )
        {
            Floors[ floor ] = true;
            if( State == ElevatorState.Ready )
            {
                TimeToFloor = Speed;
                TimeToCloseDoor = DoorSpeed;
                TimeToOpenDoor = DoorSpeed;
                TimeBeforeReady = 0.0f;
                State = ElevatorState.Closing;
                NextFloor = floor;
            }
            else
            {
                if( IsDirectionUp )
                {
                    if( CurrentFloor < floor && floor < NextFloor )
                    {
                        NextFloor = floor;
                    }
                }
                else
                {
                    if( CurrentFloor > floor && floor > NextFloor )
                    {
                        NextFloor = floor;
                    }
                }
            }
        }

        private int NextFloorAbove()
        {
            for( int i = CurrentFloor; i < Floors.Length; i++ )
            {
                if( Floors[ i ] )
                {
                    return i;
                }
            }
            return -1;
        }

        private int NextFloorBelow()
        {
            for( int i = CurrentFloor; i >= 0; i-- )
            {
                if( Floors[ i ] )
                {
                    return i;
                }
            }
            return -1;
        }

        public void Update( float timeDiff )
        {
            switch( State )
            {
                case ElevatorState.Closing:
                    TimeBeforeReady -= timeDiff;
                    if( TimeBeforeReady <= 0.0f )
                    {
                        TimeToCloseDoor -= timeDiff;
                        if( TimeToCloseDoor <= 0.0f )
                        {
                            PreviousFloor = CurrentFloor;
                            TimeToFloor = Speed;
                            State = ElevatorState.Moving;
                        }
                    }
                    break;
                case ElevatorState.Moving:
                    TimeToFloor -= timeDiff;
                    if( TimeToFloor <= 0.0f )
                    {
                        if( IsDirectionUp )
                        {
                            CurrentFloor++;
                        }
                        else
                        {
                            CurrentFloor--;
                        }

                        // Check if we reached destination
                        if( CurrentFloor == NextFloor )
                        {
                            Floors[ CurrentFloor ] = false;
                            TimeToOpenDoor = DoorSpeed;
                            State = ElevatorState.Opening;
                            if( OnFloorReached != null )
                            {
                                OnFloorReached( CurrentFloor );
                            }
                        }
                        else
                        {
                            // Keep moving
                            TimeToFloor += Speed;
                        }
                    }
                    break;
                case ElevatorState.Opening:
                    TimeToOpenDoor -= timeDiff;
                    if( TimeToOpenDoor <= 0.0f )
                    {
                        TimeBeforeReady = FloorDelay;

                        // Check if there's any other floors to go to
                        if( IsDirectionUp )
                        {
                            if( NextFloorAbove() >= 0 )
                            {
                                NextFloor = NextFloorAbove();
                            }
                            else
                            {
                                NextFloor = NextFloorBelow();
                            }
                        }
                        else
                        {
                            if( NextFloorBelow() >= 0 )
                            {
                                NextFloor = NextFloorBelow();
                            }
                            else
                            {
                                NextFloor = NextFloorAbove();
                            }
                        }

                        if( NextFloor >= 0 )
                        {
                            TimeToFloor = Speed;
                            TimeToCloseDoor = DoorSpeed;
                            State = ElevatorState.Closing;
                        }
                        else
                        {
                            NextFloor = CurrentFloor;
                            State = ElevatorState.Ready;
                        }
                    }
                    break;
                case ElevatorState.Ready:
                default:
                    TimeBeforeReady -= timeDiff;
                    break;
            }
        }
    }
}
