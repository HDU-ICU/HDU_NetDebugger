using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using HDU_NetDebugger.ViewModels;

namespace HDU_NetDebugger.Utils;

public class ResultExportUtils
{

    public static FilePickerFileType MarkdownFIle { get; } = new("Markdown File")
    {
        Patterns = ["*.md"],
        AppleUniformTypeIdentifiers = ["public.markdown"],
        MimeTypes = ["text/x-markdown", "text/plain"]
    };
    public static async Task ExportAsync(ObservableCollection<CheckItemViewModel> checkItems, DateTime checkTime)
    {
        if (checkItems is null or { Count: 0 }) return;

        var options = new FilePickerSaveOptions
        {
            Title = "检查结果导出",
            SuggestedFileName = "ResultExport.md",
            FileTypeChoices =
            [
                MarkdownFIle
            ],
        };
        var mainWindowTopLevel = Avalonia.Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop
            ? desktop.MainWindow
            : null;
        var file = await (mainWindowTopLevel?.StorageProvider?.SaveFilePickerAsync(options)
            ?? Task.FromResult<IStorageFile?>(null));
        if (file is not null)
        {
            try
            {
                // all in 流
                await using var stream = await file.OpenWriteAsync();
                await using var writer = new StreamWriter(stream);
                await writer.WriteAsync(ReportBuilder(checkItems, checkTime) ?? string.Empty);
            }
            catch (Exception ex)
            {
                // 最好有错误处理，例如弹出一个错误提示框
                Console.WriteLine($"保存文件失败: {ex.Message}");
            }
        }
    }
    public static string ReportBuilder(ObservableCollection<CheckItemViewModel> checkItems, DateTime checkTime)
    {
        StringBuilder sb = new();
        sb.AppendLine("# HDU Net Debugger 检查结果报告");
        sb.AppendLine($"生成时间: {DateTime.Now}");
        sb.AppendLine($"检查时间: {checkTime}");
        sb.AppendLine();
        foreach (var item in checkItems)
        {
            sb.AppendLine($"## {item.Name}");
            sb.AppendLine();
            sb.AppendLine($"- 状态: {item.Status}");
            sb.AppendLine($"- 总览: {item.CheckSummary}");
            sb.AppendLine($"- 建议:");
            for (int i = 0; i < item.Suggestions.Count; i++)
            {
                sb.AppendLine($"  - {item.Suggestions[i]}");
            }
            sb.AppendLine($"- 警告:");
            for (int i = 0; i < item.Warnings.Count; i++)
            {
                sb.AppendLine($"  - {item.Warnings[i]}");
            }
            sb.AppendLine($"- 详情:");
            sb.AppendLine();
            sb.AppendLine("```text");
            sb.AppendLine(item.CheckDetails);
            sb.AppendLine("```");
            sb.AppendLine();
        }
        return sb.ToString();
    }
}
