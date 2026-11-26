using System;

namespace AIOSICT011.Models
{
    // Interface: enforces simple report formatting for entities
    public interface IReportable
    {
        string GetSummaryText();
    }
}
