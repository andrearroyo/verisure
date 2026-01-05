using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace EvaluadorExcelApp.Services
{
    public class TransactionProcessor
    {
        public List<TransactionRecord> ProcessFile(string content)
        {
            var records = new List<TransactionRecord>();
            var lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                var parts = line.Split('\t');
                if (parts.Length < 12) continue;

                DateTime fRegistro = ParseDateManual(parts[0]);
                DateTime fOperacion = ParseDateManual(parts[1]);

                if (fRegistro == DateTime.MinValue || fOperacion == DateTime.MinValue) continue;

                records.Add(new TransactionRecord
                {
                    FechaRegistro = fRegistro,
                    Fecha = fOperacion,
                    Descripcion = parts[2].Trim(),
                    CodigoBoleta = parts[3].Trim(),
                    MontoPago = ParseDecimal(parts[5]),
                    MontoPago2 = ParseDecimal(parts[7]),
                    CodigoFactura = parts[11].Trim(),
                    RawParts = parts
                });
            }

            return records.OrderBy(r => r.Fecha).ToList();
        }

        private DateTime ParseDateManual(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return DateTime.MinValue;
            try
            {
                string clean = value.Trim().Replace("-", "/");
                string[] parts = clean.Split('/');
                if (parts.Length != 3) return DateTime.MinValue;

                int dia = int.Parse(parts[0]);
                int mes = int.Parse(parts[1]);
                int año = int.Parse(parts[2]);

                if (año < 100) año += 2000;

                return new DateTime(año, mes, dia);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        private decimal ParseDecimal(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return 0;
            string cleanValue = value.Replace(",", ".").Trim();
            if (decimal.TryParse(cleanValue, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result))
            {
                return result;
            }
            return 0;
        }

        public void ApplyNetting(List<TransactionRecord> allRecords, out List<TransactionRecord> reportarRecords)
        {
            reportarRecords = new List<TransactionRecord>();
            foreach (var r in allRecords) r.IsDeleted = false;

            // Sort by Date
            allRecords.Sort((a, b) => a.Fecha.CompareTo(b.Fecha));

            for (int i = 0; i < allRecords.Count; i++)
            {
                if (allRecords[i].IsDeleted) continue;
                var neg = allRecords[i];

                // Rule 1: VISA vs DEVTRANS
                if (neg.MontoPago2 < 0)
                {
                    var pos = allRecords.FirstOrDefault(r => 
                        !r.IsDeleted && 
                        Math.Abs(r.MontoPago2 + neg.MontoPago2) < 0.01m && 
                        r.CodigoFactura == neg.CodigoFactura);

                    if (pos != null)
                    {
                        bool matches = true;
                        if (string.Equals(neg.Descripcion, "DEVTRANS", StringComparison.OrdinalIgnoreCase) && 
                            string.Equals(pos.Descripcion, "VISA", StringComparison.OrdinalIgnoreCase))
                        {
                            int diffDays = Math.Abs((neg.FechaRegistro - pos.FechaRegistro).Days);
                            if (diffDays < 15)
                            {
                                neg.DiffDays = diffDays;
                                pos.DiffDays = diffDays;
                                reportarRecords.Add(neg);
                                reportarRecords.Add(pos);
                            }
                            else matches = false;
                        }
                        if (matches) { neg.IsDeleted = true; pos.IsDeleted = true; continue; }
                    }
                }

                // Rule 2: LIMPI-SA
                if (string.Equals(neg.Descripcion, "LIMPI-SA", StringComparison.OrdinalIgnoreCase) && neg.MontoPago2 < 0)
                {
                    var pos = allRecords.FirstOrDefault(r => 
                        !r.IsDeleted && 
                        string.Equals(r.Descripcion, "LIMPI-SA", StringComparison.OrdinalIgnoreCase) &&
                        Math.Abs(r.MontoPago2 + neg.MontoPago2) < 0.01m && 
                        r.Fecha.Date == neg.Fecha.Date);

                    if (pos != null) { neg.IsDeleted = true; pos.IsDeleted = true; continue; }
                }

                // Rule 3: CREDIT vs INVOICE
                if (string.Equals(neg.Descripcion, "CREDIT", StringComparison.OrdinalIgnoreCase) && neg.MontoPago < 0)
                {
                    string negId = NormalizeInvoiceId(neg.CodigoFactura);
                    var pos = allRecords.FirstOrDefault(r => 
                        !r.IsDeleted && 
                        string.Equals(r.Descripcion, "INVOICE", StringComparison.OrdinalIgnoreCase) &&
                        Math.Abs(r.MontoPago + neg.MontoPago) < 0.01m &&
                        NormalizeInvoiceId(r.CodigoFactura) == negId);

                    if (pos == null)
                    {
                         pos = allRecords.FirstOrDefault(r => 
                            !r.IsDeleted && 
                            string.Equals(r.Descripcion, "INVOICE", StringComparison.OrdinalIgnoreCase) &&
                            Math.Abs(r.MontoPago + neg.MontoPago) < 0.01m);
                    }

                    if (pos != null) { neg.IsDeleted = true; pos.IsDeleted = true; continue; }
                }
            }
        }

        private string NormalizeInvoiceId(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return "";
            string digits = new string(id.Where(char.IsDigit).ToArray());
            if (digits.Length > 6) return digits.Substring(digits.Length - 6);
            return digits;
        }
    }
}
