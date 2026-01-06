using System;

namespace EvaluadorExcelApp.Models
{
    public class TransactionRecord
    {
        public DateTime FechaRegistro { get; set; } // Column A (index 0)
        public DateTime Fecha { get; set; }         // Column B (index 1)
        public string Descripcion { get; set; }    // Column C (index 2)
        public string CodigoBoleta { get; set; }   // Column D (index 3)
        public decimal MontoPago { get; set; }     // Column F (index 5)
        public decimal MontoPago2 { get; set; }    // Column H (index 7) - Netting column
        public string CodigoFactura { get; set; }  // Column L (index 11)
        public string[] RawParts { get; set; }     // Original raw data columns

        // For internal use
        public bool IsDeleted { get; set; }
        public int DiffDays { get; set; }
        public bool HasPagoOrphanIssue { get; set; }
        public bool HasCobroOrphanIssue { get; set; }
    }
}
