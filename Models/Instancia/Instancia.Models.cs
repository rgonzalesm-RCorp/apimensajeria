public class Instancia
{
    public int InstanciaId { get; set; }
    public string InstanceIdUltraMsg { get; set; }
    public string Token { get; set; }
    public string Descripcion { get; set; }
    public string NumeroTelefono { get; set; }
    public bool MultiEmpresa { get; set; } = false;
    public DateTime FechaRegistro { get; set; } = DateTime.Now;
    public bool Estado { get; set; } = true;
    public int Codigousuario { get; set; } = 1;
}
public class InstanciaxEmpresa
{
    public int InstanciaId { get; set; }
    public string InstanceIdUltraMsg { get; set; }
    public string Token { get; set; }
    public string Descripcion { get; set; }
    
    public bool MultiEmpresa { get; set; } = false;
    public DateTime FechaRegistro { get; set; } = DateTime.Now;
    public bool Estado { get; set; } = true;
    public int Codigousuario { get; set; } = 1;
    public int EmpresaId { get; set; }
}
