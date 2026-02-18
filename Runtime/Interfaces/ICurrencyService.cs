using System;

namespace ProtoCasual.Core.Interfaces
{
    /// <summary>
    /// Dual-currency (soft + hard) wallet.
    /// The only class allowed to mutate currency values.
    /// </summary>
    public interface ICurrencyService
    {
        event Action OnCurrencyChanged;

        int SoftCurrency { get; }
        int HardCurrency { get; }

        void AddSoft(int amount);
        bool SpendSoft(int amount);
        bool HasSoft(int amount);

        void AddHard(int amount);
        bool SpendHard(int amount);
        bool HasHard(int amount);
    }
}
