public class UsuarioEmpresaInstancia
{
    public int UsuarioId { get; set; }
    public int EmpresaId { get; set; }
    public int InstanciaId { get; set; }
    public string NroTelefono { get; set; }
    public DateTime FechaRegistro { get; set; } = DateTime.Now;
    public bool Estado { get; set; } = true;
    public int Codigousuario { get; set; } = 1;
}
