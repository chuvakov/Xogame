namespace XOgame.Services.Game.Dto;

public class DoStepInput
{
    public int CellNumber { get; set; }
    public string Nickname { get; set; }
    public bool IsSendNotification { get; set; } = true;
}