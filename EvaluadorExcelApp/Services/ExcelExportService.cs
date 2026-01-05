using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClosedXML.Excel;
using EvaluadorExcelApp.Models;

namespace EvaluadorExcelApp.Services
{
    public class ExcelExportService
    {
        public byte[] GenerateExcel(List<TransactionRecord> original, List<TransactionRecord> depurado)
        {
            using (var workbook = new XLWorkbook())
            {
                // Sheet 1: Original
                var wsOriginal = workbook.Worksheets.Add("Original");
                FillOriginal(wsOriginal, original);

                // Sheet 2: Depurado
                var wsDepurado = workbook.Worksheets.Add("Depurado");
                FillStandard(wsDepurado, depurado.OrderBy(r => r.Fecha).ToList());

                // Sheet 3: Consolidado
                var wsConsolidado = workbook.Worksheets.Add("Consolidado");
                FillConsolidation(wsConsolidado, depurado.OrderBy(r => r.Fecha).ToList());

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

        private void FillOriginal(IXLWorksheet ws, List<TransactionRecord> records)
        {
            if (records == null || !records.Any()) return;
            int row = 1;
            foreach (var r in records)
            {
                if (r.RawParts == null) continue;
                for (int i = 0; i < r.RawParts.Length; i++)
                {
                    ws.Cell(row, i + 1).Value = r.RawParts[i];
                }
                row++;
            }
            ws.Columns().AdjustToContents();
        }

        private void FillStandard(IXLWorksheet ws, List<TransactionRecord> records)
        {
            string[] headers = { "Fecha (B)", "Descripción (C)", "Código Boleta (D)", "Monto Cobro (F)", "Monto Pago Cliente (H)", "Factura (L)" };
            for (int i = 0; i < headers.Length; i++) ws.Cell(1, i + 1).Value = headers[i];

            int row = 2;
            foreach (var r in records)
            {
                ws.Cell(row, 1).Value = r.Fecha.Date;
                ws.Cell(row, 2).Value = r.Descripcion;
                ws.Cell(row, 3).Value = r.CodigoBoleta;
                ws.Cell(row, 4).Value = r.MontoPago;
                ws.Cell(row, 5).Value = r.MontoPago2;
                ws.Cell(row, 6).Value = r.CodigoFactura;
                row++;
            }
            ws.Columns().AdjustToContents();
        }

        private void FillConsolidation(IXLWorksheet ws, List<TransactionRecord> records)
        {
            ws.Cell(1, 1).Value = "Fecha (B)";
            ws.Cell(1, 2).Value = "Sumatoria H";

            var consolidado = records
                .GroupBy(r => r.Fecha.Date)
                .Select(g => new { Fecha = g.Key, TotalH = g.Sum(x => x.MontoPago2) })
                .Where(x => x.TotalH > 0)
                .OrderBy(x => x.Fecha)
                .ToList();

            int row = 2;
            foreach (var item in consolidado)
            {
                ws.Cell(row, 1).Value = item.Fecha;
                ws.Cell(row, 2).Value = item.TotalH;
                row++;
            }
            ws.Columns().AdjustToContents();
        }
    }
}
