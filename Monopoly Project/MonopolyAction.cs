﻿using System;
using static Monopoly_Project.Cell;

namespace Monopoly_Project
{
    public abstract class MonopolyAction
    {
        public abstract void Execute();
        public virtual bool IsLegalMove()
        {
            return true;
        }

        public static void PrintCell(Cell cell)
        {
            if (cell.Type == CellType.PropertyCell)
            {
                PropertyCell Cell = (PropertyCell)cell;
                Console.WriteLine(ActionManager.instance.CurrentPlayer.Name + "'s cell : " + Cell.StreetName +
                    ", " + Cell.Value + "$");
            }
            else
            {
                Console.WriteLine(ActionManager.instance.CurrentPlayer.Name + " is on the " + Cell.ToString(cell.Type));
            }
        }
    }

    public class MoveAction : MonopolyAction
    {
        public int NumberOfSteps { get; set; }
        public MoveAction(int numberOfSteps)
        {
            NumberOfSteps = numberOfSteps;
        }

        public override void Execute()
        {
            Console.WriteLine("You move forward " + NumberOfSteps + " cells");
            Player p = ActionManager.instance.CurrentPlayer;
            if ((p.ActualCell.Index + NumberOfSteps) >= 40)
            {
                ActionManager.AddAction(new GetSalaryAction());
            }
            p.ActualCell = Gameboard.instance.Cells[(p.ActualCell.Index + NumberOfSteps) % 40];
            PrintCell(p.ActualCell);
        }

        public override bool IsLegalMove()
        {
            if (ActionManager.instance.CurrentPlayer.IsInJail)
            {
                Console.WriteLine("You can't move right now");
                return false;
            }
            return true;
        }
    }

    public class MoveOutOfJail : MonopolyAction
    {
        public int NumberOfSteps { get; set; }
        public MoveOutOfJail(int numberOfSteps)
        {
            NumberOfSteps = numberOfSteps;
        }

        public override void Execute()
        {
            Console.WriteLine("You move forward " + NumberOfSteps + " cells");
            Player p = ActionManager.instance.CurrentPlayer;
            p.ActualCell = Gameboard.instance.Cells[(p.ActualCell.Index + NumberOfSteps) % 40];
            p.ConsecutiveDoubles = 0;
            PrintCell(p.ActualCell);
        }
    }

    public class GoToJailAction : MonopolyAction
    {
        public GoToJailAction() { }

        public override void Execute()
        {
            Console.WriteLine("You are caught by the police and sent to jail");
            Player p = ActionManager.instance.CurrentPlayer;
            p.ActualCell = Gameboard.JailCell;
            p.IsInJail = true;
        }
    }

    public class DoublesToJailAction : MonopolyAction
    {
        public DoublesToJailAction() { }

        public override void Execute()
        {
            Console.WriteLine("You have gotten 3 doubles in a row, therefore you are caught by the police and " +
                "are sent to jail");
            Player p = ActionManager.instance.CurrentPlayer;
            p.ActualCell = Gameboard.JailCell;
            p.IsInJail = true;
        }
    }

    public class GetSalaryAction : MonopolyAction
    {
        public int NumberOfSteps { get; set; }
        public GetSalaryAction() { }

        public override void Execute()
        {
            ActionManager.instance.CurrentPlayer.Cash += Player.SALARY;
            Console.WriteLine("Player " + ActionManager.instance.CurrentPlayer.Name + " has now " + ActionManager.instance.CurrentPlayer.Cash + "$");
        }
    }

    public class RollDiceAction : MonopolyAction
    {
        public RollDiceAction() { }

        public override void Execute()
        {
            Console.WriteLine(ActionManager.instance.CurrentPlayer.Name + "'s turn to roll the dices, press enter to proceed");
            Console.ReadKey();
            int moveBy = Dices.Roll();
            if (ActionManager.instance.CurrentPlayer.ConsecutiveDoubles != 0)
            {
                Console.WriteLine("Double " + moveBy / 2 + " !");
            }
            if (ActionManager.instance.CurrentPlayer.ConsecutiveDoubles != 0 && ActionManager.instance.CurrentPlayer.IsInJail)
            {
                ActionManager.AddAction(new MoveOutOfJail(moveBy));
            }
            else
            {
                ActionManager.AddAction(new MoveAction(moveBy));
            }
        }
    }

    public class GetOutOfJailAction : MonopolyAction
    {
        public GetOutOfJailAction() { }

        public override void Execute()
        {
            ActionManager.instance.CurrentPlayer.IsInJail = false;
            ActionManager.instance.CurrentPlayer.TurnsInJail = 0;
        }
    }

    public class DummyAction : MonopolyAction
    {
        public DummyAction() { }
        public override void Execute()
        {
            return;
        }
    }
}