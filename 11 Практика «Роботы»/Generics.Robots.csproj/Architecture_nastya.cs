using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Generics.Robots
{
    public interface IRobotAI<out TypeOfCommand>
    {
        TypeOfCommand GetCommand();
    }

    public class ShooterAI : IRobotAI<ShooterCommand>
    {
        int counter = 1;

        public ShooterCommand GetCommand() => ShooterCommand.ForCounter(counter++);
    }

    public class BuilderAI : IRobotAI<BuilderCommand>
    {
        int counter = 1;

        public BuilderCommand GetCommand() => BuilderCommand.ForCounter(counter++);
    }

    public interface IDevice<in TypeOfCommand>
    {
        string ExecuteCommand(object command);
    }

    public class Mover : IDevice<IMoveCommand>
    {
        public string ExecuteCommand(object obj_command)
        {
            var command = obj_command as IMoveCommand;
            if (command == null)
                throw new ArgumentException();
            return $"MOV {command.Destination.X}, {command.Destination.Y}";
        }
    }

    public class ShooterMover : IDevice<IShooterMoveCommand>
    {
        public string ExecuteCommand(object obj_command)
        {
            var command = obj_command as IShooterMoveCommand;
            if (command == null)
                throw new ArgumentException();
            var hide = command.ShouldHide ? "YES" : "NO";
            return $"MOV {command.Destination.X}, {command.Destination.Y}, USE COVER {hide}";
        }
    }

    public static class Robot
    {
        public static Robot<TCommand> Create<TCommand>(IRobotAI<TCommand> ai, IDevice<TCommand> executor)
        => new Robot<TCommand>(ai, executor);
    }

    public class Robot<TypeOfCommand>
    {
        private readonly IRobotAI<TypeOfCommand> ai;
        private readonly IDevice<TypeOfCommand> device;

        public Robot(IRobotAI<TypeOfCommand> ai, IDevice<TypeOfCommand> executor)
        {
            this.ai = ai;
            this.device = executor;
        }

        public IEnumerable<string> Start(int steps)
        {
            for (int i = 0; i < steps; i++)
            {
                var command = ai.GetCommand();
                if (command == null)
                    break;
                yield return device.ExecuteCommand(command);
            }
        }
    }
}