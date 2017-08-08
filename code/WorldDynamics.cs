﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.RL.Stochastic
{
    /* World Dynamics defines transmission probability and generated reward signal
     * from one state to another based on a given action */
    public class WorldDynamics
    {
        private double[,,] _transProb; // Transition Probability from state to state given action
        private double[,,] _rewardMean; // Mean value of a reward given transission and action
        private readonly double _noise = 0.1;
        public WorldDynamics(int nStates, int nActions)
        {
            // Initialize Transmission probability and rewards means given state space size and action space size
            _transProb = new double[nStates, nStates, nActions]; // i = s', j = s, k = a
            ConstructTrans();
            _rewardMean = new double[nStates, nStates, nActions]; // i = s', j = s, k = a
            ConstructRewards(10);
        }

        public void ConstructTrans()
        {
            Console.WriteLine("Constructing Transition Probability");
            // Read a file to construct transition probability
            // For now build using random distribution
            Random rnd = new Random();
            for (int k = 0; k< _transProb.GetLength(2);k++)
            {
                for(int j = 0; j< _transProb.GetLength(1);j++)
                {
                    double runningTotal = 0;
                    for (int i = 0; i< _transProb.GetLength(0);i++)
                    {
                        double p = rnd.NextDouble()/_transProb.GetLength(0);
                        if (runningTotal + p < 1)
                        {
                            _transProb[i, j, k] = p;
                        }
                        else
                        {
                            _transProb[i, j, k] = 1-runningTotal;
                        }
                        runningTotal += _transProb[i, j, k];
                    }
                }
            }
        }

        public void ConstructRewards(double Factor)
        {
            // Read a file to construct Reward means
            // For now build using random distribution
            Random rnd = new Random();
            for (int i = 0; i < _rewardMean.GetLength(0); i++)
            {
                for (int j = 0; j < _rewardMean.GetLength(1); j++)
                {

                    for (int k = 0; k < _rewardMean.GetLength(2); k++)
                    {
                        double p = Factor*(rnd.NextDouble()-0.5);

                        _rewardMean[i, j, k] = p;

                    }
                }
            }
        }
        public Event Interact(int action, Agent actor, Environment env)
        {
            State s1 = env.CurrentState;

            State s2 = env.GetState(GetNextState(s1.ID, action));
            Random rnd = new Random();
            // Later use CalculateReward()
            double reward = CalculateReward(s1,s2, env.GetAction(action));

            Event sig = new Event(s2, s1, reward, env.GetAction(action), actor);

            return sig;
        }

        private double CalculateReward(State oldState, State newState, Action action)
        {
            Random rnd = new Random();
            // Later use CalculateReward()
            return (rnd.NextDouble()/2-1) *_noise + _rewardMean[oldState.ID, newState.ID, action.ID];
           
        } 
        public Event Interact(Action action, Agent actor, Environment env)
        {
            return Interact(action.ID, actor, env);
        }

        public int GetNextState(int current,int action)
        {
          // I might need to rename this method to something more meaningful
            // given current state and action, and using _transProb, find next state ID
            Random rnd = new Random();
            // Roll the dice
            double chance = rnd.NextDouble();

            // Check the odds
            for (int i = 0; i < _transProb.GetLength(0);i++ )
            {
                if(chance < _transProb[i,current,action])
                {
                    return i;
                }
                chance -= _transProb[i, current, action];
            }

                return 0;
        }
    }
}
