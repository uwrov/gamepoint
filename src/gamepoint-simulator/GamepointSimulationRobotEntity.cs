using Dargon.Robotics.Simulations2D;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;

namespace demo_robot_simulator {
   public class GamepointSimulationRobotEntity : SimulationRobotEntity {
      private readonly GamepointSimulationRobotState robotState;
      private Body turretBody;

      public GamepointSimulationRobotEntity(
         GamepointSimulationRobotState robotState,
         Vector2 centerOfMass = new Vector2(),
         Vector2 initialPosition = new Vector2(),
         float nonforwardMotionSuppressionFactor = 0)
         : base(robotState, centerOfMass, initialPosition, nonforwardMotionSuppressionFactor) {
         this.robotState = robotState;
      }

      public override void Initialize(Simulation2D simulation, World world) {
         base.Initialize(simulation,world);
         turretBody = BodyFactory.CreateRectangle(world, robotState.Width, robotState.Height, robotState.Density);
      }


   }

   public class GamepointSimulationRobotState : SimulationRobotState {
      private readonly float turretWidth;
      private readonly float turretHeight;
      private readonly float turretDensity;

      public GamepointSimulationRobotState(float width, float height, float density, float linearDamping, float angularDamping,
         float turretWidth, float turretHeight, float turretDensity,
         SimulationMotorState[] motorStates, SimulationWheelShaftEncoderState[] wheelShaftEncoderStates, SimulationGyroscopeState yawGyroscopeState)
         : base(width, height, density, linearDamping, angularDamping, motorStates, wheelShaftEncoderStates, yawGyroscopeState) {
         this.turretWidth = turretWidth;
         this.turretHeight = turretHeight;
         this.turretDensity = turretDensity;
      }

      public float TurretWidth => turretWidth;
      public float TurretHeight => turretHeight;
      public float TurretDensity => turretDensity;
   }
}
