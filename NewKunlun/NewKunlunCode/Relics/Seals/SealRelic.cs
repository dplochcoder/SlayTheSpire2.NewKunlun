namespace NewKunlun.NewKunlunCode.Relics.Seals;

public interface ISealRelic { }

public interface ISealRelic<T> : ISealRelic
    where T : NewKunlunRelic, ISealRelic<T> { }
