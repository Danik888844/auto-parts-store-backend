namespace AutoParts.DataAccess.Models.Enums;

/// <summary>
/// Как учитывать возвраты в отчёте «Продажи за период».
/// </summary>
public enum ReturnsMode
{
    /// <summary>Только завершённые продажи (без возвратов).</summary>
    Exclude = 0,

    /// <summary>Учитывать возвраты в общей сумме (продажи минус возвраты).</summary>
    Include = 1,

    /// <summary>Показывать продажи и возвраты отдельно в отчёте.</summary>
    Separate = 2
}
