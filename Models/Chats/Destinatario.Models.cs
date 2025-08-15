public class Destinatario
{
    public int DestinatarioId { get; set; }
    public string NroTelefono { get; set; }
    public string StatusSms { get; set; }
    public string JsonStatus { get; set; }
    public DateTime FechaEnvio { get; set; } = DateTime.Now;
    public DateTime FechaCreacion { get; set; } = DateTime.Now;
    public bool Estado { get; set; } = true;
    public int CodigoUsuario { get; set; }
}

public class DestinatarioSave
{
    public string NroTelefono { get; set; }
    public string StatusSms { get; set; }
    public string JsonStatus { get; set; }
    public DateTime FechaEnvio { get; set; } = DateTime.Now;
    public DateTime FechaCreacion { get; set; } = DateTime.Now;
    public bool Estado { get; set; } = true;
    public int CodigoUsuario { get; set; }
}
