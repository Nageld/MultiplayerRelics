using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace MultiplayerRelics.MultiplayerRelicsCode.Enchantments;

public sealed class FiredUp : CustomEnchantmentModel
{
    public override bool CanEnchantCardType(CardType cardType) => true;

    public override bool HasExtraCardText => true;

    public override bool ShowAmount => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<VigorPower>(0),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<VigorPower>(),
    ];

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay? cardPlay)
    {
        IEnumerable<Creature> players = from c in Card.Owner.Creature.CombatState?.GetTeammatesOf(Card.Owner.Creature)
            where c is { IsAlive: true, IsPlayer: true }
            select c;

        foreach (var player in players)
        {
            await PowerCmd.Apply<VigorPower>( new ThrowingPlayerChoiceContext(), player, DynamicVars["VigorPower"].IntValue, player, null);
        }
    }
    public override void RecalculateValues()
    {
        DynamicVars["VigorPower"].BaseValue = Amount;
    }
}