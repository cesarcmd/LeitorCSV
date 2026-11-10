using LumenWorks.Framework.IO.Csv;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Data;
using System.Text;

namespace LeitorCSV.Controllers;

public class HomeController : Controller
{
    private const long MaxFileSizeBytes = 2 * 1024 * 1024;
    private const int MaxColumns = 50;
    private const int MaxRows = 5_000;
    private const int MaxCellLength = 10_000;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Upload()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(MaxFileSizeBytes)]
    [EnableRateLimiting("upload")]
    public ActionResult Upload(IFormFile? upload)
    {
        if (upload is null || upload.Length == 0)
        {
            ModelState.AddModelError("Arquivo", "Selecione um arquivo CSV.");
            return View();
        }

        if (upload.Length > MaxFileSizeBytes)
        {
            ModelState.AddModelError("Arquivo", "O arquivo deve ter no máximo 2 MB.");
            return View();
        }

        if (!string.Equals(Path.GetExtension(upload.FileName), ".csv", StringComparison.OrdinalIgnoreCase))
        {
            ModelState.AddModelError("Arquivo", "Envie um arquivo com extensão .csv.");
            return View();
        }

        try
        {
            using var stream = upload.OpenReadStream();
            using var textReader = new StreamReader(stream, new UTF8Encoding(false, true), detectEncodingFromByteOrderMarks: true);
            using var csvReader = new CsvReader(textReader, hasHeaders: true);

            if (csvReader.FieldCount is < 1 or > MaxColumns)
            {
                ModelState.AddModelError("Arquivo", $"O CSV deve conter entre 1 e {MaxColumns} colunas.");
                return View();
            }

            var columnNames = csvReader.GetFieldHeaders();

            if (columnNames.Any(string.IsNullOrWhiteSpace) ||
                columnNames.Distinct(StringComparer.OrdinalIgnoreCase).Count() != columnNames.Length)
            {
                ModelState.AddModelError("Arquivo", "O CSV deve ter cabeçalhos preenchidos e sem duplicidades.");
                return View();
            }

            var csvTable = new DataTable();
            foreach (var columnName in columnNames)
            {
                csvTable.Columns.Add(columnName);
            }

            var rowCount = 0;
            while (csvReader.ReadNextRecord())
            {
                if (++rowCount > MaxRows)
                {
                    ModelState.AddModelError("Arquivo", $"O CSV pode conter no máximo {MaxRows:N0} linhas de dados.");
                    return View();
                }

                var row = csvTable.NewRow();
                for (var columnIndex = 0; columnIndex < csvReader.FieldCount; columnIndex++)
                {
                    var value = csvReader[columnIndex] ?? string.Empty;
                    if (value.Length > MaxCellLength)
                    {
                        ModelState.AddModelError("Arquivo", "Cada campo do CSV pode ter no máximo 10.000 caracteres.");
                        return View();
                    }

                    row[columnIndex] = value;
                }

                csvTable.Rows.Add(row);
            }

            return View(csvTable);
        }
        catch (Exception exception) when (exception is MalformedCsvException or MissingFieldCsvException or DuplicateHeaderException or DecoderFallbackException or ArgumentException)
        {
            _logger.LogWarning(exception, "Foi rejeitado um CSV inválido.");
            ModelState.AddModelError("Arquivo", "Não foi possível ler o CSV. Use UTF-8, cabeçalhos válidos e valores separados por vírgula.");
            return View();
        }
    }
}
