using XOgame.Core.Enums;

namespace XOgame.Services.Player.Dto;

public class PlayerDto
{
    public string Nickname { get; set; }
    public FigureType? FigureType { get; set; }
    public bool IsReady { get; set; }
    public Role Role { get; set; }
    public byte[] Avatar { get; set; }
}