using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;

namespace EvaluadorExcel
{
    public partial class Form1 : Form
    {
        //pruebas
        private List<TransactionRecord> originalRecords = new List<TransactionRecord>();
        private List<TransactionRecord> allRecords = new List<TransactionRecord>();
        private List<TransactionRecord> depuradoRecords = new List<TransactionRecord>();
        private List<TransactionRecord> sinInvoiceRecords = new List<TransactionRecord>();
        private List<TransactionRecord> reportarRecords = new List<TransactionRecord>();

        public Form1()
        {
            InitializeComponent();
        }

        private void btnCargar_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Archivos de texto (*.txt)|*.txt|Todos los archivos (*.*)|*.*";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    LoadFile(ofd.FileName);
                }
            }
        }

        DateTime convertirFecha(string fechaStr)
        {
            // Intentar convertir la fecha en formato dd/MM/yyyy
            DateTime fecha;

            string[] strings = fechaStr.Split('/');
            fecha = new DateTime(2000 + Int32.Parse(strings[2]), Int32.Parse(strings[1]), Int32.Parse(strings[0]));
            return fecha;
        }


        private void LoadFile(string filePath)
        {
            try
            {
                // 1. Carga completa sin filtros iniciales para garantizar los 152 registros
                string[] lines = System.IO.File.ReadAllLines(filePath, Encoding.Default);
                
                originalRecords = new List<TransactionRecord>();

                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    string[] parts = line.Split('\t');
                    if (parts.Length < 12) continue;

                    DateTime fRegistro = convertirFecha(parts[0]).Date;
                    DateTime fOperacion = convertirFecha(parts[1]).Date;

                    // Si alguna fecha es MinValue, probablemente es el encabezado
                    if (fRegistro == DateTime.MinValue || fOperacion == DateTime.MinValue) continue;

                    originalRecords.Add(new TransactionRecord
                    {
                        FechaRegistro = fRegistro,
                        Fecha = fOperacion,
                        Descripcion = parts[2].Trim(),
                        CodigoBoleta = parts[3].Trim(),
                        MontoPago = ParseDecimal(parts[5]),  // Columna F (Indice 5)
                        MontoPago2 = ParseDecimal(parts[7]), // Columna H (Indice 7)
                        CodigoFactura = parts[11].Trim(),   // Columna L (Indice 11)
                        RawParts = parts                     // Guardamos todo el registro sin formato
                    });
                }

                // --- NUEVO: Ordenamiento Cronológico Obligatorio ---
                //originalRecords = originalRecords.OrderBy(r => r.Fecha).ToList();

                // allRecords es nuestra copia de trabajo
                ReiniciarListasTrabajo();

                ActualizarGrid(allRecords);
                CalcularControlLabel(); // Cálculo inicial
                MessageBox.Show($"Archivo cargado con éxito. Total registros capturados: {allRecords.Count}");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar archivo: " + ex.Message);
            }
        }

        private void ReiniciarListasTrabajo()
        {
            allRecords = originalRecords.Select(r => new TransactionRecord 
            { 
                FechaRegistro = r.FechaRegistro,
                Fecha = r.Fecha,
                Descripcion = r.Descripcion,
                CodigoBoleta = r.CodigoBoleta,
                MontoPago = r.MontoPago,
                MontoPago2 = r.MontoPago2,
                CodigoFactura = r.CodigoFactura,
                RawParts = r.RawParts
            }).ToList();
            
            reportarRecords = new List<TransactionRecord>();
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            originalRecords.Clear();
            allRecords.Clear();
            depuradoRecords.Clear();
            sinInvoiceRecords.Clear();
            reportarRecords.Clear();
            
            dgvDatos.DataSource = null;
            lblSumCobro.Text = "Cobro (F): 0.00";
            lblSumPagoCliente.Text = "Pago Cliente (H): 0.00";
            lblConsolidado.Text = "Consolidado: 0.00";
            lblResultado.Text = "Resultado Control: 0.00";
            
            MessageBox.Show("Datos limpiados.");
        }

        private string NormalizeInvoiceId(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return "";
            // Extraemos solo los dígitos del final o los más significativos (ej: 002016)
            // Para ser más tolerantes, comparamos solo los últimos 6 dígitos si son largos
            string digits = new string(id.Where(char.IsDigit).ToArray());
            if (digits.Length > 6) return digits.Substring(digits.Length - 6);
            return digits;
        }

        private DateTime ParseDateManual(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return DateTime.MinValue;
            try
            {
                // Limpieza de posibles caracteres extraños
                string clean = value.Trim().Replace("-", "/");
                string[] parts = clean.Split('/');
                if (parts.Length != 3) return DateTime.MinValue;

                int dia = int.Parse(parts[0]);
                int mes = int.Parse(parts[1]);
                int año = int.Parse(parts[2]);

                // Manejo de año de 2 dígitos
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
            // Limpieza: Reemplazar comas por puntos y usar InvariantCulture
            string cleanValue = value.Replace(",", ".").Trim();
            if (decimal.TryParse(cleanValue, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result))
            {
                return result;
            }
            return 0;
        }

        private void ActualizarGrid(IEnumerable<TransactionRecord> records)
        {
            dgvDatos.DataSource = null;
            dgvDatos.DataSource = records.Where(r => !r.IsDeleted).ToList();
        }

        private void CalcularControlLabel()
        {
            var activeRecords = allRecords.Where(r => !r.IsDeleted).ToList();
            decimal sumCobro = activeRecords.Sum(r => r.MontoPago);
            decimal sumPagoCliente = activeRecords.Sum(r => r.MontoPago2);
            
            lblSumCobro.Text = $"Cobro (F): {sumCobro.ToString("N2", CultureInfo.InvariantCulture)}";
            lblSumPagoCliente.Text = $"Pago Cliente (H): {sumPagoCliente.ToString("N2", CultureInfo.InvariantCulture)}";
            lblResultado.Text = $"Resultado Control (Cobro - Pago Cliente): {(sumCobro - sumPagoCliente).ToString("N2", CultureInfo.InvariantCulture)}";
        }

        private void btnDepurar_Click(object sender, EventArgs e)
        {
            Netting();
        }

        private void Netting()
        {
            try
            {
                // 1. Resetear estados (para permitir re-procesar si se carga el mismo archivo)
                foreach (var r in allRecords) r.IsDeleted = false;

                // 2. Orden previo y limpieza de listas (Orden cronológico obligatorio)
                allRecords = allRecords.OrderBy(r => r.Fecha).ToList();
                reportarRecords = new List<TransactionRecord>();

                // 3. Proceso de Netting (Eliminación de Homólogos)
                for (int i = 0; i < allRecords.Count; i++)
                {
                    if (allRecords[i].IsDeleted) continue;
                    var neg = allRecords[i];

                    // --- REGLA 1: VISA vs DEVTRANS (15 días, Columna A) ---
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

                    // --- REGLA 2: LIMPI-SA (Columna H, Match Monto y Fecha, Ignora L) ---
                    if (string.Equals(neg.Descripcion, "LIMPI-SA", StringComparison.OrdinalIgnoreCase) && neg.MontoPago2 < 0)
                    {
                        var pos = allRecords.FirstOrDefault(r => 
                            !r.IsDeleted && 
                            string.Equals(r.Descripcion, "LIMPI-SA", StringComparison.OrdinalIgnoreCase) &&
                            Math.Abs(r.MontoPago2 + neg.MontoPago2) < 0.01m && 
                            r.Fecha.Date == neg.Fecha.Date);

                        if (pos != null) { neg.IsDeleted = true; pos.IsDeleted = true; continue; }
                    }

                    // --- REGLA 3: CREDIT vs INVOICE (Columna F, Match L normalizado y Monto) ---
                    if (string.Equals(neg.Descripcion, "CREDIT", StringComparison.OrdinalIgnoreCase) && neg.MontoPago < 0)
                    {
                        string negId = NormalizeInvoiceId(neg.CodigoFactura);
                        var pos = allRecords.FirstOrDefault(r => 
                            !r.IsDeleted && 
                            string.Equals(r.Descripcion, "INVOICE", StringComparison.OrdinalIgnoreCase) &&
                            Math.Abs(r.MontoPago + neg.MontoPago) < 0.01m &&
                            NormalizeInvoiceId(r.CodigoFactura) == negId);

                        // Si no lo encuentra por ID, intentamos un match más relajado por Monto exacto (si no hay ambigüedad)
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

                // 4. Filtrado Final Atómico
                allRecords = allRecords.Where(r => !r.IsDeleted).ToList();
                
                // ACTUALIZACIÓN DE LISTAS DE APOYO (DEPURADO)
                depuradoRecords = allRecords.ToList();
                sinInvoiceRecords = depuradoRecords.Where(r => r.Descripcion != "INVOICE").ToList();

                

                ActualizarGrid(allRecords);
                CalcularControlLabel();
                // REGLA FINAL: Eliminar filas donde Columna H sea exactamente 0 (Solo para la visualización y reporte final)
                // Nota: Hacemos esto al final del todo para no interferir con las reglas de netting previas.
                allRecords = allRecords.Where(r => r.MontoPago2 != 0).ToList();
                MessageBox.Show("Proceso de Netting y Filtrado finalizado correctamente.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error en el proceso de Netting: " + ex.Message);
            }
        }

        private void btnConsolidar_Click(object sender, EventArgs e)
        {
            ConsolidarLista();
        }

        private void ConsolidarLista()
        {
            var consolidado = allRecords
                .GroupBy(r => r.Fecha.Date) 
                .Select(g => new
                {
                    Fecha = g.Key,
                    TotalH = g.Sum(r => r.MontoPago2) 
                })
                .OrderBy(x => x.Fecha)
                .ToList();

            dgvDatos.DataSource = consolidado;

            // Validación Final: Sum(TotalH) == Current SUM F
            decimal sumTotalH = consolidado.Sum(x => x.TotalH);
            decimal currentSumF = allRecords.Sum(r => r.MontoPago2); 

            if (Math.Abs(sumTotalH - currentSumF) < 0.01m)
            {
                MessageBox.Show($"Validación Exitosa: Sum(TotalH) [{sumTotalH:N2}] coincide con SUM F [{currentSumF:N2}].");
            }
            else
            {
                MessageBox.Show($"Advertencia: Sum(TotalH) [{sumTotalH:N2}] no coincide con SUM H [{currentSumF:N2}].", "Validación Fallida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            lblConsolidado.Text = $"Consolidado: {sumTotalH.ToString("N2", CultureInfo.InvariantCulture)}";
        }

        private void btnSepararInvoices_Click(object sender, EventArgs e)
        {
            var invoiceList = allRecords.Where(r => !r.IsDeleted && r.Descripcion == "INVOICE").ToList();
            ActualizarGrid(invoiceList);
            MessageBox.Show($"Se han extraído {invoiceList.Count} registros de INVOICE.");
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Excel Workbook (*.xlsx)|*.xlsx";
                sfd.FileName = "Reporte_Verisure_" + DateTime.Now.ToString("yyyyMMdd_HHmm");
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    ExportarAExcelPro(sfd.FileName);
                }
            }
        }

        private void ExportarAExcelPro(string filePath)
        {
            Excel.Application xlApp = new Excel.Application();
            if (xlApp == null)
            {
                MessageBox.Show("Excel no está instalado correctamente.");
                return;
            }

            Excel.Workbooks xlWorkbooks = xlApp.Workbooks;
            Excel.Workbook xlWorkbook = xlWorkbooks.Add(System.Reflection.Missing.Value);

            try
            {
                // Hoja 1: Original
                Excel.Worksheet sheet1 = (Excel.Worksheet)xlWorkbook.Worksheets.get_Item(1);
                sheet1.Name = "Original";
                FillWorksheetOriginal(sheet1, originalRecords);

                // Hoja 2: Depurado
                Excel.Worksheet sheet2 = (Excel.Worksheet)xlWorkbook.Worksheets.Add(After: xlWorkbook.Worksheets[xlWorkbook.Worksheets.Count]);
                sheet2.Name = "Depurado";
                FillWorksheet(sheet2, depuradoRecords.OrderBy(r => r.Fecha).ToList());

                // Hoja 3: Sin Invoices
                Excel.Worksheet sheet3 = (Excel.Worksheet)xlWorkbook.Worksheets.Add(After: xlWorkbook.Worksheets[xlWorkbook.Worksheets.Count]);
                sheet3.Name = "Sin Invoices";
                FillWorksheet(sheet3, sinInvoiceRecords.OrderBy(r => r.Fecha).ToList());

                // Hoja 4: Invoices
                Excel.Worksheet sheet4 = (Excel.Worksheet)xlWorkbook.Worksheets.Add(After: xlWorkbook.Worksheets[xlWorkbook.Worksheets.Count]);
                sheet4.Name = "Invoices";
                FillInvoicesSheet(sheet4, depuradoRecords.Where(x => x.Descripcion.ToUpper() == "INVOICE").OrderBy(r => r.Fecha).ToList());

                // Hoja 5: Consolidado
                Excel.Worksheet sheet5 = (Excel.Worksheet)xlWorkbook.Worksheets.Add(After: xlWorkbook.Worksheets[xlWorkbook.Worksheets.Count]);
                sheet5.Name = "Consolidado";
                FillConsolidation(sheet5, depuradoRecords.OrderBy(r => r.Fecha).ToList());

                // Hoja 6: Reportar
                Excel.Worksheet sheet6 = (Excel.Worksheet)xlWorkbook.Worksheets.Add(After: xlWorkbook.Worksheets[xlWorkbook.Worksheets.Count]);
                sheet6.Name = "Reportar";
                FillReportarSheet(sheet6, reportarRecords.OrderBy(r => r.Fecha).ToList());

                

                xlWorkbook.SaveAs(filePath);
                MessageBox.Show("Exportación exitosa a Excel con 6 hojas.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al exportar a Excel: " + ex.Message);
            }
            finally
            {
                xlWorkbook.Close();
                xlApp.Quit();

                Marshal.ReleaseComObject(xlWorkbook);
                Marshal.ReleaseComObject(xlWorkbooks);
                Marshal.ReleaseComObject(xlApp);
            }
        }

        private void FillWorksheetOriginal(Excel.Worksheet sheet, List<TransactionRecord> records)
        {
            if (records.Count == 0 || records[0].RawParts == null) return;

            int row = 1;
            foreach (var r in records)
            {
                for (int i = 0; i < r.RawParts.Length; i++)
                {
                    sheet.Cells[row, i + 1] = r.RawParts[i];
                }
                row++;
            }
            sheet.Columns.AutoFit();
        }

        private void FillWorksheet(Excel.Worksheet sheet, List<TransactionRecord> records)
        {
            string[] headers = { "Fecha (B)", "Descripción (C)", "Código Boleta (D)", "Monto Cobro (F)", "Monto Pago Cliente (H)", "Factura (L)" };
            for (int i = 0; i < headers.Length; i++)
            {
                sheet.Cells[1, i + 1] = headers[i];
            }

            int row = 2;
            foreach (var r in records)
            {
                sheet.Cells[row, 1] = r.Fecha.Date;
                sheet.Cells[row, 2] = r.Descripcion;
                sheet.Cells[row, 3] = r.CodigoBoleta;
                sheet.Cells[row, 4] = r.MontoPago;
                sheet.Cells[row, 5] = r.MontoPago2;
                sheet.Cells[row, 6] = r.CodigoFactura;
                row++;
            }
            sheet.Columns.AutoFit();
        }

        private void FillConsolidation(Excel.Worksheet sheet, List<TransactionRecord> records)
        {
            sheet.Cells[1, 1] = "Fecha (B)";
            sheet.Cells[1, 2] = "Sumatoria H";

            var consolidado = records
                .GroupBy(r => r.Fecha.Date)
                .Select(g => new { Fecha = g.Key, TotalH = g.Sum(x => x.MontoPago2) })
                .OrderBy(x => x.Fecha)
                .ToList();

            int row = 2;
            foreach (var item in consolidado)
            {
                if(item.TotalH > 0)
                {
                    sheet.Cells[row, 1] = item.Fecha.Date;
                    sheet.Cells[row, 2] = item.TotalH;
                    row++;
                }
                
            }
            sheet.Columns.AutoFit();
        }

        private void FillReportarSheet(Excel.Worksheet sheet, List<TransactionRecord> records)
        {
            string[] headers = { "Fecha (B)", "Descripción (C)", "Código Boleta (D)", "Monto Cobro (F)", "Monto Pago Cliente (H)", "Factura (L)", "Diferencia Días" };
            for (int i = 0; i < headers.Length; i++)
            {
                sheet.Cells[1, i + 1] = headers[i];
            }

            int row = 2;
            foreach (var r in records)
            {
                sheet.Cells[row, 1] = r.Fecha.Date;
                sheet.Cells[row, 2] = r.Descripcion;
                sheet.Cells[row, 3] = r.CodigoBoleta;
                sheet.Cells[row, 4] = r.MontoPago;
                sheet.Cells[row, 5] = r.MontoPago2;
                sheet.Cells[row, 6] = r.CodigoFactura;
                sheet.Cells[row, 7] = r.DiffDays;
                row++;
            }
            sheet.Columns.AutoFit();
        }

        private void FillInvoicesSheet(Excel.Worksheet sheet, List<TransactionRecord> records)
        {
            string[] headers = { "Fecha (B)", "Monto Pago Cliente (H)", "Factura" };
            for (int i = 0; i < headers.Length; i++)
            {
                sheet.Cells[1, i + 1] = headers[i];
            }

            int row = 2;
            foreach (var r in records)
            {
                sheet.Cells[row, 1] = r.Fecha.Date;
                sheet.Cells[row, 2] = r.MontoPago;
                sheet.Cells[row, 3] = r.CodigoFactura;
                row++;
            }
            sheet.Columns.AutoFit();
        }


    }
}
