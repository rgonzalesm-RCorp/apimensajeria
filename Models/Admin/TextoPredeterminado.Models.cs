public class Lista_TextoPredeterminado
{
    public int? TextoPredeterminadoId { get; set; }
    public int? EmpresaId { get; set; }
    public string? Texto { get; set; }
    public DateTime? FechaCreacion { get; set; }
    public bool? Estado { get; set; }
    public int? CodigoUsuario { get; set; }
}
public class Request_TextoPredeterminado_Save
{
    public string? CodigoEmpresaCC { get; set; }
    public string? Texto { get; set; }
    public int? CodigoUsuario { get; set; }
}
public class Request_TextoPredeterminado_Update
{
    public int? TextoPredeterminadoId { get; set; }
    public string? Texto { get; set; }
    public int? CodigoUsuario { get; set; }
}
public class ResponseTextoPredeterminado
{
    public string status { get; set; }
    public string message { get; set; }
    public List<dynamic> data { get; set; }
}