using WorshipPad.Core.Models;

namespace WorshipPad.Core.Interfaces;

public interface IBankService
{
    IReadOnlyList<PadBank> GetBanks();
}