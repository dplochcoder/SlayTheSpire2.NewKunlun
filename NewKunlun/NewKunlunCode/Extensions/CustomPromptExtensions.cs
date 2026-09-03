using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace NewKunlun.NewKunlunCode.Extensions;

public static class CustomPromptExtensions
{
    private static LocString CustomPromptString(
        AbstractModel model,
        DynamicVarSet set,
        string sheet,
        char suffix
    )
    {
        LocString str = new(sheet, $"{model.Id.Entry}.customPrompt{suffix}");
        if (!str.Exists())
            throw new InvalidOperationException($"No .customPrompt{suffix} string for {model.Id}.");
        set.AddTo(str);
        return str;
    }

    extension(CardModel self)
    {
        private LocString CustomPromptString(char suffix) =>
            CustomPromptString(self, self.DynamicVars, "cards", suffix);

        public LocString CustomPromptA => self.CustomPromptString('A');
        public LocString CustomPromptB => self.CustomPromptString('B');
        public LocString CustomPromptC => self.CustomPromptString('C');
        public LocString CustomPromptD => self.CustomPromptString('D');
    }

    extension(RelicModel self)
    {
        private LocString CustomPromptString(char suffix) =>
            CustomPromptString(self, self.DynamicVars, "relics", suffix);

        public LocString CustomPromptA => self.CustomPromptString('A');
        public LocString CustomPromptB => self.CustomPromptString('B');
        public LocString CustomPromptC => self.CustomPromptString('C');
        public LocString CustomPromptD => self.CustomPromptString('D');
    }

    extension(PowerModel self)
    {
        private LocString CustomPromptString(char suffix) =>
            CustomPromptString(self, self.DynamicVars, "powers", suffix);

        public LocString CustomPromptA => self.CustomPromptString('A');
        public LocString CustomPromptB => self.CustomPromptString('B');
        public LocString CustomPromptC => self.CustomPromptString('C');
        public LocString CustomPromptD => self.CustomPromptString('D');
    }
}
