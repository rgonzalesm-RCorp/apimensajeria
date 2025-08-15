public class Empresa
{
    public int EmpresaId { get; set; }
    public int InstanciaId { get; set; }
    public string Nombre { get; set; }
    public string Srv { get; set; }
    public string DB { get; set; }
    public string Usuario { get; set; }
    public string Pasword { get; set; }
    public string CodigoCC { get; set; }
    public DateTime FechaRegistro { get; set; } = DateTime.Now;
    public bool Estado { get; set; } = true;
    public int Codigousuario { get; set; } = 1;
}
