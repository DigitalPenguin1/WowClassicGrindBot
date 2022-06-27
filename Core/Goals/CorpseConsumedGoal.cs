﻿using System;
using Core.GOAP;
using Microsoft.Extensions.Logging;

namespace Core.Goals
{
    public partial class CorpseConsumedGoal : GoapGoal
    {
        public override float CostOfPerformingAction => 4.7f;

        private readonly ILogger logger;
        private readonly GoapAgentState goapAgentState;
        private readonly Wait wait;

        public CorpseConsumedGoal(ILogger logger, ClassConfiguration classConfig, GoapAgentState goapAgentState, Wait wait)
            : base(nameof(CorpseConsumedGoal))
        {
            this.logger = logger;
            this.goapAgentState = goapAgentState;
            this.wait = wait;

            if (classConfig.Mode == Mode.AssistFocus)
            {
                AddPrecondition(GoapKey.incombat, false);
            }
            else
            {
                AddPrecondition(GoapKey.dangercombat, false);
            }

            AddPrecondition(GoapKey.consumecorpse, true);

            AddEffect(GoapKey.consumecorpse, false);
        }

        public override void OnEnter()
        {
            goapAgentState.LastCombatKillCount = Math.Max(goapAgentState.LastCombatKillCount - 1, 0);
            LogConsumed(logger, goapAgentState.LastCombatKillCount);

            SendActionEvent(new ActionEventArgs(GoapKey.consumecorpse, false));

            // Issue
            // After combat having multiple Corpse around the player
            // NPCNameFinder picks the same corpse after each round
            // Solve: Wait unknown amount of time
            if (goapAgentState.LastCombatKillCount > 0)
            {
                wait.Update();
                wait.Update();
                wait.Update();
                wait.Update();
            }
        }

        public override void PerformAction()
        {
        }


        [LoggerMessage(
            EventId = 101,
            Level = LogLevel.Information,
            Message = "----- Corpse consumed. Remaining: {remains}")]
        static partial void LogConsumed(ILogger logger, int remains);
    }
}