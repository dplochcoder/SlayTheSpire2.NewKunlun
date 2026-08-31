using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace NewKunlun.NewKunlunCode.Extensions;

public static class CardModelExtensions
{
    extension(CardModel self)
    {
        public async Task AddGeneratedStatusToPile<T>(PileType pileType = PileType.Discard)
            where T : CardModel
        {
            CardCmd.PreviewCardPileAdd(
                await CardPileCmd.AddGeneratedCardToCombat(
                    self.CombatState!.CreateCard<T>(self.Owner),
                    PileType.Discard,
                    self.Owner
                )
            );
        }

        private LocString CustomPromptString(char suffix)
        {
            LocString str = new("cards", $"{self.Id.Entry}.customPrompt{suffix}");
            if (!str.Exists())
                throw new InvalidOperationException(
                    $"No .customPrompt{suffix} string for {self.Id}."
                );
            self.DynamicVars.AddTo(str);
            return str;
        }

        public LocString CustomPromptA => self.CustomPromptString('A');
        public LocString CustomPromptB => self.CustomPromptString('B');
        public LocString CustomPromptC => self.CustomPromptString('C');
        public LocString CustomPromptD => self.CustomPromptString('D');
    }
}
