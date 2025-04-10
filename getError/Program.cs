using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

class Program
{
    [DllImport("ntdll.dll")]
    private static extern uint RtlNtStatusToDosError(uint Status);

    [DllImport("kernel32.dll")]
    private static extern uint FormatMessage(uint flags, IntPtr source, uint messageId, uint languageId,
        StringBuilder buffer, uint size, IntPtr arguments);

    private const uint FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000;

    static void Main(string[] args)
    {
        if (args.Length < 2 || args.Length > 3)
        {
            Console.WriteLine(GenerateJsonResponse(0, null, "getError.exe <error_code> <category> [language]"));
            return;
        }

        string errorCodeInput = args[0];
        string category = args[1].ToUpper();
        string language = args.Length == 3 ? args[2].ToLower() : null;
        uint errorCode;

        if (!TryParseErrorCode(errorCodeInput, out errorCode))
        {
            Console.WriteLine(GenerateJsonResponse(0, null, "Not a valid error code format."));
            return;
        }

        uint languageId = GetLanguageId(language);
        string description = GetErrorMessage(errorCode, category, languageId);

        if (description == null || description == "Error description not found." || description.Contains("Unknown Error"))
        {
            Console.WriteLine(GenerateJsonResponse(0, null, description ?? "Unknown error code or unknown category."));
        }
        else
        {
            var main = new
            {
                category,
                code = $"0x{errorCode:X8}",
                msg = description
            };
            Console.WriteLine(GenerateJsonResponse(1, main, null));
        }
    }

    static string GenerateJsonResponse(int status, object main, string ex)
    {
        var response = new
        {
            status,
            main,
            ex
        };

        var options = new JsonSerializerOptions
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping, // 禁用 Unicode 转义
            WriteIndented = false
        };

        return JsonSerializer.Serialize(response, options);
    }

    static bool TryParseErrorCode(string input, out uint errorCode)
    {
        if (input.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
        {
            return uint.TryParse(input.Substring(2), System.Globalization.NumberStyles.HexNumber, null, out errorCode);
        }

        return uint.TryParse(input, out errorCode);
    }

    static string GetErrorMessage(uint code, string category, uint languageId)
    {
        string description = category switch
        {
            "NTSTATUS" => GetNtStatusMessage(code, languageId),
            "WIN32" => GetWin32Message(code, languageId),
            "HRESULT" => GetHResultMessage(code, languageId),
            _ => null
        };

        return description;
    }

    static string GetNtStatusMessage(uint code, uint languageId)
    {
        uint win32Code = RtlNtStatusToDosError(code);
        return GetWin32Message(win32Code, languageId);
    }

    static string GetWin32Message(uint code, uint languageId)
    {
        var buffer = new StringBuilder(256);
        uint result = FormatMessage(FORMAT_MESSAGE_FROM_SYSTEM, IntPtr.Zero, code, languageId, buffer, (uint)buffer.Capacity, IntPtr.Zero);
        return result > 0 ? buffer.ToString().Trim() : "Error description not found.";
    }

    static string GetHResultMessage(uint code, uint languageId)
    {
        return GetWin32Message((uint)(code & 0xFFFF), languageId);
    }

    static uint GetLanguageId(string language)
    {
        if (string.IsNullOrEmpty(language))
        {
            // 动态获取系统当前语言 ID
            return (uint)CultureInfo.CurrentCulture.LCID;
        }

        try
        {
            // 根据语言代码获取对应的 LCID
            var culture = new CultureInfo(language.Replace('_', '-'));
            return (uint)culture.LCID;
        }
        catch (CultureNotFoundException)
        {
            return 0;
        }
    }
}
