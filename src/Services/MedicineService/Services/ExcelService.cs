using OfficeOpenXml;
using SharedKernel.Models;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace MedicineService.Services;

public class ExcelService
{
    private readonly ILogger<ExcelService> _logger;
    private readonly HttpClient _httpClient;
    private const string TITCK_URL = "https://www.titck.gov.tr/dinamikmodul/43";
    private static readonly Random _random = new();

    public ExcelService(ILogger<ExcelService> logger, HttpClient httpClient)
    {
        _logger = logger;
        _httpClient = httpClient;
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }

    public async Task<List<Medicine>> GetLatestMedicineDataAsync()
    {
        var excelUrl = await GetLatestExcelUrlAsync();
        if (string.IsNullOrEmpty(excelUrl))
        {
            throw new Exception("Could not find latest Excel file URL");
        }

        var stream = await _httpClient.GetStreamAsync(excelUrl);
        return await ParseExcelFileAsync(stream);
    }

    private async Task<string?> GetLatestExcelUrlAsync()
    {
        var html = await _httpClient.GetStringAsync(TITCK_URL);
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        // Find the first (latest) Excel file link
        var link = doc.DocumentNode
            .SelectNodes("//table//a[@href]")
            .FirstOrDefault(x => x.GetAttributeValue("href", "").EndsWith(".xlsx"));

        return link?.GetAttributeValue("href", null);
    }

    private async Task<List<Medicine>> ParseExcelFileAsync(Stream stream)
    {
        var medicines = new List<Medicine>();

        using var package = new ExcelPackage(stream);
        var worksheet = package.Workbook.Worksheets[0]; // First worksheet

        var rowCount = worksheet.Dimension.Rows;
        var colCount = worksheet.Dimension.Columns;

        // Skip header
        for (int row = 4; row <= rowCount; row++)
        {
            try
            {
                var name = worksheet.Cells[row, 1].Text;
                if (string.IsNullOrWhiteSpace(name)) continue;

                var medicine = new Medicine
                {
                    Name = name,
                    Barcode = worksheet.Cells[row, 2].Text,
                    Price = GenerateRandomPrice(),
                    IsActive = true,
                    LastUpdated = DateTime.UtcNow
                };

                medicines.Add(medicine);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing row {Row}", row);
            }
        }

        return medicines;
    }

    private static decimal GenerateRandomPrice()
    {
        return _random.Next(25, 100);
    }
} 