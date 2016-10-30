using Dargon.Commons;
using Dargon.Robotics;
using Dargon.Robotics.DeviceRegistries;
using Dargon.Robotics.Devices;
using Dargon.Robotics.Simulations2D;
using Dargon.Robotics.Simulations2D.Devices;
using Dargon.Ryu;
using Dargon.Ryu.Modules;
using Microsoft.Xna.Framework;
using System;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Threading;
using Dargon.Commons.Collections;
using Dargon.Robotics.Debugging;
using FarseerPhysics;
using Microsoft.Xna.Framework.Input;

namespace demo_robot_simulator {
   public class Program {
      private const float kMecanumWheelForceAngle = (float)(Math.PI / 4);
      private const float kMecanumWheelForceMagnitude = 50 * 1000.0f;
      private const float kNewtonMetersPerOzInch = 0.00706155183333f;
      private const float kMotorTorque = 12.42f; //50 * kNewtonMetersPerOzInch;
      private const float kMetersPerInch = 0.0254f;
      private const float kWheelRadius = 5 * kMetersPerInch;
      private const float kWheelForce = kMotorTorque / kWheelRadius;
      private static Simulation2D simulation2D;

      public static void Main(string[] args) {
         // create simulation state
         var constants = SimulationConstantsFactory.WideLandRobot();
//         var motors = SimulationMotorStateFactory.SkidDrive(constants.WidthMeters, constants.HeightMeters, kWheelForce);
         var motors = SimulationMotorStateFactory.HybridDrive(constants.WidthMeters, constants.HeightMeters, kMecanumWheelForceAngle, kWheelForce);
         var wheelShaftEncoders = SimulationWheelShaftEncoderStateFactory.FromMotors(motors, kWheelRadius, 128);
         var yawGyro = new SimulationGyroscopeState("Drive.Gyroscopes.Yaw");
         var robot = new SimulationRobotState(constants.WidthMeters, constants.HeightMeters, constants.Density, motors, wheelShaftEncoders, yawGyro);

//         var robotEntity = new SimulationRobotEntity(constants, robot, new Vector2(0, robot.Height / 4));
//         var robotEntity = new SimulationRobotEntity(constants, robot, new Vector2(-robot.Width / 64, robot.Height / 4), true);
         var robotEntity = new SimulationRobotEntity(constants, robot, default(Vector2), new Vector2(1,1), 0.05f);
         
         // create robot state
         var deviceRegistry = new DefaultDeviceRegistry();
         foreach (var simulationMotorState in robot.MotorStates) {
            var motor = new SimulationMotorAdapter(simulationMotorState);
            motor.Initialize();
            deviceRegistry.AddDevice(motor.Name, motor);
            motor.Set(0.1f);
         }

         foreach (var simulationWheelShaftEncoderState in wheelShaftEncoders) {
            var encoder = new SimulationIncrementalRotaryEncoderAdapter(simulationWheelShaftEncoderState, 128);
            deviceRegistry.AddDevice(encoder.Name, encoder);
         }

         deviceRegistry.AddDevice(yawGyro.Name, new SimulationGyroscopeAdapter(yawGyro));

         // Debugscene Stuff
         var debugRenderContext = new DebugRenderContext();

         // start robot code in new thread
         new Thread(() => {
            var ryuConfiguration = new RyuConfiguration();
            ryuConfiguration.AdditionalModules.Add(new RyuModule().With(m => {
               m.Required.Singleton<KeyboardGamepad>().Implements<IGamepad>();
               m.Required.Singleton<IDeviceRegistry>(x => deviceRegistry);
               m.Required.Singleton<IDebugRenderContext>(x => debugRenderContext);
            }));

            var ryu = new RyuFactory().Create(ryuConfiguration);
            ryu.GetOrActivate<IRobot>().Run();
         }).Start();

         var entities = new ConcurrentSet<ISimulationEntity>();
         entities.AddOrThrow(robotEntity);
         entities.AddOrThrow(new SimulationBallEntity(new SimulationBallConstants() {Radius = .25f, Density = 10.0f, LinearDamping = 1.0f}, initialPosition:new Vector2(2,2)));
         simulation2D = new GamepointSimulation2D(entities, debugRenderContext);
         simulation2D.Run();
      }

      class GamepointSimulation2D : Simulation2D {
         private readonly Random random = new Random();
         private KeyboardState previousKeyboardState;
         private readonly SimulationRobotEntity robotEntity;

         public GamepointSimulation2D(ConcurrentSet<ISimulationEntity> entities, IDebugRenderContext debugRenderContext) : base(entities, debugRenderContext) {
            previousKeyboardState = Keyboard.GetState();
            foreach (var entity in entities) {
               if (entity is SimulationRobotEntity) {
                  this.robotEntity = (SimulationRobotEntity)entity;
               }
            }
         }

         protected override void Update(GameTime gameTime) {
            base.Update(gameTime);
            var keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.D1) && !previousKeyboardState.IsKeyDown(Keys.D1)) {
               var cursorPosition = Mouse.GetState(Window).Position;

               var ballLocation = ConvertDisplayPointToSimulatorVector(cursorPosition);
               ballLocation.X += ((float)random.NextDouble() - 0.5f) / 10000f;
               ballLocation.Y += ((float)random.NextDouble() - 0.5f) / 10000f;
               
               AddEntity(new SimulationBallEntity(new SimulationBallConstants{ Radius = .25f, Density = 10.0f, LinearDamping = 1.0f }, initialPosition: ballLocation));
            }
            if (keyboardState.IsKeyDown(Keys.Left) && robotEntity.TurretRotation < Math.PI / 4) {
               robotEntity.TurretRotation += .03f;
            }
            if (keyboardState.IsKeyDown(Keys.Right) && robotEntity.TurretRotation > Math.PI / -4) {
               robotEntity.TurretRotation -= .03f;
            }
            previousKeyboardState = keyboardState;
         }
      }
   }
}
