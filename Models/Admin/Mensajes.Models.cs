public class ListaMensajes
{
    public string Instancia { get; set; }
    public string DescripcionTipo { get; set; } // Para la segunda columna "Descripcion"
    public string MensajeTexto { get; set; }
    public string Caption { get; set; }
    public string Documento { get; set; }
    public DateTime? FechaCreacion { get; set; }
    public int Total { get; set; }
    public int Enviado { get; set; }
    public int NoEnviado { get; set; }
    public int InstanciaId { get; set; }
    public int EmpresaId { get; set; }
    public int SmsId { get; set; }
    public int TipoSmsId { get; set; }
}
public class ListaDestinatario
{
    public int DestinatarioId { get; set; }
    public int? SmsId { get; set; }
    public string NroTelefono { get; set; }
    public string? Nombre { get; set; }
    public string StatusSms { get; set; }
    public string JsonStatus { get; set; }
    public DateTime? FechaEnvio { get; set; }
    public DateTime? FechaCreacion { get; set; }
    public bool? Estado { get; set; }
    public int? CodigoUsuario { get; set; }
    public int? CounterIntent { get; set; }
    public bool? Cobrar { get; set; }
}

public class ResponseListaMensajes
{
    public string status { get; set; }
    public string message { get; set; }
    public List<dynamic> data { get; set; }
}

public class DetailsSmsResponse
{
    public string status { get; set; }
    public string message { get; set; }
    public DetailMensaje data { get; set; }
}
public class DetailMensaje
{
    public int SmsId { get; set; }
    public int TipoSmsId { get; set; }
    public string Mensaje { get; set; }
    public string Documento { get; set; }
    public string Caption { get; set; }
    public string Base64 { get; set; }
    public string Extension { get; set; }
    public List<ListaDestinatario> Destinatarios { get; set; }
}