namespace XOgame.Services.Room.Dto;

public class RoomDto
{
    public string Name { get; set; }
    public int AmountUsers { get; set; }
    public int MaxAmountUsers { get; set; }
    public bool IsHavePassword { get; set; }
}