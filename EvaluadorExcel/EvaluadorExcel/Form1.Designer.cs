namespace EvaluadorExcel
{
    partial class Form1
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnCargar = new System.Windows.Forms.Button();
            this.dgvDatos = new System.Windows.Forms.DataGridView();
            this.lblResultado = new System.Windows.Forms.Label();
            this.btnDepurar = new System.Windows.Forms.Button();
            this.btnSepararInvoices = new System.Windows.Forms.Button();
            this.btnConsolidar = new System.Windows.Forms.Button();
            this.btnExportar = new System.Windows.Forms.Button();
            this.btnLimpiar = new System.Windows.Forms.Button();
            this.lblSumCobro = new System.Windows.Forms.Label();
            this.lblSumPagoCliente = new System.Windows.Forms.Label();
            this.lblConsolidado = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDatos)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCargar
            // 
            this.btnCargar.Location = new System.Drawing.Point(12, 12);
            this.btnCargar.Name = "btnCargar";
            this.btnCargar.Size = new System.Drawing.Size(120, 35);
            this.btnCargar.TabIndex = 0;
            this.btnCargar.Text = "Cargar Archivo";
            this.btnCargar.UseVisualStyleBackColor = true;
            this.btnCargar.Click += new System.EventHandler(this.btnCargar_Click);
            // 
            // dgvDatos
            // 
            this.dgvDatos.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvDatos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDatos.Location = new System.Drawing.Point(12, 53);
            this.dgvDatos.Name = "dgvDatos";
            this.dgvDatos.RowHeadersWidth = 51;
            this.dgvDatos.RowTemplate.Height = 24;
            this.dgvDatos.Size = new System.Drawing.Size(776, 300);
            this.dgvDatos.TabIndex = 1;
            // 
            // lblResultado
            // 
            this.lblResultado.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblResultado.AutoSize = true;
            this.lblResultado.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblResultado.Location = new System.Drawing.Point(12, 365);
            this.lblResultado.Name = "lblResultado";
            this.lblResultado.Size = new System.Drawing.Size(209, 25);
            this.lblResultado.TabIndex = 2;
            this.lblResultado.Text = "Resultado Control: 0";
            // 
            // btnDepurar
            // 
            this.btnDepurar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDepurar.Location = new System.Drawing.Point(342, 403);
            this.btnDepurar.Name = "btnDepurar";
            this.btnDepurar.Size = new System.Drawing.Size(100, 35);
            this.btnDepurar.TabIndex = 3;
            this.btnDepurar.Text = "Depurar";
            this.btnDepurar.UseVisualStyleBackColor = true;
            this.btnDepurar.Click += new System.EventHandler(this.btnDepurar_Click);
            // 
            // btnSepararInvoices
            // 
            this.btnSepararInvoices.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSepararInvoices.Location = new System.Drawing.Point(448, 403);
            this.btnSepararInvoices.Name = "btnSepararInvoices";
            this.btnSepararInvoices.Size = new System.Drawing.Size(120, 35);
            this.btnSepararInvoices.TabIndex = 4;
            this.btnSepararInvoices.Text = "Separar Invoices";
            this.btnSepararInvoices.UseVisualStyleBackColor = true;
            this.btnSepararInvoices.Click += new System.EventHandler(this.btnSepararInvoices_Click);
            // 
            // btnConsolidar
            // 
            this.btnConsolidar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConsolidar.Location = new System.Drawing.Point(574, 403);
            this.btnConsolidar.Name = "btnConsolidar";
            this.btnConsolidar.Size = new System.Drawing.Size(110, 35);
            this.btnConsolidar.TabIndex = 5;
            this.btnConsolidar.Text = "Consolidar";
            this.btnConsolidar.UseVisualStyleBackColor = true;
            this.btnConsolidar.Click += new System.EventHandler(this.btnConsolidar_Click);
            // 
            // btnExportar
            // 
            this.btnExportar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExportar.Location = new System.Drawing.Point(690, 403);
            this.btnExportar.Name = "btnExportar";
            this.btnExportar.Size = new System.Drawing.Size(98, 35);
            this.btnExportar.TabIndex = 10;
            this.btnExportar.Text = "Exportar";
            this.btnExportar.UseVisualStyleBackColor = true;
            this.btnExportar.Click += new System.EventHandler(this.btnExportar_Click);
            // 
            // btnLimpiar
            // 
            this.btnLimpiar.Location = new System.Drawing.Point(688, 12);
            this.btnLimpiar.Name = "btnLimpiar";
            this.btnLimpiar.Size = new System.Drawing.Size(100, 35);
            this.btnLimpiar.TabIndex = 7;
            this.btnLimpiar.Text = "Limpiar";
            this.btnLimpiar.UseVisualStyleBackColor = true;
            this.btnLimpiar.Click += new System.EventHandler(this.btnLimpiar_Click);
            // 
            // lblSumCobro
            // 
            this.lblSumCobro.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblSumCobro.AutoSize = true;
            this.lblSumCobro.Location = new System.Drawing.Point(12, 400);
            this.lblSumCobro.Name = "lblSumCobro";
            this.lblSumCobro.Size = new System.Drawing.Size(61, 16);
            this.lblSumCobro.TabIndex = 8;
            this.lblSumCobro.Text = "Cobro (F): 0";
            // 
            // lblSumPagoCliente
            // 
            this.lblSumPagoCliente.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblSumPagoCliente.AutoSize = true;
            this.lblSumPagoCliente.Location = new System.Drawing.Point(150, 400);
            this.lblSumPagoCliente.Name = "lblSumPagoCliente";
            this.lblSumPagoCliente.Size = new System.Drawing.Size(63, 16);
            this.lblSumPagoCliente.TabIndex = 9;
            this.lblSumPagoCliente.Text = "Pago Cliente (H): 0";
            // 
            // lblConsolidado
            // 
            this.lblConsolidado.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblConsolidado.AutoSize = true;
            this.lblConsolidado.Location = new System.Drawing.Point(116, 422);
            this.lblConsolidado.Name = "lblConsolidado";
            this.lblConsolidado.Size = new System.Drawing.Size(97, 16);
            this.lblConsolidado.TabIndex = 11;
            this.lblConsolidado.Text = "Consolidado: 0";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.lblConsolidado);
            this.Controls.Add(this.lblSumPagoCliente);
            this.Controls.Add(this.lblSumCobro);
            this.Controls.Add(this.btnLimpiar);
            this.Controls.Add(this.btnExportar);
            this.Controls.Add(this.btnConsolidar);
            this.Controls.Add(this.btnSepararInvoices);
            this.Controls.Add(this.btnDepurar);
            this.Controls.Add(this.lblResultado);
            this.Controls.Add(this.dgvDatos);
            this.Controls.Add(this.btnCargar);
            this.Name = "Form1";
            this.Text = "Evaluador Excel - Verisure";
            ((System.ComponentModel.ISupportInitialize)(this.dgvDatos)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Button btnCargar;
        private System.Windows.Forms.DataGridView dgvDatos;
        private System.Windows.Forms.Label lblResultado;
        private System.Windows.Forms.Button btnDepurar;
        private System.Windows.Forms.Button btnSepararInvoices;
        private System.Windows.Forms.Button btnConsolidar;
        private System.Windows.Forms.Button btnExportar;
        private System.Windows.Forms.Button btnLimpiar;
        private System.Windows.Forms.Label lblSumCobro;
        private System.Windows.Forms.Label lblSumPagoCliente;

        #endregion

        private System.Windows.Forms.Label lblConsolidado;
    }
}

