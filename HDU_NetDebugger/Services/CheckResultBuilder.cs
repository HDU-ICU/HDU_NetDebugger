using System.Collections.Generic;
using HDU_NetDebugger.Models;

namespace HDU_NetDebugger.Services;

public class CheckResultBuilder
{
    private CheckResultStatus _status = CheckResultStatus.Pass;
    private string _summary = string.Empty;
    private string? _details;
    private readonly List<string> _warnings = [];
    private readonly List<string> _suggestions = [];

    public CheckResultBuilder Pass(string summary, string? details = null)
    {
        ChangeStatus(CheckResultStatus.Pass);
        _summary = summary;
        _details = details;
        return this;
    }

    public CheckResultBuilder Fail(string summary, string? details = null)
    {
        ChangeStatus(CheckResultStatus.Fail);
        _summary = summary;
        _details = details;
        return this;
    }

    public CheckResultBuilder Warn(string summary, string? details = null)
    {
        ChangeStatus(CheckResultStatus.Warn);
        _summary = summary;
        _details = details;
        return this;
    }

    private void ChangeStatus(CheckResultStatus newStatus)
    {
        if (_status < newStatus)
            _status = newStatus;
    }

    public CheckResultBuilder AddWarning(string message)
    {
        if (!string.IsNullOrWhiteSpace(message) && !_warnings.Contains(message))
        {
            _warnings.Add(message);
        }
        if (_status == CheckResultStatus.Pass)
            _status = CheckResultStatus.Warn;
        return this;
    }

    public CheckResultBuilder AddSuggestion(string suggestion)
    {
        if (!string.IsNullOrWhiteSpace(suggestion) && !_suggestions.Contains(suggestion))
        {
            _suggestions.Add(suggestion);
        }
        return this;
    }

    public CheckResult Build()
    {
        return new CheckResult
        {
            Status = _status,
            Summary = _summary,
            Details = _details,
            Warnings = _warnings,
            Suggestions = _suggestions
        };
    }

    public void Reset()
    {
        _status = CheckResultStatus.Pass;
        _summary = string.Empty;
        _details = null;
        _warnings.Clear();
        _suggestions.Clear();
    }
}