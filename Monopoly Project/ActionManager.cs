﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monopoly_Project
{
    public class ActionManager
    {
        public static ActionManager instance;
        public Player CurrentPlayer { get; set; }
        public List<MonopolyAction> Actions { get; set; }

        public static void Init()
        {
            instance = new ActionManager();
            instance.Actions = new List<MonopolyAction>();
        }

        internal static void PlayTurn()
        {
            AddAction(new RollDiceAction());

            instance.ResolveActions();
        }

        internal void ResolveActions()
        {
            //RUN ALL ACTIONS UNTIL EMPTY
            while (instance.Actions.Count != 0)
            {
                //EXECUTE THE ACTUAL ACTION
                instance.Actions[0].Execute();

                //MANAGER OF SUCCESSIVE DOUBLE
                if (instance.Actions[0].GetType() == typeof(RollDiceAction))
                {
                    if (instance.CurrentPlayer.ConsecutiveDoubles != 0)
                    {
                        if (instance.CurrentPlayer.ConsecutiveDoubles == 3)
                        {
                            instance.Actions.Clear();
                            AddAction(new DummyAction());
                            AddAction(new GoToJailAction());
                        }
                        else if (instance.CurrentPlayer.IsInJail)
                        {
                            AddAction(new GetOutOfJailAction());
                        }
                        else
                        {
                            AddAction(new RollDiceAction());
                        }
                    }
                }

                //REMOVING THE ACTION WHEN IT WAS MANAGED (IT IS NOT POSSIBLE TO USE A QUEUE AS WE NEED
                //TO KEEP A ROLLDICEACTION AT THE END OF THE LIST TO ENSURE THAT THE PLAYERS PLAYS TWICE,
                //CF THE ADD ACTION METHOD FURTHER DOWN)
                instance.Actions.RemoveAt(0);
            }
        }

        public static void AddAction(MonopolyAction a)
        {
            //if the player got a double, we want the action of rerolling the dices to be kept at the end 
            //of the action list when we are resolving all of the actions.
            //as all actions are not defined at the beginning (we don't know if the player will buy a property upon visiting
            //one, we need to make sure it is taken into account here as we keep the rolldice as the last action that should
            //resolve). This is the reason we can't use a Queue instead of a List (or we could by overriding the Enqueue() 
            //method but it wouldn't make much sense and we'd loose the utility of a Queue)
            if (instance.Actions.Count != 0 && instance.Actions[instance.Actions.Count - 1].GetType() == typeof(MoveAction))
            {
                instance.Actions.Insert(instance.Actions.Count - 1, a);
            }
            else
            {
                instance.Actions.Add(a);
            }
        }
    }
}