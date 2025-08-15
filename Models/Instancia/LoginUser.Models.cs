public class LoginUser
{
    public int LoginUserId { get; set; }
    public int UsuarioId { get; set; }
    public string CodeQr { get; set; }
    public string StatusLogin { get; set; }
    public string SubStatus { get; set; }
    public string StatusJson { get; set; }
    public DateTime FechaRegistro { get; set; } = DateTime.Now;
    public bool Estado { get; set; } = true;
    public int Codigousuario { get; set; } = 1;
}
