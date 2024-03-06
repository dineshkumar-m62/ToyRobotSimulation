using System;
using System.IO;

public interface ICommand
{
    void Execute(Robot robot);
}

public class PlaceCommand : ICommand
{
    private readonly int _x;
    private readonly int _y;
    private readonly string _direction;

    public PlaceCommand(int x, int y, string direction)
    {
        _x = x;
        _y = y;
        _direction = direction;
    }

    public void Execute(Robot robot)
    {
        robot.Place(_x, _y, _direction);
    }
}

public class MoveCommand : ICommand
{
    public void Execute(Robot robot)
    {
        robot.Move();
    }
}

public class LeftCommand : ICommand
{
    public void Execute(Robot robot)
    {
        robot.Left();
    }
}

public class RightCommand : ICommand
{
    public void Execute(Robot robot)
    {
        robot.Right();
    }
}

public class ReportCommand : ICommand
{
    public void Execute(Robot robot)
    {
        Console.WriteLine(robot.Report());
    }
}

public class Robot
{
    private readonly string[] _directions = { "NORTH", "EAST", "SOUTH", "WEST" };

    public int X { get; private set; }
    public int Y { get; private set; }
    public string Direction { get; private set; }

    private readonly Tabletop _tabletop;

    public Robot(Tabletop tabletop)
    {
        _tabletop = tabletop;
    }

    public void Place(int x, int y, string direction)
    {
        if (IsValidPosition(x, y) && Array.IndexOf(_directions, direction) != -1)
        {
            X = x;
            Y = y;
            Direction = direction;
        }
    }

    private bool IsValidPosition(int x, int y)
    {
        return x >= 0 && x < _tabletop.Width && y >= 0 && y < _tabletop.Height;
    }

    public void Move()
    {
        switch (Direction)
        {
            case "NORTH" when Y < _tabletop.Height - 1:
                Y++;
                break;
            case "EAST" when X < _tabletop.Width - 1:
                X++;
                break;
            case "SOUTH" when Y > 0:
                Y--;
                break;
            case "WEST" when X > 0:
                X--;
                break;
        }
    }

    public void Left()
    {
        var currentDirectionIndex = Array.IndexOf(_directions, Direction);
        Direction = _directions[(currentDirectionIndex + 3) % 4];
    }

    public void Right()
    {
        var currentDirectionIndex = Array.IndexOf(_directions, Direction);
        Direction = _directions[(currentDirectionIndex + 1) % 4];
    }

    public string Report()
    {
        return $"{X},{Y},{Direction}";
    }
}

public class Tabletop
{
    public int Width { get; } = 5;
    public int Height { get; } = 5;
}

public class CommandParser
{
    private bool _placed = false;
    public ICommand ParseCommand(string command)
    {
        var commandParts = command.Split();
        //Console.WriteLine(commandParts);
        //Console.WriteLine(commandParts[0]);
        switch (commandParts[0])
        {
            case "PLACE":
                _placed = true;
                var args = commandParts[1].Split(',');
                //Console.WriteLine(args[0]);
                //Console.WriteLine(args[1]);
                //Console.WriteLine(args[2]);
                return new PlaceCommand(int.Parse(args[0]), int.Parse(args[1]), args[2]);
            case "MOVE":
                return _placed ? new MoveCommand() : null;
            case "LEFT":
                return _placed ? new LeftCommand() : null;
            case "RIGHT":
                return _placed ? new RightCommand() : null;
            case "REPORT":
                return _placed ? new ReportCommand() : null;
            default:
                return null;
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        Tabletop tabletop = new Tabletop();
        Robot robot = new Robot(tabletop);
        CommandParser commandParser = new CommandParser();

        //string commands = @"C:\Users\D0861164\source\repos\ToyRobot\Command\commands.txt";
        var commands = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "commands.txt");

        //Console.WriteLine("commands", commands);

        try
        {
            using (StreamReader sr = new StreamReader(commands))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                { 
                    //Console.WriteLine("line", line);
                    var cmd = commandParser.ParseCommand(line);
                    if (cmd != null)
                        cmd.Execute(robot);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"The file could not be read: {e.Message}");
        }
    }
}