using XOgame.Core;
using XOgame.Core.Models;

namespace XOgame.Tests.Common.Seeds;

public class EmojisCreator : CreatorBase
{
    public EmojisCreator(XOgameContext context) : base(context)
    {
    }

    public override void Create()
    {
        CreateEmojiGroups();
        CreateEmojis();
    }

    private void CreateEmojiGroups()
    {
        _context.EmojiGroups.AddRange
        (
            new EmojiGroup()
            {
                Id = 1,
                Name = "1",
            },
            new EmojiGroup()
            {
                Id = 2,
                Name = "2",
            }
        );
    }

    private void CreateEmojis()
    {
        _context.Emojis.AddRange
        (
            new Emoji()
            {
                Id = 1,
                EmojiGroupId = 1,
                Name = "1",
                Text = "1",
            }, 
            new Emoji()
            {
                Id = 2,
                EmojiGroupId = 2,
                Name = "2",
                Text = "2",
            },
            new Emoji()
            {
                Id = 3,
                EmojiGroupId = 2,
                Name = "3",
                Text = "3",
            }
        );
    }
}