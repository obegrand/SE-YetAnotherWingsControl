using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        List<IMyMotorAdvancedStator> wings_side = new List<IMyMotorAdvancedStator>();
        List<IMyMotorAdvancedStator> wings_roll = new List<IMyMotorAdvancedStator>();
        IMyCockpit cockpit;
        IMyGyro gyro;

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update1;

            GridTerminalSystem.GetBlockGroupWithName("!Wings Speed").GetBlocksOfType(wings_side);
            GridTerminalSystem.GetBlockGroupWithName("!Wings Roll").GetBlocksOfType(wings_roll);
            cockpit = GridTerminalSystem.GetBlockWithName("!cockpit") as IMyCockpit;
            gyro = GridTerminalSystem.GetBlockWithName("gyro") as IMyGyro;
        }

        public void Main(string argument, UpdateType updateSource)
        {
            Vector3D LinearVelocity = cockpit.GetShipVelocities().LinearVelocity;
            float speed = (float)LinearVelocity.Length();  // Реальная скорость
            foreach (IMyMotorAdvancedStator hinge in wings_side)
            {
                hinge.LowerLimitDeg = Math.Max(speed * -1, -30);
            }

            // Получаем угловую скорость корабля
            Vector3D angularVelocity = cockpit.GetShipVelocities().AngularVelocity;

            // Выводим угловую скорость на экран
            Echo("Angular Velocity X = " + angularVelocity.X.ToString());
            Echo("--------------------");
            Echo("Angular Velocity Y = " + angularVelocity.Y.ToString());
            Echo("--------------------");
            Echo("Angular Velocity Z = " + angularVelocity.Z.ToString());

            // Управление крыльями в зависимости от крена
            foreach (var hinge in wings_roll)
            {
                float targetVelocity;

                // Если крен больше порогового значения, изменяем угол
                if (Math.Abs(angularVelocity.X) > 0.01)
                {
                    targetVelocity = MathHelper.Clamp((float)angularVelocity.X * 0.5f, -1f, 1f);
                }
                else
                {
                    // Возвращаем в нулевое положение, если крен меньше порога
                    targetVelocity = hinge.Angle * -5f;  // Быстро возвращаем в 0
                }

                hinge.TargetVelocityRad = targetVelocity;
            }
        }
    }
}
