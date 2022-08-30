using System.Collections;
using System.Linq;
using Shouldly;
using XOgame.Services.Emoji;
using Xunit;

namespace XOgame.Tests.Services.Emoji;

public class EmojiService_Tests : XOgameTestBase
{
    private readonly IEmojiService _emojiService;
    
    public EmojiService_Tests()
    {
        _emojiService = Resolve<IEmojiService>();
    }

    [Fact]
    public void GetAllGroups()
    {
        // Act
        var groups = _emojiService.GetAllGroups().Result.ToList();
        
        // Assert - проверка
        groups.ShouldNotBeNull();
        groups.Count().ShouldBe(2);
        
        groups[0].Name.ShouldBe("1");
        groups[0].Emojis.ShouldNotBeNull();
        groups[0].Emojis.Count().ShouldBe(1);
        
        groups[1].Name.ShouldBe("2");
        groups[1].Emojis.ShouldNotBeNull();
        groups[1].Emojis.Count().ShouldBe(2);
    }
    
}