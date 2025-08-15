public class MensajeDto
{
    public string Nombre { get; set; }
    public string CodigoCC { get; set; }
    public string InstanceIdUltraMsg { get; set; }
    public string Token { get; set; }
    public bool Archivo { get; set; }
    public string Ruta { get; set; }
    public string Descripcion { get; set; }
    public string Mensaje { get; set; }
    public string Documento { get; set; }
    public string Caption { get; set; }
    public int DestinatarioId { get; set; }
    public string NroTelefono { get; set; }
    public int SmsId { get; set; }
    public int InstanciaId { get; set; }
    public int TipoSmsId { get; set; }
    public int EmpreaId { get; set; }
    public int Rn { get; set; } // row_number
}
